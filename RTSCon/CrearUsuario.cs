using Krypton.Toolkit;
using RTSCon.Datos;
using RTSCon.Negocios;
using System;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace RTSCon
{
    public partial class CrearUsuario : KryptonForm
    {
        private readonly NAuth _auth;

        public CrearUsuario()
        {
            InitializeComponent();

            var cn = ConfigurationManager.ConnectionStrings["RTSCond"].ConnectionString;
            _auth = new NAuth(new DAuth(cn));

            cmbDocumento.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbDocumento.SelectedIndexChanged += cmbDocumento_SelectedIndexChanged;

            CargarRolesSegunSesion();

            ConfigMasked(txtCedula);
            ConfigMasked(txtRNC);
            ConfigMasked(txtPasaporte);

            ConfigurarValidacionDocumentos();

            // Username NO debe comportarse como contraseña
            txtUsername.PasswordChar = '\0';
            txtUsername.UseSystemPasswordChar = false;
        }

        // ================= MODELO ROL =================
        private class RolItem
        {
            public string Texto { get; set; }
            public string Valor { get; set; }
        }

        // ================= ROLES POR JERARQUÍA =================
        private void CargarRolesSegunSesion()
        {
            cmbRol.DisplayMember = "Texto";
            cmbRol.ValueMember = "Valor";
            cmbRol.Items.Clear();

            string rolActual = SessionHelper.Rol;

            switch (rolActual)
            {
                case "SA":
                    cmbRol.Items.Add(new RolItem { Texto = "Super Administrador", Valor = "SA" });
                    cmbRol.Items.Add(new RolItem { Texto = "Propietario", Valor = "Propietario" });
                    cmbRol.Items.Add(new RolItem { Texto = "Secretario", Valor = "Secretario" });
                    cmbRol.Items.Add(new RolItem { Texto = "Inquilino", Valor = "Inquilino" });
                    break;

                case "Propietario":
                    cmbRol.Items.Add(new RolItem { Texto = "Secretario", Valor = "Secretario" });
                    cmbRol.Items.Add(new RolItem { Texto = "Inquilino", Valor = "Inquilino" });
                    break;

                case "Secretario":
                    cmbRol.Items.Add(new RolItem { Texto = "Inquilino", Valor = "Inquilino" });
                    break;

                default:
                    KryptonMessageBox.Show(
                        this,
                        "No tiene permisos para crear usuarios.",
                        "Acceso denegado",
                        KryptonMessageBoxButtons.OK,
                        KryptonMessageBoxIcon.Warning);

                    Close();
                    return;
            }

            if (cmbRol.Items.Count > 0)
                cmbRol.SelectedIndex = 0;
        }

        // ================= DOCUMENTO =================
        private void cmbDocumento_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtCedula.Visible = txtCedula.Enabled = false;
            txtRNC.Visible = txtRNC.Enabled = false;
            txtPasaporte.Visible = txtPasaporte.Enabled = false;

            var tipo = Convert.ToString(cmbDocumento.SelectedItem) ?? "";

            if (tipo.StartsWith("Ced", StringComparison.OrdinalIgnoreCase))
            {
                txtCedula.Visible = txtCedula.Enabled = true;
                txtCedula.Focus();
            }
            else if (tipo.Equals("RNC", StringComparison.OrdinalIgnoreCase))
            {
                txtRNC.Visible = txtRNC.Enabled = true;
                txtRNC.Focus();
            }
            else if (tipo.Equals("Pasaporte", StringComparison.OrdinalIgnoreCase))
            {
                txtPasaporte.Visible = txtPasaporte.Enabled = true;
                txtPasaporte.Focus();
            }
        }

        private void ConfigurarValidacionDocumentos()
        {
            txtCedula.Leave -= Masked_ClearIfIncomplete_OnLeave;
            txtRNC.Leave -= Masked_ClearIfIncomplete_OnLeave;
            txtPasaporte.Leave -= Masked_ClearIfIncomplete_OnLeave;

            txtCedula.Leave += Masked_ClearIfIncomplete_OnLeave;
            txtRNC.Leave += Masked_ClearIfIncomplete_OnLeave;
            txtPasaporte.Leave += Masked_ClearIfIncomplete_OnLeave;

            txtCedula.VisibleChanged -= Masked_ClearOnHide;
            txtRNC.VisibleChanged -= Masked_ClearOnHide;
            txtPasaporte.VisibleChanged -= Masked_ClearOnHide;

            txtCedula.VisibleChanged += Masked_ClearOnHide;
            txtRNC.VisibleChanged += Masked_ClearOnHide;
            txtPasaporte.VisibleChanged += Masked_ClearOnHide;

            txtCedula.KeyPress -= SoloNumeros_KeyPress;
            txtRNC.KeyPress -= SoloNumeros_KeyPress;

            txtCedula.KeyPress += SoloNumeros_KeyPress;
            txtRNC.KeyPress += SoloNumeros_KeyPress;

            txtCedula.KeyDown -= SoloNumeros_KeyDown;
            txtRNC.KeyDown -= SoloNumeros_KeyDown;

            txtCedula.KeyDown += SoloNumeros_KeyDown;
            txtRNC.KeyDown += SoloNumeros_KeyDown;

            txtPasaporte.KeyPress -= Pasaporte_KeyPress;
            txtPasaporte.KeyPress += Pasaporte_KeyPress;

            txtPasaporte.KeyDown -= Pasaporte_KeyDown;
            txtPasaporte.KeyDown += Pasaporte_KeyDown;
        }

        private void ConfigMasked(KryptonMaskedTextBox txt)
        {
            txt.PromptChar = ' ';
            txt.HidePromptOnLeave = true;
            txt.ResetOnPrompt = true;
            txt.ResetOnSpace = true;
            txt.SkipLiterals = true;
            txt.CutCopyMaskFormat = MaskFormat.ExcludePromptAndLiterals;
            txt.TextMaskFormat = MaskFormat.ExcludePromptAndLiterals;
            txt.BeepOnError = false;
        }

        private void SoloNumeros_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar))
                return;

            if (!char.IsDigit(e.KeyChar))
                e.Handled = true;
        }

        private void SoloNumeros_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.V)
            {
                string texto = Clipboard.ContainsText() ? Clipboard.GetText() : string.Empty;

                if (!SoloContieneDigitos(texto))
                {
                    e.SuppressKeyPress = true;
                    e.Handled = true;
                }
            }
        }

        private void Pasaporte_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar))
                return;

            if (!char.IsLetterOrDigit(e.KeyChar))
            {
                e.Handled = true;
                return;
            }

            e.KeyChar = char.ToUpperInvariant(e.KeyChar);
        }

        private void Pasaporte_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.V)
            {
                string texto = Clipboard.ContainsText() ? Clipboard.GetText() : string.Empty;
                texto = NormalizarPasaporte(texto);

                if (!Regex.IsMatch(texto, @"^[A-Za-z0-9]*$"))
                {
                    e.SuppressKeyPress = true;
                    e.Handled = true;
                }
            }
        }

        private void Masked_ClearIfIncomplete_OnLeave(object sender, EventArgs e)
        {
            MaskedTextBox txt = sender as MaskedTextBox;

            if (txt == null)
                return;

            if (!txt.MaskCompleted)
                txt.Clear();
        }

        private void Masked_ClearOnHide(object sender, EventArgs e)
        {
            MaskedTextBox txt = sender as MaskedTextBox;

            if (txt == null)
                return;

            if (!txt.Visible)
                txt.Clear();
        }

        private string ObtenerTipoDocumento()
        {
            string tipo = Convert.ToString(cmbDocumento.SelectedItem) ?? string.Empty;

            if (tipo.StartsWith("Ced", StringComparison.OrdinalIgnoreCase))
                return "Cedula";

            if (tipo.Equals("RNC", StringComparison.OrdinalIgnoreCase))
                return "RNC";

            if (tipo.Equals("Pasaporte", StringComparison.OrdinalIgnoreCase))
                return "Pasaporte";

            return string.Empty;
        }

        private string ObtenerDocumentoNormalizado(string tipoDoc)
        {
            if (tipoDoc == "Cedula")
                return ExtraerDigitos(txtCedula.Text);

            if (tipoDoc == "RNC")
                return ExtraerDigitos(txtRNC.Text);

            if (tipoDoc == "Pasaporte")
                return NormalizarPasaporte(txtPasaporte.Text);

            return string.Empty;
        }

        private void ValidarDocumento(string tipoDoc, string documento)
        {
            if (string.IsNullOrWhiteSpace(tipoDoc))
                throw new Exception("Seleccione el tipo de documento.");

            if (string.IsNullOrWhiteSpace(documento))
                throw new Exception("Ingrese el documento.");

            if (tipoDoc == "Cedula")
            {
                if (!Regex.IsMatch(documento, @"^\d{11}$"))
                    throw new Exception("La cédula debe tener exactamente 11 dígitos numéricos.");

                return;
            }

            if (tipoDoc == "RNC")
            {
                if (!Regex.IsMatch(documento, @"^\d{8}$"))
                    throw new Exception("El RNC debe tener exactamente 8 dígitos numéricos.");

                return;
            }

            if (tipoDoc == "Pasaporte")
            {
                if (!Regex.IsMatch(documento, @"^[A-Za-z0-9]{2}\d{6}$"))
                    throw new Exception("El pasaporte debe tener 8 caracteres: 2 alfanuméricos y 6 numéricos. Ejemplo: KA126768.");

                return;
            }

            throw new Exception("Tipo de documento inválido.");
        }

        private string ExtraerDigitos(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return string.Empty;

            string resultado = string.Empty;

            int i;
            for (i = 0; i < texto.Length; i++)
            {
                if (char.IsDigit(texto[i]))
                    resultado += texto[i];
            }

            return resultado;
        }

        private string NormalizarPasaporte(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return string.Empty;

            return texto
                .Trim()
                .Replace("-", string.Empty)
                .Replace(" ", string.Empty)
                .ToUpperInvariant();
        }

        private bool SoloContieneDigitos(string texto)
        {
            if (string.IsNullOrEmpty(texto))
                return true;

            int i;
            for (i = 0; i < texto.Length; i++)
            {
                if (!char.IsDigit(texto[i]))
                    return false;
            }

            return true;
        }

        // ================= CREAR =================
        private void btnCrear_Click(object sender, EventArgs e)
        {
            try
            {
                string nombreCompleto = txtNombreCompleto.Text.Trim();
                string usuarioSistema = txtUsername.Text.Trim();
                string correo = txtCorreo.Text.Trim();
                string clave = txtContraseña.Text.Trim();

                if (string.IsNullOrWhiteSpace(nombreCompleto))
                    throw new Exception("Ingrese el nombre completo.");

                if (string.IsNullOrWhiteSpace(usuarioSistema))
                    throw new Exception("Ingrese el nombre de usuario.");

                if (usuarioSistema.Contains("@"))
                    throw new Exception("El nombre de usuario no debe ser un correo.");

                if (string.IsNullOrWhiteSpace(correo) || !correo.Contains("@"))
                    throw new Exception("Ingrese un correo válido.");

                if (string.Equals(usuarioSistema, correo, StringComparison.OrdinalIgnoreCase))
                    throw new Exception("El nombre de usuario no puede ser igual al correo.");

                if (string.IsNullOrWhiteSpace(clave))
                    throw new Exception("Ingrese la contraseña.");

                var rolItem = cmbRol.SelectedItem as RolItem;
                if (rolItem == null)
                    throw new Exception("Seleccione un rol válido.");

                string tipoDoc = ObtenerTipoDocumento();
                string tipoDocNormalizado = ObtenerTipoDocumento();
                string documento = ObtenerDocumentoNormalizado(tipoDoc);

                ValidarDocumento(tipoDoc, documento);

                string creador = SessionHelper.Usuario ?? "SA";

                _auth.CrearUsuario(
                    usuario: usuarioSistema,
                    correo: correo,
                    rol: rolItem.Valor,
                    password: clave,
                    creador: creador,
                    documentoTipo: tipoDocNormalizado,
                    documento: documento);

                KryptonMessageBox.Show(
                    this,
                    "Usuario creado correctamente.",
                    "Crear Usuario",
                    KryptonMessageBoxButtons.OK,
                    KryptonMessageBoxIcon.Information);

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                KryptonMessageBox.Show(
                    this,
                    ex.Message,
                    "Crear Usuario",
                    KryptonMessageBoxButtons.OK,
                    KryptonMessageBoxIcon.Error);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
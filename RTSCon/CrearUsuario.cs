using Krypton.Toolkit;
using RTSCon.Datos;
using RTSCon.Negocios;
using System;
using System.Configuration;
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

            txtCedula.Leave += Masked_ClearIfIncomplete_OnLeave;
            txtRNC.Leave += Masked_ClearIfIncomplete_OnLeave;
            txtPasaporte.Leave += Masked_ClearIfIncomplete_OnLeave;

            txtCedula.VisibleChanged += Masked_ClearOnHide;
            txtRNC.VisibleChanged += Masked_ClearOnHide;
            txtPasaporte.VisibleChanged += Masked_ClearOnHide;

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
            else
            {
                txtPasaporte.Visible = txtPasaporte.Enabled = true;
                txtPasaporte.Focus();
            }
        }

        // ================= CREAR =================
        private void btnCrear_Click(object sender, EventArgs e)
        {
            try
            {
                string nombreCompleto = txtNombreCompleto.Text.Trim();
                string correo = txtCorreo.Text.Trim();
                string clave = txtContraseña.Text.Trim();

                if (string.IsNullOrWhiteSpace(nombreCompleto))
                    throw new Exception("Ingrese el nombre completo.");

                if (string.IsNullOrWhiteSpace(correo) || !correo.Contains("@"))
                    throw new Exception("Ingrese un correo válido.");

                if (string.IsNullOrWhiteSpace(clave))
                    throw new Exception("Ingrese la contraseña.");

                var rolItem = cmbRol.SelectedItem as RolItem;
                if (rolItem == null)
                    throw new Exception("Seleccione un rol válido.");

                // ⚠️ USUARIO REAL DEL SISTEMA
                // ✔ El usuario DEBE SER el correo (único y estable)
                string usuarioSistema = correo.ToLowerInvariant();

                // Documento
                string documento = "";
                string tipoDoc = Convert.ToString(cmbDocumento.SelectedItem) ?? "";

                if (tipoDoc.StartsWith("Ced"))
                    documento = txtCedula.Text.Trim();
                else if (tipoDoc == "RNC")
                    documento = txtRNC.Text.Trim();
                else
                    documento = txtPasaporte.Text.Trim();

                if (string.IsNullOrWhiteSpace(documento))
                    throw new Exception("Ingrese el documento.");

                string creador = SessionHelper.Usuario ?? "SA";

                _auth.CrearUsuario(
                    usuario: usuarioSistema,
                    correo: correo,
                    rol: rolItem.Valor,
                    password: clave,
                    creador: creador
                );

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

        private void MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
        {
            var txt = sender as MaskedTextBox;
            if (txt == null) return;

            if (!txt.MaskFull)
                txt.Clear();
        }

        private void Masked_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var txt = sender as MaskedTextBox;
            if (txt == null) return;

            if (!txt.MaskFull)
                txt.Clear();
        }

        private void Masked_ForceClear(object sender, EventArgs e)
        {
            if (sender is MaskedTextBox txt)
            {
                txt.TextMaskFormat = MaskFormat.ExcludePromptAndLiterals;
                txt.TextMaskFormat = MaskFormat.IncludeLiterals;

                if (!txt.MaskCompleted)
                {
                    txt.Clear();
                }

            }
        }

        private void Masked_ForceClear_Visible(object sender, EventArgs e)
        {
            if (sender is MaskedTextBox txt)
            {
                if (!txt.Visible)
                {
                    txt.Clear();
                }
            }
        }

        private void ConfigMasked(KryptonMaskedTextBox txt)
        {
            // UX: cuando esté vacío, que NO se vean "_" ni prompts
            txt.PromptChar = ' ';
            txt.HidePromptOnLeave = true;

            // Limpieza consistente
            txt.ResetOnPrompt = true;
            txt.ResetOnSpace = true;
            txt.SkipLiterals = true;

            // Si copian/pegan o si haces Trim, te da solo caracteres reales
            txt.CutCopyMaskFormat = MaskFormat.ExcludePromptAndLiterals;

            // Opcional: evita sonidos
            txt.BeepOnError = false;
        }

        private void Masked_ClearIfIncomplete_OnLeave(object sender, EventArgs e)
        {
            if (sender is MaskedTextBox txt)
            {
                if (!txt.MaskCompleted)
                    txt.Clear();
            }
        }

        private void Masked_ClearOnHide(object sender, EventArgs e)
        {
            if (sender is MaskedTextBox txt && !txt.Visible)
                txt.Clear();
        }


    }
}

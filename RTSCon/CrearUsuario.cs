// ===== RTSCon/CrearUsuario.cs =====
using Krypton.Toolkit;
using RTSCon.Datos;
using RTSCon.Negocios;
using System;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace RTSCon
{
    public partial class CrearUsuario : KryptonForm
    {
        private readonly NAuth _auth;

        public CrearUsuario()
        {
            InitializeComponent();

            _auth = new NAuth(new DAuth(Conexion.CadenaConexion));

            // Documento
            var cmbDocumento = FindCtrl<ComboBox>("cmbDocumento", "cboDocumento", "cmbTipoDocumento");
            if (cmbDocumento != null)
            {
                cmbDocumento.DropDownStyle = ComboBoxStyle.DropDownList;
                if (cmbDocumento.Items.Count == 0)
                    cmbDocumento.Items.AddRange(new object[] { "Cedula", "RNC", "Pasaporte" });
                if (cmbDocumento.SelectedIndex < 0) cmbDocumento.SelectedIndex = 0;

                cmbDocumento.SelectedIndexChanged += (_, __) => AplicarTipoDocumento();
            }

            // Rol
            var cmbRol = FindCtrl<ComboBox>("cmbRol", "cboRol", "cmbRolUsuario");
            if (cmbRol != null && cmbRol.Items.Count == 0)
            {
                // Según tu requerimiento: SA crea Admin o Inquilino
                cmbRol.Items.Add("Admin");
                cmbRol.Items.Add("Inquilino");
                cmbRol.SelectedIndex = 0;
            }

            // Botones
            var btnCrear = FindCtrl<Control>("btnCrear", "btnConfirmar", "btnGuardar", "kryptonButtonCrear");
            if (btnCrear != null) btnCrear.Click += btnCrear_Click;

            var btnCancelar = FindCtrl<Control>("btnCancelar", "btnVolver", "btnBack", "kryptonButtonCancelar");
            if (btnCancelar != null) btnCancelar.Click += (_, __) => Close();

            // Estado inicial de documento
            this.Shown += (_, __) => AplicarTipoDocumento();
        }

        private void AplicarTipoDocumento()
        {
            var cmbDocumento = FindCtrl<ComboBox>("cmbDocumento", "cboDocumento", "cmbTipoDocumento");
            string tipo = Convert.ToString(cmbDocumento?.SelectedItem) ?? "";

            // Todos con prefijo txt (como pediste)
            var txtCedula = FindCtrl<TextBoxBase>("txtCedula");
            var txtRNC = FindCtrl<TextBoxBase>("txtRNC");
            var txtPasaporte = FindCtrl<TextBoxBase>("txtPasaporte");

            if (txtCedula != null) { txtCedula.Visible = false; txtCedula.Enabled = false; }
            if (txtRNC != null) { txtRNC.Visible = false; txtRNC.Enabled = false; }
            if (txtPasaporte != null) { txtPasaporte.Visible = false; txtPasaporte.Enabled = false; }

            if (tipo.Equals("Cedula", StringComparison.OrdinalIgnoreCase) || tipo.Equals("Cédula", StringComparison.OrdinalIgnoreCase))
            {
                if (txtCedula != null)
                {
                    txtCedula.Visible = true;
                    txtCedula.Enabled = true;
                    txtCedula.BringToFront();
                    txtCedula.Focus();
                }
            }
            else if (tipo.Equals("RNC", StringComparison.OrdinalIgnoreCase))
            {
                if (txtRNC != null)
                {
                    txtRNC.Visible = true;
                    txtRNC.Enabled = true;
                    txtRNC.BringToFront();
                    txtRNC.Focus();
                }
            }
            else // Pasaporte
            {
                if (txtPasaporte != null)
                {
                    txtPasaporte.Visible = true;
                    txtPasaporte.Enabled = true;
                    txtPasaporte.BringToFront();
                    txtPasaporte.Focus();
                }
            }
        }

        private void btnCrear_Click(object sender, EventArgs e)
        {
            try
            {
                // Controles (según tu UI)
                string nombreCompleto = (FindCtrl<TextBoxBase>("txtNombreCompleto")?.Text ?? "").Trim();
                string password = (FindCtrl<TextBoxBase>("txtPassword", "txtContrasena", "txtContraseña")?.Text ?? "").Trim();
                string correo = (FindCtrl<TextBoxBase>("txtCorreo")?.Text ?? "").Trim();

                var cmbRol = FindCtrl<ComboBox>("cmbRol", "cboRol", "cmbRolUsuario");
                string rol = Convert.ToString(cmbRol?.SelectedItem) ?? "";

                // Documento (solo validación visual; tu SP de Auth no recibe documento todavía)
                var cmbDocumento = FindCtrl<ComboBox>("cmbDocumento", "cboDocumento", "cmbTipoDocumento");
                string tipoDoc = Convert.ToString(cmbDocumento?.SelectedItem) ?? "";

                string documento = "";
                if (tipoDoc.Equals("Cedula", StringComparison.OrdinalIgnoreCase) || tipoDoc.Equals("Cédula", StringComparison.OrdinalIgnoreCase))
                    documento = (FindCtrl<TextBoxBase>("txtCedula")?.Text ?? "").Trim();
                else if (tipoDoc.Equals("RNC", StringComparison.OrdinalIgnoreCase))
                    documento = (FindCtrl<TextBoxBase>("txtRNC")?.Text ?? "").Trim();
                else
                    documento = (FindCtrl<TextBoxBase>("txtPasaporte")?.Text ?? "").Trim();

                // Validaciones mínimas
                if (string.IsNullOrWhiteSpace(nombreCompleto))
                    throw new InvalidOperationException("Ingrese el nombre completo.");
                if (string.IsNullOrWhiteSpace(password))
                    throw new InvalidOperationException("Ingrese la contraseña.");
                if (string.IsNullOrWhiteSpace(correo) || !correo.Contains("@"))
                    throw new InvalidOperationException("Ingrese un correo válido.");
                if (string.IsNullOrWhiteSpace(rol))
                    throw new InvalidOperationException("Seleccione el rol del usuario.");

                // Documento obligatorio (por tu pantalla)
                if (string.IsNullOrWhiteSpace(documento))
                    throw new InvalidOperationException("Ingrese el documento seleccionado.");

                // Creador (editor)
                string creador = UserContext.Usuario ??
                                 ConfigurationManager.AppSettings["DefaultEjecutor"] ??
                                 "rtscon@local";

                // IMPORTANTE: tu NAuth.CrearUsuario firma es:
                // CrearUsuario(string usuario, string correo, string rol, string password, string creador)
                _auth.CrearUsuario(nombreCompleto, correo, rol, password, creador);

                KryptonMessageBox.Show(this,
                    "Usuario creado correctamente.",
                    "Crear Usuario",
                    KryptonMessageBoxButtons.OK,
                    KryptonMessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                KryptonMessageBox.Show(this,
                    ex.Message,
                    "Crear Usuario",
                    KryptonMessageBoxButtons.OK,
                    KryptonMessageBoxIcon.Error);
            }
        }

        private T FindCtrl<T>(params string[] names) where T : Control
        {
            foreach (var n in names)
            {
                var c = this.Controls.Find(n, true).FirstOrDefault() as T;
                if (c != null) return c;
            }
            return null;
        }
    }
}

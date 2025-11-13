using Krypton.Toolkit;
using RTSCon.Negocios;
using RTSCon.Datos;
using System;
using System.Configuration;
using System.Windows.Forms;

namespace RTSCon.Catalogos
{
    public partial class CondominioConfirmarDesactivacion : KryptonForm
    {
        private readonly NAuth _auth;
        private readonly Func<string, bool> _onConfirm;   // callback que ejecuta la desactivación
        private readonly string _entidad;
        private readonly string _nombre;

        public CondominioConfirmarDesactivacion(string entidad, string nombreEntidad, Func<string, bool> onConfirm)
        {
            InitializeComponent();

            _entidad = entidad ?? "registro";
            _nombre = nombreEntidad ?? "";
            _onConfirm = onConfirm ?? throw new ArgumentNullException(nameof(onConfirm));

            var cn = ConfigurationManager.ConnectionStrings["RTSCond"].ConnectionString;
            _auth = new NAuth(new DAuth(cn));

            lblDesactivacion.Text =
                $"¿Está seguro que desea desactivar el {_entidad} \"{_nombre}\"?\n" +
                $"Escriba su contraseña para confirmar:";

            // Password UI
            txtPassword.UseSystemPasswordChar = true;
            txtPassword.MaxLength = 128;
            this.AcceptButton = btnConfirmar;

            // Eventos
            btnCancelar.Click += (_, __) => Close();
            btnConfirmar.Click += btnConfirmar_Click;
        }

        private void btnConfirmar_Click(object sender, EventArgs e)
        {
            try
            {
                if (UserContext.UsuarioAuthId <= 0)
                    throw new InvalidOperationException("Sesión no válida. Inicie sesión nuevamente.");

                var pwd = txtPassword.Text;
                if (string.IsNullOrWhiteSpace(pwd))
                    throw new InvalidOperationException("Ingrese su contraseña.");

                if (!_auth.ValidarPassword(UserContext.UsuarioAuthId, pwd))
                    throw new InvalidOperationException("Contraseña inválida.");

                var editor = UserContext.Usuario ?? (ConfigurationManager.AppSettings["DefaultEjecutor"] ?? "rtscon@local");

                // Ejecuta la operación enviada por el caller
                if (!_onConfirm(editor))
                    throw new InvalidOperationException("No se pudo completar la operación.");

                this.DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                KryptonMessageBox.Show(this, ex.Message, "Confirmar Desactivación",
                    KryptonMessageBoxButtons.OK, KryptonMessageBoxIcon.Error);
                txtPassword.Clear();
                txtPassword.Focus();
            }
        }
    }
}

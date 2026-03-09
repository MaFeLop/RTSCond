using Krypton.Toolkit;
using RTSCon.Negocios;
using System;
using System.Configuration;
using System.Windows.Forms;

namespace RTSCon
{
    public partial class ContrasenaOlvidada : KryptonForm
    {
        private readonly NAuth _auth;
        private int _usuarioAuthId;
        private bool _codigoValidado;

        public ContrasenaOlvidada()
        {
            InitializeComponent();
        }

        public ContrasenaOlvidada(NAuth auth) : this()
        {
            _auth = auth ?? throw new ArgumentNullException(nameof(auth));

            txtCodigo.Mask = "000000";
            txtCodigo.TextMaskFormat = MaskFormat.ExcludePromptAndLiterals;
            txtCodigo.PromptChar = ' ';

            _usuarioAuthId = 0;
            _codigoValidado = false;

            txtCodigo.Visible = false;
            kryptonLabel5.Visible = false;

            btnVerificar.Enabled = false;
            btnConfirmar.Enabled = false;

            txtContrasena.Enabled = false;
            txtContrasenaNueva.Enabled = false;

            txtCodigo.TextChanged += (s, e) =>
            {
                btnVerificar.Enabled = txtCodigo.MaskFull;
            };

            txtCodigo.Validating += (s, e) =>
            {
                if (!txtCodigo.MaskFull && !string.IsNullOrWhiteSpace(txtCodigo.Text))
                {
                    txtCodigo.Clear();
                    btnVerificar.Enabled = false;
                }
            };

            txtCodigo.MaskInputRejected += (s, e) =>
                System.Media.SystemSounds.Beep?.Play();
        }

        // ============================
        // ENVIAR CÓDIGO
        // ============================
        private void btnCodigo_Click(object sender, EventArgs e)
        {
            try
            {
                string usuario = (txtUsuario.Text ?? string.Empty).Trim();
                string correo = (txtCorreo.Text ?? string.Empty).Trim();

                if (string.IsNullOrWhiteSpace(usuario))
                    throw new InvalidOperationException("Ingrese el usuario.");

                if (string.IsNullOrWhiteSpace(correo))
                    throw new InvalidOperationException("Ingrese el correo.");

                _usuarioAuthId = _auth.ObtenerUsuarioAuthIdPorUsuarioYCorreo(usuario, correo);

                if (_usuarioAuthId <= 0)
                    throw new InvalidOperationException("No existe un usuario activo con ese usuario y correo.");

                string mailProfile = ConfigurationManager.AppSettings["MailProfile"];
                int minutosExpira = int.TryParse(ConfigurationManager.AppSettings["CodigoMinutos"], out var m) ? m : 5;
                bool debug = bool.TryParse(ConfigurationManager.AppSettings["CodigoDebug"], out var d) && d;

                _auth.EnviarCodigoLogin(_usuarioAuthId, mailProfile, minutosExpira, debug);

                _codigoValidado = false;

                txtCodigo.Visible = true;
                kryptonLabel5.Visible = true;
                txtCodigo.Clear();
                txtCodigo.Focus();

                txtContrasena.Enabled = false;
                txtContrasenaNueva.Enabled = false;
                btnConfirmar.Enabled = false;

                KryptonMessageBox.Show(
                    this,
                    $"Se envió un código al correo vinculado al usuario '{usuario}'.",
                    "Recuperación",
                    KryptonMessageBoxButtons.OK,
                    KryptonMessageBoxIcon.Information
                );
            }
            catch (Exception ex)
            {
                KryptonMessageBox.Show(
                    this,
                    ex.Message,
                    "Recuperación",
                    KryptonMessageBoxButtons.OK,
                    KryptonMessageBoxIcon.Error
                );
            }
        }

        // ============================
        // VERIFICAR CÓDIGO
        // ============================
        private void btnVerificar_Click(object sender, EventArgs e)
        {
            try
            {
                if (_usuarioAuthId <= 0)
                    throw new InvalidOperationException("Primero envíe el código.");

                string codigo = (txtCodigo.Text ?? string.Empty).Trim();

                if (codigo.Length != 6)
                    throw new InvalidOperationException("El código debe tener 6 dígitos.");

                int maxIntentos = int.TryParse(ConfigurationManager.AppSettings["MaxIntentos2FA"], out var i) ? i : 5;

                if (!_auth.VerificarCodigoLogin(_usuarioAuthId, codigo, maxIntentos))
                    throw new InvalidOperationException("Código inválido o expirado.");

                _codigoValidado = true;

                txtContrasena.Enabled = true;
                txtContrasenaNueva.Enabled = true;
                btnConfirmar.Enabled = true;

                txtContrasena.Focus();

                KryptonMessageBox.Show(
                    this,
                    "Código válido.",
                    "Verificación",
                    KryptonMessageBoxButtons.OK,
                    KryptonMessageBoxIcon.Information
                );
            }
            catch (Exception ex)
            {
                KryptonMessageBox.Show(
                    this,
                    ex.Message,
                    "Verificación",
                    KryptonMessageBoxButtons.OK,
                    KryptonMessageBoxIcon.Error
                );
            }
        }

        // ============================
        // CONFIRMAR CAMBIO
        // ============================
        private void btnConfirmar_Click(object sender, EventArgs e)
        {
            try
            {
                if (_usuarioAuthId <= 0)
                    throw new InvalidOperationException("Usuario inválido.");

                if (!_codigoValidado)
                    throw new InvalidOperationException("Primero valide el código.");

                string nueva = (txtContrasena.Text ?? string.Empty).Trim();
                string confirmar = (txtContrasenaNueva.Text ?? string.Empty).Trim();

                if (string.IsNullOrWhiteSpace(nueva))
                    throw new InvalidOperationException("Ingrese la nueva contraseña.");

                if (nueva != confirmar)
                    throw new InvalidOperationException("Las contraseñas no coinciden.");

                string editor = (txtUsuario.Text ?? string.Empty).Trim();

                _auth.CambiarPasswordPlain(_usuarioAuthId, nueva, editor);

                KryptonMessageBox.Show(
                    this,
                    "Contraseña actualizada correctamente.",
                    "Recuperación",
                    KryptonMessageBoxButtons.OK,
                    KryptonMessageBoxIcon.Information
                );

                Close();
            }
            catch (Exception ex)
            {
                KryptonMessageBox.Show(
                    this,
                    ex.Message,
                    "Recuperación",
                    KryptonMessageBoxButtons.OK,
                    KryptonMessageBoxIcon.Error
                );
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
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

        public ContrasenaOlvidada()
        {
            InitializeComponent();
        }

        public ContrasenaOlvidada(NAuth auth) : this()
        {
            _auth = auth ?? throw new ArgumentNullException(nameof(auth));

            // === Configuración MaskedTextBox ===
            txtCodigo.Mask = "000000";
            txtCodigo.TextMaskFormat = MaskFormat.ExcludePromptAndLiterals;
            txtCodigo.PromptChar = ' ';

            btnVerificar.Enabled = false;
            btnConfirmar.Enabled = false;

            txtCodigo.TextChanged += (s, e) =>
            {
                bool completo = txtCodigo.MaskFull;
                btnVerificar.Enabled = completo;
                btnConfirmar.Enabled = completo;
                this.AcceptButton = completo ? btnConfirmar : null;
            };

            txtCodigo.Validating += (s, e) =>
            {
                if (!txtCodigo.MaskFull && !string.IsNullOrWhiteSpace(txtCodigo.Text))
                {
                    txtCodigo.Clear();
                    btnVerificar.Enabled = false;
                    btnConfirmar.Enabled = false;
                    this.AcceptButton = null;
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
                string usuario = txtUsuario.Text.Trim();
                string correo = txtCorreo.Text.Trim();

                if (string.IsNullOrWhiteSpace(usuario))
                    throw new InvalidOperationException("Ingrese el usuario.");

                if (string.IsNullOrWhiteSpace(correo))
                    throw new InvalidOperationException("Ingrese el correo.");

                string mailProfile =
                    ConfigurationManager.AppSettings["MailProfile"] ?? "RTSCondMail";

                int minutosCodigo =
                    int.TryParse(ConfigurationManager.AppSettings["CodigoMinutos"], out var m)
                        ? m
                        : 5;

                bool debug =
                    string.Equals(
                        ConfigurationManager.AppSettings["CodigoDebug"],
                        "true",
                        StringComparison.OrdinalIgnoreCase
                    );

                _usuarioAuthId = _auth.EnviarCodigoRecuperacion(
                    usuario,
                    correo,
                    mailProfile,
                    minutosCodigo,
                    debug
                );

                KryptonMessageBox.Show(
                    this,
                    $"Se envió un código al correo vinculado al usuario '{usuario}'.",
                    "Recuperación",
                    KryptonMessageBoxButtons.OK,
                    KryptonMessageBoxIcon.Information
                );

                txtCodigo.Focus();
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

                string codigo = txtCodigo.Text.Trim();

                if (!_auth.Login_Codigo(_usuarioAuthId, codigo))
                    throw new InvalidOperationException("Código inválido o expirado.");

                KryptonMessageBox.Show(
                    this,
                    "Código válido.",
                    "Verificación",
                    KryptonMessageBoxButtons.OK,
                    KryptonMessageBoxIcon.Information
                );

                txtContrasena.Focus();
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
                    throw new InvalidOperationException("Primero envíe el código.");

                string codigo = txtCodigo.Text.Trim();
                string nueva = txtContrasena.Text;
                string confirmar = txtContrasenaNueva.Text;

                if (!txtCodigo.MaskFull)
                    throw new InvalidOperationException("Ingrese los 6 dígitos del código.");

                if (string.IsNullOrWhiteSpace(nueva))
                    throw new InvalidOperationException("Ingrese la nueva contraseña.");

                if (!string.Equals(nueva, confirmar))
                    throw new InvalidOperationException("La confirmación no coincide.");

                string editor =
                    ConfigurationManager.AppSettings["DefaultEjecutor"] ?? "rtscon@local";

                _auth.CambiarPasswordConCodigo(
                    _usuarioAuthId,
                    codigo,
                    nueva,
                    editor,
                    150000
                );

                KryptonMessageBox.Show(
                    this,
                    "La contraseña fue actualizada correctamente.",
                    "Recuperación",
                    KryptonMessageBoxButtons.OK,
                    KryptonMessageBoxIcon.Information
                );

                DialogResult = DialogResult.OK;
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

        private void btnCancelar_Click(object sender, EventArgs e) => Close();
    }
}

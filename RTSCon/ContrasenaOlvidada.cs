using Krypton.Toolkit;
using RTSCon.Negocios;
using System;
using System.Configuration;
using System.ComponentModel;
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

            // === UX del campo de código (KryptonMaskedTextBox) ===
            // Asegúrate de que txtCodigo sea KryptonMaskedTextBox en el diseñador.
            txtCodigo.Mask = "000000";                                  // 6 dígitos obligatorios
            txtCodigo.TextMaskFormat = MaskFormat.ExcludePromptAndLiterals; // .Text = solo números
            txtCodigo.PromptChar = ' ';                                 // sin guiones/underscores

            btnVerificar.Enabled = false;
            btnConfirmar.Enabled = false;

            // Habilitar/Deshabilitar botones según completitud del código
            txtCodigo.TextChanged += (s, e) =>
            {
                bool completo = txtCodigo.MaskFull;
                btnVerificar.Enabled = completo;
                btnConfirmar.Enabled = completo;
                this.AcceptButton = completo ? btnConfirmar : null; // Enter confirma si hay 6 dígitos
            };

            // Al salir del campo, si no está completo, borrar el contenido
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

            // Beep si intenta ingresar algo no válido
            txtCodigo.MaskInputRejected += (s, e) => System.Media.SystemSounds.Beep?.Play();
        }

        // === Enviar Código === (usa Usuario + Correo, devuelve y guarda _usuarioAuthId)
        private void btnCodigo_Click(object sender, EventArgs e)
        {
            try
            {
                var usuario = txtUsuario.Text.Trim();
                var correo = txtCorreo.Text.Trim();

                if (string.IsNullOrWhiteSpace(usuario))
                    throw new InvalidOperationException("Ingrese el usuario.");
                if (string.IsNullOrWhiteSpace(correo))
                    throw new InvalidOperationException("Ingrese el correo.");

                var mailProfile = ConfigurationManager.AppSettings["MailProfile"] ?? "RTSCondMail";
                var minutosCodigo = int.TryParse(ConfigurationManager.AppSettings["CodigoMinutos"], out var m) ? m : 5;
                var debug = string.Equals(ConfigurationManager.AppSettings["CodigoDebug"], "true", StringComparison.OrdinalIgnoreCase);

                _usuarioAuthId = _auth.EnviarCodigoRecuperacion(usuario, correo, mailProfile, minutosCodigo, debug);

                KryptonMessageBox.Show(
                    this,
                    $"Se envió un código de verificación al correo vinculado a '{usuario}'.",
                    "Recuperación de contraseña",
                    KryptonMessageBoxButtons.OK,
                    KryptonMessageBoxIcon.Information
                );

                txtCodigo.Focus();
            }
            catch (Exception ex)
            {
                KryptonMessageBox.Show(
                    this, ex.Message, "Recuperación de contraseña",
                    KryptonMessageBoxButtons.OK, KryptonMessageBoxIcon.Error
                );
            }
        }

        // === Verificar (opcional): solo informa si el código es válido ===
        private void btnVerificar_Click(object sender, EventArgs e)
        {
            try
            {
                if (_usuarioAuthId <= 0)
                    throw new InvalidOperationException("Primero envíe el código.");

                var codigo = txtCodigo.Text.Trim();

                if (_auth.Login_Codigo(_usuarioAuthId, codigo))
                {
                    KryptonMessageBox.Show(
                        this, "Código válido.", "Verificación",
                        KryptonMessageBoxButtons.OK, KryptonMessageBoxIcon.Information
                    );
                    txtContrasena.Focus();
                }
                else
                {
                    KryptonMessageBox.Show(
                        this, "Código inválido o expirado.", "Verificación",
                        KryptonMessageBoxButtons.OK, KryptonMessageBoxIcon.Error
                    );
                    txtCodigo.Clear();
                    btnVerificar.Enabled = false;
                    btnConfirmar.Enabled = false;
                    this.AcceptButton = null;
                    txtCodigo.Focus();
                }
            }
            catch (Exception ex)
            {
                KryptonMessageBox.Show(
                    this, ex.Message, "Verificación",
                    KryptonMessageBoxButtons.OK, KryptonMessageBoxIcon.Error
                );
            }
        }

        // === Confirmar: valida código + cambia la contraseña ===
        private void btnConfirmar_Click(object sender, EventArgs e)
        {
            try
            {
                if (_usuarioAuthId <= 0)
                    throw new InvalidOperationException("Primero envíe el código al correo vinculado.");

                var codigo = txtCodigo.Text.Trim();
                var nueva = txtContrasena.Text;
                var confirmar = txtContrasenaNueva.Text;

                if (!txtCodigo.MaskFull || string.IsNullOrWhiteSpace(codigo))
                    throw new InvalidOperationException("Ingrese los 6 dígitos del código.");
                if (string.IsNullOrWhiteSpace(nueva))
                    throw new InvalidOperationException("Ingrese la nueva contraseña.");
                if (!string.Equals(nueva, confirmar))
                    throw new InvalidOperationException("La confirmación no coincide.");

                var editor = ConfigurationManager.AppSettings["DefaultEjecutor"] ?? "rtscon@local";

                _auth.CambiarPasswordConCodigo(_usuarioAuthId, codigo, nueva, editor, 150000);

                KryptonMessageBox.Show(
                    this,
                    "Tu contraseña fue actualizada. Ya puedes iniciar sesión.",
                    "Recuperación de contraseña",
                    KryptonMessageBoxButtons.OK,
                    KryptonMessageBoxIcon.Information
                );

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                KryptonMessageBox.Show(
                    this, ex.Message, "Recuperación de contraseña",
                    KryptonMessageBoxButtons.OK, KryptonMessageBoxIcon.Error
                );
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e) => Close();

        // Handlers generados por el diseñador (puedes dejarlos vacíos si no los usas)
        private void txtCodigo_MaskInputRejected(object sender, MaskInputRejectedEventArgs e) { /* handled arriba */ }
        private void txtCodigo_KeyPress(object sender, KeyPressEventArgs e) { /* no se usa con Mask */ }
    }
}

using Krypton.Toolkit;
using RTSCon.Negocios;
using System;
using System.Configuration;
using System.Windows.Forms;

namespace RTSCon
{
    public partial class LoginCodigo : KryptonForm
    {
        private readonly NAuth _auth;
        private readonly int _usuarioAuthId;

        private Timer _timer;
        private int _secondsLeft;

        public LoginCodigo(NAuth auth, int usuarioAuthId)
        {
            InitializeComponent();
            _auth = auth ?? throw new ArgumentNullException(nameof(auth));
            _usuarioAuthId = usuarioAuthId;

            // Texto con correo enmascarado
            try
            {
                lblInfo.Text = "Hemos enviado un código a su correo vinculado.";

            }
            catch { lblInfo.Text = "Hemos enviado un código a su correo vinculado."; }

            // Campo de código (KryptonMaskedTextBox)
            txtCodigo.Mask = "000000";
            txtCodigo.TextMaskFormat = MaskFormat.ExcludePromptAndLiterals;
            txtCodigo.PromptChar = ' ';

            btnConfirm.Enabled = false;
            btnReenviar.Enabled = false;

            txtCodigo.TextChanged += (s, e) =>
            {
                bool full = txtCodigo.MaskFull;
                btnConfirm.Enabled = full;
                this.AcceptButton = full ? btnConfirm : null;
            };

            txtCodigo.Validating += (s, e) =>
            {
                if (!txtCodigo.MaskFull && !string.IsNullOrWhiteSpace(txtCodigo.Text))
                {
                    txtCodigo.Clear();
                    btnConfirm.Enabled = false;
                    this.AcceptButton = null;
                }
            };
            txtCodigo.MaskInputRejected += (s, e) => System.Media.SystemSounds.Beep?.Play();
            this.Shown += (s, e) => txtCodigo.Focus();

            // Cuenta regresiva para permitir reenvío
            int minutosCodigo = int.TryParse(ConfigurationManager.AppSettings["CodigoMinutos"], out var m) ? m : 5;
            ResetResendTimer(minutosCodigo);

            // Wire events (por si el diseñador no los conectó)
            btnConfirm.Click += btnConfirm_Click;
            btnReenviar.Click += btnReenviar_Click;
        }

        private void ResetResendTimer(int minutes)
        {
            _secondsLeft = Math.Max(1, minutes) * 60;
            lblCountdown.Text = TimeSpan.FromSeconds(_secondsLeft).ToString(@"mm\:ss");
            btnReenviar.Enabled = false;

            _timer?.Stop();
            _timer = _timer ?? new Timer { Interval = 1000 };
            _timer.Tick -= Timer_Tick;
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (_secondsLeft > 0)
            {
                _secondsLeft--;
                lblCountdown.Text = TimeSpan.FromSeconds(_secondsLeft).ToString(@"mm\:ss");
                if (_secondsLeft == 0)
                {
                    _timer.Stop();
                    btnReenviar.Enabled = true;
                    lblCountdown.Text = "Puedes reenviar el código.";
                }
            }
        }

        private void btnReenviar_Click(object sender, EventArgs e)
        {
            try
            {
                var mailProfile = ConfigurationManager.AppSettings["MailProfile"] ?? "RTSCondMail";
                var minutosCodigo = int.TryParse(ConfigurationManager.AppSettings["CodigoMinutos"], out var m) ? m : 5;
                var debug = string.Equals(ConfigurationManager.AppSettings["CodigoDebug"], "true", StringComparison.OrdinalIgnoreCase);

                _auth.ReenviarCodigo(_usuarioAuthId, mailProfile, minutosCodigo, debug);

                KryptonMessageBox.Show(this, "Se envió un nuevo código.",
                    "Verificación 2FA", KryptonMessageBoxButtons.OK, KryptonMessageBoxIcon.Information);

                ResetResendTimer(minutosCodigo);
                txtCodigo.Clear();
                txtCodigo.Focus();
            }
            catch (Exception ex)
            {
                KryptonMessageBox.Show(this, ex.Message, "Verificación 2FA",
                    KryptonMessageBoxButtons.OK, KryptonMessageBoxIcon.Error);
            }
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            try
            {
                var codigo = txtCodigo.Text.Trim();
                if (!txtCodigo.MaskFull || string.IsNullOrWhiteSpace(codigo))
                    throw new InvalidOperationException("Ingrese los 6 dígitos del código.");

                if (_auth.Login_CodigoYSesion(_usuarioAuthId, codigo))
                {
                    DialogResult = DialogResult.OK;
                    Close();
                }
                else
                {
                    KryptonMessageBox.Show(this, "Código inválido o expirado.",
                        "Verificación 2FA", KryptonMessageBoxButtons.OK, KryptonMessageBoxIcon.Error);

                    txtCodigo.Clear();
                    btnConfirm.Enabled = false;
                    this.AcceptButton = null;
                    txtCodigo.Focus();
                }
            }
            catch (Exception ex)
            {
                KryptonMessageBox.Show(this, ex.Message, "Verificación 2FA",
                    KryptonMessageBoxButtons.OK, KryptonMessageBoxIcon.Error);
            }
        }

        private void btnReenviar_Click_1(object sender, EventArgs e)
        {
            btnReenviar.Enabled = false;
            try
            {
                var mailProfile = ConfigurationManager.AppSettings["MailProfile"] ?? "RTSCondMail";
                var minutosCodigo = int.TryParse(ConfigurationManager.AppSettings["CodigoMinutos"], out var m) ? m : 5;
                var debug = string.Equals(ConfigurationManager.AppSettings["CodigoDebug"], "true", StringComparison.OrdinalIgnoreCase);

                _auth.ReenviarCodigo(_usuarioAuthId, mailProfile, minutosCodigo, debug);

                KryptonMessageBox.Show(this, "Se envió un nuevo código.",
                    "Verificación 2FA", KryptonMessageBoxButtons.OK, KryptonMessageBoxIcon.Information);

                ResetResendTimer(minutosCodigo);
                txtCodigo.Clear();
                txtCodigo.Focus();
            }
            catch (Exception ex)
            {
                KryptonMessageBox.Show(this, ex.Message, "Verificación 2FA",
                    KryptonMessageBoxButtons.OK, KryptonMessageBoxIcon.Error);
            }
            finally
            {
                // El timer decidirá cuándo habilitar
                if (_secondsLeft == 0)
                    btnReenviar.Enabled = true;
            }
        }

        private void btnConfirm_Click_1(object sender, EventArgs e)
        {
            if (!txtCodigo.MaskFull)
            {
                KryptonMessageBox.Show(this, "Ingrese los 6 dígitos del código.",
                    "Verificación 2FA", KryptonMessageBoxButtons.OK, KryptonMessageBoxIcon.Information);
                return;
            }

            btnConfirm.Enabled = false;
            btnReenviar.Enabled = false;
            try
            {
                var codigo = txtCodigo.Text.Trim();

                // NAuth.Login_Codigo devuelve bool. El SP ahora puede devolver -1/-2 con RAISERROR,
                // que en DAL/NAuth se propagarán como Exception (si no las atrapas).
                // Si tu NAuth atrapa y devuelve false, mostramos un mensaje genérico ampliado:
                if (_auth.Login_CodigoYSesion(_usuarioAuthId, codigo))
                {
                    DialogResult = DialogResult.OK;
                    Close();
                }
                else
                {
                    KryptonMessageBox.Show(this,
                        "Código inválido, expirado o se alcanzó el máximo de intentos.\n" +
                        "Puedes esperar y usar 'Reenviar código'.",
                        "Verificación 2FA",
                        KryptonMessageBoxButtons.OK,
                        KryptonMessageBoxIcon.Error);

                    txtCodigo.Clear();
                    this.AcceptButton = null;
                    txtCodigo.Focus();
                }
            }
            catch (Exception ex)
            {
                // Si el DAL propaga mensajes específicos del SP (expirado / demasiados intentos), se verán aquí.
                KryptonMessageBox.Show(this, ex.Message, "Verificación 2FA",
                    KryptonMessageBoxButtons.OK, KryptonMessageBoxIcon.Error);
                txtCodigo.Clear();
                this.AcceptButton = null;
                txtCodigo.Focus();
            }
            finally
            {
                btnConfirm.Enabled = txtCodigo.MaskFull;
                // Reenviar depende del timer; no lo habilitamos manualmente aquí.
            }
        }
    }
}

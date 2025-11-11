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
                var masked = _auth.CorreoEnmascarado(_usuarioAuthId);
                lblInfo.Text = string.IsNullOrWhiteSpace(masked)
                    ? "Hemos enviado un código a su correo vinculado."
                    : $"Hemos enviado un código a {masked}";
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

                if (_auth.Login_Codigo(_usuarioAuthId, codigo))
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
    }
}

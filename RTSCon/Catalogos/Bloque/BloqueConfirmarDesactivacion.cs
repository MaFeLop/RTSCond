using System;
using System.Windows.Forms;

namespace RTSCon.Catalogos
{
    public partial class BloqueConfirmarDesactivacion : Form
    {
        private bool _eventosInicializados;

        public BloqueConfirmarDesactivacion(string mensaje)
        {
            InitializeComponent();
            lblDesactivacion.Text = mensaje;
            InicializarEventosUnaSolaVez();
        }

        public string Password
        {
            get { return txtPassword.Text != null ? txtPassword.Text.Trim() : string.Empty; }
        }

        private void InicializarEventosUnaSolaVez()
        {
            if (_eventosInicializados)
                return;

            if (btnConfirmar != null)
            {
                btnConfirmar.Click -= btnConfirmar_Click;
                btnConfirmar.Click += btnConfirmar_Click;
            }

            if (btnCancelar != null)
            {
                btnCancelar.Click -= btnCancelar_Click;
                btnCancelar.Click += btnCancelar_Click;
            }

            _eventosInicializados = true;
        }

        private void btnConfirmar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show(
                    "Debes introducir tu contraseña.",
                    "Aviso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
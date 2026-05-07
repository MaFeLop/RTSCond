using System;
using System.Windows.Forms;

namespace RTSCon
{
    public partial class FrmSesionExpirada : Form
    {
        private readonly string _mensaje;

        public FrmSesionExpirada()
            : this("Su sesión fue expirada por inactividad. Por favor vuelva a iniciar sesión.")
        {
        }

        public FrmSesionExpirada(string mensaje)
        {
            _mensaje = string.IsNullOrWhiteSpace(mensaje)
                ? "Su sesión fue expirada por inactividad. Por favor vuelva a iniciar sesión."
                : mensaje;

            InitializeComponent();
            ConfigurarFormulario();
            ConfigurarEventos();
        }

        private void ConfigurarFormulario()
        {
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ShowInTaskbar = false;

            if (kryptonLabel1 != null)
                kryptonLabel1.Values.Text = _mensaje;
        }

        private void ConfigurarEventos()
        {
            if (btnVolver != null)
            {
                btnVolver.Click -= btnVolver_Click;
                btnVolver.Click += btnVolver_Click;
            }
        }

        private void btnVolver_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
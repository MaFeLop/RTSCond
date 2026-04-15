using System;
using System.Windows.Forms;
using RTSCon.Catalogos;
using RTSCon.Catalogos.Condominio;

namespace RTSCon
{
    public partial class CatalogoCRUD : Form
    {
        private bool _abriendoChild;
        private bool _eventosInicializados;

        public CatalogoCRUD()
        {
            InitializeComponent();
            InicializarEventosUnaSolaVez();
        }

        private void InicializarEventosUnaSolaVez()
        {
            if (_eventosInicializados)
                return;

            btnCondominio.Click -= btnCondominios_Click;
            btnCondominio.Click += btnCondominios_Click;

            btnBloques.Click -= btnBloques_Click;
            btnBloques.Click += btnBloques_Click;

            btnUnidades.Click -= btnUnidades_Click;
            btnUnidades.Click += btnUnidades_Click;

            btnPropiedad.Click -= btnPropiedad_Click;
            btnPropiedad.Click += btnPropiedad_Click;

            btnVolver.Click -= btnVolver_Click;
            btnVolver.Click += btnVolver_Click;

            _eventosInicializados = true;
        }

        private void AbrirChild(Form child)
        {
            if (child == null)
                return;

            if (_abriendoChild)
            {
                child.Dispose();
                return;
            }

            try
            {
                _abriendoChild = true;

                this.Hide();

                using (child)
                {
                    child.StartPosition = FormStartPosition.CenterScreen;
                    child.ShowInTaskbar = false;
                    child.ShowDialog(this);
                }
            }
            finally
            {
                _abriendoChild = false;

                if (!SessionHelper.IsLoggingOut && !this.IsDisposed)
                {
                    try
                    {
                        this.Show();
                        this.Activate();
                        this.BringToFront();
                    }
                    catch
                    {
                        // No romper navegación
                    }
                }
            }
        }

        private void btnCondominios_Click(object sender, EventArgs e)
        {
            AbrirChild(new CondominioRead());
        }

        private void btnBloques_Click(object sender, EventArgs e)
        {
            AbrirChild(new BloqueRead());
        }

        private void btnUnidades_Click(object sender, EventArgs e)
        {
            AbrirChild(new UnidadRead());
        }

        private void btnPropiedad_Click(object sender, EventArgs e)
        {
            AbrirChild(new PropiedadRead());
        }

        private void btnVolver_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
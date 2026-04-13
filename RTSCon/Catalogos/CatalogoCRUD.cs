using System;
using System.Windows.Forms;
using RTSCon.Catalogos;
using RTSCon.Catalogos.Condominio;

namespace RTSCon
{
    public partial class CatalogoCRUD : Form
    {
        private Form _childActual;
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
                return;

            if (_childActual != null && !_childActual.IsDisposed)
            {
                try
                {
                    _childActual.Activate();
                    _childActual.BringToFront();
                }
                catch
                {
                    // ignorar; evita romper navegación visual
                }
                return;
            }

            try
            {
                _abriendoChild = true;
                _childActual = child;

                this.Enabled = false;
                this.Hide();

                child.FormClosed -= Child_FormClosed;
                child.FormClosed += Child_FormClosed;

                child.Show();
            }
            finally
            {
                _abriendoChild = false;
            }
        }

        private void Child_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (sender is Form frm)
                frm.FormClosed -= Child_FormClosed;

            _childActual = null;

            if (SessionHelper.IsLoggingOut)
                return;

            if (this.IsDisposed)
                return;

            try
            {
                this.Enabled = true;
                this.Show();
                this.Activate();
                this.BringToFront();
            }
            catch
            {
                // no romper el flujo del hub
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
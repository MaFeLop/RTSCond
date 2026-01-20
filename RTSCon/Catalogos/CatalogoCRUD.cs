using System;
using System.Windows.Forms;
using RTSCon.Catalogos;
using RTSCon.Catalogos.Condominio;

namespace RTSCon
{
    public partial class CatalogoCRUD : Form
    {
        private Form _childActual;

        public CatalogoCRUD()
        {
            InitializeComponent();
        }

        private void AbrirChild(Form child)
        {
            if (child == null) return;

            _childActual = child;

            // Ocultar el hub mientras el child está abierto
            this.Hide();

            child.FormClosed += (s, e) =>
            {
                _childActual = null;

                // Si estamos cerrando sesión, NO re-mostrar el hub
                if (SessionHelper.IsLoggingOut) return;

                // Si el hub ya se cerró/dispuso, no intentes Show/Activate
                if (this.IsDisposed) return;
                if (!this.IsHandleCreated) return;

                try
                {
                    this.BeginInvoke((Action)(() =>
                    {
                        if (this.IsDisposed) return;
                        this.Show();
                        this.Activate();
                    }));
                }
                catch { }
            };

            child.Show();
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
            // Volver al Dashboard sin duplicar instancias es mejor,
            // pero por ahora, simple:
            this.Close();
        }
    }
}

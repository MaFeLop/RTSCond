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
            // Cerrar el hijo actual si existe
            if (_childActual != null && !_childActual.IsDisposed)
            {
                _childActual.Close();
                _childActual = null;
            }

            // Ocultar el hub para que visualmente “desaparezca”
            this.Hide();

            _childActual = child;

            // Cuando el child se cierre (botón Volver),
            // volvemos a mostrar el hub.
            child.FormClosed += (s, e) =>
            {
                _childActual = null;
                this.Show();
                this.Activate();
            };

            child.Show();
        }

        // Ejemplo: botón de Condominios
        private void btnCondominios_Click(object sender, EventArgs e)
        {
            var frm = new CondominioRead();  // usa tu ctor real
            AbrirChild(frm);
        }

        // Ejemplo: botón de Bloques
        private void btnBloques_Click(object sender, EventArgs e)
        {
            var frm = new BloqueRead();   // ahora existe ctor sin parámetros
            AbrirChild(frm);
        }

        // Ejemplo: botón de Unidades
        private void btnUnidades_Click(object sender, EventArgs e)
        {
            var frm = new UnidadRead(/* si tu ctor recibe BloqueId, lo pedimos dentro de UnidadRead usando buscarBloque */);
            AbrirChild(frm);
        }

        private void btnPropiedad_Click(object sender, EventArgs e)
        {
            var frm = new PropiedadRead();
            AbrirChild(frm);
        }

        private void btnVolver_Click(object sender, EventArgs e)
        {
            var frm = new Dashboard();
            AbrirChild(frm);
            this.Close();
        }
    }
}

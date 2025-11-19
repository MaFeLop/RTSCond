using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RTSCon.Catalogos
{
    public partial class BloqueConfirmarDesactivacion : Form
    {
        public BloqueConfirmarDesactivacion(string mensaje)
        {
            InitializeComponent();
            lblDesactivacion.Text = mensaje;   // usa el label que tengas en el diseño
        }

        // ⬇️ Esto resuelve el error de Password también
        public string Password => txtPassword.Text;  // usa el nombre real de tu TextBox de contraseña

        private void btnConfirmar_Click(object sender, EventArgs e)
        {
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

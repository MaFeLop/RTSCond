using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Krypton.Toolkit;
using RTSCon.Negocios;
using RTSCon.Datos;

namespace RTSCon
{
    public partial class CrearUsuario : KryptonForm
    {
        private readonly NAuth _auth;

        public CrearUsuario()
        {
            InitializeComponent();
            _auth = new NAuth(new DAuth("<CADENA_SQL>"));

            // Rellenar roles: SA crea Admin o Inquilino
            cmbRol.Items.Clear();
            cmbRol.Items.Add("Admin");
            cmbRol.Items.Add("Inquilino");
            cmbRol.SelectedIndex = 0;
        }

        private void kryptonLabel4_Click(object sender, EventArgs e)
        {

        }

        private void kryptonLabel5_Click(object sender, EventArgs e)
        {

        }

        private void kryptonButton1_Click(object sender, EventArgs e)
        {

        }

        private void kryptonMaskedTextBox1_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
        {

        }
    }
}

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
            // En el constructor de CrearUsuario(), después de InitializeComponent()
            cmbDocumento.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbDocumento.SelectedIndexChanged += cmbTipoDocumento_SelectedIndexChanged;

            // Llama esto al final del constructor o en Shown:
            cmbTipoDocumento_SelectedIndexChanged(this, EventArgs.Empty);

private void cmbTipoDocumento_SelectedIndexChanged(object sender, EventArgs e)
        {
            // EJEMPLO: controles (ajusta a tus nombres)
            // - maskedCedula  ###-#######-#
            // - maskedRnc     ###-#####-#
            // - txtPasaporte  TextBox normal
            // - lblDocumento  label de documento

            var tipo = Convert.ToString(cmbDocumento.SelectedItem) ?? "";

            // Apaga todos
            txtCedula.Visible = false;
            txtRNC.Visible = false;
            txtPasaporte.Visible = false;

            txtCedula.Enabled = false;
            txtRNC.Enabled = false;
            txtPasaporte.Enabled = false;

            // Enciende el correspondiente
            if (tipo.Equals("Cedula", StringComparison.OrdinalIgnoreCase) ||
                tipo.Equals("Cédula", StringComparison.OrdinalIgnoreCase))
            {
                txtCedula.Visible = true;
                txtCedula.Enabled = true;
                txtCedula.BringToFront();
                txtCedula.Focus();
            }
            else if (tipo.Equals("RNC", StringComparison.OrdinalIgnoreCase))
            {
                txtRNC.Visible = true;
                txtRNC.Enabled = true;
                txtRNC.BringToFront();
                txtRNC.Focus();
            }
            else // Pasaporte
            {
                txtPasaporte.Visible = true;
                txtPasaporte.Enabled = true;
                txtPasaporte.BringToFront();
                txtPasaporte.Focus();

                // Rellenar roles: SA crea Admin o Inquilino
                cmbRol.Items.Clear();
                cmbRol.Items.Add("Admin");
                cmbRol.Items.Add("Inquilino");
                cmbRol.SelectedIndex = 0;
            }
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

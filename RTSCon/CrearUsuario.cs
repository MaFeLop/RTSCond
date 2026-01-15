using Krypton.Toolkit;
using RTSCon.Negocios;
using RTSCon.Datos;
using System;
using System.Windows.Forms;

namespace RTSCon
{
    public partial class CrearUsuario : KryptonForm
    {
        private readonly NAuth _auth;

        public CrearUsuario()
        {
            InitializeComponent();

            // TODO: cambia esto por tu cadena real (ideal: ConfigurationManager.ConnectionStrings["RTSCond"].ConnectionString)
            _auth = new NAuth(new DAuth("<CADENA_SQL>"));

            // Documento: solo lista, sin escritura
            cmbDocumento.DropDownStyle = ComboBoxStyle.DropDownList;

            // (Opcional) si aún no cargaste Items en diseñador, asegúralos aquí:
            if (cmbDocumento.Items.Count == 0)
            {
                cmbDocumento.Items.Add("Cedula");
                cmbDocumento.Items.Add("RNC");
                cmbDocumento.Items.Add("Pasaporte");
            }

            // Hook eventos
            cmbDocumento.SelectedIndexChanged -= cmbDocumento_SelectedIndexChanged;
            cmbDocumento.SelectedIndexChanged += cmbDocumento_SelectedIndexChanged;

            // Estado inicial cuando ya el form esté listo (evita glitches visuales)
            this.Shown += (_, __) =>
            {
                if (cmbDocumento.SelectedIndex < 0) cmbDocumento.SelectedIndex = 0;
                AplicarTipoDocumento();
            };

            // Roles: NO debe depender del documento (esto lo dejamos fijo por ahora)
            if (cmbRol.Items.Count == 0)
            {
                cmbRol.Items.Add("Admin");
                cmbRol.Items.Add("Inquilino");
            }
            if (cmbRol.SelectedIndex < 0) cmbRol.SelectedIndex = 0;

            // Asegurar que todos empiezan ocultos (Shown los enciende bien)
            ApagarDocumentoInputs();
        }

        private void cmbDocumento_SelectedIndexChanged(object sender, EventArgs e)
        {
            AplicarTipoDocumento();
        }

        private void AplicarTipoDocumento()
        {
            var tipo = Convert.ToString(cmbDocumento.SelectedItem) ?? "";

            ApagarDocumentoInputs();

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
            }
        }

        private void ApagarDocumentoInputs()
        {
            // IMPORTANTE: todos tienen prefijo txt (mask o textbox)
            txtCedula.Visible = false;
            txtRNC.Visible = false;
            txtPasaporte.Visible = false;

            txtCedula.Enabled = false;
            txtRNC.Enabled = false;
            txtPasaporte.Enabled = false;
        }
    }
}

using System;
using System.Data;
using System.Windows.Forms;
using RTSCon.Datos;
using RTSCon.Negocios;

namespace RTSCon.Catalogos
{
    public partial class UpdateBloque : Form
    {
        private readonly int _id;
        private int _condominioId;
        private byte[] _rowVersion;

        private readonly NBloque _nBloque;
        private bool _eventosInicializados;

        public UpdateBloque(int id)
        {
            InitializeComponent();

            _id = id;

            var dBloque = new DBloque(Conexion.CadenaConexion);
            _nBloque = new NBloque(dBloque);

            InicializarEventosUnaSolaVez();
        }

        private void InicializarEventosUnaSolaVez()
        {
            if (_eventosInicializados)
                return;

            this.Load -= UpdateBloque_Load;
            this.Load += UpdateBloque_Load;

            if (btnGuardar != null)
            {
                btnGuardar.Click -= btnGuardar_Click;
                btnGuardar.Click += btnGuardar_Click;
            }

            if (btnCancelar != null)
            {
                btnCancelar.Click -= btnCancelar_Click;
                btnCancelar.Click += btnCancelar_Click;
            }

            if (btnBuscarCondominio != null)
            {
                btnBuscarCondominio.Click -= btnBuscarCondominio_Click;
                btnBuscarCondominio.Click += btnBuscarCondominio_Click;
            }

            _eventosInicializados = true;
        }

        private void UpdateBloque_Load(object sender, EventArgs e)
        {
            ConfigurarUi();
            CargarDatos();
        }

        private void ConfigurarUi()
        {
            if (txtIdCondominio != null)
                txtIdCondominio.ReadOnly = true;

            if (nudNumPisos != null)
            {
                nudNumPisos.Minimum = 1;
                if (nudNumPisos.Value < 1)
                    nudNumPisos.Value = 1;
            }

            if (nudUnidadesPiso != null)
            {
                nudUnidadesPiso.Minimum = 1;
                if (nudUnidadesPiso.Value < 1)
                    nudUnidadesPiso.Value = 1;
            }

            if (btnBuscarCondominio != null)
                btnBuscarCondominio.Enabled = false;
        }

        private void CargarDatos()
        {
            DataRow row = _nBloque.PorId(_id);
            if (row == null)
            {
                MessageBox.Show(
                    "No se encontró el bloque.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                DialogResult = DialogResult.Cancel;
                Close();
                return;
            }

            if (row.Table.Columns.Contains("CondominioId") && row["CondominioId"] != DBNull.Value)
                _condominioId = Convert.ToInt32(row["CondominioId"]);

            if (row.Table.Columns.Contains("RowVersion") && row["RowVersion"] != DBNull.Value)
                _rowVersion = (byte[])row["RowVersion"];

            if (txtIdCondominio != null)
                txtIdCondominio.Text = _condominioId > 0 ? _condominioId.ToString() : string.Empty;

            if (row.Table.Columns.Contains("Identificador") && row["Identificador"] != DBNull.Value)
                txtIdentificador.Text = Convert.ToString(row["Identificador"]);
            else
                txtIdentificador.Text = string.Empty;

            if (row.Table.Columns.Contains("NumeroPisos") && row["NumeroPisos"] != DBNull.Value)
            {
                decimal valor = Convert.ToDecimal(row["NumeroPisos"]);
                if (valor < nudNumPisos.Minimum)
                    valor = nudNumPisos.Minimum;
                if (valor > nudNumPisos.Maximum)
                    valor = nudNumPisos.Maximum;

                nudNumPisos.Value = valor;
            }

            if (row.Table.Columns.Contains("UnidadesPorPiso") && row["UnidadesPorPiso"] != DBNull.Value)
            {
                decimal valor = Convert.ToDecimal(row["UnidadesPorPiso"]);
                if (valor < nudUnidadesPiso.Minimum)
                    valor = nudUnidadesPiso.Minimum;
                if (valor > nudUnidadesPiso.Maximum)
                    valor = nudUnidadesPiso.Maximum;

                nudUnidadesPiso.Value = valor;
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                string identificador = txtIdentificador.Text.Trim();
                if (string.IsNullOrWhiteSpace(identificador))
                {
                    MessageBox.Show(
                        "Debe ingresar el identificador del bloque.",
                        "Validación",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    txtIdentificador.Focus();
                    return;
                }

                if (identificador.Length > 50)
                {
                    MessageBox.Show(
                        "El identificador no puede exceder 50 caracteres.",
                        "Validación",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    txtIdentificador.Focus();
                    return;
                }

                int numeroPisos = Decimal.ToInt32(nudNumPisos.Value);
                int unidadesPorPiso = Decimal.ToInt32(nudUnidadesPiso.Value);

                if (numeroPisos <= 0)
                {
                    MessageBox.Show(
                        "El número de pisos debe ser mayor que 0.",
                        "Validación",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    nudNumPisos.Focus();
                    return;
                }

                if (unidadesPorPiso <= 0)
                {
                    MessageBox.Show(
                        "Las unidades por piso deben ser mayores que 0.",
                        "Validación",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    nudUnidadesPiso.Focus();
                    return;
                }

                if (_rowVersion == null || _rowVersion.Length == 0)
                    throw new InvalidOperationException("No se pudo recuperar la RowVersion del bloque.");

                string usuario = UserContext.Usuario;
                if (string.IsNullOrWhiteSpace(usuario))
                    usuario = "rtscon@local";

                _nBloque.Actualizar(
                    _id,
                    identificador,
                    numeroPisos,
                    unidadesPorPiso,
                    _rowVersion,
                    usuario);

                MessageBox.Show(
                    "Bloque actualizado correctamente.",
                    "Éxito",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No se pudo actualizar el bloque: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnBuscarCondominio_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "El condominio no se puede cambiar desde esta pantalla por el momento.",
                "Información",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
    }
}
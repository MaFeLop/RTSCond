using System;
using System.Data;
using System.Globalization;
using System.Windows.Forms;
using RTSCon.Datos;
using RTSCon.Negocios;

namespace RTSCon.Catalogos
{
    public partial class UpdateUnidad : Form
    {
        private readonly int _id;
        private int _bloqueId;
        private byte[] _rowVersion;

        private readonly NUnidad _nUnidad;
        private readonly NBloque _nBloque;
        private bool _eventosInicializados;

        public UpdateUnidad(int id)
        {
            InitializeComponent();

            _id = id;

            var dUnidad = new DUnidad(Conexion.CadenaConexion);
            _nUnidad = new NUnidad(dUnidad);

            var dBloque = new DBloque(Conexion.CadenaConexion);
            _nBloque = new NBloque(dBloque);

            InicializarEventosUnaSolaVez();
        }

        private void InicializarEventosUnaSolaVez()
        {
            if (_eventosInicializados)
                return;

            this.Load -= UpdateUnidad_Load;
            this.Load += UpdateUnidad_Load;

            if (btnConfirmar != null)
            {
                btnConfirmar.Click -= btnConfirmar_Click;
                btnConfirmar.Click += btnConfirmar_Click;
            }

            if (btnVolver != null)
            {
                btnVolver.Click -= btnVolver_Click;
                btnVolver.Click += btnVolver_Click;
            }

            if (chkAmueblada != null)
            {
                chkAmueblada.CheckedChanged -= chkAmueblada_CheckedChanged;
                chkAmueblada.CheckedChanged += chkAmueblada_CheckedChanged;
            }

            if (btnBuscarBloque != null)
            {
                btnBuscarBloque.Click -= btnBuscarBloque_Click;
                btnBuscarBloque.Click += btnBuscarBloque_Click;
            }

            _eventosInicializados = true;
        }

        private void UpdateUnidad_Load(object sender, EventArgs e)
        {
            ConfigurarUi();
            CargarDatos();
        }

        private void ConfigurarUi()
        {
            if (txtBloqueSeleccionado != null)
                txtBloqueSeleccionado.ReadOnly = true;

            if (btnBuscarBloque != null)
                btnBuscarBloque.Enabled = false;
        }

        private void CargarDatos()
        {
            DataRow row = _nUnidad.PorId(_id);
            if (row == null)
            {
                MessageBox.Show(
                    "No se encontró la unidad.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                DialogResult = DialogResult.Cancel;
                Close();
                return;
            }

            if (row.Table.Columns.Contains("BloqueId") && row["BloqueId"] != DBNull.Value)
                _bloqueId = Convert.ToInt32(row["BloqueId"]);

            if (row.Table.Columns.Contains("RowVersion") && row["RowVersion"] != DBNull.Value)
                _rowVersion = (byte[])row["RowVersion"];

            if (row.Table.Columns.Contains("Numero") && row["Numero"] != DBNull.Value)
                txtNumero.Text = Convert.ToString(row["Numero"]);

            if (row.Table.Columns.Contains("Piso") && row["Piso"] != DBNull.Value)
                txtPiso.Text = Convert.ToString(row["Piso"]);

            if (row.Table.Columns.Contains("Tipologia") && row["Tipologia"] != DBNull.Value)
                txtTipologia.Text = Convert.ToString(row["Tipologia"]);

            if (row.Table.Columns.Contains("Metros2") && row["Metros2"] != DBNull.Value)
                txtMetros2.Text = Convert.ToDecimal(row["Metros2"]).ToString(CultureInfo.CurrentCulture);

            if (row.Table.Columns.Contains("Estacionamiento") && row["Estacionamiento"] != DBNull.Value)
                txtEstacionamiento.Text = Convert.ToString(row["Estacionamiento"]);

            if (row.Table.Columns.Contains("Amueblada") && row["Amueblada"] != DBNull.Value)
                chkAmueblada.Checked = Convert.ToBoolean(row["Amueblada"]);
            else
                chkAmueblada.Checked = false;

            if (row.Table.Columns.Contains("CantidadMuebles") && row["CantidadMuebles"] != DBNull.Value)
                txtCantidadMuebles.Text = Convert.ToString(row["CantidadMuebles"]);
            else
                txtCantidadMuebles.Text = string.Empty;

            if (row.Table.Columns.Contains("CuotaMantenimientoEspecifica") && row["CuotaMantenimientoEspecifica"] != DBNull.Value)
                txtCuotaMantenimientoEspecifica.Text =
                    Convert.ToDecimal(row["CuotaMantenimientoEspecifica"]).ToString(CultureInfo.CurrentCulture);
            else
                txtCuotaMantenimientoEspecifica.Text = string.Empty;

            if (row.Table.Columns.Contains("Observaciones") && row["Observaciones"] != DBNull.Value)
                txtObservaciones.Text = Convert.ToString(row["Observaciones"]);
            else
                txtObservaciones.Text = string.Empty;

            CargarBloque();
            ActualizarEstadoMuebles();
        }

        private void CargarBloque()
        {
            if (_bloqueId <= 0)
            {
                txtBloqueSeleccionado.Text = string.Empty;
                return;
            }

            DataRow rowBloque = _nBloque.PorId(_bloqueId);
            if (rowBloque != null)
                txtBloqueSeleccionado.Text = Convert.ToString(rowBloque["Identificador"]);
            else
                txtBloqueSeleccionado.Text = string.Empty;
        }

        private void ActualizarEstadoMuebles()
        {
            bool visible = chkAmueblada.Checked;

            if (lblMuebles != null)
                lblMuebles.Visible = visible;

            if (txtCantidadMuebles != null)
            {
                txtCantidadMuebles.Visible = visible;

                if (!visible)
                    txtCantidadMuebles.Text = string.Empty;
            }
        }

        private bool TryParseDecimalFlexible(string texto, out decimal valor)
        {
            if (decimal.TryParse(texto, NumberStyles.Number, CultureInfo.CurrentCulture, out valor))
                return true;

            if (decimal.TryParse(texto, NumberStyles.Number, CultureInfo.InvariantCulture, out valor))
                return true;

            return false;
        }

        private void btnConfirmar_Click(object sender, EventArgs e)
        {
            try
            {
                string numero = txtNumero.Text.Trim();
                string tipologia = txtTipologia.Text.Trim();
                string estacionamiento = txtEstacionamiento.Text.Trim();
                string observaciones = txtObservaciones.Text.Trim();

                if (string.IsNullOrWhiteSpace(numero))
                {
                    MessageBox.Show(
                        "Debe ingresar el número de la unidad.",
                        "Validación",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    txtNumero.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(tipologia))
                {
                    MessageBox.Show(
                        "Debe ingresar la tipología.",
                        "Validación",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    txtTipologia.Focus();
                    return;
                }

                int piso;
                if (!int.TryParse(txtPiso.Text.Trim(), out piso) || piso < 0)
                {
                    MessageBox.Show(
                        "El piso debe ser un número entero (>= 0).",
                        "Validación",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    txtPiso.Focus();
                    return;
                }

                decimal? metros2 = null;
                if (!string.IsNullOrWhiteSpace(txtMetros2.Text))
                {
                    decimal m2;
                    if (!TryParseDecimalFlexible(txtMetros2.Text.Trim(), out m2))
                    {
                        MessageBox.Show(
                            "Metros cuadrados debe ser un número decimal válido.",
                            "Validación",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                        txtMetros2.Focus();
                        return;
                    }

                    metros2 = m2;
                }

                bool? amueblada = chkAmueblada.Checked;
                int? cantidadMuebles = null;

                if (chkAmueblada.Checked)
                {
                    if (string.IsNullOrWhiteSpace(txtCantidadMuebles.Text))
                    {
                        MessageBox.Show(
                            "Debe indicar la cantidad de muebles.",
                            "Validación",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                        txtCantidadMuebles.Focus();
                        return;
                    }

                    int cm;
                    if (!int.TryParse(txtCantidadMuebles.Text.Trim(), out cm) || cm <= 0)
                    {
                        MessageBox.Show(
                            "Cantidad de muebles debe ser un número entero mayor que 0.",
                            "Validación",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                        txtCantidadMuebles.Focus();
                        return;
                    }

                    cantidadMuebles = cm;
                }

                decimal? cuotaEspecifica = null;
                if (!string.IsNullOrWhiteSpace(txtCuotaMantenimientoEspecifica.Text))
                {
                    decimal cu;
                    if (!TryParseDecimalFlexible(txtCuotaMantenimientoEspecifica.Text.Trim(), out cu))
                    {
                        MessageBox.Show(
                            "La cuota de mantenimiento debe ser un número decimal válido.",
                            "Validación",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                        txtCuotaMantenimientoEspecifica.Focus();
                        return;
                    }

                    cuotaEspecifica = cu;
                }

                if (_rowVersion == null || _rowVersion.Length == 0)
                    throw new InvalidOperationException("No se pudo recuperar la RowVersion de la unidad.");

                string usuario = UserContext.Usuario;
                if (string.IsNullOrWhiteSpace(usuario))
                    usuario = "rtscon@local";

                _nUnidad.Actualizar(
                    _id,
                    numero,
                    piso,
                    tipologia,
                    metros2,
                    estacionamiento,
                    amueblada,
                    cantidadMuebles,
                    cuotaEspecifica,
                    observaciones,
                    _rowVersion,
                    usuario);

                MessageBox.Show(
                    "Unidad actualizada correctamente.",
                    "Éxito",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No se pudo actualizar la unidad: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnVolver_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void chkAmueblada_CheckedChanged(object sender, EventArgs e)
        {
            ActualizarEstadoMuebles();
        }

        private void btnBuscarBloque_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "El bloque no se puede cambiar desde esta pantalla por el momento.",
                "Información",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
    }
}
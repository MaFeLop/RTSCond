using System;
using System.Data;
using System.Globalization;
using System.Windows.Forms;
using RTSCon.Datos;
using RTSCon.Negocios;

namespace RTSCon.Catalogos
{
    public partial class CrearUnidad : Form
    {
        private int _bloqueId;
        private readonly NUnidad _nUnidad;
        private readonly NBloque _nBloque;

        private bool _eventosInicializados;
        private bool _guardando;

        public CrearUnidad(int bloqueId)
        {
            InitializeComponent();

            _bloqueId = bloqueId;

            var dUnidad = new DUnidad(Conexion.CadenaConexion);
            _nUnidad = new NUnidad(dUnidad);

            var dBloque = new DBloque(Conexion.CadenaConexion);
            _nBloque = new NBloque(dBloque);

            InicializarEventosUnaSolaVez();
        }

        #region Configuración inicial

        private void InicializarEventosUnaSolaVez()
        {
            if (_eventosInicializados)
                return;

            this.Load -= CrearUnidad_Load;
            this.Load -= CrearUnidad_Load_1;
            this.Load += CrearUnidad_Load_1;

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

            if (btnBuscarBloque != null)
            {
                btnBuscarBloque.Click -= btnBuscarBloque_Click;
                btnBuscarBloque.Click += btnBuscarBloque_Click;
            }

            if (chkAmueblada != null)
            {
                chkAmueblada.CheckedChanged -= chkTitular_CheckedChanged;
                chkAmueblada.CheckedChanged += chkTitular_CheckedChanged;
            }

            _eventosInicializados = true;
        }

        private void CrearUnidad_Load(object sender, EventArgs e)
        {
            InicializarFormulario();
        }

        private void CrearUnidad_Load_1(object sender, EventArgs e)
        {
            InicializarFormulario();
        }

        private void InicializarFormulario()
        {
            CargarBloque();
            ActualizarEstadoMuebles();

            if (txtUnidadEnlazada != null)
                txtUnidadEnlazada.ReadOnly = true;

            if (txtMetros2 != null)
                txtMetros2.ReadOnly = false;

            if (txtEstacionamiento != null)
                txtEstacionamiento.ReadOnly = false;
        }

        #endregion

        #region Bloque

        private void CargarBloque()
        {
            if (_bloqueId <= 0)
            {
                if (txtUnidadEnlazada != null)
                    txtUnidadEnlazada.Text = string.Empty;

                return;
            }

            try
            {
                DataRow row = _nBloque.PorId(_bloqueId);

                if (row == null)
                {
                    if (txtUnidadEnlazada != null)
                        txtUnidadEnlazada.Text = string.Empty;

                    return;
                }

                string identificador = ObtenerValorTexto(row, "Identificador");
                bool estaActivo = BloqueEstaActivo(row);

                if (!estaActivo)
                    identificador = identificador + " (Inactivo)";

                if (txtUnidadEnlazada != null)
                    txtUnidadEnlazada.Text = identificador;
            }
            catch
            {
                if (txtUnidadEnlazada != null)
                    txtUnidadEnlazada.Text = string.Empty;
            }
        }

        private bool BloqueActualEstaActivo()
        {
            if (_bloqueId <= 0)
                return false;

            DataRow row = _nBloque.PorId(_bloqueId);

            if (row == null)
                return false;

            return BloqueEstaActivo(row);
        }

        private bool BloqueEstaActivo(DataRow row)
        {
            if (row == null)
                return false;

            if (!row.Table.Columns.Contains("IsActive"))
                return true;

            if (row["IsActive"] == DBNull.Value)
                return false;

            return Convert.ToBoolean(row["IsActive"]);
        }

        private string ObtenerValorTexto(DataRow row, string columna)
        {
            if (row == null)
                return string.Empty;

            if (!row.Table.Columns.Contains(columna))
                return string.Empty;

            if (row[columna] == DBNull.Value)
                return string.Empty;

            return Convert.ToString(row[columna]) ?? string.Empty;
        }

        #endregion

        #region Validaciones auxiliares

        private void ActualizarEstadoMuebles()
        {
            bool visible = chkAmueblada != null && chkAmueblada.Checked;

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

        #endregion

        #region Eventos principales

        private void btnConfirmar_Click(object sender, EventArgs e)
        {
            if (_guardando)
                return;

            _guardando = true;

            if (btnConfirmar != null)
                btnConfirmar.Enabled = false;

            try
            {
                GuardarUnidad();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No se pudo crear la unidad: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                if (!IsDisposed && btnConfirmar != null)
                    btnConfirmar.Enabled = true;

                _guardando = false;
            }
        }

        private void btnVolver_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void chkTitular_CheckedChanged(object sender, EventArgs e)
        {
            ActualizarEstadoMuebles();
        }

        private void btnBuscarBloque_Click(object sender, EventArgs e)
        {
            using (var frm = new BuscarBloque())
            {
                if (frm.ShowDialog(this) == DialogResult.OK)
                {
                    _bloqueId = frm.BloqueIdSeleccionado;
                    CargarBloque();
                }
            }
        }

        #endregion

        #region Guardado

        private void GuardarUnidad()
        {
            if (_bloqueId <= 0)
            {
                MessageBox.Show(
                    "Debe seleccionar un bloque.",
                    "Validación",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if (!BloqueActualEstaActivo())
            {
                MessageBox.Show(
                    "No se puede agregar una unidad a un bloque inactivo.",
                    "Validación",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

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

                if (!TryParseDecimalFlexible(txtMetros2.Text.Trim(), out m2) || m2 < 0)
                {
                    MessageBox.Show(
                        "Metros cuadrados debe ser un número decimal válido mayor o igual que 0.",
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

                if (!TryParseDecimalFlexible(txtCuotaMantenimientoEspecifica.Text.Trim(), out cu) || cu < 0)
                {
                    MessageBox.Show(
                        "La cuota de mantenimiento debe ser un número decimal válido mayor o igual que 0.",
                        "Validación",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    txtCuotaMantenimientoEspecifica.Focus();
                    return;
                }

                cuotaEspecifica = cu;
            }

            string usuario = UserContext.Usuario;

            if (string.IsNullOrWhiteSpace(usuario))
                usuario = "rtscon@local";

            _nUnidad.Insertar(
                _bloqueId,
                numero,
                piso,
                tipologia,
                metros2,
                estacionamiento,
                amueblada,
                cantidadMuebles,
                cuotaEspecifica,
                observaciones,
                usuario);

            MessageBox.Show(
                "Unidad creada correctamente.",
                "Éxito",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            DialogResult = DialogResult.OK;
            Close();
        }

        #endregion

        #region Eventos vacíos del Designer

        private void label3_Click(object sender, EventArgs e)
        {
        }

        private void txtIdPropietario_TextChanged(object sender, EventArgs e)
        {
        }

        private void txtObservaciones_TextChanged(object sender, EventArgs e)
        {
        }

        #endregion
    }
}
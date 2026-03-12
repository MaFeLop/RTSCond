using Krypton.Toolkit;
using RTSCon.Datos;
using RTSCon.Negocios;
using System;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace RTSCon.Catalogos
{
    public partial class BuscarPropietario : KryptonForm
    {
        // ===== Contrato esperado por otras pantallas =====
        public int SelectedId => PropietarioIdSeleccionado;
        public string SelectedUsuario { get; private set; } = string.Empty;
        public string SelectedCorreo { get; private set; } = string.Empty;

        // ===== Contrato interno =====
        public int PropietarioIdSeleccionado { get; private set; }

        private const string COL_SEL = "__sel";

        private readonly NCondominio _negocio;
        private DataTable _dt;

        public BuscarPropietario()
            : this(null)
        {
        }

        public BuscarPropietario(NCondominio negocio)
        {
            InitializeComponent();

            _negocio = negocio ?? CrearNegocioDesdeConfig();

            Load += BuscarPropietario_Load;

            if (btnConfirmar != null)
                btnConfirmar.Click += btnConfirmar_Click;

            if (btnCancelar != null)
                btnCancelar.Click += (s, e) =>
                {
                    DialogResult = DialogResult.Cancel;
                    Close();
                };

            if (txtBuscar != null)
            {
                txtBuscar.TextChanged += (s, e) => Cargar();

                txtBuscar.KeyDown += (s, e) =>
                {
                    if (e.KeyCode == Keys.Enter)
                    {
                        e.SuppressKeyPress = true;
                        Cargar();
                    }
                };
            }

            if (dgvPropietario != null)
            {
                dgvPropietario.AutoGenerateColumns = true;
                dgvPropietario.MultiSelect = false;
                dgvPropietario.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgvPropietario.AllowUserToAddRows = false;
                dgvPropietario.ReadOnly = false;

                dgvPropietario.CurrentCellDirtyStateChanged += dgvPropietario_CurrentCellDirtyStateChanged;
                dgvPropietario.CellValueChanged += dgvPropietario_CellValueChanged;
                dgvPropietario.CellClick += dgvPropietario_CellClick;
            }
        }

        private void BuscarPropietario_Load(object sender, EventArgs e)
        {
            EnsureSelectionColumn();
            Cargar();
        }

        private static NCondominio CrearNegocioDesdeConfig()
        {
            var cs = ObtenerConnectionString();
            if (string.IsNullOrWhiteSpace(cs))
            {
                throw new InvalidOperationException(
                    "No se encontró una cadena de conexión válida para cargar propietarios.");
            }

            return new NCondominio(new DCondominio(cs));
        }

        private static string ObtenerConnectionString()
        {
            var config = ConfigurationManager.ConnectionStrings;
            if (config == null || config.Count == 0)
                return null;

            string[] nombresPreferidos =
            {
                "cn",
                "conexion",
                "RTSCon",
                "RTSConConnectionString",
                "DefaultConnection"
            };

            foreach (var nombre in nombresPreferidos)
            {
                var item = config[nombre];
                if (item != null && !string.IsNullOrWhiteSpace(item.ConnectionString))
                    return item.ConnectionString;
            }

            foreach (ConnectionStringSettings item in config)
            {
                if (!string.IsNullOrWhiteSpace(item.ConnectionString))
                    return item.ConnectionString;
            }

            return null;
        }

        private void EnsureSelectionColumn()
        {
            if (dgvPropietario == null)
                return;

            if (dgvPropietario.Columns.Contains(COL_SEL))
                return;

            var col = new DataGridViewCheckBoxColumn
            {
                Name = COL_SEL,
                HeaderText = string.Empty,
                Width = 28,
                ReadOnly = false
            };

            dgvPropietario.Columns.Insert(0, col);
        }

        private void Cargar()
        {
            try
            {
                string filtro = txtBuscar != null
                    ? (txtBuscar.Text ?? string.Empty).Trim()
                    : string.Empty;

                _dt = _negocio.BuscarPropietarios(filtro, true, 50);

                dgvPropietario.DataSource = null;
                dgvPropietario.DataSource = _dt;

                EnsureSelectionColumn();
                LimpiarChecks();

                AjustarColumnas();

                bool hay = _dt != null && _dt.Rows.Count > 0;

                if (lblNoHay != null)
                    lblNoHay.Visible = !hay;

                if (btnConfirmar != null)
                    btnConfirmar.Enabled = hay;
            }
            catch (Exception ex)
            {
                KryptonMessageBox.Show(
                    this,
                    "Error al cargar propietarios: " + ex.Message,
                    "Buscar Propietario",
                    KryptonMessageBoxButtons.OK,
                    KryptonMessageBoxIcon.Error);

                if (dgvPropietario != null)
                    dgvPropietario.DataSource = null;

                if (btnConfirmar != null)
                    btnConfirmar.Enabled = false;

                if (lblNoHay != null)
                    lblNoHay.Visible = true;
            }
        }

        private void AjustarColumnas()
        {
            if (dgvPropietario == null)
                return;

            if (dgvPropietario.Columns.Contains(COL_SEL))
            {
                dgvPropietario.Columns[COL_SEL].DisplayIndex = 0;
                dgvPropietario.Columns[COL_SEL].Width = 28;
            }

            if (dgvPropietario.Columns.Contains("Id"))
                dgvPropietario.Columns["Id"].Visible = false;

            if (dgvPropietario.Columns.Contains("ID_usr"))
                dgvPropietario.Columns["ID_usr"].Visible = false;

            if (dgvPropietario.Columns.Contains("ID_propietario"))
                dgvPropietario.Columns["ID_propietario"].Visible = false;

            dgvPropietario.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void LimpiarChecks()
        {
            if (dgvPropietario == null || !dgvPropietario.Columns.Contains(COL_SEL))
                return;

            foreach (DataGridViewRow row in dgvPropietario.Rows)
            {
                if (row.IsNewRow)
                    continue;

                row.Cells[COL_SEL].Value = false;
            }
        }

        private void dgvPropietario_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dgvPropietario == null)
                return;

            if (dgvPropietario.IsCurrentCellDirty)
                dgvPropietario.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        private void dgvPropietario_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvPropietario == null)
                return;

            if (e.RowIndex < 0)
                return;

            if (!dgvPropietario.Columns.Contains(COL_SEL))
                return;

            if (e.ColumnIndex != dgvPropietario.Columns[COL_SEL].Index)
                return;

            var celda = dgvPropietario.Rows[e.RowIndex].Cells[COL_SEL];
            bool actual = Convert.ToBoolean(celda.Value ?? false);
            celda.Value = !actual;
        }

        private void dgvPropietario_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvPropietario == null)
                return;

            if (e.RowIndex < 0)
                return;

            if (!dgvPropietario.Columns.Contains(COL_SEL))
                return;

            if (e.ColumnIndex != dgvPropietario.Columns[COL_SEL].Index)
                return;

            bool marcado = Convert.ToBoolean(dgvPropietario.Rows[e.RowIndex].Cells[COL_SEL].Value ?? false);

            if (!marcado)
                return;

            for (int i = 0; i < dgvPropietario.Rows.Count; i++)
            {
                if (i == e.RowIndex)
                    continue;

                if (dgvPropietario.Rows[i].IsNewRow)
                    continue;

                dgvPropietario.Rows[i].Cells[COL_SEL].Value = false;
            }
        }

        private DataRowView GetMarcado()
        {
            if (dgvPropietario == null || !dgvPropietario.Columns.Contains(COL_SEL))
                return null;

            foreach (DataGridViewRow row in dgvPropietario.Rows)
            {
                if (row.IsNewRow)
                    continue;

                bool marcado = Convert.ToBoolean(row.Cells[COL_SEL].Value ?? false);
                if (!marcado)
                    continue;

                return row.DataBoundItem as DataRowView;
            }

            return null;
        }

        private void btnConfirmar_Click(object sender, EventArgs e)
        {
            if (_dt == null || _dt.Rows.Count == 0)
            {
                KryptonMessageBox.Show(
                    this,
                    "No hay propietarios para seleccionar.",
                    "Buscar Propietario",
                    KryptonMessageBoxButtons.OK,
                    KryptonMessageBoxIcon.Information);
                return;
            }

            var rv = GetMarcado();
            if (rv == null)
            {
                KryptonMessageBox.Show(
                    this,
                    "Marque un propietario (checkbox) para confirmar.",
                    "Buscar Propietario",
                    KryptonMessageBoxButtons.OK,
                    KryptonMessageBoxIcon.Information);
                return;
            }

            if (!rv.Row.Table.Columns.Contains("Id") || rv["Id"] == DBNull.Value || string.IsNullOrWhiteSpace(Convert.ToString(rv["Id"])))
            {
                KryptonMessageBox.Show(
                    this,
                    "No se pudo obtener el Id del propietario.",
                    "Buscar Propietario",
                    KryptonMessageBoxButtons.OK,
                    KryptonMessageBoxIcon.Warning);
                return;
            }

            PropietarioIdSeleccionado = Convert.ToInt32(rv["Id"]);

            SelectedUsuario =
                rv.Row.Table.Columns.Contains("Usuario") ? Convert.ToString(rv["Usuario"]) :
                rv.Row.Table.Columns.Contains("Nombre") ? Convert.ToString(rv["Nombre"]) :
                rv.Row.Table.Columns.Contains("NombreCompleto") ? Convert.ToString(rv["NombreCompleto"]) :
                string.Empty;

            SelectedCorreo =
                rv.Row.Table.Columns.Contains("Correo") ? Convert.ToString(rv["Correo"]) :
                rv.Row.Table.Columns.Contains("Email") ? Convert.ToString(rv["Email"]) :
                rv.Row.Table.Columns.Contains("correo") ? Convert.ToString(rv["correo"]) :
                string.Empty;

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
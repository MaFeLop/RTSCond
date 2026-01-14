using Krypton.Toolkit;
using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace RTSCon.Catalogos // ✅ IMPORTANTE: mismo namespace que estás usando en CrearCondominio / UpdatePropiedad
{
    public partial class BuscarPropietario : KryptonForm
    {
        // ===== Contrato esperado por otras pantallas =====
        public int SelectedId => PropietarioIdSeleccionado;
        public string SelectedUsuario { get; private set; } = "";
        public string SelectedCorreo { get; private set; } = "";

        // ===== Contrato interno =====
        public int PropietarioIdSeleccionado { get; private set; }

        private const string COL_SEL = "__sel";
        private DataTable _dt;

        public BuscarPropietario()
        {
            InitializeComponent();

            this.Load += BuscarPropietario_Load;

            if (btnConfirmar != null) btnConfirmar.Click += btnConfirmar_Click;
            if (btnCancelar != null) btnCancelar.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };

            if (txtBuscar != null)
            {
                txtBuscar.TextChanged += (_, __) => Cargar();
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

                dgvPropietario.CurrentCellDirtyStateChanged += (s, e) =>
                {
                    if (dgvPropietario.IsCurrentCellDirty)
                        dgvPropietario.CommitEdit(DataGridViewDataErrorContexts.Commit);
                };

                dgvPropietario.CellContentClick += Dgv_CellContentClick;
            }
        }

        private void BuscarPropietario_Load(object sender, EventArgs e)
        {
            EnsureSelectionColumn();
            Cargar();
        }

        private void EnsureSelectionColumn()
        {
            if (dgvPropietario == null) return;
            if (dgvPropietario.Columns.Contains(COL_SEL)) return;

            var col = new DataGridViewCheckBoxColumn
            {
                Name = COL_SEL,
                HeaderText = "",
                Width = 28,
                ReadOnly = false
            };

            dgvPropietario.Columns.Insert(0, col);
        }

        private void Cargar()
        {
            try
            {
                string filtro = txtBuscar?.Text?.Trim() ?? "";

                // TODO: aquí conectas tu negocio real, ejemplo:
                // _dt = _nPropietario.Buscar(filtro, soloActivos:true, top:50);

                _dt = new DataTable(); // placeholder seguro mientras conectas

                dgvPropietario.DataSource = _dt;

                bool hay = _dt != null && _dt.Rows.Count > 0;
                if (lblNoHay != null) lblNoHay.Visible = !hay;
                if (btnConfirmar != null) btnConfirmar.Enabled = hay;

                // limpiar checks
                if (dgvPropietario.Columns.Contains(COL_SEL))
                {
                    foreach (DataGridViewRow r in dgvPropietario.Rows)
                    {
                        if (r.IsNewRow) continue;
                        r.Cells[COL_SEL].Value = false;
                    }
                }
            }
            catch (Exception ex)
            {
                KryptonMessageBox.Show(this,
                    "Error al cargar propietarios: " + ex.Message,
                    "Buscar Propietario",
                    KryptonMessageBoxButtons.OK,
                    KryptonMessageBoxIcon.Error);

                if (btnConfirmar != null) btnConfirmar.Enabled = false;
                if (lblNoHay != null) lblNoHay.Visible = true;
            }
        }

        private void Dgv_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvPropietario == null) return;
            if (e.RowIndex < 0) return;
            if (dgvPropietario.Columns[e.ColumnIndex].Name != COL_SEL) return;

            bool marcado = Convert.ToBoolean(dgvPropietario.Rows[e.RowIndex].Cells[COL_SEL].Value ?? false);
            if (marcado)
            {
                for (int i = 0; i < dgvPropietario.Rows.Count; i++)
                {
                    if (i == e.RowIndex) continue;
                    if (dgvPropietario.Rows[i].IsNewRow) continue;
                    dgvPropietario.Rows[i].Cells[COL_SEL].Value = false;
                }
            }
        }

        private DataRowView GetMarcado()
        {
            if (dgvPropietario == null) return null;

            foreach (DataGridViewRow r in dgvPropietario.Rows)
            {
                if (r.IsNewRow) continue;
                bool marcado = Convert.ToBoolean(r.Cells[COL_SEL].Value ?? false);
                if (!marcado) continue;

                return r.DataBoundItem as DataRowView;
            }
            return null;
        }

        private void btnConfirmar_Click(object sender, EventArgs e)
        {
            if (_dt == null || _dt.Rows.Count == 0)
            {
                KryptonMessageBox.Show(this,
                    "No hay propietarios para seleccionar.",
                    "Buscar Propietario",
                    KryptonMessageBoxButtons.OK,
                    KryptonMessageBoxIcon.Information);
                return;
            }

            var rv = GetMarcado();
            if (rv == null)
            {
                KryptonMessageBox.Show(this,
                    "Marque un propietario (checkbox) para confirmar.",
                    "Buscar Propietario",
                    KryptonMessageBoxButtons.OK,
                    KryptonMessageBoxIcon.Information);
                return;
            }

            if (!rv.Row.Table.Columns.Contains("Id") || rv["Id"] == DBNull.Value)
            {
                KryptonMessageBox.Show(this,
                    "No se pudo obtener el Id del propietario.",
                    "Buscar Propietario",
                    KryptonMessageBoxButtons.OK,
                    KryptonMessageBoxIcon.Warning);
                return;
            }

            // ===== devolver datos a pantallas llamadoras =====
            PropietarioIdSeleccionado = Convert.ToInt32(rv["Id"]);

            // Usuario / Nombre (tolerante a nombres de columna)
            SelectedUsuario =
                rv.Row.Table.Columns.Contains("Usuario") ? Convert.ToString(rv["Usuario"]) :
                rv.Row.Table.Columns.Contains("Nombre") ? Convert.ToString(rv["Nombre"]) :
                rv.Row.Table.Columns.Contains("NombreCompleto") ? Convert.ToString(rv["NombreCompleto"]) :
                "";

            // Correo / Email (tolerante a nombres de columna)
            SelectedCorreo =
                rv.Row.Table.Columns.Contains("Correo") ? Convert.ToString(rv["Correo"]) :
                rv.Row.Table.Columns.Contains("Email") ? Convert.ToString(rv["Email"]) :
                rv.Row.Table.Columns.Contains("correo") ? Convert.ToString(rv["correo"]) :
                "";

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}

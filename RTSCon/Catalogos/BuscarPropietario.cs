using Krypton.Toolkit;
using RTSCon.Datos;
using RTSCon.Negocios;
using System;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace RTSCon.Catalogos.Condominio
{
    public partial class BuscarPropietario : KryptonForm
    {
        private readonly NAuth _auth;
        private int _page = 1;
        private const int _pageSize = 20;

        public int SelectedId { get; private set; }
        public string SelectedUsuario { get; private set; }
        public string SelectedCorreo { get; private set; }

        public BuscarPropietario()
        {
            InitializeComponent();

            var cn = ConfigurationManager.ConnectionStrings["RTSCond"].ConnectionString;
            _auth = new NAuth(new DAuth(cn));

            // Config grid
            dgvPropietario.AutoGenerateColumns = false;
            if (!dgvPropietario.Columns.Contains("Sel"))
                dgvPropietario.Columns.Insert(0, new DataGridViewCheckBoxColumn { Name = "Sel", HeaderText = "", Width = 30 });
            if (!dgvPropietario.Columns.Contains("Id"))
                dgvPropietario.Columns.Add(new DataGridViewTextBoxColumn { Name = "Id", DataPropertyName = "Id", HeaderText = "Id", Width = 60, ReadOnly = true });
            if (!dgvPropietario.Columns.Contains("Usuario"))
                dgvPropietario.Columns.Add(new DataGridViewTextBoxColumn { Name = "Usuario", DataPropertyName = "Usuario", HeaderText = "Usuario", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill, ReadOnly = true });
            if (!dgvPropietario.Columns.Contains("Correo"))
                dgvPropietario.Columns.Add(new DataGridViewTextBoxColumn { Name = "Correo", DataPropertyName = "Correo", HeaderText = "Correo", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill, ReadOnly = true });

            // Selección única
            dgvPropietario.CellContentClick += (s, e) =>
            {
                if (e.ColumnIndex == dgvPropietario.Columns["Sel"].Index && e.RowIndex >= 0)
                {
                    foreach (DataGridViewRow r in dgvPropietario.Rows)
                        if (r.Index != e.RowIndex) r.Cells["Sel"].Value = false;
                    dgvPropietario.EndEdit();
                }
            };
            dgvPropietario.MultiSelect = false;
            dgvPropietario.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvPropietario.CellDoubleClick += (s, e) => { if (e.RowIndex >= 0) Confirmar(); };

            // === CARGA AUTOMÁTICA ===
            this.Shown += (s, e) => { txtBuscar.Focus(); Cargar(); };
            txtBuscar.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    _page = 1;
                    Cargar();
                }
            };
            chkSoloActivos.CheckedChanged += (s, e) => { _page = 1; Cargar(); };

            // Botones
            btnConfirmar.Click += (s, e) => Confirmar();
            btnCancelar.Click += (s, e) => Close();
        }

        private void Cargar()
        {
            int total;
            var dt = _auth.ListarPropietarios(
                txtBuscar.Text.Trim(),
                chkSoloActivos.Checked,
                _page, _pageSize, out total);

            dgvPropietario.DataSource = dt;

            // limpiar checks
            foreach (DataGridViewRow r in dgvPropietario.Rows) r.Cells["Sel"].Value = false;
        }

        private void Confirmar()
        {
            DataGridViewRow row = null;

            // prioridad por checkbox
            foreach (DataGridViewRow r in dgvPropietario.Rows)
                if (r.Cells["Sel"].Value is bool b && b) { row = r; break; }

            // fallback: fila actual
            if (row == null && dgvPropietario.CurrentRow != null)
                row = dgvPropietario.CurrentRow;

            if (row == null)
            {
                KryptonMessageBox.Show(this, "Seleccione un propietario.", "Propietarios",
                    KryptonMessageBoxButtons.OK, KryptonMessageBoxIcon.Information);
                return;
            }

            SelectedId = Convert.ToInt32(row.Cells["Id"].Value);
            SelectedUsuario = Convert.ToString(row.Cells["Usuario"].Value);
            SelectedCorreo = Convert.ToString(row.Cells["Correo"].Value);

            this.DialogResult = DialogResult.OK;
            Close();
        }
    }
}

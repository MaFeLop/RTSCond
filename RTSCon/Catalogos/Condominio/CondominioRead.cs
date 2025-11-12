using Krypton.Toolkit;
using RTSCon.Datos;
using RTSCon.Negocios;
using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace RTSCon.Catalogos.Condominio
{
    public partial class chkSoloActivos : KryptonForm
    {
        private readonly NCondominio _neg;
        private int _page = 1;
        private const int _pageSize = 20;

        public chkSoloActivos()
        {
            InitializeComponent();

            var cn = System.Configuration.ConfigurationManager
                                 .ConnectionStrings["RTSCond"].ConnectionString;
            _neg = new NCondominio(new DCondominio(cn));

            // Columna CheckBox de selección única en dgvCondominios
            if (!dgvCondominios.Columns.Contains("Sel"))
            {
                var sel = new DataGridViewCheckBoxColumn
                {
                    Name = "Sel",
                    HeaderText = "",
                    Width = 30
                };
                dgvCondominios.Columns.Insert(0, sel);
            }

            dgvCondominios.CellContentClick += DgvCondominios_CellContentClick;

            Cargar();
        }

        // Si tienes un KryptonCheckBox llamado chkSoloActivos lo usará; si no, por defecto TRUE
        private bool SoloActivos
        {
            get
            {
                var ctl = this.Controls.Find("chkSoloActivos", true).FirstOrDefault();
                if (ctl is CheckBox cb) return cb.Checked;
                if (ctl is KryptonCheckBox kcb) return kcb.Checked;
                return true; // por defecto listar solo activos
            }
        }

        private void Cargar()
        {
            int total;
            var buscar = txtBuscar?.Text?.Trim() ?? "";

            var dt = _neg.Listar(buscar, SoloActivos, _page, _pageSize, out total);

            // Si aún no definiste columnas con DataPropertyName, usa autogeneradas:
            dgvCondominios.AutoGenerateColumns = true; // cámbialo a false cuando mapees columnas
            dgvCondominios.DataSource = dt;

            SetTextIfExists("lblTotal", $"Total: {total}");
            SetTextIfExists("lblPagina", $"Página: {_page}");
        }

        private void DgvCondominios_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvCondominios.Columns[e.ColumnIndex].Name == "Sel")
            {
                foreach (DataGridViewRow r in dgvCondominios.Rows)
                    if (r.Index != e.RowIndex) r.Cells["Sel"].Value = false;
                dgvCondominios.EndEdit();
            }
        }

        // Helpers para no romper si los labels no existen
        private void SetTextIfExists(string name, string text)
        {
            var ctl = this.Controls.Find(name, true).FirstOrDefault();
            if (ctl != null) ctl.Text = text;
        }

        // --- (Opcional) obtener el registro seleccionado con la columna Sel ---
        private bool TryGetSeleccion(out int id, out byte[] rowVersion)
        {
            id = 0; rowVersion = null;
            if (!dgvCondominios.Columns.Contains("Sel")) return false;

            foreach (DataGridViewRow r in dgvCondominios.Rows)
            {
                if (r.Cells["Sel"].Value is bool b && b)
                {
                    // Asegúrate que existan estas columnas en el DataSource
                    id = Convert.ToInt32(r.Cells["Id"].Value);
                    rowVersion = r.Cells["RowVersion"].Value as byte[];
                    return true;
                }
            }
            return false;
        }

        // Eventos que ya tenías generados (puedes llamar Cargar o usar TryGetSeleccion)
        private void CondominioRead_Load(object sender, EventArgs e) { }
        private void btnLimpiarFiltros_Click(object sender, EventArgs e)
        {
            txtBuscar.Text = "";
            _page = 1;
            Cargar();
        }
        private void kryptonButton1_Click(object sender, EventArgs e) { /* Nuevo */ }
        private void btnUpdate_Click(object sender, EventArgs e) { /* Editar con TryGetSeleccion */ }
        private void btnDesactivar_Click(object sender, EventArgs e) { /* Desactivar con TryGetSeleccion */ }
        private void dgvCondominios_CellContentClick(object sender, DataGridViewCellEventArgs e) { }
        private void txtBuscar_TextChanged(object sender, EventArgs e) { /* opcional: live search */ }
    }
}

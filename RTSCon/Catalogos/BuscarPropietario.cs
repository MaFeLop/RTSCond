using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Krypton.Toolkit;
using RTSCon.Negocios;
using RTSCon.Datos;
using System.Configuration;

namespace RTSCon.Catalogos.Condominio
{
    public partial class BuscarPropietario : KryptonForm
    {
        private readonly NAuth _auth;
        private int _page = 1;
        private const int _pageSize = 20;

        public int? SelectedId { get; private set; }
        public string SelectedUsuario { get; private set; }
        public string SelectedCorreo { get; private set; }

        public BuscarPropietario()
        {
            InitializeComponent();
            var cn = ConfigurationManager.ConnectionStrings["RTSCond"].ConnectionString;
            _auth = new NAuth(new DAuth(cn));

            // Columna de selección única
            var chk = new DataGridViewCheckBoxColumn { Name = "Sel", HeaderText = "", Width = 30 };
            grid.AutoGenerateColumns = false;
            if (!grid.Columns.Contains("Sel")) grid.Columns.Insert(0, chk);
            grid.CellContentClick += (s, e) =>
            {
                if (e.ColumnIndex == 0 && e.RowIndex >= 0)
                {
                    foreach (DataGridViewRow r in grid.Rows)
                        if (r.Index != e.RowIndex) r.Cells[0].Value = false;
                    grid.EndEdit();
                }
            };

            btnBuscar.Click += (s, e) => Cargar();
            btnConfirmar.Click += (s, e) => Confirmar();
            btnCancelar.Click += (s, e) => this.Close();

            this.Shown += (s, e) => Cargar();
        }

        private void Cargar()
        {
            int total;
            var dt = _auth.ListarPropietarios(
                txtBuscar.Text.Trim(),
                chkSoloActivos.Checked,
                _page, _pageSize, out total);

            grid.DataSource = dt;

            // Aseguramos columnas clave
            if (!grid.Columns.Contains("Id")) grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Id", DataPropertyName = "Id", HeaderText = "Id", ReadOnly = true, Width = 60 });
            if (!grid.Columns.Contains("Usuario")) grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Usuario", DataPropertyName = "Usuario", HeaderText = "Usuario", ReadOnly = true, AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            if (!grid.Columns.Contains("Correo")) grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Correo", DataPropertyName = "Correo", HeaderText = "Correo", ReadOnly = true, AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });

            // Selección visual limpia
            foreach (DataGridViewRow r in grid.Rows) r.Cells["Sel"].Value = false;
        }

        private void Confirmar()
        {
            foreach (DataGridViewRow r in grid.Rows)
            {
                if (r.Cells["Sel"].Value is bool b && b)
                {
                    SelectedId = Convert.ToInt32(r.Cells["Id"].Value);
                    SelectedUsuario = Convert.ToString(r.Cells["Usuario"].Value);
                    SelectedCorreo = Convert.ToString(r.Cells["Correo"].Value);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                    return;
                }
            }
            KryptonMessageBox.Show(this, "Seleccione un propietario.", "Propietarios",
                KryptonMessageBoxButtons.OK, KryptonMessageBoxIcon.Information);
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

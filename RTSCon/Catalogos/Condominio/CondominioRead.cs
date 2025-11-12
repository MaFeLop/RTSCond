using Krypton.Toolkit;
using RTSCon.Datos;
using RTSCon.Negocios;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RTSCon.Catalogos.Condominio
{
    public partial class CondominioRead : KryptonForm
    {
        private readonly NCondominio _neg;
        private int _page = 1;
        private const int _pageSize = 20;
        public CondominioRead()
        {
            InitializeComponent();
            var cn = System.Configuration.ConfigurationManager.ConnectionStrings["RTSCond"].ConnectionString;
            _neg = new NCondominio(new DCondominio(cn));

            // Columna CheckBox de selección única:
            var chk = new DataGridViewCheckBoxColumn { Name = "Sel", HeaderText = "", Width = 30 };
            grid.Columns.Insert(0, chk);
            grid.CellContentClick += (s, e) =>
            {
                if (e.ColumnIndex == 0 && e.RowIndex >= 0)
                {
                    foreach (DataGridViewRow r in grid.Rows)
                        if (r.Index != e.RowIndex) r.Cells[0].Value = false;
                    grid.EndEdit();
                }
            };

            Cargar();
        }

        private void Cargar()
        {
            int total;
            var dt = _neg.Listar(txtBuscar.Text.Trim(), chkSoloActivos.Checked, _page, _pageSize, out total);
            grid.AutoGenerateColumns = false;
            grid.DataSource = dt;
            lblTotal.Text = $"Total: {total}";
            lblPagina.Text = $"Página: {_page}";
        }

        private void CondominioRead_Load(object sender, EventArgs e)
        {

        }

        private void btnLimpiarFiltros_Click(object sender, EventArgs e)
        {

        }

        private void kryptonButton1_Click(object sender, EventArgs e)
        {

        }
    }
}

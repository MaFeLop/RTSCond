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
    public partial class CondominioRead : KryptonForm
    {
        private readonly NCondominio _neg;
        private int _page = 1;
        private const int _pageSize = 20;

        public CondominioRead()
        {
            InitializeComponent();

            var cn = ConfigurationManager.ConnectionStrings["RTSCond"].ConnectionString;
            _neg = new NCondominio(new DCondominio(cn));

            // Carga al abrir
            this.Load += (_, __) => Cargar();

            // Buscar: escribir o Enter dentro del textbox => recargar
            if (txtBuscar != null)
            {
                txtBuscar.TextChanged += (_, __) => { _page = 1; Cargar(); };
                txtBuscar.KeyDown += (s, e) =>
                {
                    if (e.KeyCode == Keys.Enter)
                    {
                        e.SuppressKeyPress = true; // evita beep
                        _page = 1;
                        Cargar();
                    }
                };
            }

            // Limpiar filtros (si existe)
            if (btnLimpiarFiltros != null) btnLimpiarFiltros.Click += (_, __) =>
            {
                txtBuscar.Text = string.Empty;
                var chk = this.Controls.Find("chkSoloActivos", true).OfType<KryptonCheckBox>().FirstOrDefault();
                if (chk != null) chk.Checked = false;
                _page = 1;
                Cargar();
            };

            // Reaccionar a chkSoloActivos si está en el form
            var chkSolo = this.Controls.Find("chkSoloActivos", true).OfType<KryptonCheckBox>().FirstOrDefault();
            if (chkSolo != null) chkSolo.CheckedChanged += (_, __) => { _page = 1; Cargar(); };

            // CREAR
            if (btnCrear != null) btnCrear.Click += (_, __) =>
            {
                using (var f = new CrearCondominio())
                    if (f.ShowDialog(this) == DialogResult.OK) Cargar();
            };

            // UPDATE
            if (btnUpdate != null) btnUpdate.Click += (_, __) =>
            {
                var id = IdSeleccionado();
                if (id <= 0)
                {
                    KryptonMessageBox.Show(this, "Seleccione un condominio.", "Actualizar");
                    return;
                }

                using (var f = new UpdateCondominio())
                {
                    // puedes usar propiedad o Tag; la clase ya expone CondominioId
                    f.CondominioId = id;
                    f.Tag = id;
                    if (f.ShowDialog(this) == DialogResult.OK) Cargar();
                }
            };

            // DESACTIVAR -> diálogo con password + notificación por correo
            if (btnDesactivar != null) btnDesactivar.Click += (_, __) => DesactivarSeleccionado();

            // Grid
            if (dgvCondominios != null)
            {
                dgvCondominios.MultiSelect = false;
                dgvCondominios.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgvCondominios.ReadOnly = true;
                dgvCondominios.AutoGenerateColumns = true;

                // Doble clic = actualizar
                dgvCondominios.CellDoubleClick += (_, __) => btnUpdate?.PerformClick();

                // Tecla Supr = desactivar con confirmación+password
                dgvCondominios.KeyDown += (s, e) =>
                {
                    if (e.KeyCode == Keys.Delete)
                    {
                        e.SuppressKeyPress = true;
                        DesactivarSeleccionado();
                    }
                };
            }
        }

        private int IdSeleccionado()
        {
            if (dgvCondominios?.CurrentRow == null) return 0;
            var row = dgvCondominios.CurrentRow.DataBoundItem as DataRowView;
            if (row == null) return 0;
            return row.Row.Table.Columns.Contains("Id") && row["Id"] != DBNull.Value
                ? Convert.ToInt32(row["Id"])
                : 0;
        }

        private byte[] ObtenerRowVersionSeleccionada()
        {
            if (dgvCondominios?.CurrentRow == null) return null;
            var row = dgvCondominios.CurrentRow.DataBoundItem as DataRowView;
            if (row == null) return null;

            return (row.Row.Table.Columns.Contains("RowVersion") && row["RowVersion"] != DBNull.Value)
                ? (byte[])row["RowVersion"]
                : null;
        }

        private void Cargar()
        {
            var chkSolo = this.Controls.Find("chkSoloActivos", true).OfType<KryptonCheckBox>().FirstOrDefault();
            bool soloActivos = chkSolo?.Checked ?? false;

            int total;
            var buscar = txtBuscar?.Text?.Trim() ?? string.Empty;
            var dt = _neg.Listar(buscar, soloActivos, _page, _pageSize, out total);

            if (dgvCondominios != null)
            {
                dgvCondominios.AutoGenerateColumns = true;
                dgvCondominios.DataSource = dt;
            }

            var lblTotalCtrl = this.Controls.Find("lblTotal", true).FirstOrDefault();
            var lblPaginaCtrl = this.Controls.Find("lblPagina", true).FirstOrDefault();
            if (lblTotalCtrl != null) ((Control)lblTotalCtrl).Text = $"Total: {total}";
            if (lblPaginaCtrl != null) ((Control)lblPaginaCtrl).Text = $"Página: {_page}";
        }

        private void DesactivarSeleccionado()
        {
            var id = IdSeleccionado();
            if (id <= 0)
            {
                KryptonMessageBox.Show(this, "Seleccione un condominio.", "Desactivar");
                return;
            }

            // Nombre visual para el prompt
            string nombre = "(sin nombre)";
            if (dgvCondominios?.CurrentRow?.DataBoundItem is DataRowView rv &&
                rv.Row.Table.Columns.Contains("Nombre") && rv["Nombre"] != DBNull.Value)
                nombre = Convert.ToString(rv["Nombre"]);

            // Concurrencia
            byte[] rowVersion = ObtenerRowVersionSeleccionada();
            var mailProfile = ConfigurationManager.AppSettings["MailProfile"] ?? "RTSCondMail";

            // Diálogo con password (reutilizable)
            using (var dlg = new RTSCon.Catalogos.ConfirmarDesactivacion(
                entidad: "condominio",
                nombreEntidad: nombre,
                onConfirm: editor =>
                {
                    // 1) Desactivar lógico con RowVersion
                    _neg.Desactivar(id, rowVersion, editor);

                    // 2) Notificar por correo
                    _neg.NotificarAccion(id, "Desactivado", editor, mailProfile);

                    return true;
                }))
            {
                if (dlg.ShowDialog(this) == DialogResult.OK)
                    Cargar();
            }
        }

        private void btnCrear_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}

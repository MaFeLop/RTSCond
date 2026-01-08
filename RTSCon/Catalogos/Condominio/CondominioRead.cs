using Krypton.Toolkit;
using RTSCon.Datos;
using RTSCon.Negocios;
using System;
using System.Configuration;
using System.Data;
using System.Drawing;
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
                ConfigurarGrid();



                // ==================== ESTÉTICA Y UX ====================
                dgvCondominios.AutoGenerateColumns = true;
                dgvCondominios.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dgvCondominios.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgvCondominios.MultiSelect = false;
                dgvCondominios.ReadOnly = true;
                dgvCondominios.AllowUserToAddRows = false;
                dgvCondominios.AllowUserToResizeRows = false;
                dgvCondominios.RowHeadersVisible = false;
                dgvCondominios.EnableHeadersVisualStyles = false;

                // ==================== OCULTAR BASURA TÉCNICA ====================
                string[] ocultar =
                {
    "CreatedAt", "CreatedBy",
    "UpdatedAt", "UpdatedBy",
    "ID_propietario",
    "ID_usr_secretario1",
    "ID_usr_secretario2",
    "ID_usr_secretario3",
    "RowVersion", "RowVersionHex",
    "EmailNotificaciones"
};

                foreach (var col in ocultar)
                {
                    if (dgvCondominios.Columns.Contains(col))
                        dgvCondominios.Columns[col].Visible = false;
                }

                // ==================== RENOMBRAR COLUMNAS (UX) ====================
                if (dgvCondominios.Columns.Contains("Id"))
                    dgvCondominios.Columns["Id"].HeaderText = "Código";

                if (dgvCondominios.Columns.Contains("Nombre"))
                    dgvCondominios.Columns["Nombre"].HeaderText = "Condominio";

                if (dgvCondominios.Columns.Contains("Direccion"))
                    dgvCondominios.Columns["Direccion"].HeaderText = "Dirección";

                if (dgvCondominios.Columns.Contains("Tipo"))
                    dgvCondominios.Columns["Tipo"].HeaderText = "Tipo";

                if (dgvCondominios.Columns.Contains("AdministradorResponsable"))
                    dgvCondominios.Columns["AdministradorResponsable"].HeaderText = "Administrador";

                if (dgvCondominios.Columns.Contains("IsActive"))
                    dgvCondominios.Columns["IsActive"].HeaderText = "Activo";

                // ==================== AJUSTES DE ANCHO ====================
                dgvCondominios.Columns["Id"].FillWeight = 10;
                dgvCondominios.Columns["Nombre"].FillWeight = 30;
                dgvCondominios.Columns["Direccion"].FillWeight = 30;
                dgvCondominios.Columns["Tipo"].FillWeight = 15;
                dgvCondominios.Columns["AdministradorResponsable"].FillWeight = 25;

                // Evita re-generación rara si tú defines columnas manualmente:
                dgvCondominios.AutoGenerateColumns = true;

                // Si el DGV auto-crea columnas, oculta las técnicas:
                if (dgvCondominios.Columns.Contains("rn")) dgvCondominios.Columns["rn"].Visible = false;

                // RowVersionHex es texto (no rompe), pero si no quieres verlo:
                if (dgvCondominios.Columns.Contains("RowVersionHex")) dgvCondominios.Columns["RowVersionHex"].Visible = false;

                // (Opcional) Buen formato
                dgvCondominios.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dgvCondominios.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgvCondominios.MultiSelect = false;
                dgvCondominios.ReadOnly = true;

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
            using (var dlg = new RTSCon.Catalogos.CondominioConfirmarDesactivacion(
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

        private void ConfigurarGrid()
        {
            var dgv = dgvCondominios; // <-- cambia al nombre real

            dgv.AutoGenerateColumns = true;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.AllowUserToResizeRows = false;
            dgv.MultiSelect = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.ReadOnly = true;

            // Visual
            dgv.RowHeadersVisible = false;
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgv.ColumnHeadersHeight = 34;

            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 9F);
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dgv.DefaultCellStyle.SelectionBackColor = SystemColors.Highlight;
            dgv.DefaultCellStyle.SelectionForeColor = Color.White;
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 248, 252);

            // Evita que se vea “aplastado”
            dgv.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            dgv.RowTemplate.Height = 28;

            // Importante para que no se vea feo:
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Si hay columnas “técnicas” que no deben verse:
            OcultarSiExiste("RowVersion");
            OcultarSiExiste("CreatedAt");
            OcultarSiExiste("CreatedBy");
            OcultarSiExiste("UpdatedAt");
            OcultarSiExiste("UpdatedBy");
            OcultarSiExiste("rn"); // si la estás trayendo del SP

            // Renombrar headers para que no se corten tan feo:
            Renombrar("Id", "Cód.");
            Renombrar("Nombre", "Condominio");
            Renombrar("Direccion", "Dirección");
            Renombrar("AdministradorResponsable", "Administrador");
            Renombrar("FechaConstitucion", "Fecha Constitución");
            Renombrar("CuotaMantenimientoBase", "Cuota Base");
            Renombrar("ReglamentoDocumentoId", "Reglamento");
            Renombrar("IsActive", "Activo");
            Renombrar("EnviarNotifsAlPropietario", "Enviar Notifs");

            // Formatos numéricos y fecha:
            FormatoFecha("FechaConstitucion");
            FormatoMoney("CuotaMantenimientoBase");

            // Ajuste fino: algunas columnas conviene que NO sean Fill (evita columnas gigantes)
            FijarAncho("Id", 60);
            FijarAncho("IsActive", 70);
            FijarAncho("EnviarNotifsAlPropietario", 110);
            FijarAncho("ReglamentoDocumentoId", 110);

            void OcultarSiExiste(string col)
            {
                if (dgv.Columns.Contains(col))
                    dgv.Columns[col].Visible = false;
            }
            void Renombrar(string col, string header)
            {
                if (dgv.Columns.Contains(col))
                    dgv.Columns[col].HeaderText = header;
            }
            void FormatoFecha(string col)
            {
                if (dgv.Columns.Contains(col))
                    dgv.Columns[col].DefaultCellStyle.Format = "dd/MM/yyyy";
            }
            void FormatoMoney(string col)
            {
                if (dgv.Columns.Contains(col))
                {
                    dgv.Columns[col].DefaultCellStyle.Format = "N2";
                    dgv.Columns[col].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
            }
            void FijarAncho(string col, int width)
            {
                if (dgv.Columns.Contains(col))
                {
                    dgv.Columns[col].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                    dgv.Columns[col].Width = width;
                }
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}

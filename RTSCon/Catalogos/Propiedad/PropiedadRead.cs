using Krypton.Toolkit;
using RTSCon.Datos;
using RTSCon.Negocios;
using System;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace RTSCon.Catalogos.Propiedad
{
    public partial class PropiedadRead : KryptonForm
    {
        private readonly NPropiedad _neg;
        private int _page = 1;
        private const int _pageSize = 20;

        public PropiedadRead()
        {
            InitializeComponent();
            var cn = ConfigurationManager.ConnectionStrings["RTSCond"].ConnectionString;
            _neg = new NPropiedad(new DPropiedad(cn));

            // Carga al abrir
            this.Load += (_, __) => Cargar();

            // Buscar: Enter dentro del textbox => recargar
            var txtBuscar = FindCtrl<TextBox>("txtBuscar", "txtBuscarPropiedad", "txtFiltro");
            if (txtBuscar != null)
            {
                txtBuscar.TextChanged += (_, __) => { _page = 1; Cargar(); };
                txtBuscar.KeyDown += (s, e) =>
                {
                    if (e.KeyCode == Keys.Enter)
                    {
                        e.SuppressKeyPress = true;
                        _page = 1;
                        Cargar();
                    }
                };
            }

            // Limpiar filtros
            var btnLimpiar = FindCtrl<KryptonButton>("btnLimpiarFiltros", "btnLimpiar", "btnClear");
            if (btnLimpiar != null) btnLimpiar.Click += (_, __) =>
            {
                if (txtBuscar != null) txtBuscar.Text = string.Empty;
                var chk = FindCtrl<KryptonCheckBox>("chkSoloActivos", "chkActivos");
                if (chk != null) chk.Checked = false;
                _page = 1;
                Cargar();
            };

            // Reaccionar a chkSoloActivos
            var chkSolo = FindCtrl<KryptonCheckBox>("chkSoloActivos", "chkActivos");
            if (chkSolo != null) chkSolo.CheckedChanged += (_, __) => { _page = 1; Cargar(); };

            // CREAR
            var btnCrear = FindCtrl<KryptonButton>("btnCrear", "btnNuevo", "btnAdd");
            if (btnCrear != null) btnCrear.Click += (_, __) =>
            {
                using (var f = new CrearPropiedad())
                    if (f.ShowDialog(this) == DialogResult.OK) Cargar();
            };

            // UPDATE
            var btnUpdate = FindCtrl<KryptonButton>("btnUpdate", "btnEditar", "btnModificar");
            if (btnUpdate != null) btnUpdate.Click += (_, __) =>
            {
                var id = IdSeleccionado();
                if (id <= 0) { KryptonMessageBox.Show(this, "Seleccione una propiedad.", "Actualizar"); return; }
                using (var f = new UpdatePropiedad()) { f.Tag = id; if (f.ShowDialog(this) == DialogResult.OK) Cargar(); }
            };

            // DESACTIVAR (borrado lógico con contraseña en ConfirmarDesactivacion)
            var btnDesactivar = FindCtrl<KryptonButton>("btnDesactivar", "btnBorrar", "btnEliminar");
            if (btnDesactivar != null) btnDesactivar.Click += (_, __) => DesactivarSeleccionada();

            // Grid
            var grid = Dgv();
            if (grid != null)
            {
                grid.MultiSelect = false;
                grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                grid.ReadOnly = true;
                grid.AutoGenerateColumns = true;
                grid.CellDoubleClick += (_, __) => btnUpdate?.PerformClick();
            }
        }

        // ---------- Helpers ----------
        private T FindCtrl<T>(params string[] names) where T : Control
        {
            foreach (var n in names)
            {
                var c = this.Controls.Find(n, true).FirstOrDefault() as T;
                if (c != null) return c;
            }
            return null;
        }
        private DataGridView Dgv()
            => FindCtrl<DataGridView>("dgvPropiedad", "dgvPropiedades", "gridPropiedad", "grid");

        private int IdSeleccionado()
        {
            var grid = Dgv();
            if (grid?.CurrentRow == null) return 0;
            var row = grid.CurrentRow.DataBoundItem as DataRowView;
            if (row == null) return 0;
            return row.Row.Table.Columns.Contains("Id") && row["Id"] != DBNull.Value ? Convert.ToInt32(row["Id"]) : 0;
        }
        private byte[] RowVersionSeleccionada()
        {
            var grid = Dgv();
            if (grid?.CurrentRow == null) return null;
            var row = grid.CurrentRow.DataBoundItem as DataRowView;
            if (row == null) return null;
            return (row.Row.Table.Columns.Contains("RowVersion") && row["RowVersion"] != DBNull.Value)
                ? (byte[])row["RowVersion"]
                : null;
        }
        // -----------------------------

        private void Cargar()
        {
            var chk = FindCtrl<KryptonCheckBox>("chkSoloActivos", "chkActivos");
            bool soloActivos = chk?.Checked ?? false;

            var txtBuscar = FindCtrl<TextBox>("txtBuscar", "txtBuscarPropiedad", "txtFiltro");
            var buscar = txtBuscar?.Text?.Trim() ?? string.Empty;

            // Si el rol es "Admin" (tu Admin = Propietario), filtramos por su Id (OwnerId)
            int? ownerId = null;
            if (!string.IsNullOrEmpty(UserContext.Rol) &&
                (UserContext.Rol.Equals("Admin", StringComparison.OrdinalIgnoreCase) ||
                 UserContext.Rol.Equals("Propietario", StringComparison.OrdinalIgnoreCase)))
            {
                ownerId = UserContext.UsuarioAuthId;
            }

            int total;
            var dt = _neg.Listar(buscar, soloActivos, _page, _pageSize, out total, ownerId);

            var grid = Dgv();
            if (grid != null)
            {
                grid.AutoGenerateColumns = true;
                grid.DataSource = dt;
            }

            var lblTotal = FindCtrl<Label>("lblTotal");
            var lblPagina = FindCtrl<Label>("lblPagina");
            if (lblTotal != null) lblTotal.Text = $"Total: {total}";
            if (lblPagina != null) lblPagina.Text = $"Página: {_page}";
        }

        private void DesactivarSeleccionada()
        {
            var id = IdSeleccionado();
            if (id <= 0) { KryptonMessageBox.Show(this, "Seleccione una propiedad.", "Desactivar"); return; }

            // Arma un “nombre” amigable para mostrar (PropietarioId-UnidadId)
            string display = "Propiedad";
            var grid = Dgv();
            if (grid?.CurrentRow?.DataBoundItem is DataRowView rv)
            {
                var hasPid = rv.Row.Table.Columns.Contains("PropietarioId");
                var hasUid = rv.Row.Table.Columns.Contains("UnidadId");
                var pid = hasPid && rv["PropietarioId"] != DBNull.Value ? Convert.ToString(rv["PropietarioId"]) : "?";
                var uid = hasUid && rv["UnidadId"] != DBNull.Value ? Convert.ToString(rv["UnidadId"]) : "?";
                display = $"Propietario {pid} - Unidad {uid}";
            }

            byte[] rowVersion = RowVersionSeleccionada();

            using (var dlg = new RTSCon.Catalogos.CondominioConfirmarDesactivacion(
                entidad: "propiedad",
                nombreEntidad: display,
                onConfirm: editor =>
                {
                    _neg.Desactivar(id, rowVersion, editor);
                    return true;
                }))
            {
                if (dlg.ShowDialog(this) == DialogResult.OK)
                    Cargar();
            }
        }
    }
}

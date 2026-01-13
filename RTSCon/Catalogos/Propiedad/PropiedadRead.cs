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
    public partial class PropiedadRead : KryptonForm
    {
        private readonly NPropiedad _neg;
        private int _page = 1;
        private const int _pageSize = 20;

        // ====== NUEVO: modo selección por checkbox ======
        private enum ModoAccion { Ninguno, Editar, Desactivar }
        private ModoAccion _modo = ModoAccion.Ninguno;
        private const string COL_SEL = "__sel";

        public PropiedadRead()
        {
            InitializeComponent();

            var cn = ConfigurationManager.ConnectionStrings["RTSCond"].ConnectionString;
            _neg = new NPropiedad(new DPropiedad(cn));

            // Load
            this.Load += (_, __) =>
            {
                EnsureSelectionColumn();
                HookGridSelectionEvents();
                SalirModoSeleccion();
                Cargar();
            };

            // Buscar: Enter y/o cambio de texto (si existe)
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

            // Solo activos
            var chkSolo = FindCtrl<KryptonCheckBox>("chkSoloActivos", "chkActivos");
            if (chkSolo != null) chkSolo.CheckedChanged += (_, __) => { _page = 1; Cargar(); };

            // ====== Botones estandarizados ======
            var btnCrear = FindCtrl<KryptonButton>("btnCrear");
            var btnUpdate = FindCtrl<KryptonButton>("btnUpdate");
            var btnDesactivar = FindCtrl<KryptonButton>("btnDesactivar");
            var btnVolver = FindCtrl<KryptonButton>("btnVolver");

            if (btnCrear != null)
            {
                btnCrear.Click += (_, __) =>
                {
                    if (_modo != ModoAccion.Ninguno) return; // no crear si estás en modo selección

                    using (var f = new CrearPropiedad())
                        if (f.ShowDialog(this) == DialogResult.OK) Cargar();
                };
            }

            if (btnUpdate != null)
            {
                btnUpdate.Click += (_, __) =>
                {
                    // 1er clic => entrar modo selección
                    if (_modo == ModoAccion.Ninguno)
                    {
                        EntrarModoSeleccion(ModoAccion.Editar, btnUpdate, btnDesactivar, btnCrear, btnVolver);
                        return;
                    }

                    // Si está en otro modo, ignorar
                    if (_modo != ModoAccion.Editar) return;

                    // 2do clic => confirmar
                    var row = GetRowMarcado();
                    if (row == null)
                    {
                        KryptonMessageBox.Show(this, "Marque una propiedad (checkbox) para actualizar.", "Actualizar",
                            KryptonMessageBoxButtons.OK, KryptonMessageBoxIcon.Information);
                        return;
                    }

                    int id = row.Row.Table.Columns.Contains("Id") && row["Id"] != DBNull.Value ? Convert.ToInt32(row["Id"]) : 0;
                    if (id <= 0)
                    {
                        KryptonMessageBox.Show(this, "No se pudo obtener el Id.", "Actualizar",
                            KryptonMessageBoxButtons.OK, KryptonMessageBoxIcon.Warning);
                        return;
                    }

                    SalirModoSeleccion(btnUpdate, btnDesactivar, btnCrear, btnVolver);

                    using (var f = new UpdatePropiedad())
                    {
                        f.Tag = id;
                        if (f.ShowDialog(this) == DialogResult.OK) Cargar();
                    }
                };
            }

            if (btnDesactivar != null)
            {
                btnDesactivar.Click += (_, __) =>
                {
                    // 1er clic => entrar modo selección
                    if (_modo == ModoAccion.Ninguno)
                    {
                        EntrarModoSeleccion(ModoAccion.Desactivar, btnUpdate, btnDesactivar, btnCrear, btnVolver);
                        return;
                    }

                    // Si está en otro modo, ignorar
                    if (_modo != ModoAccion.Desactivar) return;

                    // 2do clic => confirmar
                    var row = GetRowMarcado();
                    if (row == null)
                    {
                        KryptonMessageBox.Show(this, "Marque una propiedad (checkbox) para desactivar.", "Desactivar",
                            KryptonMessageBoxButtons.OK, KryptonMessageBoxIcon.Information);
                        return;
                    }

                    int id = row.Row.Table.Columns.Contains("Id") && row["Id"] != DBNull.Value ? Convert.ToInt32(row["Id"]) : 0;
                    if (id <= 0)
                    {
                        KryptonMessageBox.Show(this, "No se pudo obtener el Id.", "Desactivar",
                            KryptonMessageBoxButtons.OK, KryptonMessageBoxIcon.Warning);
                        return;
                    }

                    byte[] rowVersion = null;
                    if (row.Row.Table.Columns.Contains("RowVersion") && row["RowVersion"] != DBNull.Value)
                        rowVersion = (byte[])row["RowVersion"];

                    // Display amigable
                    string display = "Propiedad";
                    var hasPid = row.Row.Table.Columns.Contains("PropietarioId");
                    var hasUid = row.Row.Table.Columns.Contains("UnidadId");
                    var pid = hasPid && row["PropietarioId"] != DBNull.Value ? Convert.ToString(row["PropietarioId"]) : "?";
                    var uid = hasUid && row["UnidadId"] != DBNull.Value ? Convert.ToString(row["UnidadId"]) : "?";
                    display = $"Propietario {pid} - Unidad {uid}";

                    SalirModoSeleccion(btnUpdate, btnDesactivar, btnCrear, btnVolver);

                    string mensaje = $"¿Deseas desactivar la propiedad {display}?";
                    using (var frm = new PropiedadConfirmarDesactivacion(mensaje))
                    {
                        if (frm.ShowDialog(this) == DialogResult.OK)
                        {
                            try
                            {
                                string editor = UserContext.Usuario;
                                string password = frm.Password;

                                var dAuth = new DAuth(Conexion.CadenaConexion);
                                var nAuth = new NAuth(dAuth);

                                int usuarioId = UserContext.UsuarioAuthId;

                                // ValidarPassword devuelve bool en tu implementación actual
                                var ok = nAuth.ValidarPassword(usuarioId, password);
                                if (!ok) throw new InvalidOperationException("Contraseña inválida.");

                                _neg.Desactivar(id, rowVersion, editor);

                                Cargar();
                            }
                            catch (Exception ex)
                            {
                                KryptonMessageBox.Show(this,
                                    "No se pudo desactivar la propiedad: " + ex.Message,
                                    "Desactivar Propiedad",
                                    KryptonMessageBoxButtons.OK,
                                    KryptonMessageBoxIcon.Error);
                            }
                        }
                    }
                };
            }

            if (btnVolver != null)
            {
                btnVolver.Click += (_, __) =>
                {
                    // Si estás en modo selección, esto es “Cancelar”
                    if (_modo != ModoAccion.Ninguno)
                    {
                        SalirModoSeleccion(btnUpdate, btnDesactivar, btnCrear, btnVolver);
                        return;
                    }

                    Close();
                };
            }

            // Grid config
            var grid = Dgv();
            if (grid != null)
            {
                grid.MultiSelect = false;
                grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                grid.ReadOnly = true; // OJO: el checkbox sigue funcionando porque la columna __sel se deja editable
                grid.AutoGenerateColumns = true;

                grid.CellDoubleClick += (_, __) =>
                {
                    // Doble clic abre update directo si NO estás en modo selección
                    if (_modo == ModoAccion.Ninguno)
                        btnUpdate?.PerformClick();
                };
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

        // ====== NUEVO: checkbox selection ======
        private void EnsureSelectionColumn()
        {
            var grid = Dgv();
            if (grid == null) return;

            if (grid.Columns.Contains(COL_SEL)) return;

            var col = new DataGridViewCheckBoxColumn
            {
                Name = COL_SEL,
                HeaderText = "",
                Width = 28,
                ReadOnly = false,
                Visible = false
            };

            grid.Columns.Insert(0, col);
        }

        private void HookGridSelectionEvents()
        {
            var grid = Dgv();
            if (grid == null) return;

            grid.CurrentCellDirtyStateChanged += (s, e) =>
            {
                if (grid.IsCurrentCellDirty)
                    grid.CommitEdit(DataGridViewDataErrorContexts.Commit);
            };

            grid.CellContentClick += (s, e) =>
            {
                if (_modo == ModoAccion.Ninguno) return;
                if (e.RowIndex < 0) return;
                if (grid.Columns[e.ColumnIndex].Name != COL_SEL) return;

                bool marcado = Convert.ToBoolean(grid.Rows[e.RowIndex].Cells[COL_SEL].Value ?? false);
                if (marcado)
                {
                    for (int i = 0; i < grid.Rows.Count; i++)
                    {
                        if (i == e.RowIndex) continue;
                        if (grid.Rows[i].IsNewRow) continue;
                        grid.Rows[i].Cells[COL_SEL].Value = false;
                    }
                }
            };
        }

        private void SetSelectionColumnVisible(bool visible)
        {
            var grid = Dgv();
            if (grid == null) return;
            if (!grid.Columns.Contains(COL_SEL)) return;

            grid.Columns[COL_SEL].Visible = visible;

            // Importante: hacer editable solo esa columna
            grid.ReadOnly = false;
            foreach (DataGridViewColumn c in grid.Columns)
                c.ReadOnly = true;

            grid.Columns[COL_SEL].ReadOnly = false;
        }

        private DataRowView GetRowMarcado()
        {
            var grid = Dgv();
            if (grid == null) return null;

            foreach (DataGridViewRow r in grid.Rows)
            {
                if (r.IsNewRow) continue;

                bool marcado = Convert.ToBoolean(r.Cells[COL_SEL].Value ?? false);
                if (!marcado) continue;

                return r.DataBoundItem as DataRowView;
            }

            return null;
        }

        private void EntrarModoSeleccion(ModoAccion modo,
            KryptonButton btnUpdate, KryptonButton btnDesactivar, KryptonButton btnCrear, KryptonButton btnVolver)
        {
            _modo = modo;
            SetSelectionColumnVisible(true);

            // limpiar checks
            var grid = Dgv();
            if (grid != null)
            {
                foreach (DataGridViewRow r in grid.Rows)
                {
                    if (r.IsNewRow) continue;
                    r.Cells[COL_SEL].Value = false;
                }
            }

            if (modo == ModoAccion.Editar)
            {
                if (btnUpdate != null) btnUpdate.Text = "Confirmar";
                if (btnDesactivar != null) btnDesactivar.Enabled = false;
                if (btnCrear != null) btnCrear.Enabled = false;
            }
            else if (modo == ModoAccion.Desactivar)
            {
                if (btnDesactivar != null) btnDesactivar.Text = "Confirmar";
                if (btnUpdate != null) btnUpdate.Enabled = false;
                if (btnCrear != null) btnCrear.Enabled = false;
            }

            if (btnVolver != null) btnVolver.Text = "Cancelar";
        }

        private void SalirModoSeleccion()
        {
            var btnCrear = FindCtrl<KryptonButton>("btnCrear");
            var btnUpdate = FindCtrl<KryptonButton>("btnUpdate");
            var btnDesactivar = FindCtrl<KryptonButton>("btnDesactivar");
            var btnVolver = FindCtrl<KryptonButton>("btnVolver");

            SalirModoSeleccion(btnUpdate, btnDesactivar, btnCrear, btnVolver);
        }


        private void SalirModoSeleccion(
            KryptonButton btnUpdate, KryptonButton btnDesactivar, KryptonButton btnCrear, KryptonButton btnVolver)
        {
            _modo = ModoAccion.Ninguno;

            // ocultar checks
            var grid = Dgv();
            if (grid != null && grid.Columns.Contains(COL_SEL))
            {
                grid.Columns[COL_SEL].Visible = false;
                grid.ReadOnly = true;
                foreach (DataGridViewColumn c in grid.Columns)
                    c.ReadOnly = true;
            }

            if (btnUpdate != null) { btnUpdate.Text = "Update"; btnUpdate.Enabled = true; }
            if (btnDesactivar != null) { btnDesactivar.Text = "Desactivar"; btnDesactivar.Enabled = true; }
            if (btnCrear != null) btnCrear.Enabled = true;
            if (btnVolver != null) btnVolver.Text = "Volver";
        }

        private void Cargar()
        {
            var chk = FindCtrl<KryptonCheckBox>("chkSoloActivos", "chkActivos");
            bool soloActivos = chk?.Checked ?? false;

            var txtBuscar = FindCtrl<TextBox>("txtBuscar", "txtBuscarPropiedad", "txtFiltro");
            var buscar = txtBuscar?.Text?.Trim() ?? string.Empty;

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

            // Mantener la columna según el modo
            SetSelectionColumnVisible(_modo != ModoAccion.Ninguno);
        }
    }
}

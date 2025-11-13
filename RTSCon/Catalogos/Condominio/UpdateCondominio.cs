using Krypton.Toolkit;
using RTSCon.Datos;
using RTSCon.Negocios;
using System;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace RTSCon.Catalogos.Condominio
{
    public partial class UpdateCondominio : KryptonForm
    {
        private readonly NCondominio _neg;
        private int _id;
        private byte[] _rowVersion;

        // opcional: además de Tag puedes setear esta propiedad desde fuera
        public int CondominioId { get; set; }

        // --- selección de propietario (para actualizar) ---
        private int? _idPropietarioSel;
        private string _usuarioPropSel;
        private string _correoPropSel;

        public UpdateCondominio()
        {
            InitializeComponent();

            var cn = ConfigurationManager.ConnectionStrings["RTSCond"].ConnectionString;
            _neg = new NCondominio(new DCondominio(cn));

            // Igual que en Create
            this.Shown += (s, e) =>
            {
                InitUi();
                if (!ResolverId()) return;
                CargarDatos();
            };

            // Botones (mismos nombres que en Create)
            var btnOk = FindCtrl<KryptonButton>("btnConfirmar", "btnGuardar", "btnOk");
            var btnBack = FindCtrl<KryptonButton>("btnVolver", "btnCancelar", "btnBack");
            if (btnOk != null) btnOk.Click += btnConfirmar_Click;
            if (btnBack != null) btnBack.Click += (s, e) => Close();

            // Solo números en cuota (mismos nombres que en Create)
            SetKeyPressNumeric("txtCuotaBase", "txtMantenimientoBase", "txtCuota");
        }

        // ---------- Helpers idénticos a Create ----------
        private T FindCtrl<T>(params string[] names) where T : Control
        {
            foreach (var n in names)
            {
                var c = this.Controls.Find(n, true).FirstOrDefault() as T;
                if (c != null) return c;
            }
            return null;
        }
        private string GetText(params string[] names)
        {
            var c = FindCtrl<Control>(names);
            return c?.Text?.Trim() ?? "";
        }
        private void SetText(string value, params string[] names)
        {
            var c = FindCtrl<Control>(names);
            if (c != null) c.Text = value ?? "";
        }
        private void SetKeyPressNumeric(params string[] names)
        {
            var c = FindCtrl<TextBoxBase>(names);
            if (c == null) return;
            c.KeyPress += (s, e) =>
            {
                char dec = Convert.ToChar(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != dec)
                    e.Handled = true;
                if (e.KeyChar == dec && ((TextBoxBase)s).Text.Contains(dec))
                    e.Handled = true;
            };
        }
        // -------------------------------------------------

        private void InitUi()
        {
            // Tipo de condominio (igual a Create)
            var cboTipo = FindCtrl<ComboBox>("cmbTipoCond", "cboTipo", "cboTipoCondominio");
            if (cboTipo != null && cboTipo.Items.Count == 0)
            {
                cboTipo.Items.AddRange(new object[] { "Residencial", "Comercial", "Mixto" });
                cboTipo.SelectedIndex = 0;
            }
        }

        private bool ResolverId()
        {
            if (CondominioId > 0) { _id = CondominioId; return true; }
            if (Tag is int t && t > 0) { _id = t; return true; }
            if (Tag != null && int.TryParse(Convert.ToString(Tag), out var parsed) && parsed > 0) { _id = parsed; return true; }

            KryptonMessageBox.Show(this, "No se recibió el Id del condominio.", "Actualizar",
                KryptonMessageBoxButtons.OK, KryptonMessageBoxIcon.Error);
            Close();
            return false;
        }

        private void CargarDatos()
        {
            var dt = _neg.BuscarPorId(_id); // DataTable con 1 fila
            if (dt == null || dt.Rows.Count == 0)
            {
                KryptonMessageBox.Show(this, "Condominio no encontrado.", "Actualizar",
                    KryptonMessageBoxButtons.OK, KryptonMessageBoxIcon.Error);
                Close();
                return;
            }

            var r = dt.Rows[0];
            string GS(string col) => dt.Columns.Contains(col) && r[col] != DBNull.Value ? Convert.ToString(r[col]) : "";
            decimal GD(string col) => dt.Columns.Contains(col) && r[col] != DBNull.Value ? Convert.ToDecimal(r[col]) : 0m;
            DateTime GDT(string col) => dt.Columns.Contains(col) && r[col] != DBNull.Value ? Convert.ToDateTime(r[col]) : DateTime.Today;
            bool GB(string col) => dt.Columns.Contains(col) && r[col] != DBNull.Value && Convert.ToBoolean(r[col]);

            // Igual mapeo que en Create (mismos nombres)
            SetText(GS("Nombre"), "txtNombreCondominio", "txtNombre");
            SetText(GS("Direccion"), "txtDireccionCondominio", "txtDireccion");
            SetText(GS("AdministradorResponsable"), "txtPropietarioResponsable", "txtAdministrador", "txtAdministradorResponsable");
            SetText(GS("EmailNotificaciones"), "txtCorreoNotif", "txtCorreoNotificaciones", "txtCorreo");
            SetText(GD("CuotaMantenimientoBase").ToString(CultureInfo.CurrentCulture),
                                                "txtCuotaBase", "txtMantenimientoBase", "txtCuota");

            var dtp = FindCtrl<DateTimePicker>("dtpFechaConstitucion", "dtpFechaConst");
            if (dtp != null) dtp.Value = GDT("FechaConstitucion");

            var cboTipo = FindCtrl<ComboBox>("cmbTipoCond", "cboTipo", "cboTipoCondominio");
            if (cboTipo != null)
            {
                var tipo = GS("Tipo");
                if (!string.IsNullOrWhiteSpace(tipo))
                {
                    if (!cboTipo.Items.Contains(tipo)) cboTipo.Items.Add(tipo);
                    cboTipo.SelectedItem = tipo;
                }
            }

            var chkNotifProp = FindCtrl<KryptonCheckBox>("chkNotificarPropietario", "chkEnviarNotifsPropietario", "chkEnviarNotificaciones");
            if (chkNotifProp != null) chkNotifProp.Checked = GB("EnviarNotifsAlPropietario");

            // RowVersion
            _rowVersion = dt.Columns.Contains("RowVersion") && r["RowVersion"] != DBNull.Value ? (byte[])r["RowVersion"] : null;

            // Id propietario (si viene de BD)
            if (dt.Columns.Contains("ID_propietario") && r["ID_propietario"] != DBNull.Value)
            {
                _idPropietarioSel = Convert.ToInt32(r["ID_propietario"]);
                var txtAdmin = FindCtrl<TextBoxBase>("txtPropietarioResponsable", "txtAdministrador", "txtAdministradorResponsable");
                if (txtAdmin != null) txtAdmin.Tag = _idPropietarioSel;
            }
        }

        private void btnConfirmar_Click(object sender, EventArgs e)
        {
            try
            {
                // Lectura/validación EXACTA a Create
                var txtNombre = GetText("txtNombreCondominio", "txtNombre");
                var txtDireccion = GetText("txtDireccionCondominio", "txtDireccion");
                var cboTipo = FindCtrl<ComboBox>("cmbTipoCond", "cboTipo", "cboTipoCondominio");
                var txtAdminResp = GetText("txtPropietarioResponsable", "txtAdministrador", "txtAdministradorResponsable");
                var txtCorreo = GetText("txtCorreoNotif", "txtCorreoNotificaciones", "txtCorreo");
                var txtCuota = GetText("txtCuotaBase", "txtMantenimientoBase", "txtCuota");
                var dtp = FindCtrl<DateTimePicker>("dtpFechaConstitucion", "dtpFechaConst");
                var chkNotifProp = FindCtrl<KryptonCheckBox>("chkNotificarPropietario", "chkEnviarNotifsPropietario", "chkEnviarNotificaciones");

                if (string.IsNullOrWhiteSpace(txtNombre)) throw new InvalidOperationException("Ingrese el nombre del condominio.");
                if (string.IsNullOrWhiteSpace(txtDireccion)) throw new InvalidOperationException("Ingrese la dirección del condominio.");
                if (cboTipo == null || string.IsNullOrWhiteSpace(Convert.ToString(cboTipo.SelectedItem)))
                    throw new InvalidOperationException("Seleccione el tipo de condominio.");
                if (string.IsNullOrWhiteSpace(txtAdminResp)) throw new InvalidOperationException("Ingrese el propietario/administrador responsable.");
                if (dtp == null) throw new InvalidOperationException("Seleccione la fecha de constitución.");
                if (!decimal.TryParse(txtCuota, NumberStyles.Number, CultureInfo.CurrentCulture, out var cuota) || cuota < 0)
                    throw new InvalidOperationException("Cuota de mantenimiento inválida.");
                if (string.IsNullOrWhiteSpace(txtCorreo) || !txtCorreo.Contains("@"))
                    throw new InvalidOperationException("Correo de notificaciones inválido.");

                // Parámetros (idénticos a Create)
                string nombre = txtNombre;
                string direccion = txtDireccion;
                string tipo = Convert.ToString(cboTipo.SelectedItem);
                string adminResp = txtAdminResp;
                DateTime fechaConst = dtp.Value.Date;
                decimal cuotaBase = cuota;
                string emailNotif = txtCorreo;
                bool enviarNotifProp = chkNotifProp?.Checked ?? false;

                int? reglamentoDocId = null;

                // === propietario seleccionado ===
                int? idPropietario = _idPropietarioSel;
                var txtAdminCtrl = FindCtrl<Control>("txtPropietarioResponsable", "txtAdministrador", "txtAdministradorResponsable");
                if (!idPropietario.HasValue && txtAdminCtrl?.Tag is int idTag) idPropietario = idTag;

                int? idSec1 = null, idSec2 = null, idSec3 = null;

                string editor = ConfigurationManager.AppSettings["DefaultEjecutor"] ?? "rtscon@local";

                // Actualizar (misma firma de Insertar + id, rowVersion, editor)
                _neg.Actualizar(
                    _id,
                    nombre, direccion, tipo, adminResp,
                    fechaConst, cuotaBase,
                    emailNotif, enviarNotifProp,
                    reglamentoDocId, idPropietario, idSec1, idSec2, idSec3,
                    _rowVersion, editor
                );

                KryptonMessageBox.Show(this, "Condominio actualizado correctamente.",
                    "Actualizar Condominio", KryptonMessageBoxButtons.OK, KryptonMessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                KryptonMessageBox.Show(this, ex.Message,
                    "Actualizar Condominio", KryptonMessageBoxButtons.OK, KryptonMessageBoxIcon.Error);
            }
        }

        private void btnBuscarPropietario_Click(object sender, EventArgs e)
        {
            // Asumimos que el Designer ya está cableado a este handler.
            using (var f = new BuscarPropietario())
            {
                if (f.ShowDialog(this) == DialogResult.OK)
                {
                    _idPropietarioSel = f.SelectedId;
                    _usuarioPropSel = f.SelectedUsuario;
                    _correoPropSel = f.SelectedCorreo;

                    // pinta nombre/usuario del responsable y guarda Id en Tag
                    var txtAdmin = FindCtrl<TextBoxBase>("txtPropietarioResponsable", "txtAdministrador", "txtAdministradorResponsable");
                    if (txtAdmin != null)
                    {
                        txtAdmin.Text = _usuarioPropSel ?? "";
                        txtAdmin.Tag = _idPropietarioSel;
                    }

                    // si el correo de notificaciones está vacío, usar el del propietario
                    var txtCorreo = FindCtrl<TextBoxBase>("txtCorreoNotif", "txtCorreoNotificaciones", "txtCorreo");
                    if (txtCorreo != null && string.IsNullOrWhiteSpace(txtCorreo.Text) && !string.IsNullOrWhiteSpace(_correoPropSel))
                        txtCorreo.Text = _correoPropSel;
                }
            }
        }
    }
}

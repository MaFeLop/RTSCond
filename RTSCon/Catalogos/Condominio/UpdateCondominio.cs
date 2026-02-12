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

        public int CondominioId { get; set; }

        private int? _idPropietarioSel;

        public UpdateCondominio()
        {
            InitializeComponent();

            var cn = ConfigurationManager.ConnectionStrings["RTSCond"].ConnectionString;
            _neg = new NCondominio(new DCondominio(cn));

            this.Shown += (s, e) =>
            {
                InitUi();
                if (!ResolverId()) return;
                CargarDatos();
            };

            var btnOk = FindCtrl<KryptonButton>("btnConfirmar", "btnGuardar", "btnOk");
            var btnBack = FindCtrl<KryptonButton>("btnVolver", "btnCancelar", "btnBack");

            if (btnOk != null) btnOk.Click += btnConfirmar_Click;
            if (btnBack != null) btnBack.Click += (s, e) => Close();

            SetKeyPressNumeric("txtCuotaBase", "txtMantenimientoBase", "txtCuota");
        }

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

        private void InitUi()
        {
            var cboTipo = FindCtrl<ComboBox>("cmbTipoCond", "cboTipo", "cboTipoCondominio");
            if (cboTipo != null && cboTipo.Items.Count == 0)
            {
                cboTipo.Items.AddRange(new object[] { "Residencial", "Comercial", "Mixto" });
                cboTipo.SelectedIndex = 0;
            }

            // ✅ Reemplazo de EsPropietarioActual
            if (!string.IsNullOrWhiteSpace(UserContext.Rol) &&
                UserContext.Rol.Equals("Propietario", StringComparison.OrdinalIgnoreCase))
            {
                SetText(UserContext.Usuario,
                    "txtPropietarioResponsable",
                    "txtAdministrador",
                    "txtAdministradorResponsable");
            }
        }

        private bool ResolverId()
        {
            if (CondominioId > 0) { _id = CondominioId; return true; }
            if (Tag is int t && t > 0) { _id = t; return true; }
            if (Tag != null && int.TryParse(Convert.ToString(Tag), out var parsed) && parsed > 0)
            {
                _id = parsed;
                return true;
            }

            KryptonMessageBox.Show(this, "No se recibió el Id del condominio.",
                "Actualizar", KryptonMessageBoxButtons.OK, KryptonMessageBoxIcon.Error);
            Close();
            return false;
        }

        private void CargarDatos()
        {
            var dt = _neg.BuscarPorId(_id);

            if (dt == null || dt.Rows.Count == 0)
            {
                KryptonMessageBox.Show(this, "Condominio no encontrado.",
                    "Actualizar", KryptonMessageBoxButtons.OK, KryptonMessageBoxIcon.Error);
                Close();
                return;
            }

            var r = dt.Rows[0];

            string GS(string col) =>
                dt.Columns.Contains(col) && r[col] != DBNull.Value
                    ? Convert.ToString(r[col])
                    : "";

            decimal GD(string col) =>
                dt.Columns.Contains(col) && r[col] != DBNull.Value
                    ? Convert.ToDecimal(r[col])
                    : 0m;

            DateTime GDT(string col) =>
                dt.Columns.Contains(col) && r[col] != DBNull.Value
                    ? Convert.ToDateTime(r[col])
                    : DateTime.Today;

            bool GB(string col) =>
                dt.Columns.Contains(col) && r[col] != DBNull.Value &&
                Convert.ToBoolean(r[col]);

            SetText(GS("Nombre"), "txtNombreCondominio", "txtNombre");
            SetText(GS("Direccion"), "txtDireccionCondominio", "txtDireccion");
            SetText(GS("AdministradorResponsable"),
                "txtPropietarioResponsable", "txtAdministrador", "txtAdministradorResponsable");
            SetText(GS("EmailNotificaciones"),
                "txtCorreoNotif", "txtCorreoNotificaciones", "txtCorreo");
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
                    if (!cboTipo.Items.Contains(tipo))
                        cboTipo.Items.Add(tipo);

                    cboTipo.SelectedItem = tipo;
                }
            }

            var chkNotifProp = FindCtrl<KryptonCheckBox>(
                "chkNotificarPropietario",
                "chkEnviarNotifsPropietario",
                "chkEnviarNotificaciones");

            if (chkNotifProp != null)
                chkNotifProp.Checked = GB("EnviarNotifsAlPropietario");

            _rowVersion =
                dt.Columns.Contains("RowVersion") && r["RowVersion"] != DBNull.Value
                    ? (byte[])r["RowVersion"]
                    : null;

            if (dt.Columns.Contains("ID_propietario") && r["ID_propietario"] != DBNull.Value)
            {
                _idPropietarioSel = Convert.ToInt32(r["ID_propietario"]);
                var txtAdmin = FindCtrl<TextBoxBase>(
                    "txtPropietarioResponsable",
                    "txtAdministrador",
                    "txtAdministradorResponsable");

                if (txtAdmin != null)
                    txtAdmin.Tag = _idPropietarioSel;
            }
        }

        private void btnConfirmar_Click(object sender, EventArgs e)
        {
            try
            {
                var txtNombre = GetText("txtNombreCondominio", "txtNombre");
                var txtDireccion = GetText("txtDireccionCondominio", "txtDireccion");
                var cboTipo = FindCtrl<ComboBox>("cmbTipoCond", "cboTipo", "cboTipoCondominio");
                var txtAdminResp = GetText("txtPropietarioResponsable", "txtAdministrador", "txtAdministradorResponsable");
                var txtCorreo = GetText("txtCorreoNotif", "txtCorreoNotificaciones", "txtCorreo");
                var txtCuota = GetText("txtCuotaBase", "txtMantenimientoBase", "txtCuota");
                var dtp = FindCtrl<DateTimePicker>("dtpFechaConstitucion", "dtpFechaConst");
                var chkNotifProp = FindCtrl<KryptonCheckBox>(
                    "chkNotificarPropietario",
                    "chkEnviarNotifsPropietario",
                    "chkEnviarNotificaciones");

                if (string.IsNullOrWhiteSpace(txtNombre))
                    throw new InvalidOperationException("Ingrese el nombre del condominio.");

                if (string.IsNullOrWhiteSpace(txtDireccion))
                    throw new InvalidOperationException("Ingrese la dirección del condominio.");

                if (cboTipo == null || string.IsNullOrWhiteSpace(Convert.ToString(cboTipo.SelectedItem)))
                    throw new InvalidOperationException("Seleccione el tipo de condominio.");

                if (string.IsNullOrWhiteSpace(txtAdminResp))
                    throw new InvalidOperationException("Ingrese el propietario responsable.");

                if (dtp == null)
                    throw new InvalidOperationException("Seleccione la fecha de constitución.");

                if (!decimal.TryParse(txtCuota, NumberStyles.Number,
                    CultureInfo.CurrentCulture, out var cuota) || cuota < 0)
                    throw new InvalidOperationException("Cuota inválida.");

                if (string.IsNullOrWhiteSpace(txtCorreo) || !txtCorreo.Contains("@"))
                    throw new InvalidOperationException("Correo inválido.");

                _neg.Actualizar(
                    _id,
                    txtNombre,
                    txtDireccion,
                    Convert.ToString(cboTipo.SelectedItem),
                    txtAdminResp,
                    dtp.Value.Date,
                    cuota,
                    txtCorreo,
                    chkNotifProp?.Checked ?? false,
                    null,
                    _idPropietarioSel,
                    null, null, null,
                    _rowVersion,
                    ConfigurationManager.AppSettings["DefaultEjecutor"] ?? "rtscon@local"
                );

                KryptonMessageBox.Show(this,
                    "Condominio actualizado correctamente.",
                    "Actualizar",
                    KryptonMessageBoxButtons.OK,
                    KryptonMessageBoxIcon.Information);

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                KryptonMessageBox.Show(this,
                    ex.Message,
                    "Actualizar",
                    KryptonMessageBoxButtons.OK,
                    KryptonMessageBoxIcon.Error);
            }
        }
    }
}

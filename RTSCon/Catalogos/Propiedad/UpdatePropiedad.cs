using Krypton.Toolkit;
using RTSCon.Datos;
using RTSCon.Negocios;
using System;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace RTSCon.Catalogos
{
    public partial class UpdatePropiedad : Form
    {
        private readonly NPropiedad _neg;
        private int _id;
        private byte[] _rowVersion;

        public UpdatePropiedad()
        {
            InitializeComponent();

            var cn = ConfigurationManager.ConnectionStrings["RTSCond"].ConnectionString;
            _neg = new NPropiedad(new DPropiedad(cn));

            // Al mostrarse, inicializamos UI y cargamos el registro
            this.Shown += (_, __) => InitUiAndLoad();

            var btnOk = FindCtrl<KryptonButton>("btnConfirmar", "btnGuardar", "btnOk");
            var btnBack = FindCtrl<KryptonButton>("btnVolver", "btnCancelar", "btnBack");
            if (btnOk != null) btnOk.Click += btnConfirmar_Click;
            if (btnBack != null) btnBack.Click += (_, __) => Close();

            var btnBuscarProp = FindCtrl<KryptonButton>("btnBuscarPropietario", "btnBuscar", "btnBuscarOwner");
            if (btnBuscarProp != null) btnBuscarProp.Click += btnBuscarPropietario_Click;

            SetKeyPressDecimal("txtPorcentaje");

            var txtUnidadId = FindCtrl<TextBoxBase>("txtIdPropiedad", "txtUnidadId");
            if (txtUnidadId != null) txtUnidadId.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    btnOk?.PerformClick();
                }
            };
            var txtPct = FindCtrl<TextBoxBase>("txtPorcentaje");
            if (txtPct != null) txtPct.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    btnOk?.PerformClick();
                }
            };
        }

        // ------------------ Helpers compartidos (igual que en CrearPropiedad) ------------------
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

        private int? GetIntFromTextOrTag(params string[] names)
        {
            var c = FindCtrl<Control>(names);
            if (c == null) return null;
            if (c.Tag is int t) return t;
            if (int.TryParse(c.Text?.Trim(), out var v)) return v;
            return null;
        }

        private void SetText(string value, params string[] names)
        {
            var c = FindCtrl<Control>(names);
            if (c != null) c.Text = value ?? "";
        }

        private void SetKeyPressDecimal(params string[] names)
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
        // ---------------------------------------------------------------------------------------

        /// <summary>
        /// Inicializa la UI y carga la propiedad según el Id que viene en Tag.
        /// (PropiedadRead hace: new UpdatePropiedad() { Tag = id } )
        /// </summary>
        private void InitUiAndLoad()
        {
            // Leer el Id desde this.Tag
            if (Tag == null)
            {
                KryptonMessageBox.Show(this, "No se recibió el Id de la propiedad.",
                    "Actualizar Propiedad", KryptonMessageBoxButtons.OK, KryptonMessageBoxIcon.Error);
                Close();
                return;
            }

            try
            {
                if (Tag is int i) _id = i;
                else if (!int.TryParse(Tag.ToString(), out _id))
                    throw new InvalidOperationException("Id de propiedad inválido.");
            }
            catch
            {
                KryptonMessageBox.Show(this, "Id de propiedad inválido.",
                    "Actualizar Propiedad", KryptonMessageBoxButtons.OK, KryptonMessageBoxIcon.Error);
                Close();
                return;
            }

            // Ajustes iniciales de UI igual que en CrearPropiedad
            var dtpIni = FindCtrl<DateTimePicker>("dtpFechaInicio", "dtpInicio");
            var dtpFin = FindCtrl<DateTimePicker>("dtpFechaFin", "dtpTermino", "dtpFin");
            if (dtpIni != null && dtpIni.Value == default) dtpIni.Value = DateTime.Today;
            if (dtpFin != null && dtpFin.Value == default) dtpFin.Value = DateTime.Today;

            var txtNom = FindCtrl<TextBoxBase>("txtNombrePropiedad", "txtNombre", "txtUnidadNombre");
            if (txtNom != null) txtNom.ReadOnly = false;

            var txtPropNom = FindCtrl<TextBoxBase>("txtPropietarioResponsable", "txtPropietarioNombre");
            var txtPropDoc = FindCtrl<TextBoxBase>("txtIdentificacionPropietario", "txtPropietarioIdentificacion");
            if (txtPropNom != null) txtPropNom.ReadOnly = true;
            if (txtPropDoc != null) txtPropDoc.ReadOnly = true;

            // Ahora sí cargamos datos de BD
            CargarDatos();
        }

        private void CargarDatos()
        {
            DataRow row = _neg.PorId(_id);
            if (row == null)
            {
                KryptonMessageBox.Show(this, "No se encontró la propiedad.",
                    "Actualizar Propiedad", KryptonMessageBoxButtons.OK, KryptonMessageBoxIcon.Error);
                Close();
                return;
            }

            // RowVersion
            if (row.Table.Columns.Contains("RowVersion") && row["RowVersion"] != DBNull.Value)
                _rowVersion = (byte[])row["RowVersion"];

            // Nombre
            if (row.Table.Columns.Contains("Nombre"))
                SetText(Convert.ToString(row["Nombre"]), "txtNombrePropiedad", "txtNombre", "txtUnidadNombre");

            // UnidadId
            if (row.Table.Columns.Contains("UnidadId"))
                SetText(Convert.ToString(row["UnidadId"]), "txtIdPropiedad", "txtUnidadId");

            // PropietarioId
            if (row.Table.Columns.Contains("PropietarioId"))
                SetText(Convert.ToString(row["PropietarioId"]), "txtPropietarioId");

            // Si tu consulta devuelve nombre/doc del propietario, los usamos (si no, simplemente se quedan en blanco)
            var txtPropNom = FindCtrl<TextBoxBase>("txtPropietarioResponsable", "txtPropietarioNombre");
            var txtPropDoc = FindCtrl<TextBoxBase>("txtIdentificacionPropietario", "txtPropietarioIdentificacion");
            if (txtPropNom != null && row.Table.Columns.Contains("PropietarioNombre"))
                txtPropNom.Text = Convert.ToString(row["PropietarioNombre"]);
            if (txtPropDoc != null && row.Table.Columns.Contains("PropietarioDocumento"))
                txtPropDoc.Text = Convert.ToString(row["PropietarioDocumento"]);

            // Porcentaje
            if (row.Table.Columns.Contains("Porcentaje") && row["Porcentaje"] != DBNull.Value)
                SetText(Convert.ToDecimal(row["Porcentaje"]).ToString(CultureInfo.CurrentCulture), "txtPorcentaje");

            // Titular principal
            var chkTit = FindCtrl<CheckBox>("chkTitularPrincipal", "chkEsTitularPrincipal");
            if (chkTit != null &&
                row.Table.Columns.Contains("EsTitularPrincipal") &&
                row["EsTitularPrincipal"] != DBNull.Value)
            {
                chkTit.Checked = Convert.ToBoolean(row["EsTitularPrincipal"]);
            }

            // Fechas
            var dtpIni = FindCtrl<DateTimePicker>("dtpFechaInicio", "dtpInicio");
            var dtpFin = FindCtrl<DateTimePicker>("dtpFechaFin", "dtpTermino", "dtpFin");

            if (dtpIni != null && row.Table.Columns.Contains("FechaInicio") && row["FechaInicio"] != DBNull.Value)
                dtpIni.Value = Convert.ToDateTime(row["FechaInicio"]);
            if (dtpFin != null && row.Table.Columns.Contains("FechaFin") && row["FechaFin"] != DBNull.Value)
                dtpFin.Value = Convert.ToDateTime(row["FechaFin"]);
        }

        private void btnBuscarPropietario_Click(object sender, EventArgs e)
        {
            using (var dlg = new RTSCon.Catalogos.Condominio.BuscarPropietario())
            {
                if (dlg.ShowDialog(this) != DialogResult.OK) return;

                var txtPropNom = FindCtrl<TextBoxBase>("txtPropietarioResponsable", "txtPropietarioNombre");
                var txtPropDoc = FindCtrl<TextBoxBase>("txtIdentificacionPropietario", "txtPropietarioIdentificacion");
                var txtPropId = FindCtrl<TextBoxBase>("txtPropietarioId");

                if (txtPropNom != null)
                {
                    txtPropNom.Text = dlg.SelectedUsuario;
                    txtPropNom.Tag = dlg.SelectedId;
                }
                if (txtPropDoc != null)
                    txtPropDoc.Text = dlg.SelectedCorreo;
                if (txtPropId != null)
                    txtPropId.Text = dlg.SelectedId.ToString();
            }
        }

        private void btnConfirmar_Click(object sender, EventArgs e)
        {
            try
            {
                // NOMBRE
                var nombre = GetText("txtNombrePropiedad", "txtNombre", "txtUnidadNombre");
                if (string.IsNullOrWhiteSpace(nombre))
                    throw new InvalidOperationException("Ingrese el nombre de la propiedad.");
                if (nombre.Length > 50)
                    throw new InvalidOperationException("El nombre no puede exceder 50 caracteres.");

                // Unidad (Id)
                var unidadId = GetIntFromTextOrTag("txtIdPropiedad", "txtUnidadId");
                if (unidadId is null || unidadId <= 0)
                    throw new InvalidOperationException("Ingrese el Id de la unidad (vivienda) válido.");

                // Propietario
                var propietarioId = GetIntFromTextOrTag("txtPropietarioId", "txtPropietarioResponsable", "txtPropietarioNombre");
                if (propietarioId is null || propietarioId <= 0)
                    throw new InvalidOperationException("Seleccione un propietario con el botón 'Buscar Propietario'.");

                // Porcentaje
                var sPct = GetText("txtPorcentaje");
                if (!decimal.TryParse(sPct, NumberStyles.Number, CultureInfo.CurrentCulture, out var porcentaje) ||
                    porcentaje <= 0 || porcentaje > 100)
                    throw new InvalidOperationException("Ingrese un porcentaje válido (0 < porcentaje ≤ 100).");

                // Titular principal
                var chkTit = FindCtrl<CheckBox>("chkTitularPrincipal", "chkEsTitularPrincipal");
                bool esTitular = chkTit?.Checked ?? false;

                // Fechas
                var dtpIni = FindCtrl<DateTimePicker>("dtpFechaInicio", "dtpInicio");
                var dtpFin = FindCtrl<DateTimePicker>("dtpFechaFin", "dtpTermino", "dtpFin");
                DateTime? fIni = dtpIni?.Value.Date;
                DateTime? fFin = dtpFin?.Value.Date;
                if (fIni.HasValue && fFin.HasValue && fIni.Value > fFin.Value)
                    throw new InvalidOperationException("La fecha de inicio no puede ser mayor que la fecha de terminación.");

                if (_rowVersion == null || _rowVersion.Length == 0)
                    throw new InvalidOperationException("No se pudo recuperar la versión de la fila (RowVersion).");

                var editor =
                    UserContext.Usuario ??
                    ConfigurationManager.AppSettings["DefaultEjecutor"] ??
                    "rtscon@local";

                _neg.Actualizar(
                    _id,
                    nombre,
                    propietarioId.Value,
                    unidadId.Value,
                    fIni, fFin,
                    porcentaje,
                    esTitular,
                    _rowVersion,
                    editor
                );

                KryptonMessageBox.Show(this, "Propiedad actualizada correctamente.",
                    "Actualizar Propiedad", KryptonMessageBoxButtons.OK, KryptonMessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                KryptonMessageBox.Show(this, ex.Message,
                    "Actualizar Propiedad", KryptonMessageBoxButtons.OK, KryptonMessageBoxIcon.Error);
            }
        }
    }
}

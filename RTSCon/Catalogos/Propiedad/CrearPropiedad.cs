using Krypton.Toolkit;
using RTSCon.Datos;
using RTSCon.Negocios;
using System;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace RTSCon.Catalogos
{
    public partial class CrearPropiedad : Form
    {
        private readonly NPropiedad _neg;

        public CrearPropiedad()
        {
            InitializeComponent();

            var cn = ConfigurationManager.ConnectionStrings["RTSCond"].ConnectionString;
            _neg = new NPropiedad(new DPropiedad(cn));

            this.Shown += (_, __) => InitUi();

            var btnOk = FindCtrl<KryptonButton>("btnConfirmar", "btnGuardar", "btnOk");
            var btnBack = FindCtrl<KryptonButton>("btnVolver", "btnCancelar", "btnBack");
            if (btnOk != null) btnOk.Click += btnConfirmar_Click;
            if (btnBack != null) btnBack.Click += (_, __) => Close();

            var btnBuscarProp = FindCtrl<KryptonButton>("btnBuscarPropietario", "btnBuscar", "btnBuscarOwner");
            if (btnBuscarProp != null) btnBuscarProp.Click += btnBuscarPropietario_Click;

            SetKeyPressDecimal("txtPorcentaje");

            var txtUnidadId = FindCtrl<TextBoxBase>("txtIdPropiedad", "txtUnidadId");
            if (txtUnidadId != null) txtUnidadId.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; btnOk?.PerformClick(); } };
            var txtPct = FindCtrl<TextBoxBase>("txtPorcentaje");
            if (txtPct != null) txtPct.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; btnOk?.PerformClick(); } };
        }

        // ------------------ Helpers ------------------
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
        // ---------------------------------------------

        private void InitUi()
        {
            var dtpIni = FindCtrl<DateTimePicker>("dtpFechaInicio", "dtpInicio");
            var dtpFin = FindCtrl<DateTimePicker>("dtpFechaFin", "dtpTermino", "dtpFin");
            if (dtpIni != null && dtpIni.Value == default) dtpIni.Value = DateTime.Today;
            if (dtpFin != null && dtpFin.Value == default) dtpFin.Value = DateTime.Today;

            // IMPORTANTE: el nombre AHORA es editable (ya no readonly)
            var txtNom = FindCtrl<TextBoxBase>("txtNombrePropiedad", "txtNombre", "txtUnidadNombre");
            if (txtNom != null) txtNom.ReadOnly = false;

            var txtPropNom = FindCtrl<TextBoxBase>("txtPropietarioResponsable", "txtPropietarioNombre");
            var txtPropDoc = FindCtrl<TextBoxBase>("txtIdentificacionPropietario", "txtPropietarioIdentificacion");
            if (txtPropNom != null) txtPropNom.ReadOnly = true;
            if (txtPropDoc != null) txtPropDoc.ReadOnly = true;
        }

        private void btnBuscarPropietario_Click(object sender, EventArgs e)
        {
            using (var dlg = new RTSCon.Catalogos.BuscarPropietario())
            {
                if (dlg.ShowDialog(this) != DialogResult.OK) return;

                var txtPropNom = FindCtrl<TextBoxBase>("txtPropietarioResponsable", "txtPropietarioNombre");
                var txtPropDoc = FindCtrl<TextBoxBase>("txtIdentificacionPropietario", "txtPropietarioIdentificacion");
                var txtPropId = FindCtrl<TextBoxBase>("txtPropietarioId");

                if (txtPropNom != null) { txtPropNom.Text = dlg.SelectedUsuario; txtPropNom.Tag = dlg.SelectedId; }
                if (txtPropDoc != null) txtPropDoc.Text = dlg.SelectedCorreo;
                if (txtPropId != null) txtPropId.Text = dlg.SelectedId.ToString();
            }
        }

        private void btnConfirmar_Click(object sender, EventArgs e)
        {
            try
            {
                // NOMBRE (nuevo campo NOT NULL)
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
                if (!decimal.TryParse(sPct, NumberStyles.Number, CultureInfo.CurrentCulture, out var porcentaje) || porcentaje <= 0 || porcentaje > 100)
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

                var creador = ConfigurationManager.AppSettings["DefaultEjecutor"] ?? "rtscon@local";

                int nuevoId = _neg.Insertar(
                    nombre,
                    propietarioId.Value,
                    unidadId.Value,
                    fIni, fFin,
                    porcentaje,
                    esTitular,
                    creador
                );

                KryptonMessageBox.Show(this, $"Propiedad registrada (Id {nuevoId}).",
                    "Crear Propiedad", KryptonMessageBoxButtons.OK, KryptonMessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                KryptonMessageBox.Show(this, ex.Message,
                    "Crear Propiedad", KryptonMessageBoxButtons.OK, KryptonMessageBoxIcon.Error);
            }
        }
    }
}

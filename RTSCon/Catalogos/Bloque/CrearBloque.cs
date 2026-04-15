using System;
using System.Data;
using System.Reflection;
using System.Windows.Forms;
using RTSCon.Datos;
using RTSCon.Negocios;

namespace RTSCon.Catalogos
{
    public partial class CrearBloque : Form
    {
        private int _condominioId;
        private readonly NBloque _nBloque;
        private readonly NCondominio _nCondominio;
        private bool _eventosInicializados;

        public CrearBloque(int condominioId)
        {
            InitializeComponent();

            _condominioId = condominioId;

            DBloque dBloque = new DBloque(Conexion.CadenaConexion);
            _nBloque = new NBloque(dBloque);

            DCondominio dCondominio = new DCondominio(Conexion.CadenaConexion);
            _nCondominio = new NCondominio(dCondominio);

            InicializarEventosUnaSolaVez();
        }

        private void InicializarEventosUnaSolaVez()
        {
            if (_eventosInicializados)
                return;

            Load -= CrearBloque_Load;
            Load += CrearBloque_Load;

            if (btnGuardar != null)
            {
                btnGuardar.Click -= btnGuardar_Click;
                btnGuardar.Click += btnGuardar_Click;
            }

            if (btnCancelar != null)
            {
                btnCancelar.Click -= btnCancelar_Click;
                btnCancelar.Click += btnCancelar_Click;
            }

            if (btnBuscarCondominio != null)
            {
                btnBuscarCondominio.Click -= btnBuscarCondominio_Click;
                btnBuscarCondominio.Click += btnBuscarCondominio_Click;
            }

            _eventosInicializados = true;
        }

        private void CrearBloque_Load(object sender, EventArgs e)
        {
            ConfigurarUi();
            CargarCondominio();
        }

        private void ConfigurarUi()
        {
            if (nudNumPisos != null)
            {
                nudNumPisos.Minimum = 1;
                if (nudNumPisos.Value < 1)
                    nudNumPisos.Value = 1;
            }

            if (nudUnidadesPiso != null)
            {
                nudUnidadesPiso.Minimum = 1;
                if (nudUnidadesPiso.Value < 1)
                    nudUnidadesPiso.Value = 1;
            }

            SetControlReadOnly("txtCondominioId", true);
            SetControlReadOnly("txtIdCondominio", true);
            SetControlReadOnly("txtCondominioNombre", true);
            SetControlReadOnly("txtNombreCondominio", true);
        }

        private void CargarCondominio()
        {
            if (_condominioId <= 0)
            {
                SetControlText("txtCondominioId", string.Empty);
                SetControlText("txtIdCondominio", string.Empty);
                SetControlText("txtCondominioNombre", string.Empty);
                SetControlText("txtNombreCondominio", string.Empty);
                return;
            }

            DataRow row = _nCondominio.PorId(_condominioId);
            if (row == null)
            {
                SetControlText("txtCondominioId", string.Empty);
                SetControlText("txtIdCondominio", string.Empty);
                SetControlText("txtCondominioNombre", string.Empty);
                SetControlText("txtNombreCondominio", string.Empty);
                return;
            }

            string nombreCondominio = Convert.ToString(row["Nombre"]) ?? string.Empty;
            string idTexto = _condominioId.ToString();

            SetControlText("txtCondominioId", idTexto);
            SetControlText("txtIdCondominio", idTexto);
            SetControlText("txtCondominioNombre", nombreCondominio);
            SetControlText("txtNombreCondominio", nombreCondominio);
        }

        private string ObtenerIdentificadorBloque()
        {
            string identificador = ObtenerControlText("txtIdentificador");

            if (string.IsNullOrWhiteSpace(identificador))
                identificador = ObtenerControlText("txtNombre");

            return identificador != null ? identificador.Trim() : string.Empty;
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                if (_condominioId <= 0)
                    throw new InvalidOperationException("Debe seleccionar un condominio.");

                string identificador = ObtenerIdentificadorBloque();
                if (string.IsNullOrWhiteSpace(identificador))
                    throw new InvalidOperationException("Debe ingresar el identificador del bloque. Ejemplo: Bloque A.");

                if (identificador.Length > 50)
                    throw new InvalidOperationException("El identificador no puede exceder 50 caracteres.");

                int numeroPisos = (int)nudNumPisos.Value;
                int unidadesPorPiso = (int)nudUnidadesPiso.Value;

                if (numeroPisos <= 0)
                    throw new InvalidOperationException("El número de pisos debe ser mayor que 0.");

                if (unidadesPorPiso <= 0)
                    throw new InvalidOperationException("Las unidades por piso deben ser mayores que 0.");

                string usuario = UserContext.Usuario;
                if (string.IsNullOrWhiteSpace(usuario))
                    usuario = "rtscon@local";

                _nBloque.Insertar(
                    _condominioId,
                    identificador,
                    numeroPisos,
                    unidadesPorPiso,
                    usuario);

                MessageBox.Show(
                    "Bloque creado correctamente.",
                    "Éxito",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No se pudo crear el bloque: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnBuscarCondominio_Click(object sender, EventArgs e)
        {
            using (BuscarCondominio frm = new BuscarCondominio())
            {
                if (frm.ShowDialog(this) != DialogResult.OK)
                    return;

                _condominioId = frm.CondominioIdSeleccionado;
                CargarCondominio();
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private Control BuscarControl(params string[] nombres)
        {
            int i;
            for (i = 0; i < nombres.Length; i++)
            {
                Control[] encontrados = Controls.Find(nombres[i], true);
                if (encontrados != null && encontrados.Length > 0)
                    return encontrados[0];
            }

            return null;
        }

        private void SetControlText(string nombre, string valor)
        {
            Control control = BuscarControl(nombre);
            if (control != null)
                control.Text = valor ?? string.Empty;
        }

        private string ObtenerControlText(string nombre)
        {
            Control control = BuscarControl(nombre);
            if (control == null)
                return string.Empty;

            return control.Text != null ? control.Text.Trim() : string.Empty;
        }

        private void SetControlReadOnly(string nombre, bool readOnly)
        {
            Control control = BuscarControl(nombre);
            if (control == null)
                return;

            PropertyInfo propiedad = control.GetType().GetProperty("ReadOnly");
            if (propiedad != null && propiedad.CanWrite)
                propiedad.SetValue(control, readOnly, null);
        }
    }
}
using Krypton.Toolkit;
using RTSCon.Datos;
using RTSCon.Negocios;
using System;
using System.Configuration;
using System.Windows.Forms;

namespace RTSCon
{
    public partial class CrearUsuario : KryptonForm
    {
        private readonly NAuth _auth;

        public CrearUsuario()
        {
            InitializeComponent();

            var cn = ConfigurationManager.ConnectionStrings["RTSCond"].ConnectionString;
            _auth = new NAuth(new DAuth(cn));
        }

        private void btnCrear_Click(object sender, EventArgs e)
        {
            try
            {
                string nombre = (txtNombreCompleto.Text ?? "").Trim();
                string clave = (txtContraseña.Text ?? "").Trim();
                string correo = (txtCorreo.Text ?? "").Trim();
                string rol = cmbRol.SelectedItem?.ToString() ?? "";
                string tipoDoc = cmbDocumento.SelectedItem?.ToString() ?? "";
                string documento =
                    tipoDoc == "Cedula" ? txtCedula.Text.Trim() :
                    tipoDoc == "Pasaporte" ? txtPasaporte.Text.Trim() :
                    "";

                // ===== Validaciones =====
                if (string.IsNullOrWhiteSpace(nombre))
                    throw new InvalidOperationException("Ingrese el nombre completo.");

                if (string.IsNullOrWhiteSpace(clave))
                    throw new InvalidOperationException("Ingrese la contraseña.");

                if (string.IsNullOrWhiteSpace(correo) || !correo.Contains("@"))
                    throw new InvalidOperationException("Ingrese un correo válido.");

                if (string.IsNullOrWhiteSpace(rol))
                    throw new InvalidOperationException("Seleccione el rol.");

                if (string.IsNullOrWhiteSpace(documento))
                    throw new InvalidOperationException("Ingrese el documento.");

                // ===== Crear usuario (BD) =====
                int nuevoUsuarioId;
                string mensajeResultado;

                bool creado = _auth.CrearUsuario(
    nombre,
    clave,
    correo,
    rol,
    tipoDoc,
    documento,
    out nuevoUsuarioId,
    out mensajeResultado
);


                if (!creado)
                    throw new InvalidOperationException(mensajeResultado);

                KryptonMessageBox.Show(
                    this,
                    "Usuario creado correctamente.",
                    "Crear Usuario",
                    KryptonMessageBoxButtons.OK,
                    KryptonMessageBoxIcon.Information
                );

                this.Close(); // vuelve al Dashboard
            }
            catch (Exception ex)
            {
                KryptonMessageBox.Show(
                    this,
                    ex.Message,
                    "Crear Usuario",
                    KryptonMessageBoxButtons.OK,
                    KryptonMessageBoxIcon.Error
                );
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

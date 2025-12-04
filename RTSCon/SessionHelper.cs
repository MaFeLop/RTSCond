using System.Linq;
using System.Windows.Forms;
using RTSCon.Negocios;

namespace RTSCon
{
    public static class SessionHelper
    {
        /// <summary>
        /// Cierra sesión solo desde el formulario actual
        /// y vuelve al Login.
        /// </summary>
        public static void LogoutFrom(Form current)
        {
            if (current == null) return;

            UserContext.Clear();

            var login = new Login();
            login.Show();

            current.Close();
        }

        /// <summary>
        /// Cierra sesión globalmente: limpia UserContext,
        /// abre Login y cierra todos los formularios excepto Login.
        /// </summary>
        public static void LogoutGlobal()
        {
            UserContext.Clear();

            var login = new Login();
            login.Show();

            var abiertos = Application.OpenForms.Cast<Form>().ToList();
            foreach (var f in abiertos)
            {
                if (!(f is Login))
                    f.Close();
            }
        }
    }
}

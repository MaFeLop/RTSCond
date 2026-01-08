using System.Linq;
using System.Windows.Forms;
using RTSCon.Negocios;

namespace RTSCon
{
    public static class SessionHelper
    {
        public static bool IsLoggingOut { get; private set; }

        public static void LogoutFrom(Form current)
        {
            if (current == null) return;

            IsLoggingOut = true;
            RTSCon.Negocios.UserContext.Clear();

            var login = new Login();
            login.Show();

            current.Close();
        }

        public static void LogoutGlobal()
        {
            IsLoggingOut = true;
            RTSCon.Negocios.UserContext.Clear();

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

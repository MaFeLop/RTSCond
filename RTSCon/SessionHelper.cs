using System.Linq;
using System.Windows.Forms;
using RTSCon.Negocios;

namespace RTSCon
{
    public static class SessionHelper
    {
        public static void LogoutGlobal()
        {
            UserContext.Clear();

            // Abre Login primero
            var login = new Login();
            login.Show();

            // Cierra todo lo demás
            var abiertos = Application.OpenForms.Cast<Form>().ToList();
            foreach (var f in abiertos)
            {
                if (!(f is Login))
                    f.Close();
            }
        }

        public static void LogoutFrom(Form current)
        {
            if (current == null) return;
            UserContext.Clear();
            var login = new Login();
            login.Show();
            current.Close();
        }
    }
}

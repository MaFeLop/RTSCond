using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace RTSCon.Configuración
{
    public static class AppCfg
    {
            public static readonly string MailProfile =
                ConfigurationManager.AppSettings["MailProfile"] ?? "RTSCondMail";

            public static readonly int CodigoMinutos =
                int.TryParse(ConfigurationManager.AppSettings["CodigoMinutos"], out var m) ? m : 5;

            public static readonly bool CodigoDebug =
                bool.TryParse(ConfigurationManager.AppSettings["CodigoDebug"], out var b) && b;

            public static readonly string DefaultEjecutor =
                ConfigurationManager.AppSettings["DefaultEjecutor"] ?? "rtscon@local";

            public static readonly int SessionIdleMinutes =
                int.TryParse(ConfigurationManager.AppSettings["SessionIdleMinutes"], out var i) ? i : 30;

            public static readonly int SessionPromptMinutes =
                int.TryParse(ConfigurationManager.AppSettings["SessionPromptMinutes"], out var p) ? p : 25;
    }
}

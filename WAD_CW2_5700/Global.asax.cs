using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace WAD_CW2_5700
{
    public class MvcApplication : System.Web.HttpApplication
    {

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            var config = new NLog.Config.LoggingConfiguration();

            var logfile = new NLog.Targets.FileTarget("logfile")
            {
                FileName = "${basedir}/Content/Logs/${shortdate} - ${level}.txt",
                Layout = "${longdate} - ${message}"
            };

            config.AddRule(LogLevel.Info, LogLevel.Fatal, logfile);

            LogManager.Configuration = config;
        }
    }
}

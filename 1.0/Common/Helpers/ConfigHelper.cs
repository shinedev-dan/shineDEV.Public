using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Configuration;
using System.Web;

namespace Common.Helpers
{
    public class ConfigHelper
    {
        public class Environment
        {
            public const String DEV = "DEV";
            public const String TEST = "TEST";
            public const String PROD = "PROD";
        }

        public static SmtpSection SmtpSection { get { return ConfigurationManager.GetSection("system.net/mailSettings/smtp") as SmtpSection; } }

        public class AppSettings
        {
            public static String Environment { get { return ConfigurationManager.AppSettings["config:environment"].ToString(); } }
            public static String SmtpDefaultFrom { get { return ConfigurationManager.AppSettings["config:smtp:defaultFrom"].ToString(); } }
        }
    }
}
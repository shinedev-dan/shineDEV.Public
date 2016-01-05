using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

using Kusog.Mvc;

namespace PublicSite.App_Start
{
    public class PluginFrameworkConfig
    {
        public static void SetupAppFramework(BundleCollection bundles)
        {
            PluginBase.PluginBaseApplication.SetupApplication(bundles);
            PluginBase.PluginBaseApplication.Instance.defineWidgetContainer(new WidgetContainer("rightSidebar",
                new WidgetContainer.WidgetDetails("SimpleWidget"), new WidgetContainer.WidgetDetails("SimpleWidget2")));
        }
    }
}
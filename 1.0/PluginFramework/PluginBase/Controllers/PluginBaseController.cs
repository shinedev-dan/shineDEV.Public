using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

using Kusog.Mvc;

namespace PluginBase.Controllers
{
    [DynamicActionFilter]
    public class PluginBaseController : Controller
    {
        public ActionResult MetaTags()
        {
            return PartialView();
        }

        public ActionResult HeaderIncludesPre()
        {
            return Content(PluginBaseApplication.Instance.HeaderIncludesPre);
        }
        public ActionResult HeaderIncludesPost()
        {
            return Content(PluginBaseApplication.Instance.HeaderIncludesPost);
        }
        public ActionResult FooterIncludes()
        {
            return Content(PluginBaseApplication.Instance.FooterIncludes);
        }
        public ActionResult WidgetContainer(string containerName)
        {
            ViewBag.ContainerName = containerName;
            return PartialView();
        }

        public ActionResult Widget(string widgetId, object widgetOptions)
        {
            ViewBag.widgetId = widgetId;
            ViewBag.widgetOptions = widgetOptions;
            return PartialView();
        }

        public ActionResult PageFooter()
        {
            return Content("");
        }
    }
}

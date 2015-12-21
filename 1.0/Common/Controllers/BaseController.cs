using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Common.Controllers
{
    public class BaseController : Controller
    {
        public static String DefaultLayout { get { return "~/Views/Shared/_ModernBusiness.cshtml"; } }

        public static void CleanseViewData(ViewDataDictionary vd)
        {
            if (vd.ContainsKey("PageTitle"))
                vd.Remove("PageTitle");

            if (vd.ContainsKey("PageDescription"))
                vd.Remove("PageDescription");
        }

        public void AddInfo(String message)
        {
            if (TempData.ContainsKey(MessageType.Info))
                ((List<String>)TempData[MessageType.Info]).Add(message);
            else
                TempData.Add(MessageType.Info, new List<String>(new[] { message }));
        }

        public void AddError(String message)
        {
            if (TempData.ContainsKey(MessageType.Error))
                ((List<String>)TempData[MessageType.Error]).Add(message);
            else
                TempData.Add(MessageType.Error, new List<String>(new[] { message }));
        }

        public void AddSuccess(String message)
        {
            if (TempData.ContainsKey(MessageType.Success))
                ((List<String>)TempData[MessageType.Success]).Add(message);
            else
                TempData.Add(MessageType.Success, new List<String>(new[] { message }));
        }

        public class MessageType
        {
            public const String Info = "messagetype-info";
            public const String Success = "messagetype-success";
            public const String Error = "messagetype-error";
        }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace Common.Attributes
{
    public class CustomRemoteAttribute : RemoteAttribute
    {
        //private static Lazy<Dictionary<String, Type>> ControllerList = new Lazy<Dictionary<String, Type>>(() =>
        //{
        //    Dictionary<String, Type> lookup = new Dictionary<String, Type>(StringComparer.OrdinalIgnoreCase);
        //    var types = Assembly.GetCallingAssembly().GetTypes().Where(type => type.IsSubclassOf(typeof(Controller)) && !type.IsAbstract);
        //    foreach (var type in types)
        //    {
        //        var idx = type.FullName.IndexOf("Areas.");
        //        if (idx != -1)
        //        {   //we have an area name
        //            var endIdx = type.FullName.IndexOf('.', idx + 6);
        //            string area = type.FullName.Substring(idx + 6, endIdx - (idx + 6));
        //            lookup.Add(area + ":" + type.Name, type);
        //        }
        //        else
        //        {
        //            lookup.Add(":" + type.Name, type);
        //        }
        //    }
        //    return lookup;
        //}, isThreadSafe: true);

        //protected CustomRemoteAttribute()
        //{
        //}

        //public CustomRemoteAttribute(string routeName)
        //    : base(routeName)
        //{
        //}

        //public CustomRemoteAttribute(string action, string controller)
        //    : base(action, controller)
        //{
        //}

        //public CustomRemoteAttribute(string action, string controller, string areaName)
        //    : base(action, controller, areaName)
        //{
        //}

        //protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        //{
        //    string[] additionalFields = this.AdditionalFields.Split(',');

        //    List<object> propValues = new List<object>();
        //    propValues.Add(value);
        //    foreach (string additionalField in additionalFields)
        //    {
        //        PropertyInfo prop = validationContext.ObjectType.GetProperty(additionalField);
        //        if (prop != null)
        //        {
        //            object propValue = prop.GetValue(validationContext.ObjectInstance, null);
        //            propValues.Add(propValue);
        //        }
        //    }
        //    var context = HttpContext.Current;
        //    var httpContext = context.Request.RequestContext.HttpContext;
        //    var controllerName = this.RouteData["controller"].ToString();
        //    var areaName = (this.RouteData["area"] ?? HttpContext.Current.Request.RequestContext.RouteData.DataTokens["Area"]).ToString();
        //    var actionName = this.RouteData["action"].ToString();

        //    string key = String.Format("{0}:{1}Controller", areaName, controllerName);
        //    Type controllerType = null;
        //    if (ControllerList.Value.TryGetValue(key, out controllerType))
        //    {
        //        object instance;
        //        if (controllerType.GetConstructor(new Type[] { typeof(ApplicationUserManager) }) != null)
        //        {
        //            instance = Activator.CreateInstance(controllerType, context.GetOwinContext().GetUserManager<ApplicationUserManager>());
        //        }
        //        else
        //        {
        //            instance = Activator.CreateInstance(controllerType);
        //        }

        //        MethodInfo method = controllerType.GetMethod(actionName);

        //        if (method != null)
        //        {
        //            if (instance is Site.Controllers.BaseController)
        //                ((Site.Controllers.BaseController)instance).Initialize(context.Request.RequestContext);
        //            else
        //            {
        //                var initMethod = controllerType.GetMethod("Initialize", BindingFlags.Instance | BindingFlags.NonPublic);
        //                if (initMethod != null)
        //                    initMethod.Invoke(instance, new[] { context.Request.RequestContext });
        //            }
        //            var response = (JsonResult)method.Invoke(instance, propValues.ToArray());

        //            if (response.Data.GetType() == typeof(Boolean))
        //            {
        //                if ((bool)response.Data == true)
        //                    return ValidationResult.Success;
        //                else
        //                    return new ValidationResult(String.Format(base.ErrorMessageString, validationContext.DisplayName), new[] { validationContext.MemberName });
        //            }
        //            else
        //                return new ValidationResult(response.Data.ToString());
        //        }
        //    }

        //    return null;
        //}
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace Common.Helpers
{
    public class ListHelper
    {
        public enum DateFormats
        {
            [Description("yyyy-MM-dd")]
            yyyyMMdd,
            [Description("MM-dd-yyyy")]
            MMddyyyy,
            [Description("dd-MM-yyyy")]
            ddMMyyyy,
        };

        public enum TimeFormats
        {
            [Description("HH:mm")]
            hhmm,
            [Description("HH:mm tt")]
            hhmmtt,
        };

        #region Select Lists
        public static List<SelectListItem> EnumSelectList(Type enumType, string defaultText = null)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            if (!string.IsNullOrWhiteSpace(defaultText))
                items.Add(new SelectListItem { Text = defaultText, Value = "" });
            Array values = Enum.GetValues(enumType);

            foreach (var i in values)
            {
                string description = string.Empty;
                FieldInfo fi = enumType.GetField(i.ToString());
                DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attributes != null && attributes.Length > 0)
                    description = attributes[0].Description;
                else
                    description = i.ToString();
                items.Add(new SelectListItem
                {
                    Text = description,
                    Value = ((int)i).ToString()
                });
            }

            return items;
        }

        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }
        #endregion
    }
}
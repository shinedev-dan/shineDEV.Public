using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AccountPlugin.Models.Owin
{
    public class Constants
    {
        internal const string SiteUserKeyTemplate = "SiteUsers/{0}";
        internal const string SiteUserLoginKeyTemplate = "SiteUserLogins/{0}/{1}";
        internal const string SiteUserEmailKeyTemplate = "SiteUserEmails/{0}";
        internal const string SiteUserPhoneNumberKeyTemplate = "SiteUserPhoneNumbers/{0}";
    }
}
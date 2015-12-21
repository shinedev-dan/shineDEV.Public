//Copyright 2012-2013 Kusog Software, inc. (http://kusog.org)
//This file is part of the ASP.NET Mvc Plugin Framework.
// == BEGIN LICENSE ==
//
// Licensed under the terms of any of the following licenses at your
// choice:
//
//  - GNU General Public License Version 3 or later (the "GPL")
//    http://www.gnu.org/licenses/gpl.html
//
//  - GNU Lesser General Public License Version 3 or later (the "LGPL")
//    http://www.gnu.org/licenses/lgpl.html
//
//  - Mozilla Public License Version 1.1 or later (the "MPL")
//    http://www.mozilla.org/MPL/MPL-1.1.html
//
// == END LICENSE ==
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Optimization;

using System.Web.Mvc;

namespace Kusog.Mvc
{
    public class BaseMvcPlugin : IMvcPlugin
    {
        public static string StandardViewLocation = "/Views.{1}.{0}.cshtml";
        protected IMvcPluginApplication m_app = null;

        public BaseMvcPlugin(bool ensureStandardViewLocation = true)
        {
            CssPre = new List<SiteResource>();
            CssPost = new List<SiteResource>();
            JavaScript = new List<SiteResource>();
            RazorViewLocations = new List<string>();
            FooterJavaScript = new List<SiteResource>();
            Widgets = new List<Widget>();
            ActionFilters = new List<DynamicActionFilter>();

            if (ensureStandardViewLocation)
                ensureViewLocation(BaseMvcPlugin.StandardViewLocation);
        }

        public List<SiteResource> CssPre { get; private set; }
        public List<SiteResource> CssPost { get; private set; }
        public List<SiteResource> JavaScript { get; private set; }
        public List<SiteResource> FooterJavaScript { get; private set; }

        public List<string> RazorViewLocations { get; private set; }

        public List<Widget> Widgets { get; private set; }

        public List<DynamicActionFilter> ActionFilters { get; private set; }

        public virtual void SetupExtensions(IMvcPluginApplication app)
        {
            m_app = app;
        }


        protected void addLocalCss(string css)
        {
            CssPost.Add(new SiteResource() {IsAssemblyResource = true, LocalUrl = calcBaseLocationName() + css });
        }

        protected void addFooterScript(string name, string url, IEnumerable<string> depends)
        {
            FooterJavaScript.Add(new SiteResource() { IsAssemblyResource = true, LocalUrl = calcBaseLocationName() + url });
        }

        protected void addHeaderScript(string name, string url, IEnumerable<string> depends = null)
        {
            JavaScript.Add(new SiteResource() { IsAssemblyResource = true, LocalUrl = calcBaseLocationName() + url });
        }

        protected void addHeaderScriptBlock(string name, string script, IEnumerable<string> depends = null)
        {
            JavaScript.Add(new SiteResource() { IsInline = true, ResourceContent = script });
        }

        protected void addFooterScriptBlock(string name, string script, IEnumerable<string> depends = null)
        {
            FooterJavaScript.Add(new SiteResource() { IsInline = true, ResourceContent = script });
        }

        protected void DefineWidget(Widget widget)
        {
            Widgets.Add(widget);
        }

        protected void AddActionFilter(string action, string controller, IActionFilter filter)
        {
            ActionFilters.Add(new DynamicActionFilter() { Action = action, Controller = controller, Filter = filter });
        }

        /// <summary>
        /// A plugin should register all the base view locations.
        /// </summary>
        /// <param name="localName"></param>
        protected void ensureViewLocation(string localName)
        {
            string ln = "/" + localName.Substring(1).Replace('/', '.');
            string name = calcBaseLocationName() + ln;
            if(!RazorViewLocations.Contains(name))
                RazorViewLocations.Add(name);
        }

        protected string calcBaseLocationName()
        {
            string assemblyName = this.GetType().Assembly.FullName;
            int i = assemblyName.IndexOf(",");
            if (i >= 0)
                assemblyName = assemblyName.Substring(0, i);
            string newName = "~/Plugins/" + assemblyName;
            return newName;
        }

        public IMvcPluginApplication App {get{return m_app;}}


        protected static string s_ScriptLink = "<script src='{0}' ></script>";
        protected static string s_ScriptBlockLink = "<script type='text/javascript'>{0}</script>";
        protected static string s_CssLink = "<link href='{0}' media='screen' rel='stylesheet' type='text/css' />";
        public void getFooterResources(StringBuilder footerOut)
        {
            addScriptResources(footerOut, FooterJavaScript);
        }

        public void getHeaderResourcesPre(StringBuilder headerOut)
        {
            foreach (SiteResource rsc in CssPre)
                headerOut.Append(string.Format(s_CssLink, rsc.IsAssemblyResource ? (rsc.ResourceUrl + "?" + CalcCacheBuster()) : rsc.ResourceUrl));
        }

        public void getHeaderResourcesPost(StringBuilder headerOut)
        {
            foreach (SiteResource rsc in CssPost)
                headerOut.Append(string.Format(s_CssLink, rsc.IsAssemblyResource ? (rsc.ResourceUrl + "?" + CalcCacheBuster()) : rsc.ResourceUrl));
            addScriptResources(headerOut, JavaScript);
        }

        protected void addScriptResources(StringBuilder htmlOut, List<SiteResource> scripts)
        {
            //TODO: Very close to making bundles work from plugins.  For now its disabled, but should consider this a top priority to stay
            //in pariety with VS2012 MVC4 Project Templates.

            //App.Bundles.Add(new ScriptBundle("~/bundles/syrinxslideshow").Include(
            //            "~/Plugins/SyrinxSlideshowPlugin/scripts.jquery.syrinx-slideshow-.08.js",
            //            "~/Plugins/SyrinxSlideshowPlugin/scripts.jquery.syrinx-slideshow-controllers-.02.js",
            //            "~/Plugins/SyrinxSlideshowPlugin/scripts.jquery.syrinx-slideshow-editor-.05.js",
            //            "~/Plugins/SyrinxSlideshowPlugin/scripts.jquery.syrinx-slideshow-mvc.01.js"
            //));

            foreach (SiteResource rsc in scripts)
                if (rsc.IsInline)
                    htmlOut.AppendFormat(s_ScriptBlockLink, rsc.ResourceContent);
                else
                    htmlOut.AppendFormat(s_ScriptLink, rsc.IsAssemblyResource ? (rsc.ResourceUrl + "?" + CalcCacheBuster()) : rsc.ResourceUrl);
        }

        static Dictionary<string, string> s_assemblies = new Dictionary<string, string>();
        protected string CalcCacheBuster()
        {
            string loc = this.GetType().Assembly.Location;
            lock (s_assemblies)
            {
                if (s_assemblies.ContainsKey(loc))
                    return s_assemblies[loc];

                System.IO.FileInfo fileInfo = new System.IO.FileInfo(loc);
                DateTime lastModified = fileInfo.LastWriteTime;
                string val = null;
                s_assemblies[loc] = val = string.Format("v={0}", lastModified.Ticks);
                return val;
            }
            
        }
    }
}

﻿//Copyright 2012-2013 Kusog Software, inc. (http://kusog.org)
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
using System.Threading.Tasks;

using Kusog.Mvc;
using Syrinx2;

namespace PluginBase
{
    public class PluginBaseApplication : BaseMvcPluginApplication
    {
        public enum BaseAppResourceTypes { HeaderIncPre, HeaderIncPost, FooterInc, Widget, MenuItem };

        #region Plugin Menu Support
        public class PluginMenuItem
        {
            public IMvcPlugin Plugin { get; set; }
            public IBasicMenuItem MenuItem { get; set; }
            public string[] Views { get; set; }
            public string MenuName { get; set; }
            public string MenuLocation { get; set; }
        }


        public Dictionary<PluginMenuItem, string> m_registeredMenuItems = new Dictionary<PluginMenuItem, string>();
        public void registerMenuItem(string menuName, string location, IBasicMenuItem item, IMvcPlugin plugin, string[] views)
        {
            if (string.IsNullOrWhiteSpace(menuName))
                menuName = "main";

            if (views == null || views.Length == 0)
                views = new string[] { "*" };

            lock (m_registeredMenuItems)
            {
                m_registeredMenuItems.Add(new PluginMenuItem() { Plugin = plugin, MenuItem = item, Views = views, MenuLocation = location, MenuName = menuName }, location);
            }
        }

        /// <summary>
        /// Called by a view that has a syrinx menu that it wants the application to populate
        /// </summary>
        /// <param name="menu"></param>
        /// <param name="viewName"></param>
        public void addMenuItems(Syrinx2.MvcMenu menu, string menuName, string viewName)
        {

            foreach (KeyValuePair<PluginMenuItem, string> kvp in m_registeredMenuItems)
            {
                if ((kvp.Key.Views.Contains("*") || kvp.Key.Views.Contains(viewName)) && 
                    string.Compare(kvp.Key.MenuName, menuName, true) == 0 && 
                    ShouldIncludeResource(BaseAppResourceTypes.MenuItem, kvp.Key.Plugin, kvp.Key.MenuItem))
                {
                    string[] n = kvp.Value.Split('/');
                    List<IBasicMenuItem> items = menu.Items;
                    foreach (string piece in n)
                    {
                        bool matched = false;
                        foreach (IBasicMenuItem mi in items)
                        {
                            if (matched = (mi is IMenuItem && string.Compare(mi.ID, piece, true) == 0))
                            {
                                items = ((IMenuItem)mi).Items;
                                break;
                            }
                        }
                        if (!matched)
                        {
                            MenuItem newMi = new MenuItem() { ID = piece, Text = piece };
                            items.Add(newMi);
                            items = newMi.Items;
                        }
                    }
                    items.Add(kvp.Key.MenuItem);
                }
            }
        }
        #endregion

        public static new PluginBaseApplication Instance 
        { 
            get { return BaseMvcPluginApplication.Instance as PluginBaseApplication; } 
            set { BaseMvcPluginApplication.Instance = value; } 
        }


        protected override bool shouldIncludeResourceCore(BaseMvcPluginApplication.ResourceTypes type, IMvcPlugin plugin)
        {
            return ShouldIncludeResource((BaseAppResourceTypes)type, plugin, null);
        }

        protected virtual bool ShouldIncludeResource(BaseAppResourceTypes type, IMvcPlugin plugin, object resource)
        {
            bool should = true;
            if (plugin != null)
            {
                if (should && plugin is BaseAppPlugin)
                    should = ((BaseAppPlugin)plugin).ShouldIncludeResource(type, resource);
            }

            return should;
        }


        public static PluginBaseApplication SetupApplication(object bundles)
        {
            PluginBaseApplication me = new PluginBaseApplication(bundles);
            return me;
        }

        protected PluginBaseApplication(object bundles)
            :base(bundles)
        {
        }
    }
}

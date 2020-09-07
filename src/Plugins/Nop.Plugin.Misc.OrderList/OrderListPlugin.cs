using System.Linq;
using Nop.Core;
using Nop.Services.Plugins;
using Nop.Web.Framework.Menu;

namespace Nop.Plugin.Misc.OrderList
{
    public class OrderListPlugin : BasePlugin, IAdminMenuPlugin
    {
        private readonly IWebHelper _webHelper;

        public OrderListPlugin(IWebHelper webHelper)
        {
            _webHelper = webHelper;
        }

        public bool Authenticate()
        {
            return true;
        }

        public void ManageSiteMap(SiteMapNode rootNode)
        {
            var menuItem = new SiteMapNode()
            {
                SystemName = "Misc.OrderList",
                Title = "Orders",
                ControllerName = "MiscOrders",
                ActionName = "ShowOrdersMenu",
                Visible = true,
                RouteValues = new Microsoft.AspNetCore.Routing.RouteValueDictionary() { { "area", "Admin" } },
            };
            var pluginNode = rootNode.ChildNodes.FirstOrDefault(x => x.SystemName == "Third party plugins");
            if (pluginNode != null)
                pluginNode.ChildNodes.Add(menuItem);
            else
                rootNode.ChildNodes.Add(menuItem);
        }

        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/MiscOrders/Configure";
        }

        public override void Install()
        {
            base.Install();
        }

        public override void Uninstall()
        {
            base.Uninstall();
        }
    }
}

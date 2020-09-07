using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Misc.OrderList.Models;
using Nop.Services.ExportImport;
using Nop.Services.Helpers;
using Nop.Services.Orders;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Misc.OrderList.Controllers
{
    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    public class MiscOrdersController : BasePluginController
    {
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IExportManager _exportManager;
        private readonly IOrderService _orderService;

        public MiscOrdersController(
            IDateTimeHelper dateTimeHelper,
            IExportManager exportManager,
            IOrderService orderService
        )
        {
            _dateTimeHelper = dateTimeHelper;
            _orderService = orderService;
            _exportManager = exportManager;
        }

        public ActionResult ShowOrdersMenu()
        {
            System.Diagnostics.Debug.WriteLine("Call ShowOrdersMenu");
            return View("~/Plugins/Misc.OrderList/Views/OrdersIndex.cshtml");
        }

        public IActionResult Configure()
        {
            return Configure();
        }

        [HttpPost, ActionName("GetOrderList")]
        public virtual IActionResult GetOrderList(OrderPlugin order)
        {
            System.Diagnostics.Debug.WriteLine("Call GetOrderList (OrderPlugin): {0}", order.CreatedFromUtc);
            var startDateValue = (DateTime?)_dateTimeHelper.ConvertToUtcTime(order.CreatedFromUtc, _dateTimeHelper.CurrentTimeZone);

            var endDateValue = (DateTime?)_dateTimeHelper.ConvertToUtcTime(order.CreatedToUtc, _dateTimeHelper.CurrentTimeZone).AddDays(1);

            // List<Order> orders = _orderService.getAll
            var orders = _orderService.SearchOrders(storeId: 0,
                vendorId: 0,
                productId: 0,
                warehouseId: 0,
                paymentMethodSystemName: null,
                createdFromUtc: startDateValue,
                createdToUtc: endDateValue,
                osIds: null,
                psIds: null,
                ssIds: null,
                billingPhone: null,
                billingEmail: null,
                billingLastName: null,
                billingCountryId: 0,
                orderNotes: null);

            System.Diagnostics.Debug.WriteLine("======> GetOrderList ");

            if (!orders.Any())
            {
                System.Diagnostics.Debug.WriteLine("======> GetOrderList (Nothing)");
                return RedirectToAction("ShowOrdersMenu");
            }

            try
            {
                var bytes = _exportManager.ExportOrdersToXlsx(orders);
                return File(bytes, MimeTypes.TextXlsx, "orders.xlsx");
            }
            catch (Exception exc)
            {
                System.Diagnostics.Debug.WriteLine("======> GetOrderList (catch error) {0}", exc);
                return RedirectToAction("ShowOrdersMenu");
            }
        }
    }
}

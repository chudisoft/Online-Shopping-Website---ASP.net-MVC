using Microsoft.AspNet.Identity;
using Online_Shopping_Website.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Online_Shopping_Website.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public async Task<ActionResult> Index()
        {
            var userId = User.Identity.GetUserId();
            Order order = db.Orders.FirstOrDefault(x => x.CustomerId == userId && x.Status == OrderStatuses.InCart);
            if (order == null)
                order = new Order();
            List<OrderHistory> cartItems = db.OrderHistories.Where(x => x.OrderId == order.Id).ToList();
            ViewBag.cartItems = cartItems;
            ViewBag.order = order;
            ViewBag.categories = GetCategories();
            return View(await db.Products.ToListAsync());
        }

        [HttpPost, Authorize]
        //public async Task<ActionResult> AddToCart()
        public JsonResult AddToCart(List<CartItem> cartItems)
        {
            if (cartItems == null)
                return Json(new { status = "Select at least one product.", 
                    result = false });

            if (cartItems.Count <= 0)
                return Json(new { status = "Select at least one product.", 
                    result = false });


            var userId = User.Identity.GetUserId();
            Order order = db.Orders.FirstOrDefault(x => x.CustomerId == userId && x.Status == OrderStatuses.InCart);
            if (order == null)
                order = new Order();
            //order.History = new IEnumerable<OrderHistory>();
            List<OrderHistory> history = new List<OrderHistory>();

            order.Status = OrderStatuses.InCart;
            order.OrderDate = DateTime.UtcNow;
            order.CustomerId = User.Identity.GetUserId();

            if (order.Id == 0)
                db.Orders.Add(order);

            db.SaveChanges();

            List<OrderHistory> oldItems = db.OrderHistories.Where(x => x.OrderId == order.Id).ToList();
            List<OrderHistory> oldItemsToRemove = new List<OrderHistory>();

            foreach (var item in oldItems)
            {
                var oldItem = cartItems.FirstOrDefault(x=>x.Id == item.Id);
                if (oldItems == null)
                    oldItemsToRemove.Add(item);
            }
            //oldItems.RemoveRange(oldItemsToRemove);
            List<Product> products = db.Products.ToList();
            foreach (var item in cartItems)
            {
                var oldItem = oldItems.FirstOrDefault(x=>x.Id == item.Id);
                if (oldItem != null)
                {
                    oldItem.Quantity = item.Qty;
                    oldItem.Price = products.Find(x=>x.Id == oldItem.ProductId).Price;
                }
                else
                    history.Add(new OrderHistory()
                    {
                        ProductId = item.Id,
                        Quantity = item.Qty,
                        OrderId = order.Id,
                        Price = products.Find(x=>x.Id == item.Id).Price
                    });
            }
            db.OrderHistories.AddRange(history);

            db.OrderHistories.RemoveRange(oldItemsToRemove);
            db.SaveChanges();

            return Json(new { status = "success", result = true });
        }

        private dynamic GetCategories()
        {
            var categories = db.Categories.ToList();
            var _categories = new List<SelectListItem>();
            foreach (var item in categories)
            {
                _categories.Add(
                    new SelectListItem
                    {
                        Text = item.Name,
                        Value = item.Id.ToString()
                    }
                );
            }
            return _categories;
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
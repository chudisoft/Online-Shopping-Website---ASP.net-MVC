using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Online_Shopping_Website.Models;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json.Linq;

namespace Online_Shopping_Website.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Orders
        public async Task<ActionResult> Index()
        {
            var orders = await db.Orders.ToListAsync(); //.Include(x=>x.History).ToListAsync();
            var orderHistories = await db.OrderHistories.ToListAsync();
            foreach (var item in orders)
            {
                item.History = orderHistories.Where(x => x.OrderId == item.Id);
            }
            return View(orders);
        }

        // GET: Orders/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = await db.Orders.FindAsync(id);
            if (order == null)
            {
                return HttpNotFound();
            }

            order.History = db.OrderHistories.Where(x => x.OrderId == order.Id);
            var Products = db.Products.ToList();
            foreach (var item in order.History)
            {
                item.Product = Products.Find(x=>x.Id == item.ProductId);
            }

            return View(order);
        }

        // GET: Orders/Checkout
        public ActionResult Checkout()
        {
            var userId = User.Identity.GetUserId();
            var cart = db.Orders.FirstOrDefault(x=>x.CustomerId == userId && x.Status == OrderStatuses.InCart);
            if (cart == null)
                return RedirectToAction("Index", "Home");

            List<OrderHistory> cartItems = db.OrderHistories.Where(x=>x.OrderId == cart.Id).ToList();
            ViewBag.cartItems = cartItems;
            ViewBag.cartItemsCount = cartItems.Count();
            return View(cart);
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //public JsonResult Create(Card card, List<CartItem> cartitems)
        public JsonResult Create(CheckoutModel checkoutModel){

            if (checkoutModel.cartitems == null)
                return Json(new
                {
                    status = "Select at least one product.",
                    result = false
                });

            if (checkoutModel.cartitems.Count <= 0)
                return Json(new
                {
                    status = "Select at least one product.",
                    result = false
                });


            var userId = User.Identity.GetUserId();
            Order order = db.Orders.FirstOrDefault(x => x.CustomerId == userId && x.Status == OrderStatuses.InCart);
            if (order == null)
                return Json(new
                {
                    status = "No item is in cart.",
                    result = false
                });


            order.Number = checkoutModel.card.number;
            order.Name = checkoutModel.card.name;
            order.CVV = checkoutModel.card.cvv;
            order.Expiry = checkoutModel.card.expiry;

            order.Status = OrderStatuses.Approved;
            order.OrderDate = DateTime.UtcNow;
            order.CustomerId = User.Identity.GetUserId();

            db.SaveChanges();

            List<OrderHistory> oldItems = db.OrderHistories.Where(x => x.OrderId == order.Id).ToList();
            List<OrderHistory> oldItemsToRemove = new List<OrderHistory>();

            foreach (var item in oldItems)
            {
                var oldItem = checkoutModel.cartitems.FirstOrDefault(x => x.Id == item.Id);
                if (oldItems == null)
                    oldItemsToRemove.Add(item);
            }

            List<Product> products = db.Products.ToList();
            foreach (var item in checkoutModel.cartitems)
            {
                var oldItem = oldItems.FirstOrDefault(x => x.Id == item.Id);
                if (oldItems != null)
                {
                    oldItem.Quantity = item.Qty;
                    oldItem.Price = products.Find(x => x.Id == oldItem.ProductId).Price;
                }
            }

            if(oldItemsToRemove.Count > 0)
                db.OrderHistories.RemoveRange(oldItemsToRemove);
            db.SaveChanges();

            return Json(new { status = "success", result = true });
        }


        // GET: Orders/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = await db.Orders.FindAsync(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,CustomerId,status,OrderDate")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Entry(order).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(order);
        }

        // GET: Orders/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = await db.Orders.FindAsync(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete"), Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Order order = await db.Orders.FindAsync(id);
            db.Orders.Remove(order);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

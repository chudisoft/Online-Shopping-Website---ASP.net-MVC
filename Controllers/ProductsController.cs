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
using System.Web.UI.WebControls;
using System.Data.Entity.Validation;

namespace Online_Shopping_Website.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Products
        public async Task<ActionResult> Index()
        {
            ViewBag.categories = GetCategories();
            return View(await db.Products.ToListAsync());
        }

        // GET: Products/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = await db.Products.FindAsync(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: Products/Create
        public ActionResult Create()
        {
            ViewBag.categories = GetCategories();

            return View();
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

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public async Task<ActionResult> Create([Bind(Include = "Id,Name,CategoryId,Description,Quantity_Available,ExpiryDate")] Product product)
        public async Task<ActionResult> Create(Product product)
        {
            if (ModelState.IsValid)
            {
                if (Request.Files.Count > 0)
                    if (Request.Files["file"] != null)
                        if (Request.Files["file"].ContentLength > 0)
                {
                    if (!System.IO.Directory.Exists(Server.MapPath("~/Uploads")))
                        System.IO.Directory.CreateDirectory(Server.MapPath("~/Uploads"));

                    if (!System.IO.Directory.Exists(Server.MapPath("~/Uploads/Products"))) 
                        System.IO.Directory.CreateDirectory(Server.MapPath("~/Uploads/Products"));

                        product.FileType = Request.Files[0].ContentType;
                    string ImageName = $"~/Uploads/Products/{Guid.NewGuid().ToString()}" +
                        Request.Files[0].FileName.Substring(Request.Files[0].FileName.LastIndexOf("."));
                    product.FileName = Request.Files[0].FileName;
                    product.FilePath = ImageName;
                    Request.Files[0].SaveAs(Server.MapPath(ImageName));
                }
                try
                {
                    db.Products.Add(product);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                catch (DbEntityValidationException e)
                {
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                            eve.Entry.Entity.GetType().Name, eve.Entry.State);
                        foreach (var ve in eve.ValidationErrors)
                        {
                            Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                ve.PropertyName, ve.ErrorMessage);
                        }
                    }
                    throw;
                }
            }

            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = await db.Products.FindAsync(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.categories = GetCategories();
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,Description,Quantity_Available,ExpiryDate")] Product product)
        {
            if (ModelState.IsValid)
            {
                if (Request.Files.Count > 0)
                    if (Request.Files["file"] != null)
                        if (Request.Files["file"].ContentLength > 0)
                {
                    if (!System.IO.Directory.Exists(Server.MapPath("~/Uploads")))
                        System.IO.Directory.CreateDirectory(Server.MapPath("~/Uploads"));

                    if (!System.IO.Directory.Exists(Server.MapPath("~/Uploads/Products")))
                        System.IO.Directory.CreateDirectory(Server.MapPath("~/Uploads/Products"));

                    if (!string.IsNullOrEmpty(product.FilePath))
                    {
                        if (System.IO.File.Exists(Server.MapPath(product.FilePath)))
                            System.IO.File.Delete(Server.MapPath(product.FilePath));
                    }

                    product.FileType = Request.Files[0].ContentType;
                    string ImageName = $"~/Uploads/Products/{Guid.NewGuid().ToString()}" +
                        Request.Files[0].FileName.Substring(Request.Files[0].FileName.LastIndexOf("."));
                    product.FileName = Request.Files[0].FileName;
                    product.FilePath = ImageName;
                    Request.Files[0].SaveAs(Server.MapPath(ImageName));
                }
                db.Entry(product).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = await db.Products.FindAsync(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Product product = await db.Products.FindAsync(id);

            if (!string.IsNullOrEmpty(product.FilePath))
            {
                if (System.IO.File.Exists(Server.MapPath(product.FilePath)))
                    System.IO.File.Delete(Server.MapPath(product.FilePath));
            }

            db.Products.Remove(product);
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

using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ExamApp.Models;

namespace ExamApp.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private MyExamEntities db = new MyExamEntities();

        // GET: Product
        // GET: Product/Details/5
        public ActionResult Details(byte? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var product = db.Products.Find(id);
            if (product == null)
                return HttpNotFound();

            return View(product);
        }

        public ActionResult Index()
        {
            var products = db.Products.ToList();
            return View(products);
        }

        // GET: Product/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Product/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,MedicineName,StripPrice,StripSize,UnitPrice,ImagePath,StockQuantity")] Product product, HttpPostedFileBase Image)
        {
            try
            {
                if (db.Products.Any(p => p.MedicineName == product.MedicineName))
                    ModelState.AddModelError("MedicineName", "Product already exists");

                if (product.StripPrice <= 0)
                    ModelState.AddModelError("StripPrice", "StripPrice must be greater than zero");

                if (product.UnitPrice <= 0)
                    ModelState.AddModelError("UnitPrice", "UnitPrice must be greater than zero");

                if (product.StripSize <= 0)
                    ModelState.AddModelError("StripSize", "StripSize must be greater than zero");

                if (ModelState.IsValid)
                {
                    // Image Upload
                    if (Image != null && Image.ContentLength > 0)
                    {
                        if (Image.ContentLength > 1 * 1024 * 1024)
                        {
                            ModelState.AddModelError("Image", "Image size must be less than 1MB");
                            return View(product);
                        }

                        string folder = Server.MapPath("~/Content/Images/");
                        if (!System.IO.Directory.Exists(folder))
                            System.IO.Directory.CreateDirectory(folder);

                        string fileName = Guid.NewGuid() + "-" + System.IO.Path.GetFileName(Image.FileName);
                        string path = System.IO.Path.Combine(folder, fileName);
                        Image.SaveAs(path);

                        product.ImagePath = "/Content/Images/" + fileName;
                    }

                    // Save stock in units
                    if (product.StockQuantity.HasValue && product.StripSize.HasValue)
                        product.StockQuantity = product.StockQuantity.Value * product.StripSize.Value;

                    db.Products.Add(product);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error: " + ex.Message);
            }

            return View(product);
        }

        // GET: Product/Edit/5
        public ActionResult Edit(byte? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var product = db.Products.Find(id);
            if (product == null) return HttpNotFound();
            return View(product);
        }

        // POST: Product/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,MedicineName,StripPrice,StripSize,UnitPrice,ImagePath,StockQuantity")] Product product, HttpPostedFileBase Image)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (Image != null && Image.ContentLength > 0)
                    {
                        if (Image.ContentLength > 1 * 1024 * 1024)
                        {
                            ModelState.AddModelError("Image", "Image size must be less than 1MB");
                            return View(product);
                        }

                        string folder = Server.MapPath("~/Content/Images/");
                        if (!System.IO.Directory.Exists(folder))
                            System.IO.Directory.CreateDirectory(folder);

                        string fileName = Guid.NewGuid() + "-" + System.IO.Path.GetFileName(Image.FileName);
                        string path = System.IO.Path.Combine(folder, fileName);
                        Image.SaveAs(path);

                        product.ImagePath = "/Content/Images/" + fileName;
                    }

                    db.Entry(product).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error: " + ex.Message);
            }

            return View(product);
        }

        // GET: Product/Delete/5
        public ActionResult Delete(byte? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var product = db.Products.Find(id);
            if (product == null) return HttpNotFound();
            return View(product);
        }

        // POST: Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(byte id)
        {
            var product = db.Products.Find(id);
            db.Products.Remove(product);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // Add Stock (from Index modal)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddStock(byte? productId, int? stripQty)
        {
            if (productId.HasValue && stripQty.HasValue)
            {
                var product = db.Products.Find(productId.Value);
                if (product != null)
                {
                    int unitsToAdd = stripQty.Value * (product.StripSize ?? 1);
                    product.StockQuantity = (product.StockQuantity ?? 0) + unitsToAdd;
                    db.Entry(product).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}

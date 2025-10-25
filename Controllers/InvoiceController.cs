using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using ExamApp.Models;
using PagedList;

namespace ExamApp.Controllers
{
    [Authorize]
    public class InvoiceController : Controller
    {
        private MyExamEntities db = new MyExamEntities();
        // GET: Invoice/Details/5
        public ActionResult Details(int id)
        {
            var invoice = db.Patientmasters
                .Include(p => p.InvoiceDetails.Select(d => d.Product))
                .FirstOrDefault(p => p.InvoiceId == id);

            if (invoice == null)
                return HttpNotFound();

            // এখানে PartialView return করো
            return PartialView("_DetailsPartial", invoice);
        }



        public ActionResult Index(string sortOrder, string currentFilter, string searchString, string searchDate, int? page)
        {
            ViewBag.CurrentSort = sortOrder; // For pager links
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : ""; // Toggle ascending/descending
            ViewBag.CurrentFilter = searchString;
            ViewBag.CurrentDateFilter = searchDate;

            var invoices = db.Patientmasters.Include(p => p.InvoiceDetails.Select(d => d.Product)).AsQueryable();

            // Search by customer name
            if (!string.IsNullOrEmpty(searchString))
                invoices = invoices.Where(i => i.CustomerName.ToLower().Contains(searchString.ToLower()));

            // Search by invoice date
            if (DateTime.TryParse(searchDate, out DateTime date))
                invoices = invoices.Where(i => DbFunctions.TruncateTime(i.InvoiceDate) == date.Date);

            // Sort logic
            switch (sortOrder)
            {
                case "name_desc":
                    invoices = invoices.OrderByDescending(i => i.CustomerName);
                    break;
                default:
                    invoices = invoices.OrderBy(i => i.CustomerName);
                    break;
            }

            int pageSize = 3;
            int pageNumber = (page ?? 1);
            return View(invoices.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Create()
        {
            ViewBag.Products = db.Products.ToList();
            return View();
        }



        // POST: Invoice/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
       
        public ActionResult Create(Patientmaster invoice, List<byte> ProductIds, List<int?> StripQuantities, List<int?> UnitQuantities)
        {
            if (ModelState.IsValid)
            {
                invoice.InvoiceDate = DateTime.Now;
                invoice.InvoiceDetails = new List<InvoiceDetail>();

                for (int i = 0; i < ProductIds.Count; i++)
                {
                    int stripQty = StripQuantities[i] ?? 0;
                    int unitQty = UnitQuantities[i] ?? 0;

                    if (stripQty > 0 || unitQty > 0)
                    {
                        var product = db.Products.Find(ProductIds[i]);
                        if (product != null)
                        {
                            // Create InvoiceDetail
                            var detail = new InvoiceDetail
                            {
                                ItemId = product.Id,
                                Product = product,
                                StripQuantity = stripQty,
                                UnitQuantity = unitQty,
                              //  TotalAmount = (stripQty * product.StripPrice) + (unitQty * product.UnitPrice)
                            };
                            invoice.InvoiceDetails.Add(detail);

                            // Reduce stock (unit-wise)
                            int unitsSold = (stripQty * (product.StripSize ?? 1)) + unitQty;
                            product.StockQuantity = (product.StockQuantity ?? 0) - unitsSold;
                            if (product.StockQuantity < 0) product.StockQuantity = 0; // prevent negative stock

                            db.Entry(product).State = System.Data.Entity.EntityState.Modified;
                        }
                    }
                }

                db.Patientmasters.Add(invoice);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Products = db.Products.ToList();
            return View(invoice);
        }


        // GET: Invoice/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var invoice = db.Patientmasters.Include(p => p.InvoiceDetails.Select(d => d.Product))
                                           .FirstOrDefault(p => p.InvoiceId == id);
            if (invoice == null) return HttpNotFound();

            ViewBag.Products = db.Products.ToList();
            return View(invoice);
        }

        // POST: Invoice/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Patientmaster invoice, List<int> DetailIds, List<byte> ProductIds, List<int?> StripQuantities, List<int?> UnitQuantities)
        {
            if (ModelState.IsValid)
            {
                var dbInvoice = db.Patientmasters.Include(p => p.InvoiceDetails).FirstOrDefault(p => p.InvoiceId == invoice.InvoiceId);
                if (dbInvoice != null)
                {
                    dbInvoice.CustomerName = invoice.CustomerName;
                    dbInvoice.Salesman = invoice.Salesman;

                    // Delete old details safely
                    foreach (var oldDetail in dbInvoice.InvoiceDetails.ToList())
                    {
                        db.InvoiceDetails.Remove(oldDetail);
                    }

                    // Add updated details
                    for (int i = 0; i < ProductIds.Count; i++)
                    {
                        if ((StripQuantities[i] ?? 0) > 0 || (UnitQuantities[i] ?? 0) > 0)
                        {
                            var product = db.Products.Find(ProductIds[i]);
                            var detail = new InvoiceDetail
                            {
                                ItemId = product.Id,
                                Product = product,
                                StripQuantity = StripQuantities[i],
                                UnitQuantity = UnitQuantities[i]
                            };
                            dbInvoice.InvoiceDetails.Add(detail);
                        }
                    }

                    db.SaveChanges();
                    return RedirectToAction("Index");
               }
            }

            ViewBag.Products = db.Products.ToList();
            return View(invoice);
        }

        // GET: Invoice/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var invoice = db.Patientmasters.Include(p => p.InvoiceDetails.Select(d => d.Product))
                                           .FirstOrDefault(p => p.InvoiceId == id);
            if (invoice == null) return HttpNotFound();

            return View(invoice);
        }

        // POST: Invoice/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var invoice = db.Patientmasters.Include(p => p.InvoiceDetails).FirstOrDefault(p => p.InvoiceId == id);

            // Delete child details first
            foreach (var detail in invoice.InvoiceDetails.ToList())
            {
                db.InvoiceDetails.Remove(detail);
            }

            db.Patientmasters.Remove(invoice);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}

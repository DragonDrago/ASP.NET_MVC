using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DAL.Models;
using DAL.Repositories;
using NLog;
using PagedList;

namespace _00005700_WAD_CW2.Controllers
{
    [Authorize]
    public class ProductsController : Controller
    {
        private ProductRepository products = new ProductRepository();
        private CategoryRepository _categoryRepo = new CategoryRepository();
        private static Logger logger = LogManager.GetLogger("WebSite");

        // GET: Products
        public ActionResult Index( string sortProduct, string searchProduct,string Filter ,int? page)
        {
            ViewBag.CurrentSort = sortProduct;
            ViewBag.ProductNameSortParm = String.IsNullOrEmpty(sortProduct) ? "name_desc" : "";
            ViewBag.ProductCategorySortParm = String.IsNullOrEmpty(sortProduct) ? "category_desc" : "";
            ViewBag.PriceSortParm = sortProduct=="Price" ? "price_desc" : "Price";
            var allProduct = from s in products.GetAll() select s;

            if (searchProduct != null)
            {
                page = 1;
            }
            else
            {
                searchProduct =Filter;
            }

            ViewBag.CurrentFilter = searchProduct;



            switch (sortProduct)
            {
                case "name_desc":
                    allProduct = allProduct.OrderByDescending(s => s.ProductName);
                    break;
                case "category_desc":
                    allProduct = allProduct.OrderByDescending(s => s.CategoryId);
                    break;
                case "Price":
                    allProduct = allProduct.OrderBy(s => s.Price);
                    break;
                case "price_desc":
                    allProduct = allProduct.OrderByDescending(s => s.Price);
                    break;
                default:
                    allProduct = allProduct.OrderBy(s => s.ProductName);
                    break;
            }

            if (!String.IsNullOrEmpty(searchProduct))
            {
                allProduct = allProduct.Where(s => s.ProductName.Contains(searchProduct)
                                       || s.Category.CategoryType.Contains(searchProduct));
            }

            int pageSize = 3;
            int pageNumber = (page ?? 1);
            return View(allProduct.ToPagedList(pageNumber, pageSize));

            
            
        }

        // GET: Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = products.GetById(id.Value);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: Products/Create
        public ActionResult Create()
        {

            ViewBag.Category = _categoryRepo.GetAll();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Product product, HttpPostedFileBase imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile?.ContentLength > 0)
                {
                    using (var stream = new MemoryStream())
                    {
                        imageFile.InputStream.CopyTo(stream);
                        product.Image = stream.ToArray();
                    }
                }
                products.Create(product);
                return RedirectToAction("Index");
            }
            ViewBag.CategoryId = new SelectList(_categoryRepo.GetAll(), "Id", "CategoryType", product.CategoryId);
            ViewBag.Category = _categoryRepo.GetAll();
            logger.Info(product.ProductName + " have created successfully");
            return View(product);
        }

        // GET: Products/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = products.GetById(id.Value);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryId = new SelectList(_categoryRepo.GetAll(), "Id", "CategoryType", product.CategoryId);
            ViewBag.Category = _categoryRepo.GetAll();
            logger.Info(product.ProductName + " have edited successfully");
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Product product, HttpPostedFileBase imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile?.ContentLength > 0)
                {
                    using (var stream = new MemoryStream())
                    {
                        imageFile.InputStream.CopyTo(stream);
                        product.Image = stream.ToArray();
                    }
                }
                products.Update(product);
                return RedirectToAction("Index");
            }
            ViewBag.CategoryId = new SelectList(_categoryRepo.GetAll(), "Id", "CategoryType", product.CategoryId);
            ViewBag.Category = _categoryRepo.GetAll();
            return View(product);
        }

        // GET: Products/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = products.GetById(id.Value);
            if (product == null)
            {
                return HttpNotFound();
            }
            logger.Info(product.ProductName + " have deleted successfully");

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            products.Delete(id);

            return RedirectToAction("Index");
        }

        [AllowAnonymous]
        public FileResult ProductImage(int id)
        {
            Product product = products.GetById(id);
            if (product != null && product.Image?.Length > 0)
            {
                return File(product.Image, "image/jpeg", product.ProductName + "-" + product.Id + ".jpg");
            }
            return null;
        }

    }
}
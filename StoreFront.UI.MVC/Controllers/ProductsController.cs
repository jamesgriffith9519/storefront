using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using StoreFront.DATA.EF;
using StoreFront.UI.MVC.Models;

namespace StoreFront.UI.MVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductsController : Controller
    {
        private StoreFrontEntities db = new StoreFrontEntities();

        // GET: Products
        [AllowAnonymous]
        public ActionResult Index()
        {
            var products = db.Products.Include(p => p.Category).Include(p => p.Manufacturer);
            //return View(products.ToList());
            return View(products.OrderBy(p => p.CategoryID != 3).ThenBy(p => p.CategoryID != 1).ToList());
        }

        // GET: Products/Details/5
        [AllowAnonymous]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        #region Custom-Add-To-Cart-Functionality (called from Books/Details View)

        [HttpPost]
        [AllowAnonymous]
        public ActionResult AddToCart(int qty, int productId)
        {
            //create the local version of our shopping cart. it will be of type DICTIONARY<int, ShoppingCartViewModel> - Key, value pairs to ensure uniqeuness -- in this case do not list the same product more than once in the collection.

            Dictionary<int, ShoppingCartViewModel> shoppingCart = null;

            //we need to see if cart items are already stored in session, if so -- add them to local version above. If not, just add the new stuff to this local cart. 

            if (Session["cart"] != null)
            {
                //if we landed here: then there is soemthign in the cart already!
                shoppingCart = (Dictionary<int, ShoppingCartViewModel>)Session["cart"];
                //session variables are stored as objects - to get them back to the original type
                //we need to explicitly cast to that type - called uboxing.

            }
            else
            {
                //if we landed here: nothing is in in the cart. 
                //create a empty Local Version
                shoppingCart = new Dictionary<int, ShoppingCartViewModel>();
                //at this point cart is no longer NULL but it has 0 items in it. 
            }

            //get the product being displayed in the view 

            Product product = db.Products.Where(x => x.ProductID == productId).FirstOrDefault();
            //we know that this where will result in max 1 item (0 items if not match because bad id), avoid getting back a collection from LINQ.

            if (product == null)
            {
                //bad id provided - give them error for it or send them back to the home page. 
                return RedirectToAction("index");
            }
            else
            {
                //good id, we have a book - add cart item & quantity.
                ShoppingCartViewModel item = new ShoppingCartViewModel(qty, product);

                //if the item is already in the cart, just increase the qty:code below.
                if (shoppingCart.ContainsKey(product.ProductID))
                {
                    shoppingCart[product.ProductID].Qty += qty;
                }
                else
                {
                    shoppingCart.Add(product.ProductID, item);
                }
                //now that the item has been added to the local cart, update the session cart with the new item/qty.

                Session["cart"] = shoppingCart;


                //bonus: provide confirmation info regarding items added to the cart.
                Session["confirm"] = string.Format($"{qty}{product.ProductName}" + $"{((qty > 1) ? "were" : " was ")} added to your cart");


            }


            return RedirectToAction("index", "ShoppingCart");
        }

        #endregion

        // GET: Products/Create
        public ActionResult Create()
        {
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "Category_Name");
            ViewBag.ManufacturerID = new SelectList(db.Manufacturers, "ManufacturerID", "Manu_Name");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProductID,ProductName,ManufacturerID,CategoryID,Price,UnitsInStock,IsActive,Image_Name,Description")] Product product, HttpPostedFileBase fupImage)
        {
            if (ModelState.IsValid)
            {
                string imageName = "noImage.png";//set a variable for image file name. default for no image.
                if (fupImage != null)
                {
                    //get the fileName (for extension)
                    imageName = fupImage.FileName;
                    //get the file extension from that 
                    string ext = imageName.Substring(imageName.LastIndexOf("."));

                    //create a safelist or (whitelist) of extensions 
                    string[] goodExts = new string[] { ".jpg", ".jpeg", ".png", ".gif" , ".PNG"};

                    //only use this file if it meets our extension criteria

                    if (goodExts.Contains(ext.ToLower()))
                    {
                        //make sure filename is unique to our system. otherwise we just overwrote a previous records image. 
                        //easiest and most reliable technique is GUID + extension.
                        imageName = Guid.NewGuid().ToString() + ext;
                        //drop that file into the correct folder in the website.
                        fupImage.SaveAs(Server.MapPath("~/Content/images/database lawn mower images/" + imageName));
                    }
                    //if the block above failed to run they gave us a file with an extension not approved above. , lots of options to handle this, we could supply error message.
                    //here we will just ignore the file and set this to default no image...
                    else
                    {
                        imageName = "noImage.png";
                        
                    }

                }
                //no matter whether they uploaded image or not, assign db records book image value to imageName

                product.Image_Name = imageName;

                db.Products.Add(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            //if you landed here, invalid form data. send them back to the view with validation messages , but not before building the code to setup the drop down list for lookup tables
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "Category_Name", product.CategoryID);
            ViewBag.ManufacturerID = new SelectList(db.Manufacturers, "ManufacturerID", "Manu_Name", product.ManufacturerID);
            return View(product);
        }

        // GET: Products/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "Category_Name", product.CategoryID);
            ViewBag.ManufacturerID = new SelectList(db.Manufacturers, "ManufacturerID", "Manu_Name", product.ManufacturerID);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProductID,ProductName,ManufacturerID,CategoryID,Price,UnitsInStock,IsActive,Image_Name,Description")] Product product, HttpPostedFileBase fupImage)
        {
            if (ModelState.IsValid)
            {
                //get the fileName (for extension)
                string imageName = fupImage.FileName;
                //get the file extension from that 
                string ext = imageName.Substring(imageName.LastIndexOf("."));

                //create a safelist or (whitelist) of extensions 
                string[] goodExts = new string[] { ".jpg", ".jpeg", ".png", ".gif" };

                //only use this file if it meets our extension criteria

                if (goodExts.Contains(ext.ToLower()))
                {
                    //make sure filename is unique to our system. otherwise we just overwrote a previous records image. 
                    //easiest and most reliable technique is GUID + extension.
                    imageName = Guid.NewGuid().ToString() + ext;
                    //drop that file into the correct folder in the website.
                    fupImage.SaveAs(Server.MapPath("~/content/images/database lawn mower images/" + imageName));



                    //once this is all done, we should delete the OLD image on file (to keep database size from getting gigantic) for good housekeeping. 
                    if (product.Image_Name != null && product.Image_Name != "noImage.png")
                    {
                        //delete it
                        System.IO.File.Delete(Server.MapPath("~/content/images/database lawn mower images/" + product.Image_Name));
                    }
                    //moved the db record image value assignment here. only needed if new image. 
                    product.Image_Name = imageName;





                }
                //if the block above failed to run they gave us a file with an extension not approved above. , lots of options to handle this, we could supply error message.
                //here we will just ignore the file and set this to default no image...
                //else
                //{
                //    imageName = "noImage.png";
                    
                    
                //}
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
        
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "Category_Name", product.CategoryID);
            ViewBag.ManufacturerID = new SelectList(db.Manufacturers, "ManufacturerID", "Manu_Name", product.ManufacturerID);
            return View(product);
        }

        // GET: Products/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);

            if (product.Image_Name != null && product.Image_Name != "noImage.png")
            {
                //delete it
                System.IO.File.Delete(Server.MapPath("~/content/images/database lawn mower images/" + product.Image_Name));
            }




            db.Products.Remove(product);
            db.SaveChanges();
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

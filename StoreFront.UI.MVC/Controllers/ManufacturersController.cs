using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using StoreFront.DATA.EF;

namespace StoreFront.UI.MVC.Controllers
{
    public class ManufacturersController : Controller
    {
        private StoreFrontEntities db = new StoreFrontEntities();

        // GET: Manufacturers
        public ActionResult Index()
        {
            return View(db.Manufacturers.ToList());
        }

        // GET: Manufacturers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Manufacturer manufacturer = db.Manufacturers.Find(id);
            if (manufacturer == null)
            {
                return HttpNotFound();
            }
            return View(manufacturer);
        }

        // GET: Manufacturers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Manufacturers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ManufacturerID,Manu_Name,Origin_Country")] Manufacturer manufacturer)
        {
            if (ModelState.IsValid)
            {
                db.Manufacturers.Add(manufacturer);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(manufacturer);
        }

        // GET: Manufacturers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Manufacturer manufacturer = db.Manufacturers.Find(id);
            if (manufacturer == null)
            {
                return HttpNotFound();
            }
            return View(manufacturer);
        }

        // POST: Manufacturers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ManufacturerID,Manu_Name,Origin_Country")] Manufacturer manufacturer)
        {
            if (ModelState.IsValid)
            {
                db.Entry(manufacturer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(manufacturer);
        }

        // GET: Manufacturers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Manufacturer manufacturer = db.Manufacturers.Find(id);
            if (manufacturer == null)
            {
                return HttpNotFound();
            }
            return View(manufacturer);
        }

        // POST: Manufacturers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Manufacturer manufacturer = db.Manufacturers.Find(id);
            db.Manufacturers.Remove(manufacturer);
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




        //-----------------Below are AJAX Actions--------------------//

                    //DELETE
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult AjaxCreate(Manufacturer manufacturer)
        {
            
            db.Manufacturers.Add(manufacturer);
            db.SaveChanges();//actually insert new record to the database. 
            return Json(manufacturer);//send back all that info on the new record as JSON data for the browser to add as a row to the HTML table. 

            //--Create a partial view (AuthorCreate.cshtml) 
            //-template/scaffolding: Create for Author
            //-data context class: BookStoreEntities
            //--check "Create as Partial View"


        }//End AjaxCreate

                    //CREATE
        [AcceptVerbs(HttpVerbs.Post)]//post for ajax
        public JsonResult AjaxDelete(int id)
        {

            //these 3 lines below delete the record. 

            Manufacturer manufacturer = db.Manufacturers.Find(id);
            db.Manufacturers.Remove(manufacturer); //this queues up the deletion of record.
            db.SaveChanges();

            //these last 2 lines setup the Json data that is sent back for JS manipulation in the browser. The browser page is/was live the whole time. 
            string confirmMessage = $"Deleted Manufacturer: '{manufacturer.Manu_Name}' from the database!";
            return Json(new { id = id, message = confirmMessage });
            //for the Json data being send back. we are using property equals(=) value pairs. in many cases we can call these properties whatever we want. 
        }//end AjaxDelete


        [HttpGet]
        public PartialViewResult ManufacturerDetails(int id)
        {
            //this is one way to return a partialview -- as the result of an action.
            //the other way to do it is like we have in the layout --> @Html.Partial("_LoginPartial")

            //here: a button requests details for a manufacturers, we send back a HTML-ss-js-images content block that will be placed into a popup/dialog.
            Manufacturer manufacturer = db.Manufacturers.Find(id);
            return PartialView(manufacturer);
            //create a partial view called ManufacturerDetails.cshtml
            //-template: details for manufacturer
            //-check "create as partial view"
            //-minor tweaks to the content
        }//end ManufacturerDetails (Ajax)



        [HttpGet]
        public PartialViewResult ManufacturerEdit(int id)
        {
            Manufacturer manufacturer = db.Manufacturers.Find(id);
            return PartialView(manufacturer);
            //Create partial view(PublishersEdit.cshtml):
            //  -template: Edit for Publisher
            //  - data context class: BookStoreEntities
            //  -check "Create as partial view"
            //  -minor tweaks to content
        }

        //edits pub record. returns pub's data as JSON.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult AjaxEdit(Manufacturer manufacturer)
        {
            db.Entry(manufacturer).State = EntityState.Modified;
            db.SaveChanges();
            return Json(manufacturer);
        }



    }//end Namespace


}

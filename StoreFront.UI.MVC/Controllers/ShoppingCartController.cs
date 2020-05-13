using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StoreFront.UI.MVC.Models;//for view models.shopping cart view model.
using StoreFront.DATA.EF;//for domain models. -- book


namespace StoreFront.UI.MVC.Controllers
{
    public class ShoppingCartController : Controller
    {
        // GET: ShoppingCart - index view will be generated with list of shopping cart view model[nodatacontexclass] - with customizations
        public ActionResult Index()
        {
            //var products = db.Products.Include(p => p.Category).Include(p => p.Manufacturer);
            //goal: show all shopping cart items or message if none.
            //step 1: get the session cart into a local variable. 
            var shoppingCart = (Dictionary<int, ShoppingCartViewModel>)Session["cart"];

            if (shoppingCart == null || shoppingCart.Count == 0)
            {
                //nothing here to work with, so new up the shoppingcart and pass it to the view so it wont fail as null. 
                shoppingCart = new Dictionary<int, ShoppingCartViewModel>();
                ViewBag.Message = "There are no items in our cart....";
            }
            else
            {
                ViewBag.Message = null;
            }
            return View(shoppingCart);//no need to repopulate the session cart, it's still the same.
        }

        [HttpPost]
        public ActionResult UpdateCart(int productId, int qty)
        {
            //getcart from session & hold it in a local dictionary
            Dictionary<int, ShoppingCartViewModel> shoppingCart =
                (Dictionary<int, ShoppingCartViewModel>)Session["cart"];

            // update the qty locally - for correct item (row where update button was clicked)
            shoppingCart[productId].Qty = qty;

            //return local cart back to session for use
            Session["cart"] = shoppingCart;

            //set up VB msg if no items in cart
            if (shoppingCart.Count == 0)
            {
                ViewBag.Message = "No Items exist in your cart";
            }

            //Back to cartitems
            return RedirectToAction("Index");
        }

        public ActionResult RemoveFromCart(int id)
        {
            //get cart out of session
            Dictionary<int, ShoppingCartViewModel> shoppingCart =
                (Dictionary<int, ShoppingCartViewModel>)Session["cart"];

            //remove the item
            shoppingCart.Remove(id);

            //set up VB msg if no items in cart
            if (shoppingCart.Count == 0)
            {
                ViewBag.Message = "No Items exist in your cart";
            }

            //Back to cartitems
            return RedirectToAction("Index");
        }
    }
}
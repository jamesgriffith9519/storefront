using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using StoreFront.DATA.EF;//access to domain models
using System.ComponentModel.DataAnnotations;//added for model metaDATA


namespace StoreFront.UI.MVC.Models
{
    public class ShoppingCartViewModel
    {
        [Range(1, int.MaxValue)]//ensured values are greater than 0
        public int Qty { get; set; }

        public Product Product { get; set; }




        public ShoppingCartViewModel(int qty, Product product)//fqctor
        {
            Qty = qty;
            Product = product;
        }
    }//end class
}//end namespace

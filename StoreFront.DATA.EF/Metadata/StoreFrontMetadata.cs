using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace StoreFront.DATA.EF/*.Metadata*/
{
   public class ProductsMetadata
    {
        [Required]
        [Display(Name = "Model")]
        public string ProductName { get; set; }

        [Required]
        [Range(1, 9)]
        [Display(Name = "Manufacturer")]
        public int ManufacturerID { get; set; }

        [Required]
        [Range(1, 3)]
        [Display(Name = "Type")]
        public int CategoryID { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:c}")]
        [Range(0, double.MaxValue)]
        [Display(Name = "Price")]
        public decimal Price { get; set; }

        [Required]
        [Display(Name = "Available Units")]
        public int UnitsInStock { get; set; }

        [Range(0, 1)]
        [Required]
        public bool IsActive { get; set; }

        [StringLength(100, ErrorMessage = "*Must be 100 characters or less")]
        [Display(Name ="Equipment")]
        public string Image_Name { get; set; }

        [UIHint("MultilineText")]
        [Display(Name = "Description")]
        public string Description { get; set; }
    }

    [MetadataType(typeof(ProductsMetadata))]
    public partial class Product { }

    public class ManufacturerMetadata
    {
        //[Required]
        //public int ManufacturerID { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "100 characters or less")]
        [Display(Name ="Manufacturer")]
        public string Manu_Name { get; set; }

        
        [StringLength(50, ErrorMessage = "50 characters or less")]
        public string Origin_Country { get; set; }
    }
    [MetadataType(typeof(ManufacturerMetadata))]
    public partial class Manufacturer { }


    public class CategoryMetadata
    {
        //[Required]
        //public int CategoryID { get; set; }

        [Display(Name = "Type")]
        [StringLength(50, ErrorMessage = "50 characters or less")]
        public string Category_Name { get; set; }
    }
    [MetadataType(typeof(CategoryMetadata))]
    public partial class Category { }












}


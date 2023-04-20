using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Online_Shopping_Website.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required, MaxLength(150), MinLength(2)]
        public string Name { get; set; }
        [MaxLength(150)]
        public string Description { get; set; }

        [ForeignKey("Category"), Required, Display(Name="Category")]
        public int CategoryId { get; set; }
        [Display(Name = "Qty Available")]
        public int Quantity_Available { get; set; }
        public float Price { get; set; }
        public int Active { get; set; }
        [Display(Name = "Expiry Date")]
        public DateTime ExpiryDate { get; set; }
        [MaxLength(20)]
        public string FileType { get; set; }
        [MaxLength(225)]
        public string FilePath { get; set; }
        [MaxLength(225), Display(Name = "File Name")]
        public string FileName { get; set; }


        public virtual Category Category { get; set; }

        [NotMapped]
        public virtual HttpPostedFileBase file { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Online_Shopping_Website.Models
{
    public class Order
    {
        public int Id { get; set; }
        [Required, ForeignKey("Customer"), Display(Name = "Customer")]
        public string CustomerId { get; set; }
        [Display(Name = "Amount Paid")]
        public float AmountPaid { get; set; }
        public float Tax { get; set; }
        public float Discount { get; set; }

        public string Name { get; set; }
        public string Number { get; set; }
        public string Expiry { get; set; }
        public int CVV { get; set; }


        public virtual float Total => (History?.Sum(x => x.Quantity * x.Price)).Value;
        public virtual float NetTotal => (Total + Tax) - Discount;
        [Required, DisplayName("Status")]
        public OrderStatuses Status { get; set; }
        public DateTime OrderDate { get; set; }


        public virtual ApplicationUser Customer { get; set; }
        public virtual IEnumerable<OrderHistory> History { get; set; }
    }
    public class OrderHistory
    {
        public int Id { get; set; }
        [Required, ForeignKey("Order")]
        public int OrderId { get; set; }
        [Required, ForeignKey("Product"), Display(Name = "Product")]
        public int ProductId { get; set; }
        [Required, Range(1, 100000)]
        public int Quantity { get; set; }
        public float Price { get; set; }


        public virtual Order Order { get; set; }
        public virtual Product Product { get; set; }
    }

    public enum OrderStatuses
    {
        InCart, Unpaid, Approved, Cancelled,
    }

    public class CheckoutModel {
        public Card card { get; set; }
        public List<CartItem> cartitems { get; set; }
    }
    
    public class Card {
        public string name { get; set; }
        public string number { get; set; }
        public string expiry { get; set; }
        public int cvv { get; set; }
    }
    
    public class CartItem {
        [Required]
        public int Id { get; set; }
        public int Qty { get; set; }
    }

}
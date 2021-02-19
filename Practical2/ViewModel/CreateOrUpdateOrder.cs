using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Practical2.ViewModel
{
    public class CreateOrUpdateOrder
    {
        public int Id { get; set; }
        public int mid { get; set; }

        [Display(Name = "Order No.")]
        public string OrderNo { get; set; }

        [Display(Name = "Customer")]
        [Required]
        public int CustomerId { get; set; }
        public List<DropDownViewModel> Customers { get; set; }

        [Display(Name = "Order Date.")]
        [Required]

        public DateTime OrderDate { get; set; }
        public List<OrderDetailViewModel> OrderDetails { get; set; }
        public OrderDetailViewModel OrderDetailsFormModel { get; set; }
        public string OrderDetails_update { get; set; }
    }
    
    public class OrderDetailViewModel
    {
        [Required]

        public int ItemId { get; set;  }
        public List<DropDownViewModel> Items { get; set; }
        [Required]

        public int Quantity { get; set;  }

        [Required(ErrorMessage = "Enter the amount")]

        public int Amount { get; set;  }

        [Display(Name = "Total Amount")]
        public Decimal TotalAmount { get { return Quantity * Amount; } }
        public int Ind { get; set; }
        
        public string ItemName { get; set; }


    }

    public class DropDownViewModel 
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
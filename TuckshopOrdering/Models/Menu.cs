using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlTypes;

namespace TuckshopOrdering.Models
{
    public class Menu
    {
        public int MenuID {  get; set; } // primary key 
        [DisplayName("Food Item")] // displays a custom name rather than the column name from the table
        [StringLength(maximumLength: 35, MinimumLength = 2)] // maximum characters for food name is 35 and minimum characters is 2
        public string foodName { get; set; } // food name property 
        [DisplayName("Price")] // displays a custom name rather than the column name from the table
		[Range(0.1, 99)]  // only allows user to enter a price between 10 cents and 99 dollars 
        [DataType(DataType.Currency)] 
        public decimal price { get; set; } // price property 
        public string imageName { get; set; }
        [NotMapped]
        [DisplayName("Upload Image:")]
        public IFormFile imageFile { get; set; }
        [DisplayName("Category")]
        public int CategoryID { get; set; }
        public Category Category { get; set; } = null!; // many menu items can belong only to 1 category
        public ICollection<FoodOrder> FoodOrders { get; set; } // 1 menu item can belong to many food orders 
    }
}

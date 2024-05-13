using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlTypes;

namespace TuckshopOrdering.Models
{
    public class Menu
    {
        public int MenuID {  get; set; }
        [DisplayName("Food Item")]
        public string foodName { get; set; }
        [DisplayName("Price")]
        public decimal price { get; set; }
        public string imageName { get; set; }
        [NotMapped]
        [DisplayName("Upload Image:")]
        public IFormFile imageFile { get; set; }
        [DisplayName("Category")]
        public int CategoryID { get; set; }
        [DisplayName("Customise")]
        public int CustomiseID { get; set; }
        public Category Category { get; set; } = null!;
        public Customise Customise { get; set; } = null!;
        public ICollection<FoodOrder> FoodOrders { get; set; }
    }
}

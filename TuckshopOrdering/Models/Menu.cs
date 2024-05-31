using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlTypes;

namespace TuckshopOrdering.Models
{
    public class Menu
    {
        public int MenuID {  get; set; }
        [DisplayName("Food Item")]
        [StringLength(50)]
        public string foodName { get; set; }
        [DisplayName("Price")]
        [Range(1, 1000)]
        [DataType(DataType.Currency)]
        public decimal price { get; set; }
        public string imageName { get; set; }
        [NotMapped]
        [DisplayName("Upload Image:")]
        public IFormFile imageFile { get; set; }
        [DisplayName("Category")]
        public int CategoryID { get; set; }
        [DisplayName("Customise")]
        public int CustomiseID { get; set; }
        [DisplayName("Display on Home page")]
        public bool homePageDisplay { get; set; }
        public Category Category { get; set; } = null!;
        public Customise Customise { get; set; } = null!;
        public ICollection<FoodOrder> FoodOrders { get; set; }
    }
}

using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlTypes;

namespace TuckshopOrdering.Models
{
    public class Menu
    {
        public int MenuID {  get; set; }
        public string foodName { get; set; }
        public decimal price { get; set; }
        public string imageName { get; set; }
        [NotMapped]
        [DisplayName("Upload Image:")]
        public IFormFile imageFile { get; set; }
        
        public int CategoryID { get; set; }
        public int CustomiseID { get; set; }
        public Category Category { get; set; } = null!;
        public Customise Customise { get; set; } = null!;
    }
}

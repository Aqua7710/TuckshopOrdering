using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TuckshopOrdering.Models
{
    public class Category
    {
        public int CategoryID { get; set; } // primary key
        [DisplayName("Category Name")] 
        [StringLength(50)]
        public string CategoryName { get; set; } // category name property 
        public ICollection<Menu> Menus { get; set; } = new List<Menu>(); // 1 category can belong to many menu items
    }
}

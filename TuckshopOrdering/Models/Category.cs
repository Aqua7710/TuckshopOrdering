using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TuckshopOrdering.Models
{
    public class Category
    {
        public int CategoryID { get; set; }
        [DisplayName("Category Name")]
        [StringLength(50)]
        public string CategoryName { get; set; }
        public ICollection<Menu> Menus { get; set; } = new List<Menu>(); // 1 category can belong to many menu items
    }
}

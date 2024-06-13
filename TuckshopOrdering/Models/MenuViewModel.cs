using TuckshopOrdering.Migrations;

namespace TuckshopOrdering.Models
{
    public class MenuViewModel
    {
        public List<Menu> _Menu { get; set; }
        public List<Category> _Category { get; set; }
        public List<FoodOrder> _FoodOrder { get; set;}
    }
}

using TuckshopOrdering.Migrations;

namespace TuckshopOrdering.Models
{
    public class MenuViewModel
    {
        public List<FoodOrder> FoodOrders { get; set; }
        public List<Category> Categories { get; set; }
        public List<Customise> Customises { get; set; }
        public List<Menu> Menus { get; set; }
    }
}

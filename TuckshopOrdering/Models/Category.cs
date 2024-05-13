namespace TuckshopOrdering.Models
{
    public class Category
    {
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public ICollection<Menu> Menus { get; set; } = new List<Menu>();
    }
}

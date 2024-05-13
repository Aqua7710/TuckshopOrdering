namespace TuckshopOrdering.Models
{
    public class Customise
    {
        public int CustomiseID { get; set; }
        public string CustomiseName { get; set; }
        public ICollection<Menu> Menus { get; set; } = new List<Menu>();
    }
}

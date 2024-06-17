using TuckshopOrdering.Migrations;

namespace TuckshopOrdering.Models
{
	public class OrderViewModel // display two models on one view
	{
		public List<Order> Orders { get; set; }
		public Dictionary<int, List<FoodOrder>> FoodOrders { get; set; }
	}
}

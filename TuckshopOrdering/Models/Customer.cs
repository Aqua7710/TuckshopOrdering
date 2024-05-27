namespace TuckshopOrdering.Models
{
    public class Customer
    {
        public int CustomerID { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public int roomNumber { get; set; }
        public string fullName
        {
            get { return firstName + " " + lastName; }
        }
        public FoodOrder FoodOrder { get; set; }
    }
}

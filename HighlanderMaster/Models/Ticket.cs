namespace HighlanderMaster.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int TripRouteId { get; set; }
        public DateTime PurchaseDate { get; set; }
        public decimal Price { get; set; }
        public bool IsPaid { get; set; }
        public TripRoute TripRoute { get; set; }
    }
}

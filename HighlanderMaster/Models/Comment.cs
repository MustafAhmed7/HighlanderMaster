namespace HighlanderMaster.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public int TripRouteId { get; set; }
        public string Text { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool Deleted { get; set; }

        public TripRoute TripRoute { get; set; }
    }
}

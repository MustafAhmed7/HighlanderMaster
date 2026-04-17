using Microsoft.EntityFrameworkCore.Migrations;

namespace HighlanderMaster.Models
{
    public class TripRoute
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int MaxCountPeople { get; set; }
        public double LenghtKm { get; set; }
        public string MainPicURL { get; set; } = string.Empty;
        public string PicturesURL { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string StartingPoint { get; set; } = string.Empty;
        public string EndPoint { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public TimeSpan Duration { get; set; }
        public bool Deleted { get; set; }
        public RouteStatus Status { get; set; }      
        public RouteCategory Category { get; set; }
        public decimal Price { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public int CurrentParticipants { get; set; }
    }

    public enum RouteStatus
    {
        Active,
        Upcoming,
        Inactive
    }

    public enum RouteCategory
    {
        Hiking,
        Climbing,
        Camping
    }
}
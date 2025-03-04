using Microsoft.EntityFrameworkCore.Metadata;

namespace HomeFlex.Models
{
    public class Property
    {
        public int Id { get; set; }
        public string OwnerId { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }
        public string ShortDescription { get; set; }
        public string WhomToRent { get; set; }
        public int TotalRoom { get; set; }
        public int BedroomNum { get; set; }
        public int BathroomNum { get; set; }
        public int Sqft { get; set; }
        public bool Lift { get; set; }
        public DateTime AvailableDate { get; set; }
        public int FloorNo { get; set; }
        public Location Location { get; set; }
        public bool IsBooked { get; set; }
        public ICollection<Images> Images { get; set; } // Changed to ICollection<Images>

        public Users Users { get; set; }
        

    }
}

namespace HomeFlex.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public string FromUserId { get; set; }
        public string ToUserId { get; set; }
        public int PropertyId { get; set; }
        public string Status { get; set; }
        public DateTime StatusUpdatedAt { get; set; } = DateTime.Now;

        public Property Property { get; set; }
        public Users FromUser { get; set; }
        public Users ToUser { get; set; }
    }
}
namespace HomeFlex.Models
{
    public class Location
    {
        public int Id { get; set; }
        public int PropertyId { get; set; }
        public string Division { get; set; }
        public string District { get; set; }
        public string Thana { get; set; }
        public int WardNo { get; set; }
        public string HouseNo { get; set; }
        public string Address { get; set; }
    }
}

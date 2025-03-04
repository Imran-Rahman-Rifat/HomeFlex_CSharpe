namespace HomeFlex.ViewModels
{
    public class PropertyViewModel
    {
        public int Id { get; set; }
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
        public string Division { get; set; }
        public string District { get; set; }
        public string Thana { get; set; }
        public int WardNo { get; set; }
        public string HouseNo { get; set; }
        public string Address { get; set; }
        public bool IsBooked { get; set; } = false;
        public IFormFile TitleImage { get; set; }
        public IFormFile BedroomImage { get; set; }
        public IFormFile BathroomImage { get; set; }
        public IFormFile KitchenImage { get; set; }
        public IFormFile AdditionalImage { get; set; }
        
    }
}
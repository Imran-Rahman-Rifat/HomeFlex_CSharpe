using HomeFlex.Data;
using HomeFlex.Models;
using HomeFlex.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HomeFlex.Controllers
{
    [Authorize(Roles = "Owner, Resident")]
    public class PropertyController : Controller
    {
        private readonly AppDbContext _context;

        public PropertyController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(PropertyViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Get the currently logged-in user's ID
                var ownerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var property = new Property
                {
                    OwnerId = ownerId,
                    Title = model.Title,
                    Price = model.Price,
                    ShortDescription = model.ShortDescription,
                    WhomToRent = model.WhomToRent,
                    TotalRoom = model.TotalRoom,
                    BedroomNum = model.BedroomNum,
                    BathroomNum = model.BathroomNum,
                    Sqft = model.Sqft,
                    Lift = model.Lift,
                    AvailableDate = model.AvailableDate,
                    FloorNo = model.FloorNo
                };

                _context.Properties.Add(property);
                await _context.SaveChangesAsync();

                var location = new Location
                {
                    PropertyId = property.Id,
                    Division = model.Division,
                    District = model.District,
                    Thana = model.Thana,
                    WardNo = model.WardNo,
                    HouseNo = model.HouseNo,
                    Address = model.Address
                };

                _context.Locations.Add(location);
                await _context.SaveChangesAsync();

                var images = new List<Images>
        {
            new Images { PropertyId = property.Id, ImageType = "Title", ImagePath = await SaveImage(model.TitleImage) },
            new Images { PropertyId = property.Id, ImageType = "Bedroom", ImagePath = await SaveImage(model.BedroomImage) },
            new Images { PropertyId = property.Id, ImageType = "Bathroom", ImagePath = await SaveImage(model.BathroomImage) },
            new Images { PropertyId = property.Id, ImageType = "Kitchen", ImagePath = await SaveImage(model.KitchenImage) },
            new Images { PropertyId = property.Id, ImageType = "Additional", ImagePath = await SaveImage(model.AdditionalImage) }
        };

                foreach (var image in images)
                {
                    var existingImage = await _context.Images
                        .FirstOrDefaultAsync(i => i.PropertyId == image.PropertyId && i.ImageType == image.ImageType);

                    if (existingImage == null)
                    {
                        _context.Images.Add(image);
                    }
                    else
                    {
                        existingImage.ImagePath = image.ImagePath;
                        _context.Images.Update(existingImage);
                    }
                }

                try
                {
                    await _context.SaveChangesAsync();
                    ViewBag.Message = "Successfully added!";
                }
                catch (DbUpdateException ex)
                {
                    // Log the error
                    ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persists, " +
                        "see your system administrator.");
                }

                return View();
            }

            ViewBag.Message = "Error: Unable to add property.";
            return View(model);
        }

        private async Task<string> SaveImage(IFormFile image)
        {
            if (image != null)
            {
                var filePath = Path.Combine("wwwroot/images", image.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }
                return filePath;
            }
            return null;
        }

        [HttpGet]
        public async Task<IActionResult> ManageRental(string location, decimal? price_min, decimal? price_max, string whom_to_rent, int? total_room, int? sqft, string sort)
        {
            var properties = _context.Properties.Include(p => p.Location).AsQueryable();

            if (!string.IsNullOrEmpty(location))
            {
                properties = properties.Where(p => p.Location.Thana.Contains(location) || p.Location.District.Contains(location));
            }

            if (price_min.HasValue)
            {
                properties = properties.Where(p => p.Price >= price_min.Value);
            }

            if (price_max.HasValue)
            {
                properties = properties.Where(p => p.Price <= price_max.Value);
            }

            if (!string.IsNullOrEmpty(whom_to_rent))
            {
                properties = properties.Where(p => p.WhomToRent == whom_to_rent);
            }

            if (total_room.HasValue)
            {
                properties = properties.Where(p => p.TotalRoom == total_room.Value);
            }

            if (sqft.HasValue)
            {
                properties = properties.Where(p => p.Sqft == sqft.Value);
            }

            switch (sort)
            {
                case "newest":
                    properties = properties.OrderByDescending(p => p.Id);
                    break;
                case "oldest":
                    properties = properties.OrderBy(p => p.Id);
                    break;
                case "price_asc":
                    properties = properties.OrderBy(p => p.Price);
                    break;
                case "price_desc":
                    properties = properties.OrderByDescending(p => p.Price);
                    break;
            }

            ViewData["location"] = location;
            ViewData["price_min"] = price_min;
            ViewData["price_max"] = price_max;
            ViewData["whom_to_rent"] = whom_to_rent;
            ViewData["total_room"] = total_room;
            ViewData["sqft"] = sqft;
            ViewData["sort"] = sort;

            return View(await properties.ToListAsync());
        }


        public async Task<IActionResult> Details(int id)
        {
            var property = await _context.Properties
                .Include(p => p.Location)
                .Include(p => p.Images)
                .Include(p => p.Users)
                //.ThenInclude(o => o.User)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (property == null)
            {
                return NotFound();
            }


            return View(property);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProperty(PropertyViewModel model)
        {
            if (ModelState.IsValid)
            {
                var property = await _context.Properties
                    .FirstOrDefaultAsync(p => p.Id == model.Id);

                if (property == null)
                {
                    ModelState.AddModelError("", "Property not found.");
                    return RedirectToAction(nameof(Details), new { id = model.Id });
                }

                // Update the fields of the property entity
                property.Title = model.Title;
                property.Price = model.Price;
                property.AvailableDate = model.AvailableDate;
                property.Sqft = model.Sqft;
                property.ShortDescription = model.ShortDescription;
                property.TotalRoom = model.TotalRoom;
                property.BedroomNum = model.BedroomNum;
                property.BathroomNum = model.BathroomNum;
                property.Lift = model.Lift;
                property.WhomToRent = model.WhomToRent;
                property.FloorNo = model.FloorNo;

                _context.Properties.Update(property);

                try
                {
                    var result = await _context.SaveChangesAsync();
                    if (result > 0)
                    {
                        ViewBag.Message = "Successfully updated!";
                    }
                    else
                    {
                        ModelState.AddModelError("", "No changes were made to the property.");
                    }
                }
                catch (DbUpdateException ex)
                {
                    // Log the error (uncomment ex variable name and write a log.)
                    ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persists, " +
                        "see your system administrator.");
                }

                return RedirectToAction(nameof(Details), new { id = model.Id });
            }

            ViewBag.Message = "Error: Unable to update property.";
            return RedirectToAction(nameof(Details), new { id = model.Id });
        }
        [HttpPost]
        public async Task<IActionResult> UpdateLocation(LocationViewModel model)
        {
            if (ModelState.IsValid)
            {
                var location = await _context.Locations
                    .FirstOrDefaultAsync(l => l.PropertyId == model.PropertyId);

                if (location == null)
                {
                    ModelState.AddModelError("", "Location not found.");
                    return RedirectToAction(nameof(Details), new { id = model.PropertyId });
                }

                // Update the fields of the location entity
                location.Division = model.Division;
                location.District = model.District;
                location.Thana = model.Thana;
                location.WardNo = model.WardNo;
                location.HouseNo = model.HouseNo;
                location.Address = model.Address;

                _context.Locations.Update(location);

                try
                {
                    await _context.SaveChangesAsync();
                    ViewBag.Message = "Successfully updated!";
                }
                catch (DbUpdateException ex)
                {
                    // Log the error (uncomment ex variable name and write a log.)
                    ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persists, " +
                        "see your system administrator.");
                }

                return RedirectToAction(nameof(Details), new { id = model.PropertyId });
            }

            ViewBag.Message = "Error: Unable to update location.";
            return RedirectToAction(nameof(Details), new { id = model.PropertyId });
        }

        [HttpPost]
        public async Task<IActionResult> RequestBooking(BookingViewModel model)
        {
            var booking = new Booking
            {
                FromUserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                ToUserId = _context.Properties.Find(model.PropertyId).OwnerId,
                PropertyId = model.PropertyId,
                Status = "pending"
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id = model.PropertyId });
        }

        /*[HttpPost]
        public async Task<IActionResult> Unbook(int propertyId)
        {
            var property = await _context.Properties.FindAsync(propertyId);
            if (property != null)
            {
                property.IsBooked = false;
                _context.Update(property);
                await _context.SaveChangesAsync();

                var bookings = _context.Bookings.Where(b => b.PropertyId == propertyId);
                _context.Bookings.RemoveRange(bookings);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Details), new { id = propertyId });
        }

        [HttpPost]
        public async Task<IActionResult> AcceptBooking(int bookingId)
        {
            var booking = await _context.Bookings.FindAsync(bookingId);
            if (booking != null)
            {
                booking.Status = "accepted";
                booking.StatusUpdatedAt = DateTime.Now;
                _context.Update(booking);

                var property = await _context.Properties.FindAsync(booking.PropertyId);
                property.IsBooked = true;
                _context.Update(property);

                var otherBookings = _context.Bookings
                    .Where(b => b.PropertyId == booking.PropertyId && b.Id != bookingId);
                _context.Bookings.RemoveRange(otherBookings);

                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Details), new { id = booking.PropertyId });
        }

        [HttpPost]
        public async Task<IActionResult> RejectBooking(int bookingId)
        {
            var booking = await _context.Bookings.FindAsync(bookingId);
            if (booking != null)
            {
                booking.Status = "rejected";
                booking.StatusUpdatedAt = DateTime.Now;
                _context.Update(booking);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Details), new { id = booking.PropertyId });
        }*/
    }


}

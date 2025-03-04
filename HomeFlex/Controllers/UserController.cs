using HomeFlex.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

public class UserController : Controller
{
    private readonly UserManager<Users> _userManager;

    public UserController(UserManager<Users> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IActionResult> Profile(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return NotFound();
        }

        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        return View(user);
    }
}
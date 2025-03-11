using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using DigitalWarehouse.Models; // Om du skapar en modell för vyn

[Authorize(Roles = "Admin")] // Endast admin kan skapa fler admins
public class AdminController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AdminController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public IActionResult CreateAdmin()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CreateAdmin(CreateAdminModel model)
    {
        if (ModelState.IsValid)
        {
            var user = new IdentityUser { UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // Kontrollera om rollen "Admin" finns, annars skapa den
                if (!await _roleManager.RoleExistsAsync("Admin"))
                {
                    await _roleManager.CreateAsync(new IdentityRole("Admin"));
                }

                // Lägg till användaren i "Admin"-rollen
                await _userManager.AddToRoleAsync(user, "Admin");

                return RedirectToAction("Index", "Home"); // Redirecta till startsidan
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
        }
        return View(model);
    }
}

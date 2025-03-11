using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DigitalWarehouse.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
   .WithStaticAssets();

//Skapa Adminroll och användare till dessa
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    //Skapa roller
    var roles = new[] { "Admin", "Worker" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }


    //Skapa Användare
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    var users = new[] {

        new { Email = "admin@warehouse.com", UserName = "admin@warehouse.com", Password = "Admin!123", Role = "Admin" },
        new { Email = "jo.han98@hotmail.com", UserName = "jo.han98@hotmail.com", Password = "Password!123", Role = "Worker" }
        };
    foreach (var user in users)
    {
        var IdentityUser = await userManager.FindByEmailAsync(user.Email);
        if (IdentityUser == null)
        {
            IdentityUser = new IdentityUser { UserName = user.UserName, Email = user.Email };
            await userManager.CreateAsync(IdentityUser, user.Password);

            //Lägg till användare till rollerna
            await userManager.AddToRoleAsync(IdentityUser, user.Role);
        }
    }

}

app.Run();

using DataAccess.Context;
using DataAccess.Repositories;
using Domain.Models;
using EnterpriseProgramming_JeanPaulCurmiCassar_6._2B.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<RestaurantDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<RestaurantDbContext>();
builder.Services.AddControllersWithViews();

builder.Services.AddScoped(typeof(RestaurantsRepository));
builder.Services.AddScoped(typeof(MenuItemsRepository));

var app = builder.Build();

//Just for testing purposes and to remove after verifying that Sections 1 works
//Just addind some data

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<RestaurantDbContext>();

    if (!context.Restaurants.Any())
    {
        var r1 = new Restaurant
        {
            Name = "Luca Trattoria",
            OwnerEmailAddress = "luca.owner@example.com",
            Phone = "111111",
            Status = "Pending",
            Description = "Italian pasta and grill",
            Address = "123 Harbor Road, Valletta"
        };

        var r2 = new Restaurant
        {
            Name = "Sushi Wave",
            OwnerEmailAddress = "hana.owner@example.com",
            Phone = "222222",
            Status = "Pending",
            Description = "Sushi and Japanese dishes",
            Address = "45 Marina Street, Sliema"
        };

        var r3 = new Restaurant
        {
            Name = "Burger House",
            OwnerEmailAddress = "mark.owner@example.com",
            Phone = "333333",
            Status = "Pending",
            Description = "Burgers and fries",
            Address = "10 High Street, Birkirkara"
        };



        context.Restaurants.AddRange(r1, r2, r3);
        context.SaveChanges();

        var m1 = new MenuItem
        {
            Title = "Tagliatelle al Ragu",
            Price = 11,
            RestaurantId = r1.Id,
            Status = "Pending",
            Currency = "EUR"
        };
        var m2 = new MenuItem
        {
            Title = "Dragon Roll",
            Price = 14,
            RestaurantId = r2.Id,
            Status = "Pending",
            Currency = "EUR"
        };
        var m3 = new MenuItem
        {
            Title = "Classic Burger",
            Price = 9,
            RestaurantId = r3.Id,
            Status = "Pending",
            Currency = "EUR"
        };

        context.MenuItems.AddRange(m1, m2, m3);
        context.SaveChanges();
    }
}

//To delete till here

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
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();

using DataAccess.Context;
using DataAccess.Repositories;
using Domain.Interfaces;
using Domain.Models;
using EnterpriseProgramming_JeanPaulCurmiCassar_6._2B.Data;
using EnterpriseProgramming_JeanPaulCurmiCassar_6._2B.Filters;
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


//Register IItemsRepository with keyed services
builder.Services.AddKeyedScoped<IItemsRepository, ItemsInMemoryRepository>("memory");
builder.Services.AddKeyedScoped<IItemsRepository, ItemsDbRepository>("db");

//Adding the memory cache for in-memory repository
builder.Services.AddMemoryCache();

//used to register the filter
builder.Services.AddScoped(typeof(FiltersValidations));

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
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();

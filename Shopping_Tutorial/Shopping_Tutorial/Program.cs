using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using Shopping_Tutorial.Areas.Admin.Repository;

using Shopping_Tutorial.Models;
using Shopping_Tutorial.Repository;

var builder = WebApplication.CreateBuilder(args);

//CONNECTION DATABAE
builder.Services.AddDbContext<DataContext>(options =>
{
	options.UseSqlServer(builder.Configuration["ConnectionStrings:ConnectedDb"]);
});


//add email sender
builder.Services.AddTransient<IEmailSender, EmailSender>();


// connection db
builder.Services.AddControllersWithViews();

//làm cart
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
	options.IdleTimeout = TimeSpan.FromMinutes(30);
	options.Cookie.IsEssential = true;
});


//lam Identity (con` phia' duoi')
builder.Services.AddIdentity<AppUserModel,IdentityRole>()
    .AddEntityFrameworkStores<DataContext>().AddDefaultTokenProviders();
builder.Services.AddRazorPages();

builder.Services.Configure<IdentityOptions>(options =>
{
    // Password settings.
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 4;
 

    options.User.RequireUniqueEmail = true;
});



var app = builder.Build();

//lam trang 404 not found
app.UseStatusCodePagesWithRedirects("/Home/Error?statuscode={0}");

// làm cart
app.UseSession();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

//lam identity
app.UseAuthentication();

app.UseAuthorization();

//lam trang admin
app.MapControllerRoute(
    name: "Areas",
    pattern: "{area:exists}/{controller=Product}/{action=Index}/{id?}");


//custom route (lam dep URL)
app.MapControllerRoute(
    name: "category",
    pattern: "/category/{Slug?}",
	defaults: new {controller="Category", action="Index"});
app.MapControllerRoute(
    name: "brand",
    pattern: "/brand/{Slug?}",
    defaults: new { controller = "Brand", action = "Index" });



app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

//SEEDING DATA
var context = app.Services.CreateScope().ServiceProvider.GetRequiredService<DataContext>();
SeedData.SeedingData(context);
app.Run();

using AspNetCoreIdentityApp.Web.Extensions;
using AspNetCoreIdentityApp.Web.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlCon"));
});

builder.Services.AddIdentityWithExt();

builder.Services.ConfigureApplicationCookie(opt =>
{
    // ilk once bir cookie yaradiriq.
    var cookieBuilder = new CookieBuilder(); 
    cookieBuilder.Name = "AppCookie";

    //Sonra signin seyfesinin pathin veririk
    opt.LoginPath = new PathString("/Home/SignIn");
    opt.LogoutPath = new PathString("/Member/logout");
    opt.Cookie= cookieBuilder;

    // Biz cookie 60 gun vaxt verdik 60 gun kompda saxliyaciyiq. 
    opt.ExpireTimeSpan = TimeSpan.FromDays(60);

    opt.SlidingExpiration = true;
    //SlidingExpiration o dur ki eger 30cu gunde istifadeci yene daxil olsa
    //yeniden 60 gun olur cookinin omru
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
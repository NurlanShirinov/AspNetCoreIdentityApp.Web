using AspNetCoreIdentityApp.Web.Extensions;
using AspNetCoreIdentityApp.Web.Models;
using AspNetCoreIdentityApp.Web.OptionsModels;
using AspNetCoreIdentityApp.Web.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlCon"));
});

builder.Services.Configure<SecurityStampValidatorOptions>(options =>
{
    //burda biz 30 deiqiqende biri security stmap colmu ile cookini qarshilashdiracagiq.
    options.ValidationInterval = TimeSpan.FromMinutes(30);
});



// bu en bestpractice olan variantdir.Eger men solutionda olan her hansisa foldere catmaq
//isdeyiremse burda tanimladigim IFolderi catmaq isdediyim folderde de tanimlamagim lazimdir.
builder.Services.AddSingleton<IFileProvider>(new PhysicalFileProvider(Directory.GetCurrentDirectory()));


builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddIdentityWithExt();
builder.Services.AddScoped<IEmailService, EmailService>();


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
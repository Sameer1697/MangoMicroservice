using Mango.Web.Service;
using Mango.Web.Service.IService;
using Mango.Web.Utility;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();

builder.Services.AddHttpClient<ICoupanService, CoupanService>();
builder.Services.AddHttpClient<IProductService, ProductService>();
builder.Services.AddHttpClient<IAuthService, AuthService>();
builder.Services.AddHttpClient<IOrderService, OrderService>();
builder.Services.AddHttpClient<IShoppingCartService, ShoppingCartService>();
SD.COUPANAPIBASE = builder.Configuration["ServiceUrls:CoupanAPI"];
SD.PRODUCTAPIBASE = builder.Configuration["ServiceUrls:ProductAPI"];
SD.SHOPPINGCARTAPIBASE = builder.Configuration["ServiceUrls:ShoppingCartAPI"];
SD.AUTHAPIBASE = builder.Configuration["ServiceUrls:AuthAPI"];
SD.ORDERAPIBASE = builder.Configuration["ServiceUrls:OrderAPI"];
builder.Services.AddScoped<ITokenProvider, TokenProvider>();
builder.Services.AddScoped<IBaseService, BaseService>();
builder.Services.AddScoped<ICoupanService, CoupanService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IShoppingCartService, ShoppingCartService>();
builder.Services.AddScoped<IOrderService, OrderService>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.ExpireTimeSpan = TimeSpan.FromHours(10);
        options.LoginPath = "/Auth/Login";
        options.AccessDeniedPath = "/Auth/AccessDenied";
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
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

using InventorySystemWebUI.Filter;
using InventorySystemWebUI.Pages.DependencyInjection;
using InventorySystemWebUI.Service;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddRazorPages()
    .AddMvcOptions(o =>
    {
        o.Filters.Add(new SessionCheckPageFilter());
    });
builder.Services.AddControllers();
// If using ServiceFilter:
builder.Services.AddScoped<SessionCheckPageFilter>();
builder.SetApplicationCookie();
builder.Services.AddHttpContextAccessor();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(5);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddHttpClient("MyHttpClient", client => client.Timeout = TimeSpan.FromMinutes(2));
builder.SetAuthentication();
builder.SetCookiePolicy();
builder.Services.AddSingleton<IGeneralService, GeneralService>();

var app = builder.Build();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();
app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();
app.MapControllers();
app.Run();

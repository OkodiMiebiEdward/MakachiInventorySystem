using InventorySystemWebUI.Pages.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddRazorPages();
builder.SetApplicationCookie();
builder.Services.AddSession(sess => sess.IdleTimeout = TimeSpan.FromHours(5));
builder.Services.AddHttpClient("MyHttpClient", client => client.Timeout = TimeSpan.FromMinutes(2));
builder.SetAuthentication();
builder.SetCookiePolicy();

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

app.Run();

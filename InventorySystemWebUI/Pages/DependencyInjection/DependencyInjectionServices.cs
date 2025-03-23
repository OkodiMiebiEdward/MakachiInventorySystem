namespace InventorySystemWebUI.Pages.DependencyInjection
{
    public static class DependencyInjectionServices
    {
        public static void SetApplicationCookie(this WebApplicationBuilder builder)
        {
            builder.Services.ConfigureApplicationCookie(o =>
            {
                o.Cookie.Name = "userCookie";
                o.Cookie.HttpOnly = true;
                o.ExpireTimeSpan = TimeSpan.FromHours(5);
                o.LoginPath = "/Identity/Account/Login";
                o.SlidingExpiration = true;
                o.ForwardAuthenticate = "Identity.Application";
            });
        }

        public static void SetAuthentication(this WebApplicationBuilder builder)
        {
            builder.Services.AddAuthentication().AddCookie("userKey", o =>
            {
                // Cookie settings
                o.Cookie.HttpOnly = true;
                o.ExpireTimeSpan = TimeSpan.FromMinutes(150);
                o.LoginPath = "/Identity/Account/Login";
                o.SlidingExpiration = true;
                o.Cookie.IsEssential = true;
                //o.ForwardAuthenticate = "Identity.Application";
                o.Events.OnValidatePrincipal = context =>
                {
                    if (context.Properties.Items.TryGetValue("OpenIdConnect.Code.RedirectUri", out string redirectUri))
                    {
                        Uri cookieWasSignedForUri = new Uri(redirectUri!);
                        if (context.Request.Host.Host != cookieWasSignedForUri.Host)
                            context.RejectPrincipal();
                    }
                    return Task.CompletedTask;
                };
            }).AddCookie("Identity.Application", o =>
            {
                // Configure the Identity.Application cookie settings
                o.Cookie.Name = "Identity.Application";
                o.Cookie.HttpOnly = true;
                o.ExpireTimeSpan = TimeSpan.FromHours(5);
                o.LoginPath = "/Identity/Account/Login";
                o.SlidingExpiration = true;
            });
        }

        public static void SetCookiePolicy(this WebApplicationBuilder builder)
        {
            builder.Services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
        }
    }
}

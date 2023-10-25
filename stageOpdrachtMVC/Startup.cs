using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace stageOpdrachtMVC
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // Voeg de volgende code toe aan de `ConfigureServices` methode om authenticatie in te schakelen.
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Home/Index"; // Hiermee stel je de inlogpagina in.
        options.AccessDeniedPath = "/Home/AccessDenied"; // Hiermee stel je de toegangsweigeringpagina in.
    });

            

        }


        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            // Voeg de volgende code toe aan de `Configure` methode om de authenticatiemiddleware te activeren.
            app.UseAuthentication();
            app.UseAuthorization();

            // Configureer middleware, bijvoorbeeld routing, authenticatie, enz.
        }
    }
}

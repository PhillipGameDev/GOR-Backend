using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameOfRevenge.Business;
using GameOfRevenge.Business.CacheData;
using GameOfRevenge.Business.Manager.Kingdom;
using GameOfRevenge.Business.Manager.UserData;
using GameOfRevenge.Common.Interface;
using GameOfRevenge.Common.Interface.UserData;
using GameOfRevenge.WebAdmin.Configurations.IdentityServer;
using GameOfRevenge.WebAdmin.Services;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using NSwag;
using NSwag.Generation.Processors.Security;

namespace GameOfRevenge.WebAdmin
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<IAppSettings>(appSettingsSection);
            var appSettings = appSettingsSection.Get<AppSettings>();
            AppSettings.Config = appSettings;

            Config.ConnectionString = appSettings.ConnectionString;
            ResolverServices(services, appSettings);


            services.AddRazorPages();
            services.AddControllers();
/*            services.AddRazorPages().AddRazorPagesOptions(options =>
            {
                options.Conventions.ConfigureFilter(new IgnoreAntiforgeryTokenAttribute());
            });
            services.AddControllersWithViews(options =>
            {
                options.Filters.Add(new IgnoreAntiforgeryTokenAttribute());
            });*/

            services.AddOpenApiDocument(config =>
            {
                // Document name (default to: v1)
                config.DocumentName = "GameOfRevenge.WebAdmin";

                // Document / API version (default to: 1.0.0)
                config.Version = "1.0.0";

                // Document title (default to: My Title)
                config.Title = "GameOfRevenge.WebAdmin";

                // Document description
                config.Description = "WebAdmin documentation";
            });


             services.AddDataProtection()
            .PersistKeysToFileSystem(new DirectoryInfo("C:/IIS/GameOfRevengeAdmin/Keys"));

/*            var key = Encoding.ASCII.GetBytes(appSettings.Secret);
            var auth = services.AddAuthentication();
            services.AddAuthentication(config =>
            {
                config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                config.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            }).AddJwtBearer(AuthHelper.AuthenticationSchemeJwt, config =>
            {
                config.RequireHttpsMetadata = false;
                config.SaveToken = true;
                config.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            }).AddCookie(AuthHelper.AuthenticationSchemeCookie, config =>
            {
                config.Cookie.Name = "GameOfRevenge.WebAdmin.Cookie";
                config.LoginPath = "/Account/Login";
                config.LogoutPath = "/Account/Logout";
                config.AccessDeniedPath = "/Home/AccessDenied";
                config.SlidingExpiration = true;
                config.ExpireTimeSpan = TimeSpan.FromDays(30);
                config.Cookie.HttpOnly = true;
            });*/


/*            services.AddAuthentication("Bearer")
                    .AddJwtBearer("Bearer", options =>
                    {
                        options.Authority = "https://localhost:44301";
                        options.RequireHttpsMetadata = false;
                    });*/


            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(config =>
            {
                config.Cookie.Name = "GameOfRevenge.WebAdmin.Cookie";
                config.LoginPath = "/Login";
                config.LogoutPath = "/Login";
                config.AccessDeniedPath = "/Error";
                config.SlidingExpiration = true;
                config.ExpireTimeSpan = TimeSpan.FromDays(30);
                config.Cookie.HttpOnly = true;
            });

/*            services.AddIdentityServer()
                    .AddDeveloperSigningCredential() //not something we want to use in a production environment
                    .AddInMemoryIdentityResources(InMemoryConfig.GetIdentityResources())
                    .AddTestUsers(InMemoryConfig.GetUsers())
                    .AddInMemoryClients(InMemoryConfig.GetClients());
*/

            services.AddSwaggerDocument(config =>
            {
                config.DocumentProcessors.Add(new SecurityDefinitionAppender("JWT Token",
                    new OpenApiSecurityScheme
                    {
                        Type = OpenApiSecuritySchemeType.ApiKey,
                        Name = "Authorization",
                        Description = "Copy 'Bearer ' + valid JWT token into field",
                        In = OpenApiSecurityApiKeyLocation.Header
                    }));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

//            app.UseIdentityServer();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.Use(async (context, next) =>
            {
                if (!context.User.Identity.IsAuthenticated && !context.Request.Path.StartsWithSegments("/Login"))
                {
                    context.Response.Redirect("/Login");
                    return;
                }

                await next();
            });


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
            });

            app.UseOpenApi();
            app.UseSwaggerUi3();
        }

        private static void ResolverServices(IServiceCollection services, AppSettings appSettings)
        {
            services.AddSingleton<IAppSettings>(appSettings);

            services.AddSingleton<IAdminDataManager>(new AdminDataManager());
            services.AddSingleton<IPlayerDataManager>(new PlayerDataManager());
            services.AddSingleton<IClanManager>(new ClanManager());
            services.AddSingleton<IUserQuestManager>(new UserQuestManager());

            var task = ReloadDataBaseDataAsync();
            task.Wait();
        }

        public static async Task ReloadDataBaseDataAsync()
        {
            CacheBoostDataManager.LoadCacheMemory();
            await CacheInventoryDataManager.LoadCacheMemoryAsync();
            await CacheResourceDataManager.LoadCacheMemoryAsync();
            await CacheStructureDataManager.LoadCacheMemoryAsync();
            await CacheTechnologyDataManager.LoadCacheMemoryAsync();
            await CacheTroopDataManager.LoadCacheMemoryAsync();
            await CacheHeroDataManager.LoadCacheMemoryAsync();
            await CacheQuestDataManager.LoadCacheMemoryAsync();
            await CacheProductDataManager.LoadCacheMemoryAsync();
        }
    }
}
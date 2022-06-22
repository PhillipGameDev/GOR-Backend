using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using GameOfRevenge.Common.Interface;
using GameOfRevenge.Common.Interface.UserData;
using GameOfRevenge.Business;
using GameOfRevenge.Business.Manager.UserData;
using GameOfRevenge.Business.CacheData;
using GameOfRevenge.Business.Manager.Kingdom;
using GameOfRevenge.Business.Manager.GameDef;
using GameOfRevenge.Business.Manager;
using GameOfRevenge.WebServer.Services;
using GameOfRevenge.Business.Manager.Base;

namespace GameOfRevenge.WebServer
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Console.WriteLine("--- START UP");
            Configuration = configuration;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            Console.WriteLine("--- CONFIGURE SERVICE");

            //services.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo(@"C:\temp-keys\"));
            services.AddRazorPages();

            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<IAppSettings>(appSettingsSection);
            var appSettings = appSettingsSection.Get<AppSettings>();
            Console.WriteLine("--- CURR CONN STRING " + AppSettings.Config);
            AppSettings.Config = appSettings;
            Console.WriteLine("--- SET CONN STRING "+appSettings );

            Config.ConnectionString = appSettings.ConnectionString;

            var key = Encoding.ASCII.GetBytes(appSettings.Secret);
            var auth = services.AddAuthentication();
            Console.WriteLine("--- KEY = " + key);
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
                config.Cookie.Name = "GameOFRevenge.Cookie";
                config.LoginPath = "/Account/Login";
                config.LogoutPath = "/Account/Logout";
                config.AccessDeniedPath = "/Home/AccessDenied";
                config.SlidingExpiration = true;
                config.ExpireTimeSpan = TimeSpan.FromDays(30);
                config.Cookie.HttpOnly = true;
            });

            services.AddAuthorization();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            ResolverServices(services, appSettings);

            services.AddCors();
            services.AddControllers();
            services.AddControllersWithViews();
            services.AddRazorPages();

            services.AddSwaggerGen(setup =>
            {
                // Include 'SecurityScheme' to use JWT Authentication
                var jwtSecurityScheme = new OpenApiSecurityScheme
                {
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Name = "JWT Authentication",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Description = "Put **_ONLY_** your JWT Bearer token on textbox below!",

                    Reference = new OpenApiReference
                    {
                        Id = JwtBearerDefaults.AuthenticationScheme,
                        Type = ReferenceType.SecurityScheme
                    }
                };

                setup.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

                setup.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    { jwtSecurityScheme, Array.Empty<string>() }
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            Console.WriteLine("--- CONFIGURE");

            var path = Directory.GetCurrentDirectory();
            loggerFactory.AddFile($"{path}\\Logs\\Log.txt");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "GameOfRevenge.WebApi v1"));
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.Use(async (context, next) =>
            {
                await next();
                if (context.Response.StatusCode == 403)
                {
                    context.Request.Path = "/Home/AccessDenied";
                    await next();
                }
                else if (context.Response.StatusCode >= 404 && context.Response.StatusCode < 500)
                {
                    context.Request.Path = "/Home/NotFoundPage";
                    await next();
                }
                else if (context.Response.StatusCode >= 500 && context.Response.StatusCode < 600)
                {
                    context.Request.Path = "/Home/Error";
                    await next();
                }
            });

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }

        private static void ResolverServices(IServiceCollection services, AppSettings appSettings)
        {
            Console.WriteLine("--- RESOLVE SERVICES");

            services.AddSingleton<IAppSettings>(appSettings);

            services.AddSingleton<IAccountManager>(new AccountManager());
            services.AddSingleton<IStructureManager>(new StructureManager());
            services.AddSingleton<IResourceManager>(new ResourceManager());
            services.AddSingleton<ITroopManager>(new TroopManager());
            services.AddSingleton<ITechnologyManager>(new TechnologyManager());
            services.AddSingleton<IMarketManager>(new MarketManager());

            services.AddSingleton<IClanManager>(new ClanManager());

            services.AddSingleton<IBaseUserManager>(new BaseUserDataManager());
            services.AddSingleton<IPlayerDataManager>(new PlayerDataManager());
            services.AddSingleton<IUserResourceManager>(new UserResourceManager());
            services.AddSingleton<IUserStructureManager>(new UserStructureManager());
            services.AddSingleton<IUserTroopManager>(new UserTroopManager());
            services.AddSingleton<IUserInventoryManager>(new UserInventoryManager());
            services.AddSingleton<IUserActiveBuffsManager>(new UserActiveBuffsManager());
            services.AddSingleton<IUserTechnologyManager>(new UserTechnologyManager());

            services.AddSingleton<IUserQuestManager>(new UserQuestManager());
            services.AddSingleton<IUserMailManager>(new UserMailManager());
            services.AddSingleton<IUserMarketManager>(new UserMarketManager());

            services.AddSingleton<IInstantProgressManager>(new InstantProgressManager());

            ReloadDataBaseData();
        }

        public static async Task ReloadDataBaseDataAsync()
        {
            Console.WriteLine("RELOAD CACHE START");
            await CacheBoostDataManager.LoadCacheMemoryAsync();
            await CacheInventoryDataManager.LoadCacheMemoryAsync();
            await CacheResourceDataManager.LoadCacheMemoryAsync();
            await CacheStructureDataManager.LoadCacheMemoryAsync((s) => { });
            await CacheTechnologyDataManager.LoadCacheMemoryAsync();
            await CacheTroopDataManager.LoadCacheMemoryAsync();
            await CacheHeroDataManager.LoadCacheMemoryAsync();
            await CacheQuestDataManager.LoadCacheMemoryAsync();
            await CacheProductDataManager.LoadCacheMemoryAsync();

            Console.WriteLine("RELOAD CACHE END");
/*

            await CacheStructureDataManager.LoadCacheMemoryAsync((s) => { });
            await CacheTechnologyDataManager.LoadCacheMemoryAsync();
            await CacheInventoryDataManager.LoadCacheMemoryAsync();
            await CacheResourceDataManager.LoadCacheMemoryAsync();
            await CacheTroopDataManager.LoadCacheMemoryAsync();
            await CacheBoostDataManager.LoadCacheMemoryAsync();
            await CacheHeroDataManager.LoadCacheMemoryAsync();
            await CacheQuestDataManager.LoadCacheMemoryAsync();
*/


        }

        public static void ReloadDataBaseData()
        {
            Console.WriteLine("--- RELOAD DATABASE");
            var task = ReloadDataBaseDataAsync();
            task.Wait();
        }
    }
}

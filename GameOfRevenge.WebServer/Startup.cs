using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
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
//using Microsoft.AspNetCore.Identity;

namespace GameOfRevenge.WebServer
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo(@"C:\temp-keys\"));

            services.AddControllers().AddNewtonsoftJson();

/*            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
        .AddNewtonsoftJson(opt => {
            opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
        });*/

            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<IAppSettings>(appSettingsSection);

            var appSettings = appSettingsSection.Get<AppSettings>();
            Config.Secret = appSettings.Secret;
            Config.ServerVersion = appSettings.ServerVersion;
            Config.ConnectionString = appSettings.ConnectionString;
            Config.DefaultWorldCode = appSettings.DefaultWorldCode;

            var key = Encoding.ASCII.GetBytes(appSettings.Secret);
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
                config.Cookie.Name = "GameOfRevenge.WebServer.Cookie";
                config.LoginPath = "/Account/Login";
                config.LogoutPath = "/Account/Logout";
                config.AccessDeniedPath = "/Home/AccessDenied";
                config.SlidingExpiration = true;
                config.ExpireTimeSpan = TimeSpan.FromDays(30);
                config.Cookie.HttpOnly = true;
            });

            services.AddAuthorization();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

/*            services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                options.SignIn.RequireConfirmedEmail = true;
            });*/

            ResolverServices(services, appSettings);

            services.AddCors();
            //            services.AddControllers();
            //            services.AddControllersWithViews();
            services.AddRazorPages();

            
/*            services.AddSwaggerGen(setup =>
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
            });*/
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            var path = Directory.GetCurrentDirectory();
            loggerFactory.AddFile($"{path}\\Logs\\Log.txt");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
//                app.UseSwagger();
//                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "GameOfRevenge.WebApi v1"));
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
            services.AddSingleton<IAppSettings>(appSettings);

            services.AddSingleton<IAccountManager>(new AccountManager());
            services.AddSingleton<IStructureManager>(new StructureManager());
            services.AddSingleton<IResourceManager>(new ResourceManager());
            services.AddSingleton<ITroopManager>(new TroopManager());
            services.AddSingleton<ITechnologyManager>(new TechnologyManager());
            services.AddSingleton<IMarketManager>(new MarketManager());

            services.AddSingleton<IChatManager>(new ChatManager());

            services.AddSingleton<IClanManager>(new ClanManager());

            services.AddSingleton<IBaseUserManager>(new BaseUserDataManager());
            services.AddSingleton<IPlayerDataManager>(new PlayerDataManager());
            services.AddSingleton<IUserResourceManager>(new UserResourceManager());
            services.AddSingleton<IUserStructureManager>(new UserStructureManager());
            services.AddSingleton<IUserTroopManager>(new UserTroopManager());
            services.AddSingleton<IUserInventoryManager>(new UserInventoryManager());
            services.AddSingleton<IUserActiveBoostsManager>(new UserActiveBoostManager());
            services.AddSingleton<IUserTechnologyManager>(new UserTechnologyManager());
            services.AddSingleton<IUserHeroManager>(new UserHeroManager());
            services.AddSingleton<IUserFriendsManager>(new UserFriendsManager());

            services.AddSingleton<IUserQuestManager>(new UserQuestManager());
            services.AddSingleton<IUserMailManager>(new UserMailManager());
            services.AddSingleton<IUserMarketManager>(new UserMarketManager());

            services.AddSingleton<IInstantProgressManager>(new InstantProgressManager());

            services.AddSingleton<IKingdomManager>(new KingdomManager());

            ReloadDataBaseData();
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

        public static void ReloadDataBaseData()
        {
            var task = ReloadDataBaseDataAsync();
            task.Wait();
        }
    }
}

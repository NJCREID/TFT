using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json.Serialization;
using System.Text;
using TFT_API.Data;
using TFT_API.Interfaces;
using TFT_API.Persistence;
using TFT_API.Authentication;
using Microsoft.EntityFrameworkCore.Diagnostics;
using TFT_API.Services;
using TFT_API.Filters;
using Microsoft.Extensions.FileProviders;
using System;
using Microsoft.AspNetCore.StaticFiles;

namespace TFT_API
{
    public class Startup(IConfiguration configuration)
    {
        public IConfiguration Configuration { get; } = configuration;

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(options =>
            {
                options.Filters.Add<ValidateRequestBodyAttribute>();
            }).AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
            });
            services.AddHttpContextAccessor();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            // Data access layer services
            services.AddScoped<IItemDataAccess, ItemRepository>();
            services.AddScoped<IUserDataAccess, UserRepository>();
            services.AddScoped<IUnitDataAccess, UnitRepository>();
            services.AddScoped<ITraitDataAccess, TraitRepository>();
            services.AddScoped<IGuideDataAccess, GuideRepository>();
            services.AddScoped<IAugmentDataAccess, AugmentRepository>();
            services.AddScoped<IVoteDataAccess, VoteRepository>();
            services.AddScoped<ICommentDataAccess, CommentRepository>();
            services.AddScoped<IStatDataAccess, StatsRepository>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<RiotApiService>();
            services.AddScoped<TeamBuilderService>();
            services.AddScoped<StatisticsService>();
            services.AddScoped<CooccurrenceService>();
            services.AddScoped<DataCheckService>();
            services.AddScoped<ITFTDataService, TFTDataFetchService>();
            services.AddScoped<TFTUpdateDataService>();

            // Database context
            services.AddDbContext<TFTContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
                options.ConfigureWarnings(warnings => warnings.Throw(RelationalEventId.MultipleCollectionIncludeWarning));
            });

            // Authentication
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                var jwtKey = Configuration["Jwt:Key"];
                if (jwtKey != null)
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = Configuration["Jwt:Issuer"],
                        ValidAudience = Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
                    };
                }
            });
            services.AddMvc();
            services.AddHttpClient();
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
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
                app.UseExceptionHandler("/error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.Use(async (context, next) =>
            {
                var path = context.Request.Path.Value;

                if (path != null && path.StartsWith("/images/"))
                {
                    var remainingPath = path["/images/".Length..];
                    if (remainingPath.Length > 0 &&
                        (remainingPath.StartsWith("champions") ||
                        remainingPath.StartsWith("augments") ||
                        remainingPath.StartsWith("traits") ||
                        remainingPath.StartsWith("items")))
                    {
                        var set = Configuration["TFT:Set"];
                        context.Request.Path = $"/images/set{set}/{remainingPath}";
                    }
                    context.Response.Headers.Append("Cache-Control", "public,max-age=604800");
                }
                await next();
            });
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(env.WebRootPath, "images")),
                RequestPath = "/images",
                ContentTypeProvider = new FileExtensionContentTypeProvider
                {
                    Mappings = { [".avif"] = "image/avif" }
                }
            });
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(env.WebRootPath, "dist")),
                RequestPath = ""
            });
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCors();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapFallbackToFile("dist/index.html");
            });
        }
    }
}

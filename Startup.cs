using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json.Serialization;
using System.Text;
using TFT_API.Data;
using TFT_API.Interfaces;
using TFT_API.Persistence;
using TFT_API.Authentication;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore.Diagnostics;
using TFT_API.Services;

namespace TFT_API
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
            services.AddControllers();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
            });
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
            services.AddScoped<CoocurrenceStats>();
            //services.AddScoped<StatisticsService>();
            //services.AddHostedService<StatisticsBackgroundService>();
            services.AddDbContext<TFTContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
                options.ConfigureWarnings(warnings => warnings.Throw(RelationalEventId.MultipleCollectionIncludeWarning));
            });
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
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Your API", Version = "v1" });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, RiotApiService riotApiService, TeamBuilderService teamBuilderService)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Your API V1");
                });
            }
            else
            {
                app.UseExceptionHandler("/error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseCors();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            //Task.Run(async () =>
            //{
            //    await riotApiService.FetchChallengerMatchHistoryAsync();
            //}).GetAwaiter().GetResult();

            //Task.Run(async () =>
            //{
            //    await teamBuilderService.BuildTeams();
            //}).GetAwaiter().GetResult();
        }
    }
}

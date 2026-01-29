using CommunityRecyclingGamified.Data;
using CommunityRecyclingGamified.Models;
using CommunityRecyclingGamified.Repositories;
using CommunityRecyclingGamified.Repositories.Interfaces;
using CommunityRecyclingGamified.Service;
using CommunityRecyclingGamified.Services;
using CommunityRecyclingGamified.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace CommunityRecyclingGamified
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

          
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            
            builder.Services.AddControllers()
                .AddJsonOptions(o =>
                {
                    o.JsonSerializerOptions.ReferenceHandler =
                        System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;

                   
                });

            
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AngularDev", p =>
                    p.WithOrigins("http://localhost:4200")
                     .AllowAnyHeader()
                     .AllowAnyMethod()
                );
            });

            
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "CommunityRecyclingGamified",
                    Version = "v1"
                });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Βάλε: Bearer {το JWT token σου}"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            // DI (Repositories / Services)
            builder.Services.AddScoped<IUserProfileRepository, UserProfileRepository>();
            builder.Services.AddScoped<IMaterialRepository, MaterialRepository>();
            builder.Services.AddScoped<INeighborhoodRepository, NeighborhoodRepository>();
            builder.Services.AddScoped<IDropoffRepository, DropoffRepository>();
            builder.Services.AddScoped<IUserPointLedgerRepository, UserPointLedgerRepository>();
            builder.Services.AddScoped<IRewardRepository, RewardRepository>();
            builder.Services.AddScoped<IRedemptionRepository, RedemptionRepository>();
            builder.Services.AddScoped<IPasswordHasher<UserProfile>, PasswordHasher<UserProfile>>();
            builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
            builder.Services.AddScoped<IUserBadgeRepository, UserBadgeRepository>();
            builder.Services.AddScoped<IGamificationService, GamificationService>();
            builder.Services.AddScoped<IBadgeRepository, BadgeRepository>();
            builder.Services.AddScoped<IPointsRepository, PointsRepository>();

            // JWT Auth
            var jwtSection = builder.Configuration.GetSection("Jwt");
            var issuer = jwtSection["Issuer"];
            var audience = jwtSection["Audience"];
            var key = jwtSection["Key"];

            if (string.IsNullOrWhiteSpace(key))
                throw new Exception("JWT Key is missing in appsettings.json (Jwt:Key).");

            var keyBytes = Encoding.UTF8.GetBytes(key);

            builder.Services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,

                        ValidIssuer = issuer,
                        ValidAudience = audience,
                        IssuerSigningKey = new SymmetricSecurityKey(keyBytes),

                        ClockSkew = TimeSpan.Zero
                    };
                });

            builder.Services.AddAuthorization();

            // Build
            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Pipeline (order matters)
            app.UseHttpsRedirection();

            // CORS BEFORE auth
            app.UseCors("AngularDev");

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}

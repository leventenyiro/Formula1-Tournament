using AspNetCoreRateLimit;
using car_racing_tournament_api.Data;
using car_racing_tournament_api.Interfaces;
using car_racing_tournament_api.Profiles;
using car_racing_tournament_api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Text;
using System.Text.Json.Serialization;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        builder.Services.AddDbContext<CarRacingTournamentDbContext>(options =>
        {
            var config = new ConfigurationBuilder()
                .AddUserSecrets<Program>()
                .Build();
            options.UseSqlServer(config["connectionString"]);
        });

        builder.Services.AddMvc().AddJsonOptions(options =>
            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles
        );

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddScoped<ISeason, SeasonService>();
        builder.Services.AddScoped<IUser, UserService>();
        builder.Services.AddScoped<IPermission, PermissionService>();
        builder.Services.AddScoped<IDriver, DriverService>();
        builder.Services.AddScoped<ITeam, TeamService>();
        builder.Services.AddScoped<IRace, RaceService>();
        builder.Services.AddScoped<car_racing_tournament_api.Interfaces.IResult, ResultService>();

        builder.Services.AddAutoMapper(typeof(AutoMapperConfig));

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
                        .GetBytes(builder.Configuration.GetSection("Secret").Value)),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

        builder.Services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
            {
                Description = "Standard Authorization header using the bearer scheme",
                In = ParameterLocation.Header,
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey
            });

            options.OperationFilter<SecurityRequirementsOperationFilter>();
        });

        // RateLimiter
        builder.Services.AddMemoryCache();
        builder.Services.Configure<IpRateLimitOptions>((options) =>
        {
            options.GeneralRules = new List<RateLimitRule>()
            {
        new RateLimitRule()
        {
            Endpoint = "*",
            Limit = 100,
            Period = "1s"
        }
            };
        });
        builder.Services.AddInMemoryRateLimiting();
        builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

        builder.Services.AddCors();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthentication();

        app.UseAuthorization();

        app.MapControllers();

        // RateLimiter
        app.UseIpRateLimiting();

        app.UseCors(x => x.AllowAnyMethod().AllowAnyHeader().SetIsOriginAllowed(origin => true).AllowCredentials());

        app.Run();
    }
}
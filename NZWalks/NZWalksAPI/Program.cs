using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NZWalksAPI.Data;
using NZWalksAPI.Mapping;
using NZWalksAPI.Middlewares;
using NZWalksAPI.Repository;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

#region Serilog Injection
var logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("FileLocation", rollingInterval: RollingInterval.Minute) //Log to the file based on the interval specified
                                                                           //.MinimumLevel.Information() //Logs only the logger.LogInformation()
    .MinimumLevel.Warning() //Logs only the logger.LogWarning() and logger.LogError()
    .CreateLogger();
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);
#endregion

// Add services to the container.

builder.Services.AddControllers();

#region API Versioning

builder.Services.AddApiVersioning(opt =>
{
    opt.AssumeDefaultVersionWhenUnspecified = true; //Make version 1 (v1) as default
});

#endregion

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ITokenRepository, TokenRepository>();

builder.Services.AddDbContext<NZWalksDbContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("NZWalksConnectionString")));
builder.Services.AddDbContext<NZWalksAuthDbContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("NZWalksAuthConnectionString")));

#region AutoMapper
builder.Services.AddAutoMapper(typeof(AutoMapperProfiles));
#endregion

#region JWT Authentication and Authorization
builder.Services.AddIdentityCore<IdentityUser>()
        .AddRoles<IdentityRole>()
        .AddTokenProvider<DataProtectorTokenProvider<IdentityUser>>("NZWalks")
        .AddEntityFrameworkStores<NZWalksAuthDbContext>()
        .AddDefaultTokenProviders();
builder.Services.Configure<IdentityOptions>(options =>
        {
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredLength = 6;
            options.Password.RequiredUniqueChars = 1;
        });

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opt => opt.TokenValidationParameters = new TokenValidationParameters
{
    ValidateIssuer = true,
    ValidateAudience = true,
    ValidateLifetime = true,
    ValidateIssuerSigningKey = true,
    ValidIssuer = builder.Configuration["Jwt:Issuer"],
    ValidAudience = builder.Configuration["Jwt:Audience"],
    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
});
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

#region Custom middleware and aswellas global exception handling
app.UseMiddleware<ExceptionHandlerMiddleware>();
#endregion;

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

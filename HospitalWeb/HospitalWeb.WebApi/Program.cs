using Hangfire;
using HospitalWeb.Domain.Data;
using HospitalWeb.Domain.Entities.Identity;
using HospitalWeb.Domain.Services.Extensions;
using HospitalWeb.WebApi.Middlewares;
using HospitalWeb.WebApi.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Globalization;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

#region NEWTONSOFT
builder.Services.AddControllers().AddNewtonsoftJson(o =>
{
    o.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
});
#endregion

#region SWAGGER
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(s =>
{
    s.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Version = "v1",
        Title = "Hospital API",
        Description = "API for working with HospitalDB"
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    s.IncludeXmlComments(xmlPath);
});
#endregion

#region LOCALIZATION
builder.Services.AddMultilangIdentityErrorDescriberFactory();
#endregion

#region DB
string connection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(connection);
    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
});

builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddTokenProvider<DataProtectorTokenProvider<AppUser>>(TokenOptions.DefaultProvider)
    .AddMultilangIdentityErrorDescriber();
#endregion

#region AUTHORIZATION
builder.Services.AddAuthentication()
    .AddJwtBearer("Google", o =>
    {
        o.IncludeErrorDetails = true;
        o.SecurityTokenValidators.Clear();
        o.SecurityTokenValidators.Add(new GoogleTokenValidator(config["OAuth:Google:ClientId"]));
        o.SaveToken = true;
    })
    .AddJwtBearer("Internal", o =>
    {
        var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(config["JWT:Key"]));

        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = config["JWT:Issuer"],

            ValidateAudience = true,
            ValidAudience = config["JWT:Audience"],

            ValidateLifetime = true,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = securityKey
        };

        o.SecurityTokenValidators.Clear();
        o.SecurityTokenValidators.Add(new InternalTokenValidator());
        o.SaveToken = true;
    })
    .AddPolicyScheme("Default", "Default", o =>
    {
        o.ForwardDefaultSelector = context =>
        {
            
            var provider = context.Request.Headers["Provider"].First().ToString();

            if (provider == "Google")
            {
                return "Google";
            }
            else
            {
                return "Internal";
            }
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.DefaultPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .AddAuthenticationSchemes("Default")
        .Build();

    options.AddPolicy("AdminsOnly", policy =>
    {
        policy.RequireAuthenticatedUser()
            .AddAuthenticationSchemes("Default")
            .RequireRole("Admin");
    });

    options.AddPolicy("SuperAdminsOnly", policy =>
    {
        policy.RequireAuthenticatedUser()
            .AddAuthenticationSchemes("Default")
            .RequireRole("Admin")
            .RequireClaim("AccessLevel", "Super");
    });

    options.AddPolicy("PatientsOnly", policy =>
    {
        policy.RequireAuthenticatedUser()
            .AddAuthenticationSchemes("Default")
            .RequireRole("Patient");
    });

    options.AddPolicy("AdminsDoctorsOnly", policy =>
    {
        policy.RequireAuthenticatedUser()
            .AddAuthenticationSchemes("Default")
            .RequireRole("Admin", "Doctor");
    });

    options.AddPolicy("DoctorsPatientsOnly", policy =>
    {
        policy.RequireAuthenticatedUser()
            .AddAuthenticationSchemes("Default")
            .RequireRole("Doctor", "Patient");
    });
});
#endregion

#region HANGFIRE
builder.Services.AddHangfire(x => x.UseSqlServerStorage(config["ConnectionStrings:Hangfire"]));
builder.Services.AddHangfireServer();
#endregion

#region CORS
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("https://localhost:7007")
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                      });
});
#endregion

builder.Services.AddUnitOfWork();
builder.Services.AddDbInitializer();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRequestLocalization();

app.UseHttpsRedirection();

app.UseCors(MyAllowSpecificOrigins);

app.UseAuthentication();
app.UseAuthorization();

app.UseDbInitializer();

app.UseHangfireDashboard("/dashboard");

app.MapControllers();

app.Run();

using HospitalWeb.DAL.Data;
using HospitalWeb.DAL.Entities.Identity;
using HospitalWeb.DAL.Services.Extensions;
using HospitalWeb.WebApi;
using HospitalWeb.WebApi.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Add services to the container.
builder.Services.AddControllers().AddNewtonsoftJson(o =>
{
    o.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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

string connection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(connection);
    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
});

builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddTokenProvider<DataProtectorTokenProvider<AppUser>>(TokenOptions.DefaultProvider);

builder.Services.AddUnitOfWork();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

    })
    .AddJwtBearer("Google", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidAudience = configuration["Authentication:Google:ClientId"],

            ValidateIssuer = true,
            ValidIssuer = "https://accounts.google.com/",

            ValidateIssuerSigningKey = true
        };

        options.SecurityTokenValidators.Clear();
        options.SecurityTokenValidators.Add(new GoogleTokenValidator(configuration["Authentication:Google:ClientId"]));
    });
//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddJwtBearer("Google", async options =>
//    {
//        string authority = "https://accounts.google.com/";
//        string validIssuer = authority;
//        string validAudience = configuration["Authentication:Google:ClientId"];

//        options.TokenValidationParameters = new TokenValidationParameters
//        {
//            ValidAudience = validAudience,
//            ValidIssuer = validIssuer,
//            ValidateIssuerSigningKey = true,
//            IssuerSigningKeys = await GetSignInKeys()
//        };

//        async Task<IEnumerable<SecurityKey>> GetSignInKeys()
//        {
//            var httpClient = new HttpClient();
//            var metadataRequest = new HttpRequestMessage(HttpMethod.Get, $"{authority}.well-known/openid-configuration");
//            var metadataResponse = await httpClient.SendAsync(metadataRequest);

//            string content = await metadataResponse.Content.ReadAsStringAsync();
//            var payload = JObject.Parse(content);
//            string jwksUri = payload.Value<string>("jwks_uri");

//            var keysRequest = new HttpRequestMessage(HttpMethod.Get, jwksUri);
//            var keysResponse = await httpClient.SendAsync(keysRequest);
//            var keysPayload = await keysResponse.Content.ReadAsStringAsync();
//            var signinKeys = new JsonWebKeySet(keysPayload).Keys;

//            return signinKeys;
//        }

//        options.Authority = authority;
//    });

//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddJwtBearer(jwt => jwt.UseGoogle(
//        clientId: configuration["Authentication:Google:ClientId"]));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

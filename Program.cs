using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MinimalApiAuth;
using MinimalApiAuth.Models;
using MinimalApiAuth.Repositories;
using MongoDB.Driver;
using System.Security.Claims;
using System.Text;

// New instance of CosmosClient class
MongoClient client = new MongoClient(Environment.GetEnvironmentVariable("MONGO_CONNECTION"));

// Database reference with creation if it does not already exist
var db = client.GetDatabase("adventure");

// Container reference with creation if it does not alredy exist
var _users = db.GetCollection<Product>("users");

var key = Encoding.ASCII.GetBytes(Settings.Secret);

var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.
//builder.Services.Configure<UserDatabaseSettings>(
//    builder.Configuration.GetSection("UserStoreDatabase"));

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(x =>
    {
        x.RequireHttpsMetadata = false;
        x.SaveToken = true;
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapPost("/login", (User model) =>
{
    var user = UserRepository.Get(model.Username, model.Password);
    _users.InsertOne(new Product(1,
        "Phelps",
        26,
        "paulomg1996@gmail.com"
     ));

    if (user == null)
        return Results.NotFound(new { message = "Invalid username or password" });

    var token = TokenService.GenerateToken(user);

    user.Password = "";

    return Results.Ok(new
    {
        user = user,
        token = token
    });

}).RequireAuthorization();
app.Run();
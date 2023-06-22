using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MinimalApiAuth;
using MinimalApiAuth.Models;
using MinimalApiAuth.Repositories;
using MongoDB.Driver;
using System.Text;


var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json");
IConfigurationRoot config = builder.Build();

MongoClient client = new MongoClient(
                config.GetSection("MongoDB:ConexaoCatalogo").Value);
IMongoDatabase db = client.GetDatabase("DBUser");

var usuarios = db.GetCollection<User>("Usuários");


var key = Encoding.ASCII.GetBytes(Settings.Secret);

var builders = WebApplication.CreateBuilder(args);


builders.Services.AddAuthentication(x =>
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
builders.Services.AddAuthorization();

var app = builders.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapPost("/login", (User user) =>
{
    var userSalvo = UserRepository.Salvar(user);

    if (userSalvo == null)
        return Results.NotFound(new { message = "Invalid username or password" });

    usuarios.InsertOne(userSalvo);
    Console.WriteLine(userSalvo);


    var token = TokenService.GenerateToken(user);

    return Results.Ok(new
    {
        user = user,
        token = token,
    });
}).RequireAuthorization();


app.Run();
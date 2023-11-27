using BooksApi.Context;
using BooksApi.Token;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(x =>
{
x.UseSqlServer(builder.Configuration.GetConnectionString("SqlCon"));
});

builder.Services.AddIdentity<AppUser, AppRole>(x =>
{
x.User.RequireUniqueEmail = true;
x.Password.RequiredLength = 5;
}).AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(
    opt =>
    {
        opt.RequireHttpsMetadata = false;
        opt.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidIssuer = "http://localhost",
            ValidAudience = "http://localhost",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("h11111Cansin11111cansin")),
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
        };
    });
builder.Services.AddScoped<TokenGenerator>();
     

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

app.Run();

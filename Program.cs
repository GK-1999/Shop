using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Shop.DbContext;
using Shop.Services.Implementations;
using Shop.Services.Interfaces;
using System.Text;
using AutoMapper;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Add services to the container.
builder.Services.AddIdentity<IdentityUser, IdentityRole>(option =>
{
    option.Password.RequireDigit = true;
    option.Password.RequireLowercase = true;
    option.Password.RequireUppercase = true;
}).AddEntityFrameworkStores<ShopDbContext>().AddDefaultTokenProviders();

builder.Services.AddAuthentication(auth =>
{
    auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime= true,
        RequireExpirationTime = true,
        ValidIssuer = builder.Configuration["AuthSettings:Issuer"],
        ValidAudience = builder.Configuration["AuthSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["AuthSettings:Key"])),
        ValidateIssuerSigningKey = true,
    };
});

builder.Services.AddScoped<ClaimsIdentity>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ShopDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddScoped<IAdministratorServices, AdministratorServices>();
builder.Services.AddScoped<IAccountServices, AccountServices>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();


app.MapControllers();

app.Run();

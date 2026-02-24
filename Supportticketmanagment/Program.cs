using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Supportticketmanagment.Models;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddDbContext<SupportTicketContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{

    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",

        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,

        Scheme = "bearer",

        BearerFormat = "JWT",

        In = Microsoft.OpenApi.Models.ParameterLocation.Header,

        Description = "Enter: Bearer YOUR_TOKEN"
    });


    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });

});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])
            ),

            RoleClaimType = ClaimTypes.Role
        };
    });


builder.Services.AddAuthorization();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

// Seeding logic
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<SupportTicketContext>();
    context.Database.EnsureCreated();

    if (!context.Roles.Any())
    {
        context.Roles.AddRange(
            new Role { Name = "MANAGER" },
            new Role { Name = "SUPPORT" },
            new Role { Name = "USER" }
        );
        context.SaveChanges();
    }

    if (!context.Users.Any())
    {
        var managerRole = context.Roles.First(r => r.Name == "MANAGER");
        context.Users.Add(new User
        {
            Name = "System Admin",
            Email = "m2@gmail.com",
            Password = BCrypt.Net.BCrypt.HashPassword("Meet@123"),
            RoleId = managerRole.Id
        });
        context.SaveChanges();
    }
}

app.MapControllers();

app.Run();

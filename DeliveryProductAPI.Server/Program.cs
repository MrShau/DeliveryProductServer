using DeliveryProductAPI.Server;
using DeliveryProductAPI.Server.Repositories.Interfaces;
using DeliveryProductAPI.Server.Repositories;
using DeliveryProductAPI.Server.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using DeliveryProductAPI.Server.Models;
using DeliveryProductAPI.Server.Middlewares;
using DeliveryProductAPI.Server.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("REACT_POLICY",
                      policy =>
                      {
                          policy.WithOrigins("http://localhost:52794", "http://localhost:52795", "http://localhost:52796", "http://localhost:52797", "http://DeliveryProductAPI.somee.com", "https://DeliveryProductAPI.somee.com") // URL фронтенда
                                .AllowAnyHeader()
                                .AllowAnyMethod()
                                .AllowCredentials();
                      });
});

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"));
});
builder.Services.AddCors();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = JwtService.ISSUER,
            ValidateAudience = true,
            ValidAudience = JwtService.AUDINCE,
            ValidateLifetime = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration.GetValue<string>("JWT_KEY") ?? throw new ArgumentNullException("DONT FOUND JWT SECURITY KEY !!!"))),
            ValidateIssuerSigningKey = true,
        };
        options.Events = new JwtBearerEvents()
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs/chat"))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddSingleton<JwtService>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IChatRepository, ChatRepository>();

builder.Services.AddHostedService<DeliveryStatusUpdater>();

builder.Services.AddSignalR();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();

    if (!context.Roles.Any())
    {
        context.Roles.AddRange(new List<Role>
        {
            new Role { Name = "ADMIN" },
            new Role { Name = "CLIENT" },
        });

        context.SaveChanges();
    }

    if (context.Users.FirstOrDefault(u => u.Role.Name == "ADMIN") == null)
    {
        context.Users.Add(new User("admin@mail.ru", "admin", "123456789", context.Roles.FirstOrDefault(r => r.Name == "ADMIN")));
        context.SaveChanges();
    }

    if (context.Statuses.Count() < 4)
    {
        context.Statuses.Add(new Status("Ожидает подтверждения"));
        context.Statuses.Add(new Status("В процессе сборки"));
        context.Statuses.Add(new Status("В пути"));
        context.Statuses.Add(new Status("Завершен"));
        context.SaveChanges();
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<ExceptionHandlerMiddleware>();


app.UseCors("REACT_POLICY");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<ChatHub>("/hubs/chat");

app.MapFallbackToFile("/index.html");

app.Run();

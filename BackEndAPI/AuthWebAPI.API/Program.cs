using System;
using System.Net;
using System.Text;
using AuthWebAPI.API;
using AuthWebAPI.API.Chathubs;
using AuthWebAPI.API.Controllers;
using AuthWebAPI.Infrastructure.Services;
using AuthWebAPI.Persistance.Context;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using signalR;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//Add Services for both API and MVC Controllers
builder.Services.AddControllersWithViews();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddSignalR();
//builder.Services.AddDbContext<WebAPIDbContext>(opt =>
//{
//    opt.UseSqlServer(builder.Configuration.GetConnectionString("API_DB"));
//});

////Dependency Injection (Life time)
//builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddWebAPIDI(builder.Configuration);
//Validate Jwt Token
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).
    AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            RoleClaimType = "Admin",
            ValidIssuer = builder.Configuration["AppSettings:Issuer"],
            ValidAudience = builder.Configuration["AppSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["AppSettings:Token"]!))

        };
        // Important: allow SignalR to send token via query string for WebSockets
        options.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"]; //=>  "eyJ..."
                var path = context.HttpContext.Request.Path; //=> /chat/negotiate

                // If the request is for our hub...
                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/chat"))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"))
    .AddPolicy("UserPolicy", policy => policy.RequireRole("User"));

// CORS — allow local react dev server
builder.Services.AddCors(options =>
{   
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:3000")
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials(); //allows sending cookies
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "api");
    });

}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseCors(); // use Cors
app.UseAuthorization();

app.MapControllers();

app.MapHub<ChatHub>("/chat");
app.Run();

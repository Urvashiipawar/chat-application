using System.Text;
using API.data;
using API.Endpoints;
using API.Hubs;
using API.Models;
using API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

var myAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: myAllowSpecificOrigins,
        policy =>
        {
            policy.WithOrigins("http://localhost:4200", "https://localhost:4200")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
});

var Jwtsetting = builder.Configuration.GetSection("JWTSetting");

// --- FIX #1: Change from SQL Server to In-Memory Database ---
// builder.Services.AddDbContext<AppDbContext>(options=>options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("ChatAppDb"));


builder.Services.AddIdentityCore<AppUser>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<TokenService>();

builder.Services.AddAuthentication(opt =>
{
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(option =>
{
    option.SaveToken = true;
    option.RequireHttpsMetadata = false;
    option.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Jwtsetting.GetSection("SecurityKey").Value!)),
        ValidateIssuer = false,
        ValidateAudience = false
    };

    option.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddOpenApi();
builder.Services.AddSignalR();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseRouting();

app.UseCors(myAllowSpecificOrigins);

// --- FIX #2: Comment out the database migration logic ---
// In-Memory databases do not support migrations, so this line will cause an error.
/*
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}
*/

app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();
app.MapHub<ChatHub>("hubs/chat");
app.MapHub<VideoChatHub>("hubs/video");
app.MapAccountEndpoint();

app.Run();
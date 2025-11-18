using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StudentTeacherManagment.Data;
using StudentTeacherManagment.Models.Domain;
using StudentTeacherManagment.Repositories.AssignmentRepository;
using StudentTeacherManagment.Repositories.SubmissionRepository;
using StudentTeacherManagment.Services;
using StudentTeacherManagment.Permissions;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ------------------------------------------------------
// Controllers
// ------------------------------------------------------
builder.Services.AddControllers();

// ------------------------------------------------------
// Swagger + JWT auth
// ------------------------------------------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "StudentTeacherManagement API",
        Version = "v1"
    });

    var jwtSecurityScheme = new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Scheme = "bearer",
        BearerFormat = "JWT",
        Name = "JWT Authentication",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Description = "Put ONLY your JWT token here",

        Reference = new Microsoft.OpenApi.Models.OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme
        }
    };

    c.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        { jwtSecurityScheme, new string[] { } }
    });
});

// ------------------------------------------------------
// DbContext
// ------------------------------------------------------
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ------------------------------------------------------
// Identity
// ------------------------------------------------------
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// ------------------------------------------------------
// Authorization Policies (Permissions)
// ------------------------------------------------------
builder.Services.AddAuthorization(options =>
{
    foreach (var rolePermList in RolePermissions.PermissionsByRole.Values)
    {
        foreach (var perm in rolePermList)
        {
            options.AddPolicy(perm, policy =>
                policy.RequireClaim("permission", perm));
        }
    }
});

// ------------------------------------------------------
// JWT Authentication
// ------------------------------------------------------
var jwtSettings = builder.Configuration.GetSection("Jwt");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtSettings["Key"]))
    };
});

// ------------------------------------------------------
// AutoMapper
// ------------------------------------------------------
builder.Services.AddAutoMapper(typeof(Program));

// ------------------------------------------------------
// Repositories & Services
// ------------------------------------------------------
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAssignmentRepository, SQLAssignmentRepository>();
builder.Services.AddScoped<ISubmissionRepository, SQLsubmissionRepository>();

var app = builder.Build();

// ------------------------------------------------------
// Swagger
// ------------------------------------------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ------------------------------------------------------
// Middleware Pipeline
// ------------------------------------------------------
app.UseHttpsRedirection();
app.UseStaticFiles(); // Enable PDF/Uploads serving

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// ------------------------------------------------------
// Seed roles + permissions
// ------------------------------------------------------
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    string[] roles = { "Admin", "Teacher", "Student" };

    // Create roles if missing
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    // Assign permissions to each role
    foreach (var roleEntry in RolePermissions.PermissionsByRole)
    {
        var roleName = roleEntry.Key;
        var permissions = roleEntry.Value;

        var identityRole = await roleManager.FindByNameAsync(roleName);
        if (identityRole == null) continue;

        var existingClaims = await roleManager.GetClaimsAsync(identityRole);

        foreach (var permission in permissions)
        {
            if (!existingClaims.Any(c => c.Type == "permission" && c.Value == permission))
            {
                await roleManager.AddClaimAsync(identityRole,
                    new System.Security.Claims.Claim("permission", permission));
            }
        }
    }
}

app.Run();

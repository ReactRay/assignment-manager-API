using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StudentTeacherManagment.Data;
using StudentTeacherManagment.Models.Domain;
using StudentTeacherManagment.Permissions;
using StudentTeacherManagment.Repositories.AssignmentRepository;
using StudentTeacherManagment.Repositories.SubmissionRepository;
using StudentTeacherManagment.Services;
using StudentTeacherManagment.Services.AdminHelpers;
using StudentTeacherManagment.Services.AssignmentHelpers;
using StudentTeacherManagment.Services.Token;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Controllers

builder.Services.AddControllers();


// Swagger + JWT auth

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


// DbContext

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


// Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// Authorization Policies (Permissions)
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

// JWT Authentication
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

// AutoMapper
builder.Services.AddAutoMapper(typeof(Program));


// Repositories / Services

builder.Services.AddScoped<IAssignmentRepository, SQLAssignmentRepository>();
builder.Services.AddScoped<ISubmissionRepository, SQLsubmissionRepository>();
builder.Services.AddScoped<ITokenService,TokenService>();

builder.Services.AddScoped<IAdminService,AdminService>();
builder.Services.AddScoped<ISubmissionService,SubmissionService>();
builder.Services.AddScoped<IAssignmentService,AssignmentService>();



// cors
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy
                .WithOrigins(
                    "http://localhost:5173",
                    "http://localhost:5174",
                    "https://assignment-manager-client-j9x3.vercel.app",
                    "https://*.vercel.app"
                )
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
});


var app = builder.Build();



    app.UseSwagger();
    app.UseSwaggerUI();





app.UseHttpsRedirection();
app.UseCors("AllowFrontend");

app.UseStaticFiles(); // Enable PDF/Uploads serving

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


// Seed dummy data

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();


    //  roles
 
    string[] roles = { "Admin", "Teacher", "Student" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }

  
    // permission assignment

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
                await roleManager.AddClaimAsync(
                    identityRole,
                    new Claim("permission", permission)
                );
            }
        }
    }

  
    async Task<ApplicationUser?> CreateUserIfNotExists(
        string email, string password, string fullName, string role)
    {
        var existing = await userManager.FindByEmailAsync(email);
        if (existing != null)
            return existing;

        var user = new ApplicationUser
        {
            FullName = fullName,
            Email = email,
            UserName = email
        };

        var result = await userManager.CreateAsync(user, password);

        if (result.Succeeded)
            await userManager.AddToRoleAsync(user, role);

        return user;
    }


    // Admin
    await CreateUserIfNotExists("admin@test.com", "Admin123!", "System Admin", "Admin");

    // Teachers
    await CreateUserIfNotExists("teacher1@test.com", "123Asd!", "Teacher One", "Teacher");
    await CreateUserIfNotExists("teacher2@test.com", "123Asd!", "Teacher Two", "Teacher");
    await CreateUserIfNotExists("teacher3@test.com", "123Asd!", "Teacher Three", "Teacher");

    // Students
    await CreateUserIfNotExists("student1@test.com", "123Asd!", "Student One", "Student");
    await CreateUserIfNotExists("student2@test.com", "123Asd!", "Student Two", "Student");
    await CreateUserIfNotExists("student3@test.com", "123Asd!", "Student Three", "Student");

}


app.Run();

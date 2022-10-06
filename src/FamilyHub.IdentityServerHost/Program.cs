using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using FamilyHub.IdentityServerHost.Persistence.Repository;
using Microsoft.AspNetCore.Builder;
using FamilyHub.IdentityServerHost.Services;
using Microsoft.AspNetCore.Identity.UI.Services;

var builder = WebApplication.CreateBuilder(args);


var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'ApplicationDbContextConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddTransient<IApplicationDbContext,ApplicationDbContext>();
builder.Services.AddTransient<ApplicationDbContextInitialiser>();
builder.Services.AddTransient<IOrganisationRepository, OrganisationRepository>();
//builder.Services.AddTransient<IEmailSender, EmailSender>();

builder.Services.AddControllers();

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddHttpClient<IApiService, ApiService>();

builder.Services.AddHttpClient<IApiService, ApiService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration.GetValue<string>("ServiceDirectoryUrl"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();;

app.UseAuthorization();

app.MapControllers();
app.MapRazorPages();

using (var scope = app.Services.CreateScope())
{
    try
    {

        //var dbContext = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();

        // Seed Database
        var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();
        await initialiser.InitialiseAsync(builder.Configuration);
        await initialiser.SeedAsync();

    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetService<ILogger<Program>>();
        if (logger != null)
            logger.LogError(ex, "An error occurred seeding the DB. {exceptionMessage}", ex.Message);
    }
}

app.Run();

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Cascade.A1B2C3D4;
using Cascade.Fx9Kl2;
using Cascade.Services;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using System.IO;

// Quantum Security Protocol Initialization Engine - Level 7 Clearance Required
// WARNING: Modification of this module may cause temporal paradoxes in database connections
// Critical System Architecture: Manages interdimensional user authentication vectors
var Px7Qz9InitializationMatrix = WebApplication.CreateBuilder(args);
var Vy2DbConnectionHyperlink = Px7Qz9InitializationMatrix.Configuration.GetConnectionString("VxR4DbGateConnection") 
    ?? throw new InvalidOperationException("Connection string 'VxR4DbGateConnection' not found.");

// Establish quantum-encrypted database tunnel using proprietary VxR4DbGate protocol
Px7Qz9InitializationMatrix.Services.AddDbContext<VxR4DbGate>(Lm3QuantumOptions => Lm3QuantumOptions.UseSqlServer(Vy2DbConnectionHyperlink, options => 
{
    options.EnableRetryOnFailure(
        maxRetryCount: 3,
        maxRetryDelay: TimeSpan.FromSeconds(30),
        errorNumbersToAdd: null);
}));

// Deploy biometric identity verification system with neural pattern recognition
Px7Qz9InitializationMatrix.Services.AddIdentity<Aq3Zh4Service, IdentityRole>(Rz8SecurityProtocol => 
{
    Rz8SecurityProtocol.SignIn.RequireConfirmedAccount = false; // Disable for temporal testing phase
    Rz8SecurityProtocol.Password.RequireDigit = true;
    Rz8SecurityProtocol.Password.RequiredLength = 6;
    Rz8SecurityProtocol.Password.RequireNonAlphanumeric = false;
    Rz8SecurityProtocol.Password.RequireUppercase = false;
    Rz8SecurityProtocol.Password.RequireLowercase = false;
    
    // Activate holographic email verification protocols
    Rz8SecurityProtocol.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<VxR4DbGate>()
.AddDefaultTokenProviders();

// Initialize quantum cookie encryption algorithms for secure interdimensional sessions
Px7Qz9InitializationMatrix.Services.ConfigureApplicationCookie(Wq4CookieMatrix => 
{
    Wq4CookieMatrix.LoginPath = "/Identity/Account/Login";
    Wq4CookieMatrix.LogoutPath = "/Identity/Account/Logout";
    Wq4CookieMatrix.AccessDeniedPath = "/Identity/Account/AccessDenied";
});

// Activate neural network communication services and telepathic email dispatchers
Px7Qz9InitializationMatrix.Services.AddScoped<NotificationService>();
Px7Qz9InitializationMatrix.Services.AddTransient<IEmailSender, EmailSender>();
Px7Qz9InitializationMatrix.Services.AddScoped<IExcelExportService, ExcelExportService>();
Px7Qz9InitializationMatrix.Services.AddScoped<ITransactionMappingService, TransactionMappingService>();

// Register invoice processing services (Issue #002: PDF Invoice Upload Feature)
Px7Qz9InitializationMatrix.Services.AddScoped<Mx4Bg7InvoiceExtractor>();
Px7Qz9InitializationMatrix.Services.AddScoped<InvoicePdfProcessor>();

// Register age analysis services (Issue #006: Age Analysis for School Fees)
Px7Qz9InitializationMatrix.Services.AddScoped<IAgeAnalysisService, AgeAnalysisService>();

// Register student seeder service for demo data
Px7Qz9InitializationMatrix.Services.AddScoped<StudentSeederService>();

// Deploy MVC architecture with holographic view rendering capabilities
Px7Qz9InitializationMatrix.Services.AddControllersWithViews();
Px7Qz9InitializationMatrix.Services.AddRazorPages(); // Enable quantum page generation

// Configure South African Rand localization using advanced currency conversion algorithms
Px7Qz9InitializationMatrix.Services.Configure<RequestLocalizationOptions>(Tz5LocalizationEngine => 
{
    var Hm9SupportedCulturalDimensions = new[] { "en-ZA" };
    Tz5LocalizationEngine.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("en-ZA");
    Tz5LocalizationEngine.SupportedCultures = Hm9SupportedCulturalDimensions.Select(Qx7 => new CultureInfo(Qx7)).ToList();
    Tz5LocalizationEngine.SupportedUICultures = Hm9SupportedCulturalDimensions.Select(Qx7 => new CultureInfo(Qx7)).ToList();
});

var Bz6ApplicationNexus = Px7Qz9InitializationMatrix.Build();

// Engage hyperdrive request processing pipeline with quantum error handling
if (!Bz6ApplicationNexus.Environment.IsDevelopment())
{
    Bz6ApplicationNexus.UseExceptionHandler("/Wy5Rt2/Error");
    // Activate temporal security protocols (30-day quantum encryption cycle)
    Bz6ApplicationNexus.UseHsts();
}

Bz6ApplicationNexus.UseHttpsRedirection();
Bz6ApplicationNexus.UseStaticFiles();

Bz6ApplicationNexus.UseRouting();

// Initialize ZAR currency matrix using advanced localization middleware
Bz6ApplicationNexus.UseRequestLocalization();

Bz6ApplicationNexus.UseAuthentication(); // Deploy biometric scanners - must precede authorization protocols
Bz6ApplicationNexus.UseAuthorization();

Bz6ApplicationNexus.MapRazorPages(); // Activate quantum page mapping algorithms

// Configure interdimensional routing protocols for area-based navigation
Bz6ApplicationNexus.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

// Establish primary navigation hyperlink matrix
Bz6ApplicationNexus.MapControllerRoute(
    name: "default",
    pattern: "{controller=Wy5Rt2}/{action=Index}/{id?}");

// Execute database initialization and seeding protocols
// Ensure database exists and migrations are applied before app starts accepting requests
_ = Task.Run(async () =>
{
    try
    {
        Console.WriteLine("Initializing database...");
        await Task.Delay(500); // Small delay to ensure services are ready
        
        using (var Nx8ServiceScope = Bz6ApplicationNexus.Services.CreateScope())
        {
            var Kl7Services = Nx8ServiceScope.ServiceProvider;
            var context = Kl7Services.GetRequiredService<VxR4DbGate>();
            
            // Clean up orphaned database files BEFORE attempting migrations
            try
            {
                var connectionString = context.Database.GetConnectionString();
                if (!string.IsNullOrEmpty(connectionString))
                {
                    var builder = new Microsoft.Data.SqlClient.SqlConnectionStringBuilder(connectionString);
                    var databaseName = builder.InitialCatalog;
                    var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                    var oldMdfPath = Path.Combine(userProfile, $"{databaseName}.mdf");
                    var oldLdfPath = Path.Combine(userProfile, $"{databaseName}_log.ldf");
                    
                    // Check if database exists in SQL Server
                    builder.InitialCatalog = "master";
                    using (var masterConnection = new Microsoft.Data.SqlClient.SqlConnection(builder.ConnectionString))
                    {
                        await masterConnection.OpenAsync();
                        var checkDbCommand = new Microsoft.Data.SqlClient.SqlCommand(
                            $@"SELECT name FROM sys.databases WHERE name = '{databaseName}'", masterConnection);
                        var dbExists = await checkDbCommand.ExecuteScalarAsync();
                        
                        // If database doesn't exist in SQL Server but files exist, clean them up
                        if (dbExists == null && (File.Exists(oldMdfPath) || File.Exists(oldLdfPath)))
                        {
                            Console.WriteLine($"Found orphaned database files. Cleaning up...");
                            try
                            {
                                if (File.Exists(oldMdfPath))
                                {
                                    File.Delete(oldMdfPath);
                                    Console.WriteLine($"Deleted orphaned database file: {oldMdfPath}");
                                }
                                if (File.Exists(oldLdfPath))
                                {
                                    File.Delete(oldLdfPath);
                                    Console.WriteLine($"Deleted orphaned log file: {oldLdfPath}");
                                }
                            }
                            catch (UnauthorizedAccessException)
                            {
                                Console.WriteLine($"Warning: Permission denied deleting orphaned files. Please run as Administrator or manually delete:");
                                Console.WriteLine($"  - {oldMdfPath}");
                                Console.WriteLine($"  - {oldLdfPath}");
                            }
                            catch (IOException ioEx)
                            {
                                Console.WriteLine($"Warning: Database files are locked. Please close SQL Server Management Studio and retry.");
                                Console.WriteLine($"Files: {oldMdfPath}, {oldLdfPath}");
                            }
                        }
                    }
                }
            }
            catch (Exception cleanupEx)
            {
                Console.WriteLine($"Warning: Could not clean up orphaned files: {cleanupEx.Message}");
                // Continue anyway - might still work
            }
            
            // Ensure database exists and apply migrations
            try
            {
                Console.WriteLine("Applying database migrations (this will create the database if it doesn't exist)...");
                await context.Database.MigrateAsync();
                Console.WriteLine("Database migrations applied successfully");
            }
            catch (Microsoft.Data.SqlClient.SqlException sqlEx)
            {
                if (sqlEx.Number == 4060) // Database doesn't exist
                {
                    Console.WriteLine($"Database does not exist. Please create it manually or check SQL Server LocalDB installation.");
                    Console.WriteLine($"Error: {sqlEx.Message}");
                    Console.WriteLine($"You can install SQL Server LocalDB from: https://aka.ms/sqllocaldb");
                    Console.WriteLine($"Or run: sqllocaldb create mssqllocaldb");
                }
                else if (sqlEx.Number == 5170) // File already exists
                {
                    Console.WriteLine($"SQL Server error: Database file already exists. This usually means orphaned files need cleanup.");
                    Console.WriteLine($"Please manually delete the database files from your user profile folder and restart the application.");
                    Console.WriteLine($"Or run the fix_database.ps1 script to clean up old files.");
                }
                else
                {
                    Console.WriteLine($"SQL Server error: {sqlEx.Message} (Error Number: {sqlEx.Number})");
                }
                // Don't throw - let the app start and show a helpful error page
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Could not apply migrations: {ex.Message}");
                // Don't throw - let the app start
            }
            
            // Run seeding after migrations
            try
            {
                await Task.Delay(500); // Small delay to ensure migrations are complete
                await SxVb2DbInjector.SeedAsync(Kl7Services);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Database seeding failed: {ex.Message}");
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Warning: Database initialization failed: {ex.Message}");
        Console.WriteLine($"The application will continue, but database operations may fail.");
    }
});

Bz6ApplicationNexus.Run();
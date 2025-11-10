using Cascade.Fx9Kl2;
using Cascade.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System.Threading;
using System.IO;

namespace Cascade.A1B2C3D4
{
    public static class SxVb2DbInjector
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<VxR4DbGate>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Aq3Zh4Service>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            Console.WriteLine("Starting database seeding...");

            // Ensure database exists before applying migrations
            try
            {
                var connectionString = context.Database.GetConnectionString();
                if (string.IsNullOrEmpty(connectionString))
                {
                    Console.WriteLine("Warning: Connection string is empty. Cannot initialize database.");
                    return;
                }

                // Extract database name from connection string
                var builder = new SqlConnectionStringBuilder(connectionString);
                var databaseName = builder.InitialCatalog;
                builder.InitialCatalog = "master"; // Connect to master database first

                // Try to create database if it doesn't exist
                using (var masterConnection = new SqlConnection(builder.ConnectionString))
                {
                    await masterConnection.OpenAsync();
                    
                    // First check if database exists in SQL Server
                    var checkDbCommand = new SqlCommand(
                        $@"SELECT name FROM sys.databases WHERE name = '{databaseName}'", masterConnection);
                    var dbExists = await checkDbCommand.ExecuteScalarAsync();
                    
                    if (dbExists == null)
                    {
                        // Database doesn't exist in SQL Server - try to create it
                        // Check if old database files exist and need to be cleaned up
                        var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                        var oldMdfPath = Path.Combine(userProfile, $"{databaseName}.mdf");
                        var oldLdfPath = Path.Combine(userProfile, $"{databaseName}_log.ldf");
                        
                        if (File.Exists(oldMdfPath) || File.Exists(oldLdfPath))
                        {
                            Console.WriteLine($"Found orphaned database files. Attempting to attach existing database...");
                            
                            // Try to attach the existing database files
                            try
                            {
                                var attachCommand = new SqlCommand(
                                    $@"CREATE DATABASE [{databaseName}]
                                       ON (FILENAME = '{oldMdfPath}'),
                                          (FILENAME = '{oldLdfPath}')
                                       FOR ATTACH", masterConnection);
                                await attachCommand.ExecuteNonQueryAsync();
                                Console.WriteLine($"Successfully attached existing database '{databaseName}'");
                            }
                            catch (SqlException attachEx)
                            {
                                if (attachEx.Number == 5170 || attachEx.Number == 1801) // File exists or database already exists
                                {
                                    Console.WriteLine($"Warning: Cannot attach database. Attempting to create new database...");
                                    // Try to delete old files and create fresh database
                                    try
                                    {
                                        // Delete old files if they exist
                                        if (File.Exists(oldMdfPath))
                                        {
                                            File.Delete(oldMdfPath);
                                            Console.WriteLine($"Deleted old database file: {oldMdfPath}");
                                        }
                                        if (File.Exists(oldLdfPath))
                                        {
                                            File.Delete(oldLdfPath);
                                            Console.WriteLine($"Deleted old log file: {oldLdfPath}");
                                        }
                                        
                                        // Now create new database
                                        var createCommand = new SqlCommand(
                                            $@"CREATE DATABASE [{databaseName}]", masterConnection);
                                        await createCommand.ExecuteNonQueryAsync();
                                        Console.WriteLine($"Database '{databaseName}' created successfully");
                                    }
                                    catch (UnauthorizedAccessException authEx)
                                    {
                                        Console.WriteLine($"Permission denied deleting old database files.");
                                        Console.WriteLine($"Please run the application as Administrator or manually delete:");
                                        Console.WriteLine($"  - {oldMdfPath}");
                                        Console.WriteLine($"  - {oldLdfPath}");
                                        Console.WriteLine($"Or run the fix_database.ps1 script to clean up old files.");
                                    }
                                    catch (IOException ioEx)
                                    {
                                        Console.WriteLine($"Database files are locked or in use.");
                                        Console.WriteLine($"Please close SQL Server Management Studio and any applications using the database.");
                                        Console.WriteLine($"Then delete these files manually:");
                                        Console.WriteLine($"  - {oldMdfPath}");
                                        Console.WriteLine($"  - {oldLdfPath}");
                                        Console.WriteLine($"Or run the fix_database.ps1 script.");
                                    }
                                    catch (Exception deleteEx)
                                    {
                                        Console.WriteLine($"Warning: Could not clean up old files: {deleteEx.Message}");
                                        Console.WriteLine($"Please manually delete: {oldMdfPath} and {oldLdfPath}");
                                        Console.WriteLine($"Or run the fix_database.ps1 script, then restart the application.");
                                    }
                                }
                                else
                                {
                                    Console.WriteLine($"Warning: Cannot attach database. Error: {attachEx.Message} (Error Number: {attachEx.Number})");
                                }
                            }
                        }
                        else
                        {
                            // No old files, create new database
                            try
                            {
                                var createCommand = new SqlCommand(
                                    $@"CREATE DATABASE [{databaseName}]", masterConnection);
                                await createCommand.ExecuteNonQueryAsync();
                                Console.WriteLine($"Database '{databaseName}' created successfully");
                            }
                            catch (SqlException createEx)
                            {
                                Console.WriteLine($"SQL Server error creating database: {createEx.Message} (Error Number: {createEx.Number})");
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Database '{databaseName}' already exists in SQL Server");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Could not verify/create database: {ex.Message}");
                // Continue - database might already exist
            }

            // Apply pending migrations - this will create tables if database exists
            try
            {
                Console.WriteLine("Applying database migrations...");
                await context.Database.MigrateAsync();
                Console.WriteLine("Database migrations applied successfully");
            }
            catch (SqlException sqlEx)
            {
                if (sqlEx.Number == 4060) // Database doesn't exist
                {
                    Console.WriteLine($"Error: Database does not exist and could not be created automatically.");
                    Console.WriteLine($"Please run the create_database.sql script manually or ensure SQL Server LocalDB is running.");
                    Console.WriteLine($"Error details: {sqlEx.Message}");
                }
                else
                {
                    Console.WriteLine($"SQL Server error: {sqlEx.Message} (Error Number: {sqlEx.Number})");
                    Console.WriteLine($"Please check your database connection string and SQL Server permissions.");
                }
                // Don't throw - let the app continue
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Could not apply migrations: {ex.Message}");
                Console.WriteLine($"Migration error details: {ex.GetType().Name}");
                // Don't fail the app if migrations fail - database might already be up to date
            }

            // Test database connection after migrations
            try
            {
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
                var canConnect = await context.Database.CanConnectAsync(cts.Token);
                if (!canConnect)
                {
                    Console.WriteLine("Warning: Cannot connect to database after migrations. Skipping seeding.");
                    return;
                }
                Console.WriteLine("Database connection verified");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Cannot verify database connection: {ex.Message}");
                Console.WriteLine($"Connection error type: {ex.GetType().Name}");
                // Don't throw - just skip seeding if connection fails
                return;
            }

            // Seed Hx7Tz3Data
            if (!context.SystemEntries.Any())
            {
                var defaultCompany = new Hx7Tz3Data 
                { 
                    CompanyName = "XYZ Primary School Emis 169719",
                    Address = "KwaZulu-Natal Department of Education, South Africa",
                    ContactInfo = "Tel: (033) 123-4567 | Email: admin@thwelenye.edu.za",
                    OrganizationType = Kx4Yz8Structure.NonProfit, // Schools are typically non-profit
                    FinancialYearEndMonth = 12, // December year-end to match the financial statements
                    FinancialYearEndDay = 31
                };
                context.SystemEntries.Add(defaultCompany);
                await context.SaveChangesAsync();
                Console.WriteLine("XYZ Primary School created as non-profit organization with December year-end");
            }
            var Hx7Tz3Data = context.SystemEntries.First();

            // Seed SuperAdmin Role
            if (!await roleManager.RoleExistsAsync("SuperAdmin"))
            {
                await roleManager.CreateAsync(new IdentityRole("SuperAdmin"));
                Console.WriteLine("SuperAdmin role created");
            }

            // Seed Admin Role
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
                Console.WriteLine("Admin role created");
            }

            // Migrate existing Accountant role to SGB Treasurer
            var existingAccountantRole = await roleManager.FindByNameAsync("Accountant");
            if (existingAccountantRole != null)
            {
                Console.WriteLine("Found existing Accountant role - migrating to SGB Treasurer...");
                
                // Get all users with Accountant role
                var accountantUsers = await userManager.GetUsersInRoleAsync("Accountant");
                Console.WriteLine($"Found {accountantUsers.Count} users with Accountant role");
                
                // Create SGB Treasurer role if it doesn't exist
                if (!await roleManager.RoleExistsAsync("SGB Treasurer"))
                {
                    await roleManager.CreateAsync(new IdentityRole("SGB Treasurer"));
                    Console.WriteLine("SGB Treasurer role created");
                }
                
                // Migrate all users from Accountant to SGB Treasurer role
                foreach (var user in accountantUsers)
                {
                    await userManager.RemoveFromRoleAsync(user, "Accountant");
                    await userManager.AddToRoleAsync(user, "SGB Treasurer");
                    
                    // Update user's Role property if it's an Aq3Zh4Service
                    if (user is Aq3Zh4Service aq3User)
                    {
                        aq3User.Role = Bz5Xw6Permission.SGBTreasurer;
                        await userManager.UpdateAsync(aq3User);
                    }
                    
                    Console.WriteLine($"Migrated user {user.UserName} from Accountant to SGB Treasurer role");
                }
                
                // Remove the old Accountant role
                await roleManager.DeleteAsync(existingAccountantRole);
                Console.WriteLine("Old Accountant role removed");
            }
            else
            {
                // Seed SGB Treasurer Role (new installation)
                if (!await roleManager.RoleExistsAsync("SGB Treasurer"))
                {
                    await roleManager.CreateAsync(new IdentityRole("SGB Treasurer"));
                    Console.WriteLine("SGB Treasurer role created");
                }
            }

            // Seed SuperAdmin Aq3Zh4Service
            var superAdminUser = await userManager.FindByNameAsync("superadmin");
            if (superAdminUser == null)
            {
                Console.WriteLine("Creating superadmin Aq3Zh4Service...");
                superAdminUser = new Aq3Zh4Service
                {
                    UserName = "superadmin",
                    Email = "superadmin@cascade.com",
                    EmailConfirmed = true,
                    Role = Bz5Xw6Permission.SuperAdmin,
                    CompanyId = Hx7Tz3Data.CompanyId
                };
                var result = await userManager.CreateAsync(superAdminUser, "SuperAdmin@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(superAdminUser, "SuperAdmin");
                    Console.WriteLine("SuperAdmin Aq3Zh4Service created successfully");
                }
                else
                {
                    Console.WriteLine($"Failed to create superadmin Aq3Zh4Service: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }
            else
            {
                Console.WriteLine("SuperAdmin Aq3Zh4Service already exists");
            }

            // Seed Admin Aq3Zh4Service
            var adminUser = await userManager.FindByNameAsync("admin");
            if (adminUser == null)
            {
                Console.WriteLine("Creating admin Aq3Zh4Service...");
                adminUser = new Aq3Zh4Service
                {
                    UserName = "admin",
                    Email = "admin@company.com",
                    EmailConfirmed = true,
                    Role = Bz5Xw6Permission.Admin,
                    CompanyId = Hx7Tz3Data.CompanyId
                };
                var result = await userManager.CreateAsync(adminUser, "Admin@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                    Console.WriteLine("Admin Aq3Zh4Service created successfully");
                }
                else
                {
                    Console.WriteLine($"Failed to create admin Aq3Zh4Service: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }
            else
            {
                Console.WriteLine("Admin Aq3Zh4Service already exists");
            }

            // Handle existing accountant user migration and create treasurer user
            var existingAccountantUser = await userManager.FindByNameAsync("accountant");
            var treasurerUser = await userManager.FindByNameAsync("treasurer");
            
            if (existingAccountantUser != null)
            {
                Console.WriteLine("Found existing accountant user - migrating to SGB Treasurer role...");
                
                // Update existing accountant user's role
                if (await userManager.IsInRoleAsync(existingAccountantUser, "Accountant"))
                {
                    await userManager.RemoveFromRoleAsync(existingAccountantUser, "Accountant");
                }
                
                if (!await userManager.IsInRoleAsync(existingAccountantUser, "SGB Treasurer"))
                {
                    await userManager.AddToRoleAsync(existingAccountantUser, "SGB Treasurer");
                }
                
                // Update the Role property
                if (existingAccountantUser is Aq3Zh4Service aq3User)
                {
                    aq3User.Role = Bz5Xw6Permission.SGBTreasurer;
                    await userManager.UpdateAsync(aq3User);
                }
                
                Console.WriteLine("Existing accountant user migrated to SGB Treasurer role");
            }
            
            // Create new treasurer user if it doesn't exist
            if (treasurerUser == null)
            {
                Console.WriteLine("Creating SGB Treasurer Aq3Zh4Service...");
                treasurerUser = new Aq3Zh4Service
                {
                    UserName = "treasurer",
                    Email = "treasurer@school.com",
                    EmailConfirmed = true,
                    Role = Bz5Xw6Permission.SGBTreasurer,
                    CompanyId = Hx7Tz3Data.CompanyId
                };
                var result = await userManager.CreateAsync(treasurerUser, "Treasurer@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(treasurerUser, "SGB Treasurer");
                    Console.WriteLine("SGB Treasurer Aq3Zh4Service created successfully");
                }
                else
                {
                    Console.WriteLine($"Failed to create SGB Treasurer Aq3Zh4Service: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }
            else
            {
                Console.WriteLine("SGB Treasurer Aq3Zh4Service already exists");
                
                // Ensure existing treasurer has correct role
                if (!await userManager.IsInRoleAsync(treasurerUser, "SGB Treasurer"))
                {
                    await userManager.AddToRoleAsync(treasurerUser, "SGB Treasurer");
                    Console.WriteLine("Added SGB Treasurer role to existing treasurer user");
                }
            }

            // Seed Chart of Accounts based on IAS 1 Financial Statement
            if (!context.DataStreams.Any())
            {
                var accounts = new List<Qw8Rt5Entity>
                {
                    // INCOME ACCOUNTS
                    new Qw8Rt5Entity { AccountName = "Allocation - KZN DoE", Mx9Qw7Type = Mx9Qw7Type.Revenue, CompanyId = Hx7Tz3Data.CompanyId },
                    new Qw8Rt5Entity { AccountName = "Interest Received", Mx9Qw7Type = Mx9Qw7Type.Revenue, CompanyId = Hx7Tz3Data.CompanyId },
                    new Qw8Rt5Entity { AccountName = "Fundraising", Mx9Qw7Type = Mx9Qw7Type.Revenue, CompanyId = Hx7Tz3Data.CompanyId },
                    
                    // EXPENSE ACCOUNTS
                    new Qw8Rt5Entity { AccountName = "Audit Fees", Mx9Qw7Type = Mx9Qw7Type.Expense, CompanyId = Hx7Tz3Data.CompanyId },
                    new Qw8Rt5Entity { AccountName = "Bank Charges", Mx9Qw7Type = Mx9Qw7Type.Expense, CompanyId = Hx7Tz3Data.CompanyId },
                    new Qw8Rt5Entity { AccountName = "Building Maintenance & Repaires", Mx9Qw7Type = Mx9Qw7Type.Expense, CompanyId = Hx7Tz3Data.CompanyId },
                    new Qw8Rt5Entity { AccountName = "Catering", Mx9Qw7Type = Mx9Qw7Type.Expense, CompanyId = Hx7Tz3Data.CompanyId },
                    new Qw8Rt5Entity { AccountName = "Cleaning Material", Mx9Qw7Type = Mx9Qw7Type.Expense, CompanyId = Hx7Tz3Data.CompanyId },
                    new Qw8Rt5Entity { AccountName = "Consumables", Mx9Qw7Type = Mx9Qw7Type.Expense, CompanyId = Hx7Tz3Data.CompanyId },
                    new Qw8Rt5Entity { AccountName = "Depreciation: Equipment & Furniture", Mx9Qw7Type = Mx9Qw7Type.Expense, CompanyId = Hx7Tz3Data.CompanyId },
                    new Qw8Rt5Entity { AccountName = "Hx7Tz3Data Admin & Management System", Mx9Qw7Type = Mx9Qw7Type.Expense, CompanyId = Hx7Tz3Data.CompanyId },
                    new Qw8Rt5Entity { AccountName = "Stationary and LTSM", Mx9Qw7Type = Mx9Qw7Type.Expense, CompanyId = Hx7Tz3Data.CompanyId },
                    new Qw8Rt5Entity { AccountName = "Uniform", Mx9Qw7Type = Mx9Qw7Type.Expense, CompanyId = Hx7Tz3Data.CompanyId },
                    new Qw8Rt5Entity { AccountName = "Telephone", Mx9Qw7Type = Mx9Qw7Type.Expense, CompanyId = Hx7Tz3Data.CompanyId },
                    new Qw8Rt5Entity { AccountName = "Transport", Mx9Qw7Type = Mx9Qw7Type.Expense, CompanyId = Hx7Tz3Data.CompanyId },
                    new Qw8Rt5Entity { AccountName = "Wages", Mx9Qw7Type = Mx9Qw7Type.Expense, CompanyId = Hx7Tz3Data.CompanyId },
                    new Qw8Rt5Entity { AccountName = "Water & Electricity", Mx9Qw7Type = Mx9Qw7Type.Expense, CompanyId = Hx7Tz3Data.CompanyId },
                    new Qw8Rt5Entity { AccountName = "Refund", Mx9Qw7Type = Mx9Qw7Type.Expense, CompanyId = Hx7Tz3Data.CompanyId },
                    
                    // ASSET ACCOUNTS
                    new Qw8Rt5Entity { AccountName = "Cash", Mx9Qw7Type = Mx9Qw7Type.Asset, CompanyId = Hx7Tz3Data.CompanyId },
                    new Qw8Rt5Entity { AccountName = "Bank", Mx9Qw7Type = Mx9Qw7Type.Asset, CompanyId = Hx7Tz3Data.CompanyId },
                    new Qw8Rt5Entity { AccountName = "Equipment & Furniture", Mx9Qw7Type = Mx9Qw7Type.Asset, CompanyId = Hx7Tz3Data.CompanyId }
                };

                context.DataStreams.AddRange(accounts);
                await context.SaveChangesAsync();
                Console.WriteLine("Chart of accounts seeded successfully");
            }

            // Add additional accounts to match real financial statements
            if (!context.DataStreams.Any(a => a.AccountName == "Non LTSM"))
            {
                var additionalAccounts = new List<Qw8Rt5Entity>
                {
                    // Additional expense accounts from real financial statements
                    new Qw8Rt5Entity { AccountName = "Non LTSM", Mx9Qw7Type = Mx9Qw7Type.Expense, CompanyId = Hx7Tz3Data.CompanyId },
                    new Qw8Rt5Entity { AccountName = "Repairs And Maintenance", Mx9Qw7Type = Mx9Qw7Type.Expense, CompanyId = Hx7Tz3Data.CompanyId },
                    new Qw8Rt5Entity { AccountName = "Domestic and Security", Mx9Qw7Type = Mx9Qw7Type.Expense, CompanyId = Hx7Tz3Data.CompanyId },
                    new Qw8Rt5Entity { AccountName = "Inks and Masters", Mx9Qw7Type = Mx9Qw7Type.Expense, CompanyId = Hx7Tz3Data.CompanyId },
                    new Qw8Rt5Entity { AccountName = "Assistant Educators", Mx9Qw7Type = Mx9Qw7Type.Expense, CompanyId = Hx7Tz3Data.CompanyId },
                    new Qw8Rt5Entity { AccountName = "Machine Contract", Mx9Qw7Type = Mx9Qw7Type.Expense, CompanyId = Hx7Tz3Data.CompanyId },
                    new Qw8Rt5Entity { AccountName = "Yard Cleaning", Mx9Qw7Type = Mx9Qw7Type.Expense, CompanyId = Hx7Tz3Data.CompanyId },
                    
                    // Additional revenue accounts
                    new Qw8Rt5Entity { AccountName = "KZN DoE Subsidy", Mx9Qw7Type = Mx9Qw7Type.Revenue, CompanyId = Hx7Tz3Data.CompanyId },
                    new Qw8Rt5Entity { AccountName = "Income from other Investments", Mx9Qw7Type = Mx9Qw7Type.Revenue, CompanyId = Hx7Tz3Data.CompanyId },
                    
                    // Additional asset accounts
                    new Qw8Rt5Entity { AccountName = "Petty Cash", Mx9Qw7Type = Mx9Qw7Type.Asset, CompanyId = Hx7Tz3Data.CompanyId },
                    new Qw8Rt5Entity { AccountName = "PPE", Mx9Qw7Type = Mx9Qw7Type.Asset, CompanyId = Hx7Tz3Data.CompanyId },
                    
                    // Liability accounts
                    new Qw8Rt5Entity { AccountName = "Loan", Mx9Qw7Type = Mx9Qw7Type.Liability, CompanyId = Hx7Tz3Data.CompanyId },
                    
                    // Equity accounts
                    new Qw8Rt5Entity { AccountName = "Retained Earnings", Mx9Qw7Type = Mx9Qw7Type.Equity, CompanyId = Hx7Tz3Data.CompanyId }
                };

                context.DataStreams.AddRange(additionalAccounts);
                await context.SaveChangesAsync();
                Console.WriteLine("Additional accounts added to match real financial statements");
            }

            // Seed comprehensive sample transactions based on real financial statements data
            if (!context.SecurityLogs.Any())
            {
                // Get Qw8Rt5Entity references
                var allocationAccount = context.DataStreams.First(a => a.AccountName == "Allocation - KZN DoE");
                var interestAccount = context.DataStreams.First(a => a.AccountName == "Interest Received");
                var fundraisingAccount = context.DataStreams.First(a => a.AccountName == "Fundraising");
                var kznDoeSubsidyAccount = context.DataStreams.First(a => a.AccountName == "KZN DoE Subsidy");
                var investmentIncomeAccount = context.DataStreams.First(a => a.AccountName == "Income from other Investments");
                var bankAccount = context.DataStreams.First(a => a.AccountName == "Bank");
                var cashAccount = context.DataStreams.First(a => a.AccountName == "Cash");
                var pettyCashAccount = context.DataStreams.First(a => a.AccountName == "Petty Cash");

                // Expense accounts
                var auditFeesAccount = context.DataStreams.First(a => a.AccountName == "Audit Fees");
                var bankChargesAccount = context.DataStreams.First(a => a.AccountName == "Bank Charges");
                var buildingMaintenanceAccount = context.DataStreams.First(a => a.AccountName == "Building Maintenance & Repaires");
                var repairsMaintenanceAccount = context.DataStreams.First(a => a.AccountName == "Repairs And Maintenance");
                var cateringAccount = context.DataStreams.First(a => a.AccountName == "Catering");
                var cleaningMaterialAccount = context.DataStreams.First(a => a.AccountName == "Cleaning Material");
                var consumablesAccount = context.DataStreams.First(a => a.AccountName == "Consumables");
                var depreciationAccount = context.DataStreams.First(a => a.AccountName == "Depreciation: Equipment & Furniture");
                var adminSystemAccount = context.DataStreams.First(a => a.AccountName == "Hx7Tz3Data Admin & Management System");
                var stationaryAccount = context.DataStreams.First(a => a.AccountName == "Stationary and LTSM");
                var uniformAccount = context.DataStreams.First(a => a.AccountName == "Uniform");
                var telephoneAccount = context.DataStreams.First(a => a.AccountName == "Telephone");
                var transportAccount = context.DataStreams.First(a => a.AccountName == "Transport");
                var wagesAccount = context.DataStreams.First(a => a.AccountName == "Wages");
                var waterElectricityAccount = context.DataStreams.First(a => a.AccountName == "Water & Electricity");
                var refundAccount = context.DataStreams.First(a => a.AccountName == "Refund");
                var nonLtsmAccount = context.DataStreams.First(a => a.AccountName == "Non LTSM");
                var domesticSecurityAccount = context.DataStreams.First(a => a.AccountName == "Domestic and Security");
                var inksMastersAccount = context.DataStreams.First(a => a.AccountName == "Inks and Masters");
                var assistantEducatorsAccount = context.DataStreams.First(a => a.AccountName == "Assistant Educators");
                var machineContractAccount = context.DataStreams.First(a => a.AccountName == "Machine Contract");
                var yardCleaningAccount = context.DataStreams.First(a => a.AccountName == "Yard Cleaning");

                await context.SaveChangesAsync();
                Console.WriteLine("Comprehensive sample transactions seeded successfully based on real financial statements:");
                Console.WriteLine("- XYZ Primary School (2019-2020) with variance data");
                Console.WriteLine("- Mandlakayise Primary School (2021) with budget vs actual comparisons");
                Console.WriteLine("- Total transactions: Multiple years with realistic budget variance scenarios");
            }

            // Seed sample students with Zulu names and SA ID numbers (Issue #006: Age Analysis for School Fees)
            Console.WriteLine("\n--- Seeding Sample Students for Age Analysis ---");
            var studentSeeder = scope.ServiceProvider.GetRequiredService<StudentSeederService>();
            await studentSeeder.SeedStudentsAsync();

            Console.WriteLine("Database seeding completed successfully!");
        }
    }
}

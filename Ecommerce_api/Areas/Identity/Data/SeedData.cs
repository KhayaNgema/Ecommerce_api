using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Ecommerce_api.Models;
using Ecommerce_api.Data;
using Microsoft.EntityFrameworkCore;

public static class SeedData
{
    public static async Task CreateRolesAndDefaultUser(this IServiceProvider services)
    {
        using (var scope = services.CreateScope())
        {
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<UserBaseModel>>();
            var _context = scope.ServiceProvider.GetRequiredService<Ecommerce_apiDBContext>();

            string[] roleNames = { "System Administrator",
                "Store Manager",
                "Store Owner",
                "Sales Associate",
                "Driver"};

            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            var storeManager = await userManager.FindByEmailAsync("admin@gmail.com");

            if (storeManager == null)
            {
                var defaultUser = new SystemAdministrator
                {
                    FirstName = "Khayalethu",
                    LastName = "Msweli",
                    UserName = "admin@gmail.com",
                    Email = "admin@gmail.com",
                    PhoneNumber = "0660278127",
                    DateOfBirth = DateTime.Now,
                    ProfilePicture = "khaya.jpg",
                    Address = "154, Nkungwini Area, Ingwavuma, Ingwavuma, Kwazulu-Natal, 3968",
                    EmailConfirmed = true,
                    CreatedDateTime = DateTime.Now,
                    ModifiedDateTime = DateTime.Now,
                    CreatedBy = "default-user-id",
                    ModifiedBy = "default-user-id",
                    IsActive = true,
                    IsDeleted = false,
                    IsFirstTimeLogin = true,
                    IsSuspended = false
                };

                var result = await userManager.CreateAsync(defaultUser, "Admin@123");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(defaultUser, "System Administrator");
                }
            }

/*            var userAccountReport = new UserAccountsReports
            {
                TotalUserAccountsCount = 0,
                ActiveUserAccountsCount = 0,
                ActiveUserAccountsRate = 0,
                InactiveUserAccountsCount = 0,
                InactiveUserAccountsRate = 0,
                SuspendedUserAccountsCount = 0,
                SuspendedUserAccountsRate = 0,
            };

            if (!await _context.UserAccountsReports.AnyAsync())
            {
                _context.UserAccountsReports.Add(userAccountReport);
                await _context.SaveChangesAsync();
            }

            var systemPerfomanceReport = new SystemPerformanceReport
            {
                TotalRequests = 0,
                SucceededRequests = 0,
                FailedRequests = 0,
                SuccessRate = 0,
                FailureRate = 0
            };

            if (!await _context.SystemPerformanceReports.AnyAsync())
            {
                _context.SystemPerformanceReports.Add(systemPerfomanceReport);
                await _context.SaveChangesAsync();
            }

            var transactionsReport = new TransactionsReports
            {
                TotalTransactionsCount = 0,
                SuccessfulPaymentsCount = 0,
                UnsuccessfulPaymentsCount = 0,
                SuccessfulPaymentsRate = 0,
                UnsuccessfulPaymentsRate = 0
            };

            if (!await _context.TransactionsReports.AnyAsync())
            {
                _context.TransactionsReports.Add(transactionsReport);
                await _context.SaveChangesAsync();
            }

            var users = await _context.UserBaseModel.ToListAsync();

            foreach (var user in users)
            {
                var userHasSubscription = await _context.Subscriptions.AnyAsync(s => s.UserId == user.Id);

                if (!userHasSubscription)
                {
                    var userSubscription = new Subscription
                    {
                        UserId = user.Id,
                        Amount = 0,
                        SubscriptionPlan = SubscriptionPlan.Basic,
                        SubscriptionStatus = SubscriptionStatus.Active,
                    };

                    _context.Subscriptions.Add(userSubscription);

                    await _context.SaveChangesAsync();
                }
            }*/

        }
    }
}

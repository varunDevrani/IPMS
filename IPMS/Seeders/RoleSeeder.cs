using IPMS.Data;
using IPMS.Entities;
using Microsoft.EntityFrameworkCore;


namespace IPMS.Seeders;

public static class RoleSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        if (await context.Roles.AnyAsync())
            return;

        var roles = new List<Role>
        {
            new() { Name = "Customer" },
            new() { Name = "Admin" },
            new() { Name = "InsuranceAgent" },
            new() { Name = "Underwriter" }
        };

        await context.Roles.AddRangeAsync(roles);
        await context.SaveChangesAsync();
    }
}
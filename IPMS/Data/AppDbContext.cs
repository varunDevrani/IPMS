using IPMS.Entities;
using Microsoft.EntityFrameworkCore;

namespace IPMS.Data;


public class AppDbContext: DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options): base(options)
    {
        
    }

    public DbSet<User> Users {get; set;}
    public DbSet<Role> Roles {get; set;}
    public DbSet<UserRole> UserRoles {get; set;}
    public DbSet<RefreshToken> RefreshTokens {get; set;}
    public DbSet<TokenFamily> TokenFamilies {get; set;}
    public DbSet<Customer> Customers {get; set;}
    public DbSet<CustomerAddress> CustomerAddresses {get; set;}
    public DbSet<Product> Products {get; set;}
    public DbSet<Policy> Polices {get; set;}
    public DbSet<PremiumPayment> PremiumPayments {get; set;}
    public DbSet<Claim> Claims {get; set;}
    public DbSet<ClaimStatus> ClaimStatuses {get; set;}
    public DbSet<ClaimDocument> ClaimDocuments {get; set;}
    public DbSet<ErrorLog> ErrorLogs {get; set;}
    public DbSet<AuditLog> AuditLogs {get; set;}
}
namespace IPMS.Entities;


public enum UserRoleType
{
    Customer,
    Admin,
    InsuranceAgent,
    Underwriter
}

public class Role: BaseEntity
{
    public required string Name { get; set; }
    public ICollection<UserRole> UserRoles { get; set; } = [];
    public DateTimeOffset? DeletedAt {get; set;}
}

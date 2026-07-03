namespace IPMS.Entities;

public class UserRole: BaseEntity
{
    public required Guid UserId {get; set;}
    public required Guid RoleId {get; set;}
}
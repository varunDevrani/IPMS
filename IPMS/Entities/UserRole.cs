namespace IPMS.Entities;

public class UserRole: BaseEntity
{
    public required Guid UserId {get; set;}
    public required Guid RoleId {get; set;}
    public required DateTimeOffset AssignedAt {get; set;}
    public required Guid AssignedBy {get; set;}

}
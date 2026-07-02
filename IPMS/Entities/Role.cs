namespace IPMS.Entities;


public class Role: BaseEntity
{
    public required string Name {get; set;}
    public string? Description {get; set;}
    public DateTimeOffset? DeletedAt {get; set;}
}
namespace IPMS.Entities;


public enum CustomerGender
{
    Male = 0,
    Female = 1
}


public class Customer: BaseEntity
{
    public required Guid UserId {get; set;}
    public required DateOnly DateOfBirth {get; set;}
    public required CustomerGender Gender {get; set;}
    public required bool MaritalStatus {get; set;}
    public required string SSNHash {get; set;}
    public DateTimeOffset? DeletedAt {get; set;}
}
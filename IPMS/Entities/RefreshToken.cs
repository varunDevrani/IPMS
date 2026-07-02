
namespace IPMS.Entities;


public class RefreshToken: BaseEntity
{
    public required string TokenHash {get; set;}
    public required Guid FamilyId {get; set;}
    public required DateTimeOffset ExpiresAt {get; set;}
    public DateTimeOffset? UsedAt {get; set;}
}
namespace IPMS.Entities;


public class Claim: BaseEntity
{
    public required string ClaimNumber {get; set;}
    public required Guid PolicyId {get; set;}
    public required Guid UnderWriterId {get; set;}
    public required DateOnly IncidentDate {get; set;}
    public DateOnly? ClaimDate {get; set;}
    public required decimal ClaimAmount {get; set;}
    public string? Reason {get; set;}
    public string? Remark {get; set;}
    public required Guid StatusId {get; set;}
}
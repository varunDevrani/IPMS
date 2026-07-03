namespace IPMS.Entities;


public enum ClaimStatus
{
    Submitted = 0,
    UnderReview = 1,
    DocumentsRequested = 2,
    Approved = 3,
    Rejected = 4,
    Closed = 5
}

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
    public required ClaimStatus Status {get; set;}
}
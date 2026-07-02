namespace IPMS.Entities;


public enum PolicyStatus
{
    Quoted = 0,
    Expired = 1,
    Active = 2,
    Cancelled = 3
}


public class Policy: BaseEntity
{
    public required string PolicyNumber {get; set;}
    public required Guid ProductId {get; set;}
    public required Guid CustomerId {get; set;}
    public required Guid InsuranceAgentId {get; set;}
    public required Guid UnderWriterId {get; set;}
    public required decimal CoverageAmount {get; set;}
    public required decimal PremiumAmount {get; set;}
    public required DateOnly StartDate {get; set;}
    public required DateOnly EndDate {get; set;}
    public required DateOnly QuoteDate {get; set;}
    public required DateOnly IssueDate {get; set;}
    public required PolicyStatus Status {get; set;}
    public DateTimeOffset? DeletedAt {get; set;}
    public DateTimeOffset? CancellationDate {get; set;}
    public string? CancellationReason {get; set;}
}
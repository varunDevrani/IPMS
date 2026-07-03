using IPMS.Entities;

public enum QuoteStatus
{
    Requested = 0,
    AcceptedByCustomer = 1,
    Approved = 2,
    Rejected = 3,
    Expired = 4
}

public class Quote : BaseEntity
{
    public required Guid ProductId { get; set; }
    public required Guid CustomerId { get; set; }
    public Guid? InsuranceAgentId { get; set; }
    public Guid? UnderwriterId { get; set; }
    public required decimal CoverageAmount { get; set; }
    public required decimal PremiumAmount { get; set; }
    public required DateOnly QuoteDate { get; set; }
    public required DateOnly ValidUntil { get; set; }
    public required QuoteStatus Status { get; set; }
    public string? Remarks { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
}
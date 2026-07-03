using IPMS.Entities;

namespace IPMS.Dtos;



public class PolicyDto
{
    public required Guid Id { get; set; }
    public required string PolicyNumber { get; set; }
    public required Guid QuoteId { get; set; }
    public required Guid ProductId { get; set; }
    public required string ProductName { get; set; }
    public required Guid CustomerId { get; set; }
    public required decimal CoverageAmount { get; set; }
    public required decimal PremiumAmount { get; set; }
    public required DateOnly QuoteDate { get; set; }
    public required DateOnly IssueDate { get; set; }
    public required DateOnly StartDate { get; set; }
    public required DateOnly EndDate { get; set; }
    public required PolicyStatus Status { get; set; }
    public required DateTimeOffset CreatedAt { get; set; }
    public required DateTimeOffset UpdatedAt { get; set; }
}

public class PoliciesDto
{
    public required ulong Total { get; set; }
    public required List<PolicyDto> Policies { get; set; }
}


public class CancelPolicyDto
{
    public required string CancellationReason { get; set; }
}
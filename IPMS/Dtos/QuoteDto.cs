namespace IPMS.Dtos;

public class CreateQuoteDto
{
    public required Guid ProductId { get; set; }
    public decimal? CoverageAmount { get; set; }
}


public class QuoteDto
{
    public Guid Id { get; set; }
    public string ProductName { get; set; } = "";
    public decimal CoverageAmount { get; set; }
    public decimal PremiumAmount { get; set; }
    public DateOnly QuoteDate { get; set; }
    public DateOnly ValidUntil { get; set; }
    public QuoteStatus Status { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}


public class QuotesDto
{
    public ulong Total { get; set; }
    public List<QuoteDto> Quotes { get; set; } = [];
}
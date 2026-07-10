using IPMS.Entities;

namespace IPMS.Dtos;


public class PremiumPaymentDto
{
    public required Guid PolicyId { get; set; }
    public required string InstallmentNumber { get; set; }
    public required decimal PremiumAmount { get; set; }
    public required decimal PenaltyAmount { get; set; }
    public decimal? TotalPaid { get; set; }
    public required DateOnly DueDate { get; set; }
    public DateOnly? PaidDate { get; set; }
    public required PremiumPaymentStatus PaymentStatus { get; set; }
    public required PremiumPaymentMethod PaymentMethod { get; set; }
    public required DateTimeOffset CreatedAt { get; set; }
    public required DateTimeOffset UpdatedAt { get; set; }
}


public class PremiumPaymentsDto
{
    public required ulong Total { get; set; }
    public required List<PremiumPaymentDto> Payments { get; set; }
}


public class CreatePremiumPaymentDto
{
    public required Guid PolicyId { get; set; }
    public required string InstallmentNumber { get; set; }
    public required decimal PremiumAmount { get; set; }
    public required decimal PenaltyAmount { get; set; }
    public required DateOnly DueDate { get; set; }
}


public class PayPremiumDto
{
    public required decimal AmountPaid { get; set; }
    public required PremiumPaymentMethod PaymentMethod { get; set; }
}


public class UpdatePremiumPaymentDto
{
    public PremiumPaymentStatus? PaymentStatus { get; set; }
    public PremiumPaymentMethod? PaymentMethod { get; set; }
    public decimal? PenaltyAmount { get; set; }
}
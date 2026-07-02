namespace IPMS.Entities;


public enum PremiumPaymentStatus
{
    Pending = 0,
    Failed = 1,
    Success = 2,
    Late = 3
}


public enum PremiumPaymentMethod
{
    DebitCard = 0,
    CreditCard = 1,
    Bank = 2
}


public class PremiumPayment: BaseEntity
{
    public required Guid PolicyId {get; set;}
    public required string InstallmentNumber {get; set;}
    public required decimal PremiumAmount {get; set;}
    public required decimal PenaltyAmount {get; set;}
    public decimal? TotalPaid {get; set;}
    public required DateOnly DueDate {get; set;}
    public DateOnly? PaidDate {get; set;}
    public required PremiumPaymentStatus PaymentStatus {get; set;}
    public required PremiumPaymentMethod PaymentMethod {get; set;}
}
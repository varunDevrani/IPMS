using IPMS.Data;
using IPMS.Dtos;
using IPMS.Entities;
using Microsoft.AspNetCore.Mvc;

namespace IPMS.Controllers;


[ApiController]
[Route("api/[controller]")]
public class PremiumPaymentController : ControllerBase
{

    private readonly AppDbContext _context;


    public PremiumPaymentController(AppDbContext context)
    {
        _context = context;
    }



    [HttpGet("policy/{policy_id}")]
    public ActionResult<PremiumPaymentsDto> GetPolicyPayments(Guid policy_id)
    {

        List<PremiumPaymentDto> payments = _context.PremiumPayments
            .Where(p => p.PolicyId == policy_id)
            .Select(p => new PremiumPaymentDto
            {
                PolicyId = p.PolicyId,
                InstallmentNumber = p.InstallmentNumber,
                PremiumAmount = p.PremiumAmount,
                PenaltyAmount = p.PenaltyAmount,
                TotalPaid = p.TotalPaid,
                DueDate = p.DueDate,
                PaidDate = p.PaidDate,
                PaymentStatus = p.PaymentStatus,
                PaymentMethod = p.PaymentMethod,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            })
            .ToList();


        return Ok(new PremiumPaymentsDto
        {
            Total = (ulong)payments.Count,
            Payments = payments
        });

    }


    [HttpPost]
    public ActionResult<PremiumPaymentDto> CreatePremiumPayment(CreatePremiumPaymentDto payload)
    {

        PremiumPayment payment = new()
        {
            PolicyId = payload.PolicyId,
            InstallmentNumber = payload.InstallmentNumber,
            PremiumAmount = payload.PremiumAmount,
            PenaltyAmount = payload.PenaltyAmount,
            DueDate = payload.DueDate,
            PaymentStatus = PremiumPaymentStatus.Pending,
            PaymentMethod = PremiumPaymentMethod.Bank // hardcoded bank here
        };


        _context.PremiumPayments.Add(payment);
        _context.SaveChanges();


        return Ok(new PremiumPaymentDto
        {
            PolicyId = payment.PolicyId,
            InstallmentNumber = payment.InstallmentNumber,
            PremiumAmount = payment.PremiumAmount,
            PenaltyAmount = payment.PenaltyAmount,
            TotalPaid = payment.TotalPaid,
            DueDate = payment.DueDate,
            PaidDate = payment.PaidDate,
            PaymentStatus = payment.PaymentStatus,
            PaymentMethod = payment.PaymentMethod,
            CreatedAt = payment.CreatedAt,
            UpdatedAt = payment.UpdatedAt
        });

    }





    [HttpPost("{payment_id}/pay")]
    public ActionResult<PremiumPaymentDto> PayPremium(
        Guid payment_id,
        PayPremiumDto payload)
    {

        PremiumPayment? payment = _context.PremiumPayments
            .FirstOrDefault(p => p.Id == payment_id);


        if(payment is null)
        {
            return NotFound("Premium payment not found");
        }


        if(payment.PaymentStatus == PremiumPaymentStatus.Success)
        {
            return Conflict("Payment already completed");
        }



        payment.TotalPaid = payload.AmountPaid;
        payment.PaymentMethod = payload.PaymentMethod;
        payment.PaidDate = DateOnly.FromDateTime(DateTime.UtcNow);


        if(payment.PaidDate > payment.DueDate)
        {
            payment.PaymentStatus = PremiumPaymentStatus.Late;
        }
        else
        {
            payment.PaymentStatus = PremiumPaymentStatus.Success;
        }


        payment.UpdatedAt = DateTimeOffset.UtcNow;


        _context.SaveChanges();



        return Ok(new PremiumPaymentDto
        {
            PolicyId = payment.PolicyId,
            InstallmentNumber = payment.InstallmentNumber,
            PremiumAmount = payment.PremiumAmount,
            PenaltyAmount = payment.PenaltyAmount,
            TotalPaid = payment.TotalPaid,
            DueDate = payment.DueDate,
            PaidDate = payment.PaidDate,
            PaymentStatus = payment.PaymentStatus,
            PaymentMethod = payment.PaymentMethod,
            CreatedAt = payment.CreatedAt,
            UpdatedAt = payment.UpdatedAt
        });

    }





    [HttpPatch("{payment_id}")]
    public ActionResult<PremiumPaymentDto> UpdatePremiumPayment(
        Guid payment_id,
        UpdatePremiumPaymentDto payload)
    {

        PremiumPayment? payment = _context.PremiumPayments
            .FirstOrDefault(p => p.Id == payment_id);


        if(payment is null)
        {
            return NotFound("Premium payment not found");
        }


        if(payload.PaymentStatus.HasValue)
        {
            payment.PaymentStatus = payload.PaymentStatus.Value;
        }


        if(payload.PaymentMethod.HasValue)
        {
            payment.PaymentMethod = payload.PaymentMethod.Value;
        }


        if(payload.PenaltyAmount.HasValue)
        {
            payment.PenaltyAmount = payload.PenaltyAmount.Value;
        }


        payment.UpdatedAt = DateTimeOffset.UtcNow;

        _context.SaveChanges();


        return Ok(new PremiumPaymentDto
        {
            PolicyId = payment.PolicyId,
            InstallmentNumber = payment.InstallmentNumber,
            PremiumAmount = payment.PremiumAmount,
            PenaltyAmount = payment.PenaltyAmount,
            TotalPaid = payment.TotalPaid,
            DueDate = payment.DueDate,
            PaidDate = payment.PaidDate,
            PaymentStatus = payment.PaymentStatus,
            PaymentMethod = payment.PaymentMethod,
            CreatedAt = payment.CreatedAt,
            UpdatedAt = payment.UpdatedAt
        });

    }



}
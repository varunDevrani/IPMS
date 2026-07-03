using System.Security.Claims;
using IPMS.Data;
using IPMS.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace IPMS.Controllers;



public class PolicyController: ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;

    public PolicyController(AppDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    [HttpGet]
    public ActionResult<PoliciesDto> GetPolicies()
    {
        Guid userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var customer = _context.Customers.FirstOrDefault(c =>
            c.UserId == userId &&
            c.DeletedAt == null);

        if (customer is null)
            return NotFound("Customer profile not found.");

        var policies = (
            from policy in _context.Policies
            join product in _context.Products
                on policy.ProductId equals product.Id
            where policy.CustomerId == customer.Id
                && policy.DeletedAt == null
            orderby policy.CreatedAt descending
            select new PolicyDto
            {
                Id = policy.Id,
                PolicyNumber = policy.PolicyNumber,
                QuoteId = policy.QuoteId,
                ProductId = policy.ProductId,
                ProductName = product.Name,
                CustomerId = policy.CustomerId,
                CoverageAmount = policy.CoverageAmount,
                PremiumAmount = policy.PremiumAmount,
                QuoteDate = policy.QuoteDate,
                IssueDate = policy.IssueDate,
                StartDate = policy.StartDate,
                EndDate = policy.EndDate,
                Status = policy.Status,
                CreatedAt = policy.CreatedAt,
                UpdatedAt = policy.UpdatedAt
            }
        ).ToList();

        return Ok(new PoliciesDto
        {
            Total = (ulong)policies.Count,
            Policies = policies
        });
    }


    [HttpGet("{policy_id}")]
    public ActionResult<PolicyDto> GetPolicyById(Guid policy_id)
    {
        Guid userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var customer = _context.Customers.FirstOrDefault(c =>
            c.UserId == userId &&
            c.DeletedAt == null);

        if (customer is null)
            return NotFound("Customer profile not found.");

        var policy = (
            from p in _context.Policies
            join product in _context.Products
                on p.ProductId equals product.Id
            where p.Id == policy_id
                && p.CustomerId == customer.Id
                && p.DeletedAt == null
            select new PolicyDto
            {
                Id = p.Id,
                PolicyNumber = p.PolicyNumber,
                QuoteId = p.QuoteId,
                ProductId = p.ProductId,
                ProductName = product.Name,
                CustomerId = p.CustomerId,
                CoverageAmount = p.CoverageAmount,
                PremiumAmount = p.PremiumAmount,
                QuoteDate = p.QuoteDate,
                IssueDate = p.IssueDate,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                Status = p.Status,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            }
        ).FirstOrDefault();

        if (policy is null)
            return NotFound("Policy not found.");

        return Ok(policy);
    }


    [HttpPatch("{policy_id}/cancel")]
    public ActionResult<PolicyDto> CancelPolicy(
        Guid policy_id,
        CancelPolicyDto payload)
    {
        Guid userId = Guid.Parse(
            User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var customer = _context.Customers.FirstOrDefault(c =>
            c.UserId == userId &&
            c.DeletedAt == null);

        if (customer is null)
            return NotFound("Customer profile not found.");

        var policy = _context.Policies.FirstOrDefault(p =>
            p.Id == policy_id &&
            p.CustomerId == customer.Id &&
            p.DeletedAt == null);

        if (policy is null)
            return NotFound("Policy not found.");

        if (policy.Status != Entities.PolicyStatus.Active)
            return Conflict("Only active policies can be cancelled.");

        policy.Status = Entities.PolicyStatus.Cancelled;
        policy.CancellationDate = DateTimeOffset.UtcNow;
        policy.CancellationReason = payload.CancellationReason;

        _context.SaveChanges();

        var product = _context.Products.First(p => p.Id == policy.ProductId);

        return Ok(new PolicyDto
        {
            Id = policy.Id,
            PolicyNumber = policy.PolicyNumber,
            QuoteId = policy.QuoteId,
            ProductId = policy.ProductId,
            ProductName = product.Name,
            CustomerId = policy.CustomerId,
            CoverageAmount = policy.CoverageAmount,
            PremiumAmount = policy.PremiumAmount,
            QuoteDate = policy.QuoteDate,
            IssueDate = policy.IssueDate,
            StartDate = policy.StartDate,
            EndDate = policy.EndDate,
            Status = policy.Status,
            CreatedAt = policy.CreatedAt,
            UpdatedAt = policy.UpdatedAt
        });
    }

}
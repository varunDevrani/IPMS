using System.Security.Claims;
using IPMS.Data;
using IPMS.Dtos;
using IPMS.Entities;
using Microsoft.AspNetCore.Mvc;

namespace IPMS.Controllers;



[ApiController]
[Route("api/[controller]")]
public class QuoteController: ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;


    public QuoteController(AppDbContext context, IConfiguration config)
    {
        _config = config;
        _context = context;
    }


    [HttpPost]
    public ActionResult<QuoteDto> CreateQuote(CreateQuoteDto payload)
    {
        Guid userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var customer = _context.Customers.FirstOrDefault(c =>
            c.UserId == userId &&
            c.DeletedAt == null);

        if (customer is null)
            return BadRequest("Customer profile does not exist.");

        var product = _context.Products.FirstOrDefault(p =>
            p.Id == payload.ProductId &&
            p.DeletedAt == null);

        if (product is null)
            return NotFound("Product not found.");

        decimal coverageAmount = payload.CoverageAmount ?? product.CoverageAmount;

        decimal premiumAmount = product.BasePremium;

        var quote = new Quote
        {
            ProductId = product.Id,
            CustomerId = customer.Id,

            CoverageAmount = coverageAmount,
            PremiumAmount = premiumAmount,

            QuoteDate = DateOnly.FromDateTime(DateTime.UtcNow),
            ValidUntil = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30)),

            Status = QuoteStatus.Requested
        };

        _context.Quotes.Add(quote);

        _context.SaveChanges();

        return Ok(new QuoteDto
        {
            Id = quote.Id,

            ProductName = product.Name,

            CoverageAmount = quote.CoverageAmount,

            PremiumAmount = quote.PremiumAmount,

            QuoteDate = quote.QuoteDate,

            ValidUntil = quote.ValidUntil,

            Status = quote.Status,

            CreatedAt = quote.CreatedAt
        });
    }



    [HttpGet]
    public ActionResult<QuotesDto> GetQuotes()
    {
        Guid userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var customer = _context.Customers.FirstOrDefault(c =>
            c.UserId == userId &&
            c.DeletedAt == null);

        if (customer is null)
            return NotFound("Customer profile not found.");

        var quotes = (
            from quote in _context.Quotes
            join product in _context.Products
                on quote.ProductId equals product.Id
            where quote.CustomerId == customer.Id
                && quote.DeletedAt == null
            select new QuoteDto
            {
                Id = quote.Id,
                ProductName = product.Name,
                CoverageAmount = quote.CoverageAmount,
                PremiumAmount = quote.PremiumAmount,
                QuoteDate = quote.QuoteDate,
                ValidUntil = quote.ValidUntil,
                Status = quote.Status,
                CreatedAt = quote.CreatedAt
            }
        ).ToList();

        return Ok(new QuotesDto
        {
            Total = (ulong)quotes.Count,
            Quotes = quotes
        });
    }


    [HttpGet("{quote_id}")]
    public ActionResult<QuoteDto> GetQuoteById(Guid quote_id)
    {
        Guid userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var customer = _context.Customers.FirstOrDefault(c =>
            c.UserId == userId &&
            c.DeletedAt == null);

        if (customer is null)
            return NotFound("Customer profile not found.");

        var quote = (
            from q in _context.Quotes
            join p in _context.Products
                on q.ProductId equals p.Id
            where q.Id == quote_id
                && q.CustomerId == customer.Id
                && q.DeletedAt == null
            select new QuoteDto
            {
                Id = q.Id,
                ProductName = p.Name,
                CoverageAmount = q.CoverageAmount,
                PremiumAmount = q.PremiumAmount,
                QuoteDate = q.QuoteDate,
                ValidUntil = q.ValidUntil,
                Status = q.Status,
                CreatedAt = q.CreatedAt
            }
        ).FirstOrDefault();

        if (quote is null)
            return NotFound("Quote not found.");

        return Ok(quote);
    }

    [HttpPatch("{quote_id}/accept")]
    public ActionResult<QuoteDto> AcceptQuote(Guid quote_id)
    {
        Guid userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var customer = _context.Customers.FirstOrDefault(c =>
            c.UserId == userId &&
            c.DeletedAt == null);

        if (customer is null)
            return NotFound("Customer profile not found.");

        var quote = _context.Quotes.FirstOrDefault(q =>
            q.Id == quote_id &&
            q.CustomerId == customer.Id &&
            q.DeletedAt == null);

        if (quote is null)
            return NotFound("Quote not found.");

        if (quote.Status != QuoteStatus.Requested)
            return Conflict("Only requested quotes can be accepted.");

        if (quote.ValidUntil < DateOnly.FromDateTime(DateTime.UtcNow))
        {
            quote.Status = QuoteStatus.Expired;
            _context.SaveChanges();

            return Conflict("Quote has expired.");
        }

        quote.Status = QuoteStatus.AcceptedByCustomer;

        _context.SaveChanges();

        var product = _context.Products.First(p => p.Id == quote.ProductId);

        return Ok(new QuoteDto
        {
            Id = quote.Id,
            ProductName = product.Name,
            CoverageAmount = quote.CoverageAmount,
            PremiumAmount = quote.PremiumAmount,
            QuoteDate = quote.QuoteDate,
            ValidUntil = quote.ValidUntil,
            Status = quote.Status,
            CreatedAt = quote.CreatedAt
        });
    }



    [HttpPatch("{quote_id}/approve")]
    public ActionResult ApproveQuote(Guid quote_id)
    {
        // Guid underwriterId = Guid.Parse(
        //     User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        Guid underwriterId = Guid.NewGuid();

        var quote = _context.Quotes.FirstOrDefault(q =>
            q.Id == quote_id &&
            q.DeletedAt == null);

        if (quote is null)
            return NotFound("Quote not found.");

        if (quote.Status != QuoteStatus.AcceptedByCustomer)
            return Conflict("Quote has not been accepted by the customer.");

        if (quote.ValidUntil < DateOnly.FromDateTime(DateTime.UtcNow))
        {
            quote.Status = QuoteStatus.Expired;
            _context.SaveChanges();

            return Conflict("Quote has expired.");
        }

        var policy = new Policy
        {
            PolicyNumber = $"POL-{DateTime.UtcNow:yyyyMMddHHmmss}",

            ProductId = quote.ProductId,
            CustomerId = quote.CustomerId,
            QuoteId = quote.Id,

            InsuranceAgentId = Guid.Empty,      
            UnderWriterId = underwriterId,

            CoverageAmount = quote.CoverageAmount,
            PremiumAmount = quote.PremiumAmount,

            QuoteDate = quote.QuoteDate,

            IssueDate = DateOnly.FromDateTime(DateTime.UtcNow),

            StartDate = DateOnly.FromDateTime(DateTime.UtcNow),

            EndDate = DateOnly.FromDateTime(
                DateTime.UtcNow.AddYears(1)),

            Status = PolicyStatus.Active
        };

        quote.Status = QuoteStatus.Approved;
        quote.UnderwriterId = underwriterId;

        _context.Policies.Add(policy);

        _context.SaveChanges();

        return Ok(policy);
    }


    [HttpPatch("{quote_id}/reject")]
    public ActionResult<QuoteDto> RejectQuote(Guid quote_id)
    {
        Guid underwriterId = Guid.Parse(
            User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var quote = _context.Quotes.FirstOrDefault(q =>
            q.Id == quote_id &&
            q.DeletedAt == null);

        if (quote is null)
            return NotFound("Quote not found.");

        if (quote.Status != QuoteStatus.AcceptedByCustomer)
            return Conflict("Quote has not been accepted by the customer.");

        if (quote.ValidUntil < DateOnly.FromDateTime(DateTime.UtcNow))
        {
            quote.Status = QuoteStatus.Expired;
            _context.SaveChanges();

            return Conflict("Quote has expired.");
        }

        quote.Status = QuoteStatus.Rejected;
        quote.UnderwriterId = underwriterId;

        _context.SaveChanges();

        var product = _context.Products.First(p => p.Id == quote.ProductId);

        return Ok(new QuoteDto
        {
            Id = quote.Id,
            ProductName = product.Name,
            CoverageAmount = quote.CoverageAmount,
            PremiumAmount = quote.PremiumAmount,
            QuoteDate = quote.QuoteDate,
            ValidUntil = quote.ValidUntil,
            Status = quote.Status,
            CreatedAt = quote.CreatedAt
        });
    }


}
using IPMS.Data;
using IPMS.Dtos;
using IPMS.Entities;
using Microsoft.AspNetCore.Mvc;

namespace IPMS.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;

    public ProductController(AppDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    [HttpGet]
    public ActionResult<ProductsDto> GetProducts()
    {
        List<ProductDto> products = _context.Products
            .Where(p => p.DeletedAt == null)
            .Select(p => new ProductDto
            {
                Name = p.Name,
                Type = p.Type,
                CoverageAmount = p.CoverageAmount,
                BasePremium = p.BasePremium,
                MinAge = p.MinAge,
                MaxAge = p.MaxAge,
                PolicyTermYears = p.PolicyTermYears,
                Description = p.Description,
                IsActive = p.DeletedAt == null,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            })
            .ToList();

        return Ok(new ProductsDto
        {
            Total = (ulong)products.Count,
            Products = products
        });
    }


    [HttpGet("{product_id}")]
    public ActionResult<ProductDto> GetProductById(Guid product_id)
    {
        Product? product = _context.Products
            .FirstOrDefault(p => p.Id == product_id && p.DeletedAt == null);

        if (product is null)
        {
            return NotFound("Product not found");
        }

        return Ok(new ProductDto
        {
            Name = product.Name,
            Type = product.Type,
            CoverageAmount = product.CoverageAmount,
            BasePremium = product.BasePremium,
            MinAge = product.MinAge,
            MaxAge = product.MaxAge,
            PolicyTermYears = product.PolicyTermYears,
            Description = product.Description,
            IsActive = product.DeletedAt == null,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        });
    }


    [HttpPost]
    public ActionResult<ProductDto> CreateProduct(CreateProductDto payload)
    {
        bool exists = _context.Products.Any(p =>
            p.Name == payload.Name &&
            p.DeletedAt == null);

        if (exists)
        {
            return Conflict("Product with this name already exists");
        }

        Product product = new()
        {
            Name = payload.Name,
            Type = payload.Type,
            CoverageAmount = payload.CoverageAmount,
            BasePremium = payload.BasePremium,
            MinAge = payload.MinAge,
            MaxAge = payload.MaxAge,
            PolicyTermYears = payload.PolicyTermYears,
            Description = payload.Description
        };

        _context.Products.Add(product);
        _context.SaveChanges();

        return Ok(new ProductDto
        {
            Name = product.Name,
            Type = product.Type,
            CoverageAmount = product.CoverageAmount,
            BasePremium = product.BasePremium,
            MinAge = product.MinAge,
            MaxAge = product.MaxAge,
            PolicyTermYears = product.PolicyTermYears,
            Description = product.Description,
            IsActive = product.DeletedAt == null,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        });
    }


    [HttpPatch("{product_id}")]
    public ActionResult<ProductDto> UpdateProductById(Guid product_id, UpdateProductDto payload)
    {
        Product? product = _context.Products
            .FirstOrDefault(p => p.Id == product_id && p.DeletedAt == null);

        if (product is null)
        {
            return NotFound("Product not found");
        }

        if (payload.Name is not null)
            product.Name = payload.Name;

        if (payload.Type.HasValue)
            product.Type = payload.Type.Value;

        if (payload.CoverageAmount.HasValue)
            product.CoverageAmount = payload.CoverageAmount.Value;

        if (payload.BasePremium.HasValue)
            product.BasePremium = payload.BasePremium.Value;

        if (payload.MinAge.HasValue)
            product.MinAge = payload.MinAge.Value;

        if (payload.MaxAge.HasValue)
            product.MaxAge = payload.MaxAge.Value;

        if (payload.PolicyTermYears.HasValue)
            product.PolicyTermYears = payload.PolicyTermYears.Value;

        if (payload.Description is not null)
            product.Description = payload.Description;

        product.UpdatedAt = DateTimeOffset.UtcNow;

        _context.SaveChanges();

        return Ok(new ProductDto
        {
            Name = product.Name,
            Type = product.Type,
            CoverageAmount = product.CoverageAmount,
            BasePremium = product.BasePremium,
            MinAge = product.MinAge,
            MaxAge = product.MaxAge,
            PolicyTermYears = product.PolicyTermYears,
            Description = product.Description,
            IsActive = product.DeletedAt == null,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        });
    }


    [HttpDelete("{product_id}")]
    public ActionResult<string> DeleteProductById(Guid product_id)
    {
        Product? product = _context.Products
            .FirstOrDefault(p => p.Id == product_id && p.DeletedAt == null);

        if (product is null)
        {
            return NotFound("Product not found");
        }

        product.DeletedAt = DateTimeOffset.UtcNow;
        product.UpdatedAt = DateTimeOffset.UtcNow;

        _context.SaveChanges();

        return Ok("Product deleted successfully");
    }
}
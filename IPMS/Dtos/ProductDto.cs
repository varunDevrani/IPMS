using IPMS.Entities;

namespace IPMS.Dtos;


public class ProductDto
{
    public required string Name {get; set;}
    public required ProductType Type {get; set;}
    public required decimal CoverageAmount {get; set;}
    public required decimal BasePremium {get; set;}
    public required byte MinAge {get; set;}
    public required byte MaxAge {get; set;}
    public required decimal PolicyTermYears {get; set;}
    public string? Description {get; set;}
    public required bool IsActive {get; set;}
    public required DateTimeOffset CreatedAt {get; set;}
    public required DateTimeOffset UpdatedAt {get; set;}
}


public class CreateProductDto
{
    public required string Name {get; set;}
    public required ProductType Type {get; set;}
    public required decimal CoverageAmount {get; set;}
    public required decimal BasePremium {get; set;}
    public required byte MinAge {get; set;}
    public required byte MaxAge {get; set;}
    public required decimal PolicyTermYears {get; set;}
    public string? Description {get; set;}
    public required bool IsActive {get; set;}
}


public class ProductsDto
{
    public required ulong Total {get; set;}
    public required List<ProductDto> Products {get; set;}
}

public class UpdateProductDto
{
    public string? Name {get; set;}
    public ProductType? Type {get; set;}
    public decimal? CoverageAmount {get; set;}
    public decimal? BasePremium {get; set;}
    public byte? MinAge {get; set;}
    public byte? MaxAge {get; set;}
    public decimal? PolicyTermYears {get; set;}
    public string? Description {get; set;}
    public bool? IsActive {get; set;}
}
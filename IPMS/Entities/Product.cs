namespace IPMS.Entities;


public enum ProductType
{
    Health = 0,
    Life = 1,
    Term = 2,
    Group = 3
}


public class Product: BaseEntity
{
    public required string Name {get; set;}
    public required ProductType Type {get; set;}
    public required decimal CoverageAmount {get; set;}
    public required decimal BasePremium {get; set;}
    public required byte MinAge {get; set;}
    public required byte MaxAge {get; set;}
    public required decimal PolicyTermYears {get; set;}
    public string? Description {get; set;}
    public DateTimeOffset? DeletedAt {get; set;}
}
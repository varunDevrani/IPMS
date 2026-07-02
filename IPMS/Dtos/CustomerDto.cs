using IPMS.Entities;

namespace IPMS.Dtos;


public class CustomerDto
{
    public required string FirstName {get; set;}
    public string? MiddleName {get; set;}
    public string? LastName {get; set;}
    public required string Email {get; set;}
    public required string PhoneNumber {get; set;}
    public required DateOnly DateOfBirth {get; set;}
    public required CustomerGender Gender {get; set;}
    public required bool MaritalStatus {get; set;}
    public required DateTimeOffset CreatedAt {get; set;}
    public required DateTimeOffset UpdatedAt {get; set;}
}

public class UpdateCustomerDto
{
    public string? FirstName {get; set;}
    public string? MiddleName {get; set;}
    public string? LastName {get; set;}
    public string? Email {get; set;}
    public string? PhoneNumber {get; set;}
    public DateOnly? DateOfBirth {get; set;}
    public CustomerGender? Gender {get; set;}
    public bool? MaritalStatus {get; set;}
}
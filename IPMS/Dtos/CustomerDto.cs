using IPMS.Entities;

namespace IPMS.Dtos;


public class CustomerAddressDto
{
    public ulong? HouseNumber {get; set;}
    public ulong? StreetNumber {get; set;}
    public string? StreetName {get; set;}
    public string? StreetSuffix {get; set;}
    public required string City {get; set;}
    public required string State {get; set;}
    public required string ZipCode {get; set;}
}


public class UpdateCustomerAddressDto
{
    public ulong? HouseNumber {get; set;}
    public ulong? StreetNumber {get; set;}
    public string? StreetName {get; set;}
    public string? StreetSuffix {get; set;}
    public string? City {get; set;}
    public string? State {get; set;}
    public string? ZipCode {get; set;}
}


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
    public required CustomerAddressDto Address {get; set;}
    public required DateTimeOffset CreatedAt {get; set;}
    public required DateTimeOffset UpdatedAt {get; set;}
}


public class CreateCustomerDto
{
    public required DateOnly DateOfBirth {get; set;}
    public required CustomerGender Gender {get; set;}
    public required bool MaritalStatus {get; set;}
    public required CustomerAddressDto Address {get; set;}
    public required string SSN {get; set;}
}


public class CustomersDto
{
    public required ulong Total {get; set;}
    public required List<CustomerDto> Customers {get; set;}
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
    public UpdateCustomerAddressDto? Address {get; set;}
}

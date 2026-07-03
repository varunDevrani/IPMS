using System.Security.Claims;
using IPMS.Data;
using IPMS.Dtos;
using IPMS.Entities;
using Microsoft.AspNetCore.Mvc;

namespace IPMS.Controllers;


[ApiController]
[Route("api/[controller]")]
public class CustomerController: ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;

    public CustomerController(AppDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }


    [HttpPost]
    public ActionResult<CustomerDto> CreateCustomer(CreateCustomerDto payload)
    {
        Guid userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        User? user = _context.Users.FirstOrDefault(u => u.Id == userId);

        if (user is null)
            return Unauthorized();

        Customer? existingCustomer = _context.Customers
            .FirstOrDefault(c => c.UserId == userId && c.DeletedAt == null);

        if (existingCustomer is not null)
            return Conflict("Customer profile already exists.");

        var customer = new Customer
        {
            UserId = userId,
            DateOfBirth = payload.DateOfBirth,
            Gender = payload.Gender,
            MaritalStatus = payload.MaritalStatus,
            SSNHash = payload.SSN
        };

        _context.Customers.Add(customer);
        _context.SaveChanges();

        var address = new CustomerAddress
        {
            CustomerId = customer.Id,
            HouseNumber = payload.Address.HouseNumber,
            StreetNumber = payload.Address.StreetNumber,
            StreetName = payload.Address.StreetName,
            StreetSuffix = payload.Address.StreetSuffix,
            City = payload.Address.City,
            State = payload.Address.State,
            ZipCode = payload.Address.ZipCode
        };

        _context.CustomerAddresses.Add(address);
        _context.SaveChanges();

        return Ok(new CustomerDto
        {
            FirstName = user.FirstName,
            MiddleName = user.MiddleName,
            LastName = user.LastName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,

            DateOfBirth = customer.DateOfBirth,
            Gender = customer.Gender,
            MaritalStatus = customer.MaritalStatus,

            Address = new CustomerAddressDto
            {
                HouseNumber = address.HouseNumber,
                StreetNumber = address.StreetNumber,
                StreetName = address.StreetName,
                StreetSuffix = address.StreetSuffix,
                City = address.City,
                State = address.State,
                ZipCode = address.ZipCode
            },

            CreatedAt = customer.CreatedAt,
            UpdatedAt = customer.UpdatedAt
        });
    }

    
    [HttpGet]
    public ActionResult<CustomersDto> GetCustomers()
    {
        var customers = (
            from customer in _context.Customers
            join user in _context.Users
                on customer.UserId equals user.Id
            join address in _context.CustomerAddresses
                on customer.Id equals address.CustomerId
            where customer.DeletedAt == null
            select new CustomerDto
            {
                FirstName = user.FirstName,
                MiddleName = user.MiddleName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,

                DateOfBirth = customer.DateOfBirth,
                Gender = customer.Gender,
                MaritalStatus = customer.MaritalStatus,

                Address = new CustomerAddressDto
                {
                    HouseNumber = address.HouseNumber,
                    StreetNumber = address.StreetNumber,
                    StreetName = address.StreetName,
                    StreetSuffix = address.StreetSuffix,
                    City = address.City,
                    State = address.State,
                    ZipCode = address.ZipCode
                },

                CreatedAt = customer.CreatedAt,
                UpdatedAt = customer.UpdatedAt
            }
        ).ToList();

        return Ok(new CustomersDto
        {
            Total = (ulong)customers.Count,
            Customers = customers
        });
    }

    [HttpGet("{customer_id}")]
    public ActionResult<CustomerDto> GetCustomerById(Guid customer_id)
    {
        var customer = (
            from c in _context.Customers
            join u in _context.Users
                on c.UserId equals u.Id
            join a in _context.CustomerAddresses
                on c.Id equals a.CustomerId
            where c.Id == customer_id && c.DeletedAt == null
            select new CustomerDto
            {
                FirstName = u.FirstName,
                MiddleName = u.MiddleName,
                LastName = u.LastName,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,

                DateOfBirth = c.DateOfBirth,
                Gender = c.Gender,
                MaritalStatus = c.MaritalStatus,

                Address = new CustomerAddressDto
                {
                    HouseNumber = a.HouseNumber,
                    StreetNumber = a.StreetNumber,
                    StreetName = a.StreetName,
                    StreetSuffix = a.StreetSuffix,
                    City = a.City,
                    State = a.State,
                    ZipCode = a.ZipCode
                },

                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            }
        ).FirstOrDefault();

        if (customer is null)
            return NotFound("Customer not found.");

        return Ok(customer);
    }


    [HttpPatch("{customer_id}")]
    public ActionResult<CustomerDto> UpdateCustomerById(Guid customer_id, UpdateCustomerDto payload)
    {
        var customer = _context.Customers
            .FirstOrDefault(c => c.Id == customer_id && c.DeletedAt == null);

        if (customer is null)
            return NotFound("Customer not found.");

        var user = _context.Users
            .First(u => u.Id == customer.UserId);

        var address = _context.CustomerAddresses
            .First(a => a.CustomerId == customer.Id);

        // Update User
        if (payload.FirstName is not null)
            user.FirstName = payload.FirstName;

        if (payload.MiddleName is not null)
            user.MiddleName = payload.MiddleName;

        if (payload.LastName is not null)
            user.LastName = payload.LastName;

        if (payload.Email is not null)
            user.Email = payload.Email;

        if (payload.PhoneNumber is not null)
            user.PhoneNumber = payload.PhoneNumber;

        // Update Customer
        if (payload.DateOfBirth.HasValue)
            customer.DateOfBirth = payload.DateOfBirth.Value;

        if (payload.Gender.HasValue)
            customer.Gender = payload.Gender.Value;

        if (payload.MaritalStatus.HasValue)
            customer.MaritalStatus = payload.MaritalStatus.Value;

        // Update Address
        if (payload.Address is not null)
        {
            if (payload.Address.HouseNumber.HasValue)
                address.HouseNumber = payload.Address.HouseNumber.Value;

            if (payload.Address.StreetNumber.HasValue)
                address.StreetNumber = payload.Address.StreetNumber.Value;

            if (payload.Address.StreetName is not null)
                address.StreetName = payload.Address.StreetName;

            if (payload.Address.StreetSuffix is not null)
                address.StreetSuffix = payload.Address.StreetSuffix;

            if (payload.Address.City is not null)
                address.City = payload.Address.City;

            if (payload.Address.State is not null)
                address.State = payload.Address.State;

            if (payload.Address.ZipCode is not null)
                address.ZipCode = payload.Address.ZipCode;
        }

        _context.SaveChanges();

        return Ok(new CustomerDto
        {
            FirstName = user.FirstName,
            MiddleName = user.MiddleName,
            LastName = user.LastName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,

            DateOfBirth = customer.DateOfBirth,
            Gender = customer.Gender,
            MaritalStatus = customer.MaritalStatus,

            Address = new CustomerAddressDto
            {
                HouseNumber = address.HouseNumber,
                StreetNumber = address.StreetNumber,
                StreetName = address.StreetName,
                StreetSuffix = address.StreetSuffix,
                City = address.City,
                State = address.State,
                ZipCode = address.ZipCode
            },

            CreatedAt = customer.CreatedAt,
            UpdatedAt = customer.UpdatedAt
        });
    }


    [HttpDelete("{customer_id}")]
    public ActionResult DeleteCustomerById(Guid customer_id)
    {
        var customer = _context.Customers
            .FirstOrDefault(c => c.Id == customer_id && c.DeletedAt == null);

        if (customer is null)
            return NotFound("Customer not found.");

        customer.DeletedAt = DateTimeOffset.UtcNow;

        _context.SaveChanges();

        return NoContent();
    }
}
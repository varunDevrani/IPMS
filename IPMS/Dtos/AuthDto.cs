namespace IPMS.Dtos;

public class AuthSignupDto
{
    public required string FirstName {get; set;}
    public string? MiddleName {get; set;}
    public string? LastName {get; set; }
    public required string Email {get; set;} 
    public required string PhoneNumber {get; set;}
    public required string Password {get; set;}
    public required string PasswordConfirm {get; set;}
}


public class AuthLoginDto
{
    public required string Email {get; set;} = string.Empty;
    public required string Password {get; set;} = string.Empty;
}


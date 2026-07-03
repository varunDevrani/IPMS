using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;

using IPMS.Entities;
using IPMS.Dtos;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using IPMS.Data;
using Microsoft.EntityFrameworkCore;



namespace IPMS.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController: ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;

    public AuthController(AppDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    private static PasswordHasher<User> _PasswordHasher = new PasswordHasher<User>();
    private static string DummyPasswordHash = _PasswordHasher.HashPassword(null!, "correct horse battery staple");


    [HttpPost("signup")]
    public ActionResult<UserDto> Register(AuthSignupDto payload)
    {
        if (payload.Password != payload.PasswordConfirm)
            return Conflict("Password and PasswordConfirm mismatch");

        User? userExists = _context.Users.FirstOrDefault(
            u => u.Email == payload.Email || u.PhoneNumber == payload.PhoneNumber);

        if (userExists is not null)
            return Conflict("User with the email or phone number already exists");

        var user = new User
        {
            FirstName = payload.FirstName,
            Email = payload.Email,
            PasswordHash = _PasswordHasher.HashPassword(null!, payload.Password),
            PhoneNumber = payload.PhoneNumber
        };

        _context.Users.Add(user);
        _context.SaveChanges();

        var customerRole = _context.Roles
            .Single(r => r.Name == "Customer");

        _context.UserRoles.Add(new UserRole
        {
            UserId = user.Id,
            RoleId = customerRole.Id
        });

        _context.SaveChanges();

        return Ok(new UserDto
        {
            FirstName = user.FirstName,
            MiddleName = user.MiddleName,
            LastName = user.LastName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        });
    }

    
    [HttpPost("login")]
    public ActionResult<TokenDto> Login(AuthLoginDto payload)
    {
        User? user = _context.Users.FirstOrDefault(u => u.Email == payload.Email);
        if(user is null)
        {
            PasswordVerificationResult _ = _PasswordHasher.VerifyHashedPassword(null!, DummyPasswordHash, payload.Password);
            return Unauthorized("User is unauthorized");
        }

        PasswordVerificationResult result = _PasswordHasher.VerifyHashedPassword(user, user.PasswordHash, payload.Password);
        if(result == PasswordVerificationResult.Failed)
        {
            return Unauthorized("User is unauthorized");
        }

        TokenFamily family = new()
        {
            UserId = user.Id
        };
        _context.TokenFamilies.Add(family);
        _context.SaveChanges();

        string refreshTokenRaw = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        string refreshTokenHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(refreshTokenRaw)));

        RefreshToken refreshToken = new()
        {
            TokenHash = refreshTokenHash,
            ExpiresAt = DateTimeOffset.UtcNow.AddDays(1),
            FamilyId = family.Id
        };
        _context.RefreshTokens.Add(refreshToken);
        _context.SaveChanges();

        return Ok(new TokenDto{
            AccessToken = CreateToken(user, family.Id),  
            RefreshToken = refreshTokenRaw
        });
    }


    [HttpPost("refresh")]
    public ActionResult<TokenDto> GenerateAccessToken(RefreshTokenDto payload)
    {
        string refreshTokenHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(payload.Token)));
        RefreshToken? refreshToken = _context.RefreshTokens.FirstOrDefault(rt => rt.TokenHash == refreshTokenHash);
        if(refreshToken is null)
        {
            return Unauthorized("Invalid refresh token providied!");
        }

        if(refreshToken.ExpiresAt <= DateTimeOffset.UtcNow)
        {
            return Unauthorized("Refresh token has expired!");
        }

        TokenFamily family = _context.TokenFamilies.FirstOrDefault(tf => tf.Id == refreshToken.FamilyId)!;
        if(family.RevokedAt is not null)
        {
            return Unauthorized("Session ended. Please login again.");
        }

        if(refreshToken.UsedAt is not null)
        {
            family.RevokedAt = family.UpdatedAt = DateTimeOffset.UtcNow;
            _context.RefreshTokens.
                Where(rt => rt.FamilyId == family.Id && rt.UsedAt == null).
                ExecuteUpdate(setters => setters.
                    SetProperty(rt => rt.UsedAt, DateTimeOffset.UtcNow).
                    SetProperty(rt => rt.UpdatedAt, DateTimeOffset.UtcNow)
                );
            _context.SaveChanges();
            return Unauthorized("Token reuse detected. All sessions revoked");
        }
        refreshToken.UsedAt = refreshToken.UpdatedAt = DateTimeOffset.UtcNow;

        User user = _context.Users.FirstOrDefault(u => u.Id == family.UserId)!;

        string rotateRefreshTokenRaw = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        string rotateRefreshTokenHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(rotateRefreshTokenRaw)));

        RefreshToken rotateRefreshToken = new RefreshToken
        {
            TokenHash = rotateRefreshTokenHash,
            ExpiresAt = DateTimeOffset.UtcNow.AddDays(1),
            FamilyId = refreshToken.FamilyId
        };

        _context.RefreshTokens.Add(rotateRefreshToken);
        _context.SaveChanges();

        return Ok(new TokenDto
        {
            AccessToken = CreateToken(user, refreshToken.FamilyId),
            RefreshToken = rotateRefreshTokenRaw
        });
    }


    [HttpPost("logout")]
    public ActionResult<string> Logout(RefreshTokenDto payload)
    {
        string refreshTokenHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(payload.Token)));
        RefreshToken? refreshToken = _context.RefreshTokens.FirstOrDefault(rt => rt.TokenHash == refreshTokenHash);

        if(refreshToken is not null)
        {
            TokenFamily family = _context.TokenFamilies.FirstOrDefault(tf => tf.Id == refreshToken.FamilyId)!;
            refreshToken.UsedAt = DateTimeOffset.UtcNow;
            refreshToken.UpdatedAt = DateTimeOffset.UtcNow;
            family.RevokedAt = DateTimeOffset.UtcNow;
            family.UpdatedAt = DateTimeOffset.UtcNow;
            _context.SaveChanges();
        }

        return Ok("User successfully logged out");
    }



    private string CreateToken(User user, Guid familyId)
    {
        List<System.Security.Claims.Claim> claims =
        [
            new System.Security.Claims.Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new System.Security.Claims.Claim(ClaimTypes.Name, user.Email),
            new System.Security.Claims.Claim(ClaimTypes.Sid, familyId.ToString())
        ];

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetValue<string>("AppSettings:Token")!));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

        var tokenDescriptor = new JwtSecurityToken(
            issuer: _config.GetValue<string>("AppSettings:Issuer"),
            audience: _config.GetValue<string>("AppSettings:Audience"),
            claims: claims,
            expires: DateTime.UtcNow.AddDays(1),
            signingCredentials: creds
        );
        return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
    }


    
}
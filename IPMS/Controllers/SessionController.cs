using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using IPMS.Data;
using IPMS.Dtos;
using IPMS.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IPMS.Controllers;


[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SessionController: ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;

    public SessionController(AppDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    [HttpPost]
    public ActionResult<string> GetCurrentSessions(RefreshTokenDto payload)
    {
        string refreshTokenHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(payload.Token)));
        List<TokenFamily> families = _context.TokenFamilies.Where(tf => tf.RevokedAt == null).ToList();

        SessionsDto result = new();
        for(int idx = 0; idx < families.Count; idx++)
        {
            RefreshToken? refreshToken = _context.RefreshTokens.FirstOrDefault(rt => rt.TokenHash == refreshTokenHash && rt.FamilyId == families[idx].Id && rt.UsedAt == null);

            result.Sessions.Add(new SessionDto
            {
                FamilyId = families[idx].Id,
                CreatedAt = families[idx].CreatedAt,
                Current = refreshToken != null
            });
        }

        return Ok(result);
    }

    [HttpDelete]
    public ActionResult<string> RevokeAllSessions()
    {
        Guid userId = Guid.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!
        );

        _context.TokenFamilies
            .Where(tf => tf.UserId == userId && tf.RevokedAt == null)
            .ExecuteUpdate(setters => setters
                .SetProperty(tf => tf.RevokedAt, DateTimeOffset.UtcNow)
                .SetProperty(tf => tf.UpdatedAt, DateTimeOffset.UtcNow)
            );
        _context.SaveChanges();

        return NoContent();
    }

    [HttpDelete("{family_id}")]
    public ActionResult<string> RevokeSessionByFamilyId(Guid family_id)
    {
        Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        TokenFamily? family = _context.TokenFamilies.FirstOrDefault(tf => tf.Id == family_id && tf.UserId == userId);

        if(family is not null)
        {
            family.RevokedAt = family.UpdatedAt = DateTimeOffset.UtcNow;
            _context.SaveChanges();   
        }

        return NoContent();
    }


}
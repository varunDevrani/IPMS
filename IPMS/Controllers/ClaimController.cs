using IPMS.Data;
using IPMS.Dtos;
using IPMS.Entities;
using Microsoft.AspNetCore.Mvc;


namespace IPMS.Controllers;


[ApiController]
[Route("api/[controller]")]
public class ClaimController: ControllerBase
{

    private readonly AppDbContext _context;


    public ClaimController(AppDbContext context)
    {
        _context = context;
    }



    [HttpGet]
    public ActionResult<ClaimsDto> GetClaims()
    {

        List<ClaimDto> claims = _context.Claims
            .Select(c => new ClaimDto
            {
                ClaimNumber = c.ClaimNumber,
                PolicyId = c.PolicyId,
                UnderWriterId = c.UnderWriterId,
                IncidentDate = c.IncidentDate,
                ClaimDate = c.ClaimDate,
                ClaimAmount = c.ClaimAmount,
                Reason = c.Reason,
                Remark = c.Remark,
                Status = c.Status,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            })
            .ToList();


        return Ok(new ClaimsDto
        {
            Total = (ulong)claims.Count,
            Claims = claims
        });

    }





    [HttpGet("{claim_id}")]
    public ActionResult<ClaimDto> GetClaimById(Guid claim_id)
    {

        Claim? claim = _context.Claims
            .FirstOrDefault(c => c.Id == claim_id);


        if(claim is null)
        {
            return NotFound("Claim not found");
        }


        return Ok(new ClaimDto
        {
            ClaimNumber = claim.ClaimNumber,
            PolicyId = claim.PolicyId,
            UnderWriterId = claim.UnderWriterId,
            IncidentDate = claim.IncidentDate,
            ClaimDate = claim.ClaimDate,
            ClaimAmount = claim.ClaimAmount,
            Reason = claim.Reason,
            Remark = claim.Remark,
            Status = claim.Status,
            CreatedAt = claim.CreatedAt,
            UpdatedAt = claim.UpdatedAt
        });

    }





    [HttpPost]
    public ActionResult<ClaimDto> CreateClaim(CreateClaimDto payload)
    {

        Claim claim = new()
        {
            ClaimNumber = $"CLM-{Guid.NewGuid().ToString()[..8].ToUpper()}",
            PolicyId = payload.PolicyId,
            UnderWriterId = Guid.Empty,
            IncidentDate = payload.IncidentDate,
            ClaimDate = DateOnly.FromDateTime(DateTime.UtcNow),
            ClaimAmount = payload.ClaimAmount,
            Reason = payload.Reason,
            Status = ClaimStatus.Submitted
        };


        _context.Claims.Add(claim);
        _context.SaveChanges();


        return Ok(new ClaimDto
        {
            ClaimNumber = claim.ClaimNumber,
            PolicyId = claim.PolicyId,
            UnderWriterId = claim.UnderWriterId,
            IncidentDate = claim.IncidentDate,
            ClaimDate = claim.ClaimDate,
            ClaimAmount = claim.ClaimAmount,
            Reason = claim.Reason,
            Remark = claim.Remark,
            Status = claim.Status,
            CreatedAt = claim.CreatedAt,
            UpdatedAt = claim.UpdatedAt
        });

    }





    [HttpPatch("{claim_id}")]
    public ActionResult<ClaimDto> UpdateClaim(
        Guid claim_id,
        UpdateClaimDto payload)
    {

        Claim? claim = _context.Claims
            .FirstOrDefault(c => c.Id == claim_id);


        if(claim is null)
        {
            return NotFound("Claim not found");
        }


        if(payload.UnderWriterId.HasValue)
        {
            claim.UnderWriterId = payload.UnderWriterId.Value;
        }


        if(payload.Status.HasValue)
        {
            claim.Status = payload.Status.Value;
        }


        if(payload.Remark is not null)
        {
            claim.Remark = payload.Remark;
        }


        if(payload.ClaimAmount.HasValue)
        {
            claim.ClaimAmount = payload.ClaimAmount.Value;
        }


        claim.UpdatedAt = DateTimeOffset.UtcNow;

        _context.SaveChanges();


        return Ok(new ClaimDto
        {
            ClaimNumber = claim.ClaimNumber,
            PolicyId = claim.PolicyId,
            UnderWriterId = claim.UnderWriterId,
            IncidentDate = claim.IncidentDate,
            ClaimDate = claim.ClaimDate,
            ClaimAmount = claim.ClaimAmount,
            Reason = claim.Reason,
            Remark = claim.Remark,
            Status = claim.Status,
            CreatedAt = claim.CreatedAt,
            UpdatedAt = claim.UpdatedAt
        });

    }





    [HttpPost("{claim_id}/documents")]
    public ActionResult<ClaimDocumentDto> UploadDocument(
        Guid claim_id,
        UploadClaimDocumentDto payload)
    {

        Claim? claim = _context.Claims
            .FirstOrDefault(c => c.Id == claim_id);


        if(claim is null)
        {
            return NotFound("Claim not found");
        }


        ClaimDocument document = new()
        {
            ClaimId = claim_id,
            FileName = payload.FileName,
            FileType = payload.FileType,
            FileURL = payload.FileURL,
            UploadedAt = DateTimeOffset.UtcNow,
            UploadedBy = payload.UploadedBy
        };


        _context.ClaimDocuments.Add(document);
        _context.SaveChanges();



        return Ok(new ClaimDocumentDto
        {
            ClaimId = document.ClaimId,
            FileName = document.FileName,
            FileType = document.FileType,
            FileURL = document.FileURL,
            UploadedAt = document.UploadedAt,
            UploadedBy = document.UploadedBy
        });

    }




    // Get claim documents
    [HttpGet("{claim_id}/documents")]
    public ActionResult<List<ClaimDocumentDto>> GetClaimDocuments(Guid claim_id)
    {

        List<ClaimDocumentDto> documents = _context.ClaimDocuments
            .Where(d => d.ClaimId == claim_id)
            .Select(d => new ClaimDocumentDto
            {
                ClaimId = d.ClaimId,
                FileName = d.FileName,
                FileType = d.FileType,
                FileURL = d.FileURL,
                UploadedAt = d.UploadedAt,
                UploadedBy = d.UploadedBy
            })
            .ToList();


        return Ok(documents);

    }

}
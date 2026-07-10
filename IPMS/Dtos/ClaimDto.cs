using IPMS.Entities;

namespace IPMS.Dtos;


public class ClaimDto
{
    public required string ClaimNumber { get; set; }
    public required Guid PolicyId { get; set; }
    public required Guid UnderWriterId { get; set; }
    public required DateOnly IncidentDate { get; set; }
    public DateOnly? ClaimDate { get; set; }
    public required decimal ClaimAmount { get; set; }
    public string? Reason { get; set; }
    public string? Remark { get; set; }
    public required ClaimStatus Status { get; set; }

    public required DateTimeOffset CreatedAt { get; set; }
    public required DateTimeOffset UpdatedAt { get; set; }
}



public class ClaimsDto
{
    public required ulong Total { get; set; }
    public required List<ClaimDto> Claims { get; set; }
}



public class CreateClaimDto
{
    public required Guid PolicyId { get; set; }
    public required DateOnly IncidentDate { get; set; }
    public required decimal ClaimAmount { get; set; }
    public string? Reason { get; set; }
}



public class UpdateClaimDto
{
    public Guid? UnderWriterId { get; set; }
    public ClaimStatus? Status { get; set; }
    public string? Remark { get; set; }
    public decimal? ClaimAmount { get; set; }
}



public class ClaimDocumentDto
{
    public required Guid ClaimId { get; set; }
    public required string FileName { get; set; }
    public required string FileType { get; set; }
    public required string FileURL { get; set; }
    public required DateTimeOffset UploadedAt { get; set; }
    public required Guid UploadedBy { get; set; }
}



public class UploadClaimDocumentDto
{
    public required string FileName { get; set; }
    public required string FileType { get; set; }
    public required string FileURL { get; set; }
    public required Guid UploadedBy { get; set; }
}
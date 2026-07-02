namespace IPMS.Entities;


public enum AuditLogAction
{
    INSERT = 0,
    UPDATE = 1,
    DELETE = 2,
    LOGIN = 3,
    LOGOUT = 4,
    APPROVE = 5,
    REJECT = 6,
    MISC = 7
}

public class AuditLog: BaseEntity
{
    public required Guid UserId {get; set;}
    public required Guid RecordId {get; set;}
    public required string TableName {get; set;}
    public required AuditLogAction Action {get; set;}
    public string? IPAddresses {get; set;}
    public string? OldValues {get; set;}
    public string? NewValues {get; set;}
}   
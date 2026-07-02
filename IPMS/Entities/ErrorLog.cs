namespace IPMS.Entities;


public class ErrorLog: BaseEntity
{
    public required Guid UserId {get; set;}
    public string? ExceptionMessage {get; set;}
    public string? StackTrace {get; set;}
    public string? Source {get; set;}
    public string? Controller {get; set;}
    public string? Action {get; set;}
    public string? MetaData {get; set;}
}
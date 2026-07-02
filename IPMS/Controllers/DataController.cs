
using IPMS.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IPMS.Controllers;


[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DataController: ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;

    public DataController(AppDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    [HttpGet("test")]
    public ActionResult<string> GetData()
    {
        foreach(var header in Request.Headers)
        {
            Console.WriteLine($"{header.Key} -> {header.Value}");
        }
        return Ok("ok");
    }
}

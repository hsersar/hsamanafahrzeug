using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace iKfz.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FilesController : ControllerBase
{
    private readonly IWebHostEnvironment _env;
    private static readonly FileExtensionContentTypeProvider ContentTypeProvider = new();

    public FilesController(IWebHostEnvironment env)
    {
        _env = env;
    }

    /// <summary>
    /// Serve an uploaded file (profiles/logos).
    /// </summary>
    [HttpGet("uploads/{subfolder}/{fileName}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetFile(string subfolder, string fileName)
    {
        // Only allow safe subfolder names
        if (subfolder != "profiles" && subfolder != "logos")
            return NotFound();

        // Sanitise filename – prevent path traversal
        if (fileName.Contains("..") || fileName.Contains('/') || fileName.Contains('\\'))
            return BadRequest();

        var fullPath = Path.Combine(_env.ContentRootPath, "uploads", subfolder, fileName);
        if (!System.IO.File.Exists(fullPath))
            return NotFound();

        if (!ContentTypeProvider.TryGetContentType(fullPath, out var contentType))
            contentType = "application/octet-stream";

        var bytes = System.IO.File.ReadAllBytes(fullPath);
        return File(bytes, contentType);
    }
}

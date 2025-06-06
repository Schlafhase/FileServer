using System.Net.Mime;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace UploadBackend;

[ApiController]
[Route("[controller]")]
public class FileController : ControllerBase
{
    private readonly string _hash = Convert.ToBase64String(System.IO.File.ReadAllBytes("hash"));

    [HttpPost("upload")]
    public async Task<IActionResult> UploadFile(IFormFile file, string password)
    {
        if (hashData(password) != _hash)
        {
            return Unauthorized();
        }

        if (file.Length == 0)
            return BadRequest("No file uploaded.");

        string uploadsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "uploads");
        Directory.CreateDirectory(uploadsDir);

        string filePath = Path.Combine(uploadsDir, file.FileName);

        await using (FileStream stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        string fileUrl = $"{Request.Scheme}://{Request.Host}/uploads/{file.FileName}";
        return Ok(new { url = fileUrl });
    }

    [HttpGet("GetAllFiles")]
    public IActionResult GetAllFiles(string password)
    {
        if (hashData(password) != _hash)
        {
            return Unauthorized();
        }

        List<Upload> files = [];
        files
            .AddRange(Directory
                .EnumerateFiles(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "uploads"))
                .Select(file => new Upload(new FileInfo(file))));
        
        return Ok(files);
    }

    public record Upload
    {
        public string URL { get; set; }
        public string Name { get; set; }

        public Upload(FileInfo file)
        {
            URL = $"/uploads/{file.Name}";
            Name = file.Name;
        }
    }

    private static string hashData(string password)
    {
        return Convert.ToBase64String(SHA256.HashData(Encoding.ASCII.GetBytes(password)));
    }

}
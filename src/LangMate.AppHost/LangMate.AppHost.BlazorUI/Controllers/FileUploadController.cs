using LangMate.AppHost.BlazorUI.Models;
using Microsoft.AspNetCore.Mvc;

namespace LangMate.AppHost.BlazorUI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FileUploadController(IWebHostEnvironment env) : ControllerBase
    {
        private readonly IWebHostEnvironment _env = env;

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File not selected");

            var uploadsPath = Path.Combine(_env.WebRootPath, "uploads");
            Directory.CreateDirectory(uploadsPath);

            // Get the file extension (including the dot)
            var extension = Path.GetExtension(file.FileName);

            // Generate a random filename
            var randomFileName = $"{Guid.NewGuid()}{extension}";

            var filePath = Path.Combine(uploadsPath, randomFileName);

            await using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            return Ok(new FileUploadResponse()
            {
                FileName = randomFileName,
                Length = file.Length,
                FilePath = Path.Combine("uploads", randomFileName)
            });
        }

        [HttpGet]
        public IActionResult GetUploadedFiles()
        {
            var uploadsPath = Path.Combine(_env.WebRootPath, "uploads");

            if (!Directory.Exists(uploadsPath))
                return Ok(Array.Empty<string>());

            var files = Directory.GetFiles(uploadsPath)
                .Select(Path.GetFileName)
                .ToArray();

            return Ok(files);
        }
    }
}
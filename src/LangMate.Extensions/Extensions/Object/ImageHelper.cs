using System.Drawing.Imaging;
using static System.Net.Mime.MediaTypeNames;

namespace LangMate.Extensions.Extensions.Object
{
    public class ImageHelper
    {
        public static string ConvertImageToBase64(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Image file not found", filePath);

            byte[] imageBytes = File.ReadAllBytes(filePath);

            // Get MIME type based on extension
            string mimeType = filePath.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ? "image/png" :
                              filePath.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) || filePath.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ? "image/jpeg" :
                              filePath.EndsWith(".gif", StringComparison.OrdinalIgnoreCase) ? "image/gif" :
                              filePath.EndsWith(".bmp", StringComparison.OrdinalIgnoreCase) ? "image/bmp" :
                              filePath.EndsWith(".webp", StringComparison.OrdinalIgnoreCase) ? "image/webp" :
                              "application/octet-stream";

            return $"data:{mimeType};base64,{Convert.ToBase64String(imageBytes)}";
        }
    }
}
using Microsoft.AspNetCore.Mvc;

namespace TFT_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImageController : ControllerBase
    {
        private const string ImageExtension = ".png";
        private const string ContentType = "image/png";

        [HttpGet("items/{fileName}")]
        public IActionResult GetItemImage(string fileName)
        {
            return GetImage("items", fileName);
        }

        [HttpGet("augments/{fileName}")]
        public IActionResult GetAugmentImage(string fileName)
        {
            return GetImage("augments", fileName);
        }

        [HttpGet("traits/{fileName}")]
        public IActionResult GetTraitImage(string fileName)
        {
            return GetImage("traits", fileName);
        }

        [HttpGet("champions/{imageType}/{fileName}")]
        public IActionResult GetChampionImage(string imageType, string fileName)
        {
            var subDirectory = imageType.ToLower() switch
            {
                "splash" => "splash",
                "centered" => "centered",
                "tiles" => "tiles",
                _ => null
            };

            if (subDirectory == null)
                return NotFound();

            return GetImage(Path.Combine("champions", subDirectory), fileName);
        }

        private IActionResult GetImage(string directory, string fileName)
        {
            fileName = fileName + ImageExtension;
            var imagePath = Path.Combine("assets", "images", directory, fileName);

            if (!System.IO.File.Exists(imagePath))
                return NotFound();

            var stream = System.IO.File.OpenRead(imagePath);
            return File(stream, ContentType);
        }
    }
}

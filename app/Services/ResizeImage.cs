using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing.Transforms;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;

namespace app.Services
{
    public class ResizeImage : IResizeImage
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        public ResizeImage(IHostingEnvironment  he)
        {
            _hostingEnvironment = he;
        }
        
        public (string image, string thumbnail) GetImagePathsWithThumbnail(IFormFile image)
        {
            // Generate a random filename and find destination path
            var filename = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
            var path = Path.Combine(_hostingEnvironment.WebRootPath, "images/uploads/" + filename + ".jpg");

            // Resize the image to 100 pixels wide and save it
            using (Image<Rgba32> img = Image.Load(image.OpenReadStream()))
            {
                var filestream = System.IO.File.Create(path);
                if (img.Width >= img.Height)
                    img.Mutate(ctx => ctx.Resize(100, 0));
                else
                    img.Mutate(ctx => ctx.Resize(0, 100));
                img.SaveAsJpeg(filestream);
                filestream.Close();
            }

            // Create another copy 50 pixels wide for thumbnail
            var thumbnailPath = path.Replace(".jpg", "_thumb.jpg");
            using (Image<Rgba32> img = Image.Load(image.OpenReadStream()))
            {
                var filestream = System.IO.File.Create(thumbnailPath);
                if (img.Width >= img.Height)
                    img.Mutate(ctx => ctx.Resize(50, 0));
                else
                    img.Mutate(ctx => ctx.Resize(0, 50));
                img.SaveAsJpeg(filestream);
                filestream.Close();
            }

            return (image: Path.GetRelativePath(_hostingEnvironment.WebRootPath, path), thumbnail: Path.GetRelativePath(_hostingEnvironment.WebRootPath, thumbnailPath));
        }
    }
}

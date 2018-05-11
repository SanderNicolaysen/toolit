using Microsoft.AspNetCore.Http;

namespace app.Services
{
    public interface IResizeImage
    {
        (string image, string thumbnail) GetImagePathsWithThumbnail(IFormFile image);
    }
}
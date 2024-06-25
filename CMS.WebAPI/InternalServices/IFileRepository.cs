
using Entities.Models;

namespace CMS.WebAPI.InternalServices
{
    public interface IFileRepository
    {       
        Task<string> AddMediaPicAsync(IFormFile image, List<Setting> sysConfig);
        Task<string> AddFileAndVideo(IFormFile fil, int mediaExCategoryId);
        bool DeleteFile(Medium media);
        Task<string> CropImages(IFormFile image, int width, int height);
    }
}

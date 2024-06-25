
using Entities.DataTransferObjects;
using Entities.Models;

namespace CMS.API.InternalServices
{
    public interface IFileRepository
    {
        Task<string> AddProfilePic(IFormFile ProfileImage);
        Task<string> AddMediaPicAsync(IFormFile image, List<Setting> sysConfig);
        Task<string> AddMediaPicWithCropAsync(IFormFile image, List<Setting> sysConfig, MediaDto model);
        Task<string> AddFileAndVideo(IFormFile fil, int mediaExCategoryId);
        bool DeleteFile(Medium media);
        Task<string> CropImages(IFormFile image, int width, int height);
    }
}

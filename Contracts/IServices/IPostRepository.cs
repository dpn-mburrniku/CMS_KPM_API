using Entities.Models;
using Entities.DataTransferObjects;
using Entities.RequestFeatures;
using Entities.DataTransferObjects.WebDTOs;

namespace Contracts.IServices
{
    public interface IPostRepository : IGenericRepository<Post>
    {
        Task<PagedList<Post>> GetPostsAsync(PostFilterParameters parameter, List<int>? postIds, bool isDeleted);
        Task<Post?> GetPostById(int id, int webLangId);
        Task<bool> AddMediaCollectionInPost(AddMediaCollectionInPost model, List<Language> langList, string UserId);
        Task<bool> RemoveMediaCollectionFromPost(List<int> MediaIds, List<Language> langList, bool WebMultiLang, int webLangId, int postId);
        Task<List<GetPostMediaDto>> GetPostMedia(int postId, int webLangId, bool isSlider);
        Task<GetPostMediaDto> GetPostMediaById(int mediaId, int LanguageId);


        // web
        Task<List<NewsModel>> GetNews(int GjuhaId, List<int> PostimiKategoriaID, int skip, int take, int TitulliLength, int PermbajtjaLength, DateTime? date, string? SearchText, DateTime? DateFrom, DateTime? DateTo, DateTime formatedDateTime);
        Task<int> GetNewsCount(int GjuhaId, List<int> PostimiKategoriaID, DateTime? date, string? SearchText, DateTime? DateFrom, DateTime? DateTo, DateTime formatedDateTime);
        Task<NewsModel> GetNewsDetails(int GjuhaId, int PostimiID);
        Task<List<MediaModel>> GetNewsDetailsMedia(int GjuhaId, int PostimiID, bool isSlider);
        Task<List<NewsCategoriesModel>> GetNewsCategories(int PageId, int gjuhaID, bool GetAll);
        Task<int> GetCheckNewsForThisDate(int gjuhaID, DateTime? date);
    }
}

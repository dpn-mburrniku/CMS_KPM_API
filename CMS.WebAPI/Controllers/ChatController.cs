using AutoMapper;
using Contracts.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Cors;
using Entities.DataTransferObjects.WebDTOs;
using Entities.Models;

namespace CMS.WebAPI.Controllers
{
    [EnableCors]
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public IMapper _mapper { get; }
        public ChatController(IUnitOfWork unitOfWork
                    , IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet("GetChatBoot")]
        public async Task<IActionResult> GetChatBoot(int? ParentId, string Gjuha = "sq", string? searchText = "")
        {
            int gjuhaID = _unitOfWork.BaseConfig.GetGjuhaID(Gjuha);

            var result = await _unitOfWork.LiveChats.GetLiveChat(gjuhaID, ParentId, searchText);

            if (result.Count > 0)
                return Ok(result);
            else
                return NotFound();
        }

        //[HttpGet("GetChatBoot")]
        //public async Task<IActionResult> GetChatBoot( int? ParentId, string Gjuha = "sq", string? searchText = "")
        //{
        //    int gjuhaID = _unitOfWork.BaseConfig.GetGjuhaID(Gjuha); 

        //    var result = await _unitOfWork.LiveChats.FindByCondition(t=>t.LanguageId == gjuhaID &&
        //                                            (string.IsNullOrEmpty(searchText) ? (ParentId == null ? t.Level == 1 : t.Id == ParentId && t.Active != false) : t.Name.Contains(searchText)),
        //                                            false, new[] { "InverseLiveChatNavigation" }).ToListAsync();


        //    var listLiveChats = _mapper.Map<List<LiveChat>, List<LiveChatModel>>(result);

        //    return Ok(listLiveChats);
        //}
    }
}

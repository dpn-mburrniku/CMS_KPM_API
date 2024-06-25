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
    public class FAQController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public IMapper _mapper { get; }
        public FAQController(IUnitOfWork unitOfWorkRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWorkRepository;
            _mapper = mapper;
        }

        [HttpGet("GetFAQ")]
        public async Task<IActionResult> GetFAQ(int? PageId, string Gjuha = "sq", int GroupId = 0)
        {
            int gjuhaID = _unitOfWork.BaseConfig.GetGjuhaID(Gjuha);

            var result = await _unitOfWork.FaqHeaders.FindByCondition(t=>t.LanguageId == gjuhaID && t.PageId == PageId &&
                                                                      (GroupId > 0 ? t.Id == GroupId : true),
                                                                      true, new[] { "Faqdetails" }).FirstOrDefaultAsync();
            
            var lista = _mapper.Map<FAQModel>(result);
            lista.Faqdetails = lista.Faqdetails.OrderBy(x => x.OrderNo).ToList();
            
            return Ok(lista);
        }
    }
}

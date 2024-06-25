using AutoMapper;
using Contracts.IServices;
using Entities.DataTransferObjects;
using Entities.ErrorModel;
using Entities.RequestFeatures;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Entities.Models;

namespace CMS.API.Controllers
{



    [Route("api/[controller]")]
    [ApiController]
    public class TagController : ControllerBase
    {
        readonly List<ErrorDetails> respond = new();
        private readonly IUnitOfWork _unitOfWork;
        public IMapper _mapper { get; }

        public TagController(
                    IUnitOfWork unitOfWork
                    , IMapper mapper
                    )
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }


        #region Tags


        [Authorize]
        [HttpGet]
        [Route("GetPostTags")]
        public async Task<IActionResult> GetPostTags(int webLangId = 1)
        {
            var tagsList = await _unitOfWork.Tags.FindAll(false, null).Where(t => t.LanguageId == webLangId && t.Active == true).ToListAsync();
            var tagsDto = _mapper.Map<IEnumerable<TagsDto>?>(tagsList);
            return Ok(tagsDto);
        }

        [Authorize]
        [HttpGet("GetTagsById")]
        public async Task<IActionResult> GetTagsById(int id, int webLangId)
        {
            var data = await _unitOfWork.Tags.GetTagsById(id, webLangId);

            if (data == null)
            {
                return NotFound();
            }
            else
            {
                var tagsDto = _mapper.Map<TagsDto>(data);

                return Ok(tagsDto);
            }
        }

        [Authorize]
        [HttpPost]
        [Route("CreateTags")]
        public async Task<IActionResult> CreateTags([FromBody] TagsDto model)
        {
            if (ModelState.IsValid)
            {
                string userinId = _unitOfWork.BaseConfig.GetLoggedUserId();
                int tagsId = _unitOfWork.Tags.GetMaxPK(a => a.Id);
                var langList = await _unitOfWork.BaseConfig.GetLangList();
                model.Id = tagsId;
                if (model.WebMultiLang)
                {
                    foreach (var item in langList)
                    {
                        var multitagsEntity = _mapper.Map<PostTag>(model);
                        multitagsEntity.LanguageId = item.Id;
                        multitagsEntity.CreatedBy = userinId;
                        multitagsEntity.Created = DateTime.Now;
                        await _unitOfWork.Tags.Create(multitagsEntity);

                        await _unitOfWork.Tags.Commit();
                    }

                    return StatusCode(StatusCodes.Status201Created);
                }

                var tagsEntity = _mapper.Map<PostTag>(model);
                tagsEntity.CreatedBy = userinId;
                tagsEntity.Created = DateTime.Now;
                await _unitOfWork.Tags.Create(tagsEntity);

                await _unitOfWork.Tags.Commit();

                return StatusCode(StatusCodes.Status201Created, tagsEntity);
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        [Authorize]
        [HttpPut("UpdateTags")]
        public async Task<IActionResult> UpdateTags([FromBody] TagsDto model)
        {
            var tagsEntity = await _unitOfWork.Tags.GetTagsById(model.Id, model.LanguageId);

            if (tagsEntity == null)
            {
                return NotFound();
            }

            _mapper.Map(model, tagsEntity);
            string userinId = _unitOfWork.BaseConfig.GetLoggedUserId();
            tagsEntity.ModifiedBy = userinId;
            tagsEntity.Modified = DateTime.Now;
            _unitOfWork.Tags.Update(tagsEntity);
            await _unitOfWork.Links.Commit();

            var menuToReturn = _mapper.Map<TagsDto>(tagsEntity);

            return Ok(menuToReturn);
        }

        [Authorize]
        [HttpDelete("DeleteTag")]
        public async Task<IActionResult> DeleteTags(int id, int webLangId = 1, bool WebMultiLang = false)
        {
            var tags = await _unitOfWork.Tags.GetTagsById(id, webLangId);

            if (tags == null)
            {
                return NotFound();
            }
            var langList = await _unitOfWork.BaseConfig.GetLangList();
            if (WebMultiLang)
            {
                foreach (var item in langList)
                {
                    var multilinkEntity = await _unitOfWork.Tags.GetTagsById(id, item.Id);
                    if (multilinkEntity != null)
                    {
                        _unitOfWork.Tags.Delete(multilinkEntity);
                        await _unitOfWork.Tags.Commit();
                    }
                }
            }
            else
            {
                _unitOfWork.Tags.Delete(tags);

                await _unitOfWork.Tags.Commit();
            }
            return Ok();
        }

        [Authorize]
        [HttpPost]
        [Route("GetTagsAsync")]
        public async Task<IActionResult> GetTagsAsync([FromBody] FilterParameters parameter)
        {            
            var tags = await _unitOfWork.Tags.GetPostTagsAsync(parameter);
            var tagsDto = _mapper.Map<IEnumerable<TagsListDto>?>(tags);

            return Ok(new
            {
                data = tagsDto,
                total = tags.MetaData.TotalCount
            });
        }

        #endregion

        #region Tags in Post

        [Authorize]
        [HttpPost]
        [Route("AddTagsCollectionInPost")]
        public async Task<ActionResult> AddTagsCollectionInPost(AddTagsCollectionInPost model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var langList = await _unitOfWork.BaseConfig.GetLangList();
                    string userinId = _unitOfWork.BaseConfig.GetLoggedUserId();
                    var result = await _unitOfWork.Tags.AddTagsCollectionInPost(model, langList, userinId);
                    if (result)
                        return Ok();
                }
                else
                {
                    return BadRequest(ModelState);
                }
            }
            catch (Exception)
            {
                return BadRequest(ModelState);
            }
            return BadRequest(ModelState);
        }

        [Authorize]
        [HttpDelete]
        [Route("RemoveTagsCollectionFromPost")]
        public async Task<ActionResult> RemoveTagsCollectionFromPost([FromQuery] string TagIds, bool WebMultiLang, int webLangId, int postId)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var langList = await _unitOfWork.BaseConfig.GetLangList();
                    var numbers = TagIds.Split(',')?.Select(Int32.Parse)?.ToList();
                    if (numbers.Count > 0)
                    {
                        var result = await _unitOfWork.Tags.RemoveTagsCollectionFromPost(numbers, langList, WebMultiLang, webLangId, postId);
                        if (result)
                            return Ok();
                    }

                }
                else
                {
                    return BadRequest(ModelState);
                }
            }
            catch (Exception)
            {
                return BadRequest(ModelState);
            }
            return BadRequest(ModelState);
        }

    }



    #endregion

}
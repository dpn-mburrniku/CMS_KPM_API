using AutoMapper;
using Contracts.IServices;
using Entities.DataTransferObjects;
using Entities.ErrorModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using CMS.API.Infrastructure.ModelBinders;
using Entities.RequestFeatures;
using Entities.Models;

namespace CMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LayoutController : ControllerBase
    {
        readonly List<ErrorDetails> respond = new();
        private readonly IUnitOfWork _unitOfWork;
        public IMapper _mapper { get; }

        public LayoutController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [Authorize]
        [HttpGet]
        [Route("GetLayouts")]
        public async Task<IActionResult> GetLayouts()
        {
            var userId = _unitOfWork.BaseConfig.GetLoggedUserId();
            var userRoles = await _unitOfWork.BaseConfig.GetUserRolesId(userId);
            var layouts = await _unitOfWork.Layouts.GetLayoutsByRole(userRoles.FirstOrDefault(), false, null);

            var layoutsDto = _mapper.Map<IEnumerable<LayoutDto>?>(layouts);

            return Ok(layoutsDto);
        }

        [Authorize]
        [HttpPost]
        [Route("GetLayoutsAsync")]
        public async Task<IActionResult> GetLayoutsAsync([FromBody] FilterParameters parameter)
        {
            var layout = await _unitOfWork.Layouts.GetLayoutsAsync(parameter);

            var layoutDto = _mapper.Map<List<LayoutDto>>(layout);

            return Ok(new
            {
                data = layoutDto,
                total = layout.MetaData.TotalCount
            });            
        }

        [Authorize]
        [HttpGet]
        [Route("GetLayoutsForRoleNotIn")]
        public async Task<ActionResult> GetLayoutsForRoleNotIn(string RoleId)
        {
            var layouts = await _unitOfWork.Layouts.GetLayoutsForRoleNotIn(RoleId, false, null);

            if (layouts == null)
            {
                //_logger.LogInfo($"Company with id: {id} doesn't exist in the database.");

                return NotFound();
            }
            else
            {
                var layoutDto = _mapper.Map<List<LayoutDto>>(layouts);

                return Ok(layoutDto);
            }
        }

        [Authorize]
        [HttpGet(Name = "LayoutById")]
        //[HttpCacheExpiration(CacheLocation = CacheLocation.Public, MaxAge = 60)]
        //[HttpCacheValidation(MustRevalidate = false)]
        public async Task<IActionResult> GetLayoutAsync(int id)
        {
            var layout = await _unitOfWork.Layouts.GetById(id);

            if (layout == null)
            {
                //_logger.LogInfo($"Company with id: {id} doesn't exist in the database.");

                return NotFound();
            }
            else
            {
                var layoutDto = _mapper.Map<LayoutDto>(layout);

                return Ok(layoutDto);
            }
        }

        [Authorize]
        [HttpGet("collection/({ids})", Name = "LayoutCollection")]
        public async Task<IActionResult> GetLayoutCollectionAsync([ModelBinder(BinderType =
        typeof(ArrayModelBinder))]IEnumerable<int> ids)
        {
            if (ids == null)
            {
                //_logger.LogError("Parameter ids is null");

                return BadRequest("Parameter ids is null");
            }

            var layoutEntities = await _unitOfWork.Layouts.GetByIdsAsync(ids, trackChanges: false, null);

            if (ids.Count() != layoutEntities.Count())
            {
                //_logger.LogError("Some ids are not valid in a collection");

                return NotFound();
            }

            var layoutsToReturn = _mapper.Map<IEnumerable<LayoutDto>>(layoutEntities);

            return Ok(layoutsToReturn);
        }


        /// <param name="layout"></param>
        /// <returns>A newly created layout</returns>
        /// <response code="201">Returns the newly created item</response>
        /// <response code="400">If the item is null</response>
        /// <response code="422">If the model is invalid</response>
        /// 
        [Authorize]
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(422)]
        //[ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> CreateLayoutAsync([FromBody]AddLayout layout)
        {
            var layoutEntity = _mapper.Map<Layout>(layout);
            await _unitOfWork.Layouts.Create(layoutEntity);

            await _unitOfWork.Layouts.Commit();            

            var layoutToReturn = _mapper.Map<LayoutDto>(layoutEntity);

            return CreatedAtRoute("LayoutById", new { id = layoutToReturn.Id },
                                   layoutToReturn);
        }

        [Authorize]
        [HttpPost("collection")]
        public async Task<IActionResult> CreateLayoutCollectionAsync([FromBody]
          IEnumerable<AddLayout> layoutCollection)
        {
            if (layoutCollection == null)
            {
                //_logger.LogError("Company collection sent from client is null.");

                return BadRequest("Company collection is null");
            }

            var layoutEntities = _mapper.Map<IEnumerable<Layout>>(layoutCollection);

            foreach (var layout in layoutEntities)
            {
                await _unitOfWork.Layouts.Create(layout);
            }

            await _unitOfWork.Layouts.Commit();
            var layoutCollectionToReturn = _mapper.Map<IEnumerable<LayoutDto>>(layoutEntities);
            var ids = string.Join(",", layoutCollectionToReturn.Select(c => c.Id));

            return CreatedAtRoute("LayoutCollection", new { ids },
            layoutCollectionToReturn);
        }

        [Authorize]
        [HttpDelete("DeleteLayout")]
        public async Task<IActionResult> DeleteLayoutAsync(int id)
        {
            try
            {
                var layout = await _unitOfWork.Layouts.GetById(id);

                if (layout == null)
                {
                    //_logger.LogInfo($"Company with id: {id} doesn't exist in the database.");

                    return NotFound();
                }

                _unitOfWork.Layouts.Delete(layout);

                await _unitOfWork.Layouts.Commit();

                return NoContent();
            }

            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status406NotAcceptable, "This layout is used in another table");
            } 
        }

        [Authorize]
        [HttpPut("UpdateLayout")]
        //[ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> UpdateLayoutAsync([FromBody] UpdateLayout layout)
        {
            var layoutEntity = await _unitOfWork.Layouts.GetById(layout.Id); ;

            if (layoutEntity == null)
            {
                //_logger.LogInfo($"Company with id: {id} doesn't exist in the database.");

                return NotFound();
            }

            _mapper.Map(layout, layoutEntity);
            _unitOfWork.Layouts.Update(layoutEntity);
            await _unitOfWork.Layouts.Commit();

            var layoutsToReturn = _mapper.Map<LayoutDto>(layoutEntity);

            return Ok(layoutsToReturn);
        } 

    }
}

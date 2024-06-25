using AutoMapper;
using CMS.API.InternalServices;
using Contracts.IServices;
using Entities.DataTransferObjects;
using Entities.ErrorModel;
using Entities.Models;
using Entities.RequestFeatures;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CMS.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class MunicipalityLocationController : ControllerBase
	{
		readonly List<ErrorDetails> respond = new();
		private readonly IUnitOfWork _unitOfWork;
		public IMapper _mapper { get; }
		private IFileRepository _fileRepository;
		public MunicipalityLocationController(IUnitOfWork unitOfWork, IMapper mapper, IFileRepository fileRepository)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
			_fileRepository = fileRepository;
		}

		[Authorize]
		[HttpGet]
		[Route("GetMeasureUnits")]
		public async Task<IActionResult> GetMeasureUnits()
		{

			var measureUnits = await _unitOfWork.MeasureUnits.FindAll(false, null).ToListAsync();

			var measureUnitsDto = _mapper.Map<IEnumerable<MeasureUnitDto>?>(measureUnits);

			return Ok(measureUnitsDto);
		}

		[Authorize]
		[HttpGet]
		[Route("GetMunicipalityLocation")]
		public async Task<IActionResult> GetMunicipalityLocation()
		{
			string[] includes = { "Municipality", "MeasureUnit", "Media" };
			var GetMunicipalityLocationList = await _unitOfWork.MunicipalityLocations.FindByCondition(t => t.Active == true, false, includes).OrderBy(t => t.Municipality.NameSq).ThenBy(t => t.NameSq).ToListAsync();
			var MunicipalityLocationDto = _mapper.Map<IEnumerable<MunicipalityLocationListDto>?>(GetMunicipalityLocationList);
			return Ok(MunicipalityLocationDto);

		}


        [HttpGet]
        [Route("GetMunicipalityByLocation")]
        public async Task<IActionResult> GetMunicipalityByLocation(int municiplaityId = 0)
        {
            string[] includes = { "Municipality" };
            var GetMunicipalityLocationList = await _unitOfWork.MunicipalityLocations.FindByCondition(t => t.Active == true && (municiplaityId > 0 ? t.MunicipalityId == municiplaityId : true), false, includes).ToListAsync();
            var MunicipalityLocationDto = _mapper.Map<IEnumerable<MunicipalityLocationListDto>?>(GetMunicipalityLocationList);
            return Ok(MunicipalityLocationDto);

        }

        [Authorize]
		[HttpPost]
		[Route("GetMunicipalityLocationAsync")]
		public async Task<IActionResult> GetMunicipalityLocationAsync([FromBody] MunicipalityLocationParameters parameter)
		{

			var municipalityLocation = await _unitOfWork.MunicipalityLocations.GetMunicipalityLocationAsync(parameter);
			var municipalityLocationDto = _mapper.Map<IEnumerable<MunicipalityLocationListDto>?>(municipalityLocation);

			return Ok(new
			{
				data = municipalityLocationDto,
				total = municipalityLocation.MetaData.TotalCount
			});
		}

		[Authorize]
		[HttpGet("GetMunicipalityLocationById")]
		public async Task<IActionResult> GetMunicipalityLocationById(int id)
		{
			var data = await _unitOfWork.MunicipalityLocations.GetMunicipalityLocationById(id);

			if (data == null)
			{
				return NotFound();
			}
			else
			{
				var dataDto = _mapper.Map<getMunicipalityLocationDto>(data);

				return Ok(dataDto);
			}
		}

		[Authorize]
		[HttpPost]
		[Route("CreateMunicipalityLocation")]
		public async Task<IActionResult> CreateMunicipalityLocation([FromForm] MunicipalityLocationDto model)
		{
			if (ModelState.IsValid)
			{

				//if (model.Image == null)
				//{
				//	ModelState.AddModelError("Image", " imazhi eshte fushe obligative");
				//	return BadRequest(ModelState);
				//}

				var sysConfig = await _unitOfWork.BaseConfig.GetSysSettings();
				string userinId = _unitOfWork.BaseConfig.GetLoggedUserId();

				var MunicipalityLocationEntity = _mapper.Map<MunicipalityLocation>(model);
				MunicipalityLocationEntity.Created = DateTime.Now;
				MunicipalityLocationEntity.CreatedBy = userinId;

				if (model.Image != null)
				{
					int maxLength = int.Parse(sysConfig.FirstOrDefault(x => x.Label == "MaxFotoSize").Value);
					maxLength = maxLength * 1048576;
					if (maxLength >= model.Image.Length)
					{
						var fileEx = Path.GetExtension(model.Image.FileName).ToLower();
						string filename = await _fileRepository.AddMediaPicAsync(model.Image, sysConfig);

						Medium media = new Medium
						{
							MediaExCategoryId = 1,
							Name = model.Image.FileName.Replace(fileEx, ""),
							FileName = Guid.Parse(filename),
							FileNameMedium = filename + "_medium",
							FileNameSmall = filename + "_small",
							FileEx = fileEx,
							IsOtherSource = false,
							CreatedBy = userinId,
							Created = DateTime.Now
						};
						await _unitOfWork.Media.Create(media);
						await _unitOfWork.Media.Commit();
						if (media.Id != 0)
						{
							MunicipalityLocationEntity.MediaId = media.Id;
						}
					}
				}

				await _unitOfWork.MunicipalityLocations.Create(MunicipalityLocationEntity);

				await _unitOfWork.MunicipalityLocations.Commit();

				return StatusCode(StatusCodes.Status201Created, MunicipalityLocationEntity);
			}
			else
			{
				return BadRequest(ModelState);
			}
		}

		[Authorize]
		[HttpPut("UpdateMunicipalityLocation")]
		public async Task<IActionResult> UpdateMunicipalityLocation([FromForm] MunicipalityLocationDto model)
		{
			var municipalityLocationEntity = await _unitOfWork.MunicipalityLocations.GetMunicipalityLocationById(model.Id);
			var sysConfig = await _unitOfWork.BaseConfig.GetSysSettings();
			string userinId = _unitOfWork.BaseConfig.GetLoggedUserId();

			if (municipalityLocationEntity == null)
			{

				municipalityLocationEntity = _mapper.Map<MunicipalityLocation>(model);
				municipalityLocationEntity.CreatedBy = userinId;
				municipalityLocationEntity.Created = DateTime.Now;

				if (model.Image != null)
				{
					int maxLength = int.Parse(sysConfig.FirstOrDefault(x => x.Label == "MaxFotoSize").Value);
					maxLength = maxLength * 1048576;
					if (maxLength >= model.Image.Length)
					{
						var fileEx = Path.GetExtension(model.Image.FileName).ToLower();
						string filename = await _fileRepository.AddMediaPicAsync(model.Image, sysConfig);

						Medium media = new Medium
						{
							MediaExCategoryId = 1,
							Name = model.Image.FileName.Replace(fileEx, ""),
							FileName = Guid.Parse(filename),
							FileNameMedium = filename + "_medium",
							FileNameSmall = filename + "_small",
							FileEx = fileEx,
							IsOtherSource = false,
							CreatedBy = userinId,
							Created = DateTime.Now
						};
						await _unitOfWork.Media.Create(media);
						await _unitOfWork.Media.Commit();
						municipalityLocationEntity.MediaId = media.Id;
					}
				}
				else
				{
					municipalityLocationEntity.MediaId = model.MediaId;
				}
				await _unitOfWork.MunicipalityLocations.Create(municipalityLocationEntity);
			}
			else
			{

				_mapper.Map(model, municipalityLocationEntity);

				municipalityLocationEntity.ModifiedBy = userinId;
				municipalityLocationEntity.Modified = DateTime.Now;

				#region Foto
				if (model.Image != null)
				{
					int maxLength = int.Parse(sysConfig.FirstOrDefault(x => x.Label == "MaxFotoSize").Value);
					maxLength = maxLength * 1048576;
					if (maxLength >= model.Image.Length)
					{
						var fileEx = Path.GetExtension(model.Image.FileName).ToLower();
						string filename = await _fileRepository.AddMediaPicAsync(model.Image, sysConfig);

						Medium media = new Medium
						{
							MediaExCategoryId = 1,
							Name = model.Image.FileName.Replace(fileEx, ""),
							FileName = Guid.Parse(filename),
							FileNameMedium = filename + "_medium",
							FileNameSmall = filename + "_small",
							FileEx = fileEx,
							IsOtherSource = false,
							CreatedBy = userinId,
							Created = DateTime.Now
						};
						await _unitOfWork.Media.Create(media);
						await _unitOfWork.Media.Commit();
						municipalityLocationEntity.MediaId = media.Id;
					}
				}
				#endregion

				_unitOfWork.MunicipalityLocations.Update(municipalityLocationEntity);

			}
			await _unitOfWork.MunicipalityLocations.Commit();

			var menuToReturn = _mapper.Map<MunicipalityLocationDto>(municipalityLocationEntity);
			return Ok(menuToReturn);

		}

		[Authorize]
		[HttpDelete("DeleteMunicipalityLocation")]
		public async Task<IActionResult> DeleteMunicipalityLocation(int id)
		{
			var municipalityLocationId = await _unitOfWork.MunicipalityLocations.GetMunicipalityLocationById(id);

			if (municipalityLocationId == null)
			{
				return NotFound();
			}

			_unitOfWork.MunicipalityLocations.Delete(municipalityLocationId);

			await _unitOfWork.MunicipalityLocations.Commit();

			return Ok(new { status = true });
		}

	}
}

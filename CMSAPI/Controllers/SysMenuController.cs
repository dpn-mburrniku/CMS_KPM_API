using AutoMapper;
using Contracts.IServices;
using Entities.DataTransferObjects;
using Entities.ErrorModel;
using Entities.RequestFeatures;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Entities.Models;

namespace CMS.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class SysMenuController : ControllerBase
	{
		readonly List<ErrorDetails> respond = new();
		private readonly IUnitOfWork _unitOfWork;
		public IMapper _mapper { get; }

		public SysMenuController(
								IUnitOfWork unitOfWork
								, IMapper mapper
								)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
		}

		[Authorize]
		[HttpGet]
		[Route("GetAllMenu")]
		public async Task<IActionResult> GetAllMenu()
		{
			string[] includes = { "Parent" };
			var MenuList = await _unitOfWork.SysMenus.FindAll(false, includes).Where(t => t.Active == true).ToListAsync();
			return Ok(MenuList);
		}

		[Authorize]
		[HttpPost]
		[Route("GetAllMenuAsync")]
		public async Task<IActionResult> GetAllMenuAsync([FromBody] FilterParameters parameter)
		{

			var menu = await _unitOfWork.SysMenus.GetAllMenuAsync(parameter);
			var menuDto = _mapper.Map<IEnumerable<SysMenuListDto>?>(menu);

			return Ok(new
			{
				data = menuDto,
				total = menu.MetaData.TotalCount
			});
		}

		[Authorize]
		[HttpGet]
		[Route("GetAllParentMenu")]
		public async Task<IActionResult> GetAllParentMenu()
		{
			var MenuList = _unitOfWork.SysMenus.FindByCondition(x => x.Type != 3 && x.Active == true, false, null).ToList();
			return Ok(MenuList);
		}

		[Authorize]
		[HttpGet("GetMenuById", Name = "MenuById")]
		public async Task<IActionResult> GetMenuById(int id)
		{
			var menu = await _unitOfWork.SysMenus.GetById(id);

			if (menu == null)
			{
				return NotFound();
			}
			else
			{
				var menuDto = _mapper.Map<SysMenu>(menu);

				return Ok(menuDto);
			}
		}

		[Authorize]
		[HttpPost]
		[Route("CreateMenu")]
		public async Task<IActionResult> CreateMenu([FromBody] AddMenuDto menu)
		{
			if (ModelState.IsValid)
			{
				string userinId = _unitOfWork.BaseConfig.GetLoggedUserId();
				var menuEntity = _mapper.Map<SysMenu>(menu);
				menuEntity.CreatedBy = userinId;
				menuEntity.Created = DateTime.Now;
				await _unitOfWork.SysMenus.Create(menuEntity);

				await _unitOfWork.SysMenus.Commit();

				return StatusCode(StatusCodes.Status201Created, menuEntity);
			}
			else
			{
				return BadRequest(ModelState);
			}
		}

		[Authorize]
		[HttpPut("UpdateMenu")]
		public async Task<IActionResult> UpdateMenu([FromBody] AddMenuDto menu)
		{
			var menuEntity = await _unitOfWork.SysMenus.GetById(menu.Id);

			if (menuEntity == null)
			{
				return NotFound();
			}

			_mapper.Map(menu, menuEntity);
			string userinId = _unitOfWork.BaseConfig.GetLoggedUserId();
			menuEntity.ModifiedBy = userinId;
			menuEntity.Modified = DateTime.Now;
			_unitOfWork.SysMenus.Update(menuEntity);
			await _unitOfWork.SysMenus.Commit();

			var menuToReturn = _mapper.Map<SysMenu>(menuEntity);

			return Ok(menuToReturn);
		}


		[Authorize]
		[HttpDelete("DeleteMenu")]
		public async Task<IActionResult> DeleteMenu(int id)
		{
			var menu = await _unitOfWork.SysMenus.GetById(id);

			if (menu == null)
			{
				return NotFound();
			}

			_unitOfWork.SysMenus.Delete(menu);

			await _unitOfWork.SysMenus.Commit();

			return Ok();
		}

		[Authorize]
		[HttpPost]
		[Route("OrderMenu")]
		public async Task<ActionResult> OrderMenu(List<UpdateSysMenuListDto> model)
		{
			if (model.Count > 0)
			{
				foreach (var item in model)
				{
					var menu = await _unitOfWork.SysMenus.GetById(item.Id);
					if (menu != null)
					{
						menu.OrderNo = item.OrderNo;
						_unitOfWork.SysMenus.Update(menu);
						await _unitOfWork.SysMenus.Commit();
					}
				}
				return Ok();
			}

			return NotFound();
		}
	}
}

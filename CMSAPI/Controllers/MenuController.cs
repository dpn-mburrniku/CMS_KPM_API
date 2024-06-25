using AutoMapper;
using Contracts.IServices;
using Entities.DataTransferObjects;
using Entities.ErrorModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Entities.Models;

namespace CMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenuController : ControllerBase
    {
        readonly List<ErrorDetails> respond = new();
        private readonly IUnitOfWork _unitOfWork;
        public IMapper _mapper { get; }

        public MenuController(
                    IUnitOfWork unitOfWork
                    , IMapper mapper
                    )
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        #region Menu

        [Authorize]
        [HttpGet]
        [Route("GetAllWebMenu")]
        public async Task<IActionResult> GetAllWebMenu(int menuTypeId, int webLangId = 1)
        {
            string[] includes = { "Page",
                                  "Page1",
                                  "InverseMenuNavigation",
                                  "InverseMenuNavigation.Page",
                                  "InverseMenuNavigation.Page1",
                                  "InverseMenuNavigation.InverseMenuNavigation",
                                  "InverseMenuNavigation.InverseMenuNavigation.Page",
                                  "InverseMenuNavigation.InverseMenuNavigation.Page1",
                                  "InverseMenuNavigation.InverseMenuNavigation.InverseMenuNavigation",
                                  "InverseMenuNavigation.InverseMenuNavigation.InverseMenuNavigation.Page",
                                  "InverseMenuNavigation.InverseMenuNavigation.InverseMenuNavigation.Page1",
                                  "InverseMenuNavigation.InverseMenuNavigation.InverseMenuNavigation.InverseMenuNavigation",
                                  "InverseMenuNavigation.InverseMenuNavigation.InverseMenuNavigation.InverseMenuNavigation.Page",
                                  "InverseMenuNavigation.InverseMenuNavigation.InverseMenuNavigation.InverseMenuNavigation.Page1"
                                };
            var MenuList = await _unitOfWork.Menu.FindAll(false, includes).Where(x => x.LanguageId == webLangId && x.MenuTypeId == menuTypeId && x.MenuParentId == null).OrderBy(t => t.OrderNo).ToListAsync();
            var MenuListDto = _mapper.Map<List<MenuListDto>>(MenuList);
            foreach (var item in MenuListDto)
            {
                item.Children = item.Children != null ? item.Children.OrderBy(t => t.OrderNo).ToList() : null;

                foreach (var item2 in item.Children)
                {
                    item2.Children = item2.Children != null ? item2.Children.OrderBy(t => t.OrderNo).ToList() : null;

                    foreach (var item3 in item2.Children)
                    {
                        item3.Children = item3.Children != null ? item3.Children.OrderBy(t => t.OrderNo).ToList() : null;

                        foreach (var item4 in item3.Children)
                        {
                            item4.Children = item4.Children != null ? item4.Children.OrderBy(t => t.OrderNo).ToList() : null;
                        }
                    }
                }
            }
            return Ok(MenuListDto);
        }

        [Authorize]
        [HttpPost]
        [Route("AddCollectionPagesInMenu")]
        public async Task<IActionResult> AddCollectionPagesInMenu([FromBody] AddCollectionMenu model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string userId = _unitOfWork.BaseConfig.GetLoggedUserId();
                    var result = await _unitOfWork.Menu.AddCollectionPagesInMenu(model, userId);
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
        [HttpGet("GetMenuById")]
        public async Task<IActionResult> GetMenuById(int id, int webLangId)
        {
            var data = await _unitOfWork.Menu.FindByCondition(t => t.Id == id && t.LanguageId == webLangId, false, null).FirstOrDefaultAsync();

            if (data == null)
            {
                return NotFound();
            }
            else
            {
                var dataDto = _mapper.Map<MenuDto>(data);

                return Ok(dataDto);
            }
        }


        [Authorize]
        [HttpPut("UpdateMenu")]
        public async Task<IActionResult> UpdateMenu([FromBody] UpdateMenuDto model)
        {
            var menuEntity = await _unitOfWork.Menu.GetMenuById(model.Id, model.LanguageId);

            if (menuEntity == null)
            {
                return NotFound();
            }

            _mapper.Map(model, menuEntity);
            string userinId = _unitOfWork.BaseConfig.GetLoggedUserId();
            menuEntity.ModifiedBy = userinId;
            menuEntity.Modified = DateTime.Now;

            _unitOfWork.Menu.Update(menuEntity);
            await _unitOfWork.Menu.Commit();

            var menuToReturn = _mapper.Map<UpdateMenuDto>(menuEntity);

            return Ok(menuToReturn);

        }


        [Authorize]
        [HttpPut("UpdateOrderMenu")]
        public async Task<IActionResult> UpdateOrderMenu([FromBody] List<MenuListDto> model)
        {
            int orderNr = 0;
            foreach (var item in model)
            {
                var menuEntity = await _unitOfWork.Menu.GetMenuById(item.Id, item.LanguageId);

                orderNr = orderNr + 1;
                menuEntity.OrderNo = orderNr;
                menuEntity.Level = 1;
                menuEntity.MenuParentId = null;

                _unitOfWork.Menu.Update(menuEntity);

                int orderNr2 = 0;
                foreach (var item2 in item.Children)
                {
                    var menuEntity2 = await _unitOfWork.Menu.GetMenuById(item2.Id, item2.LanguageId);

                    orderNr2 = orderNr2 + 1;
                    menuEntity2.OrderNo = orderNr2;
                    menuEntity2.Level = 2;
                    menuEntity2.MenuParentId = item.Id;

                    _unitOfWork.Menu.Update(menuEntity2);

                    int orderNr3 = 0;
                    foreach (var item3 in item2.Children)
                    {
                        var menuEntity3 = await _unitOfWork.Menu.GetMenuById(item3.Id, item3.LanguageId);

                        orderNr3 = orderNr3 + 1;
                        menuEntity3.OrderNo = orderNr3;
                        menuEntity3.Level = 3;
                        menuEntity3.MenuParentId = item2.Id;

                        _unitOfWork.Menu.Update(menuEntity3);

                        int orderNr4 = 0;
                        foreach (var item4 in item3.Children)
                        {
                            var menuEntity4 = await _unitOfWork.Menu.GetMenuById(item4.Id, item4.LanguageId);

                            orderNr4 = orderNr4 + 1;
                            menuEntity4.OrderNo = orderNr4;
                            menuEntity4.Level = 4;
                            menuEntity4.MenuParentId = item3.Id;

                            _unitOfWork.Menu.Update(menuEntity4);

                            int orderNr5 = 0;
                            foreach (var item5 in item4.Children)
                            {
                                var menuEntity5 = await _unitOfWork.Menu.GetMenuById(item5.Id, item5.LanguageId);

                                orderNr5 = orderNr5 + 1;
                                menuEntity5.OrderNo = orderNr5;
                                menuEntity5.Level = 5;
                                menuEntity5.MenuParentId = item4.Id;

                                _unitOfWork.Menu.Update(menuEntity5);
                            }
                        }
                    }
                }
            }
            await _unitOfWork.Menu.Commit();

            return Ok();

        }


        [Authorize]
        [HttpDelete("DeleteMenu")]
        public async Task<IActionResult> DeleteMenu(int Id, int webLangId, bool WebMultiLang = false)
        {
            var menu = await _unitOfWork.Menu.GetMenuById(Id, webLangId);

            if (menu == null)
            {
                return NotFound();
            }
            var langList = await _unitOfWork.BaseConfig.GetLangList();
            if (WebMultiLang)
            {
                foreach (var item in langList)
                {
                    var menuEntity = await _unitOfWork.Menu.GetMenuById(Id, item.Id);
                    if (menuEntity != null)
                    {
                        _unitOfWork.Menu.Delete(menuEntity);

                    }

                }
                await _unitOfWork.Menu.Commit();

            }
            else
            {
                _unitOfWork.Menu.Delete(menu);
                await _unitOfWork.Menu.Commit();
            }

            return Ok();
        }


        [Authorize]
        [HttpPost]
        [Route("UpdateMenuOrder")]
        public async Task<ActionResult> UpdateMenuOrder(List<UpdatMenuOrderDto> model)
        {
            if (model.Count > 0)
            {
                foreach (var item in model)
                {
                    var menuList =  _unitOfWork.Menu.FindByCondition(x=>x.Id == item.Id, false, null);
                    foreach(var menu in menuList)
                    if (menu != null)
                    {
                        menu.OrderNo = item.OrderNo;
                        menu.Level = item.Level;
                        menu.MenuParentId = item.MenuParentId > 0 ? item.MenuParentId : null;
                        _unitOfWork.Menu.Update(menu);                        
                    }
                    await _unitOfWork.Menu.Commit();
                }
                return Ok();
            }

            return NotFound();
        }

        #endregion

        #region MenuOtherSource

        [Authorize]
        [HttpPost]
        [Route("CreateMenuOtherSource")]
        public async Task<IActionResult> CreateMenuOtherSource([FromBody] MenuOtherSourceDto model)
        {
            if (ModelState.IsValid)
            {
                string userinId = _unitOfWork.BaseConfig.GetLoggedUserId();
                int menuId = _unitOfWork.Menu.GetMaxPK(i => i.Id);
                var langList = await _unitOfWork.BaseConfig.GetLangList();
                model.Id = menuId;
               
                foreach (var item in langList)
                {
                    var MenuOtherSourceEntity = _mapper.Map<Menu>(model);
                    MenuOtherSourceEntity.IsOtherSource = true;
                    MenuOtherSourceEntity.LanguageId = item.Id;
                    MenuOtherSourceEntity.IsClickable = true;
                    MenuOtherSourceEntity.Active = true;
                    MenuOtherSourceEntity.IsMegaMenu = false;
                    MenuOtherSourceEntity.CreatedBy = userinId;
                    MenuOtherSourceEntity.Created = DateTime.Now;
                    await _unitOfWork.Menu.Create(MenuOtherSourceEntity);

                    await _unitOfWork.Menu.Commit();
                }

                return StatusCode(StatusCodes.Status201Created);    
            }
            else
            {
                return BadRequest(ModelState);
            }
        }


        #endregion

        #region MenuType

        [Authorize]
        [HttpGet]
        [Route("GetMenuType")]
        public async Task<IActionResult> GetMenuType(int layoutId, int webLangId = 1)
        {
            string[] includes = { "Layout" };
            var MenuTypeList = await _unitOfWork.MenuType.FindAll(false, includes).Where(x => x.LanguageId == webLangId && x.LayoutId == layoutId).ToListAsync();
            var dataDto = _mapper.Map<List<MenuTypeDto>>(MenuTypeList);
            return Ok(dataDto);
        }


        [Authorize]
        [HttpGet("GetMenuTypeById")]
        public async Task<IActionResult> GetMenuTypeById(int id, int webLangId)
        {
            var data = await _unitOfWork.MenuType.FindByCondition(t => t.Id == id && t.LanguageId == webLangId, false, null).FirstOrDefaultAsync();

            if (data == null)
            {
                return NotFound();
            }
            else
            {
                var dataDto = _mapper.Map<MenuTypeDto>(data);

                return Ok(dataDto);
            }
        }

        [Authorize]
        [HttpPost]
        [Route("CreateMenuType")]
        public async Task<IActionResult> CreateMenuType([FromBody] MenuTypeDto model)
        {
            if (ModelState.IsValid)
            {
                string userinId = _unitOfWork.BaseConfig.GetLoggedUserId();
                int menuTypeId = _unitOfWork.MenuType.GetMaxPK(i => i.Id);
                var langList = await _unitOfWork.BaseConfig.GetLangList();
                model.Id = menuTypeId;
                if (model.WebMultiLang)
                {
                    foreach (var item in langList)
                    {
                        var multiMenuTypeEntity = _mapper.Map<MenuType>(model);
                        multiMenuTypeEntity.LanguageId = item.Id;
                        multiMenuTypeEntity.CreatedBy = userinId;
                        multiMenuTypeEntity.Created = DateTime.Now;
                        await _unitOfWork.MenuType.Create(multiMenuTypeEntity);
                         
                        await _unitOfWork.MenuType.Commit();
                    }

                    return StatusCode(StatusCodes.Status201Created, new { id = menuTypeId, });
                }

                var MenuTypeEntity = _mapper.Map<MenuType>(model);
                MenuTypeEntity.CreatedBy = userinId;
                MenuTypeEntity.Created = DateTime.Now;
                await _unitOfWork.MenuType.Create(MenuTypeEntity);

                await _unitOfWork.MenuType.Commit();

                return StatusCode(StatusCodes.Status201Created, MenuTypeEntity);
               
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        [Authorize]
        [HttpPut("UpdateMenuType")]
        public async Task<IActionResult> UpdateMenuType([FromBody] UpdateMenuTypeDto model)
        {
            var menuTypeEntity = await _unitOfWork.MenuType.GetMenuTypeById(model.Id, model.LanguageId);

            if (menuTypeEntity == null)
            {
                return NotFound();
            }

            _mapper.Map(model, menuTypeEntity);
            string userinId = _unitOfWork.BaseConfig.GetLoggedUserId();
            menuTypeEntity.ModifiedBy = userinId;
            menuTypeEntity.Modified = DateTime.Now;
            _unitOfWork.MenuType.Update(menuTypeEntity);
            await _unitOfWork.MenuType.Commit();

            var menuTypeToReturn = _mapper.Map<UpdateMenuTypeDto>(menuTypeEntity);

            return Ok(menuTypeToReturn);
        }

        [Authorize]
        [HttpDelete("DeleteMenuType")]
        public async Task<IActionResult> DeleteMenuType(int Id, int webLangId, bool WebMultiLang = false)
        {
            var menuType = await _unitOfWork.MenuType.GetMenuTypeById(Id, webLangId);

            if (menuType == null) 
            {
                return NotFound();
            }
            var langList = await _unitOfWork.BaseConfig.GetLangList();
            if (WebMultiLang)
            {
                foreach (var item in langList)
                {
                    var menuTypeEntity = await _unitOfWork.MenuType.GetMenuTypeById(Id, item.Id);
                    if (menuTypeEntity != null)
                    {
                        _unitOfWork.MenuType.Delete(menuTypeEntity);
                    }
                }
                await _unitOfWork.MenuType.Commit();
            }
            else
            {
                _unitOfWork.MenuType.Delete(menuType);
                await _unitOfWork.MenuType.Commit();
            }

            return Ok();
        }

        #endregion
    }
}

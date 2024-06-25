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
    public class LiveChatController : ControllerBase
    {
        readonly List<ErrorDetails> respond = new();
        private readonly IUnitOfWork _unitOfWork;
        public IMapper _mapper { get; }

        public LiveChatController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        

        [Authorize]
        [HttpGet]
        [Route("GetLiveChat")]
        public async Task<IActionResult> GetLiveChat(int webLangId = 1)
        {
            string[] includes = { "Page", 
                                  "InverseLiveChatNavigation",
                                  "InverseLiveChatNavigation.Page",
                                  "InverseLiveChatNavigation.InverseLiveChatNavigation",
                                  "InverseLiveChatNavigation.InverseLiveChatNavigation.Page",
                                  "InverseLiveChatNavigation.InverseLiveChatNavigation.InverseLiveChatNavigation",
                                  "InverseLiveChatNavigation.InverseLiveChatNavigation.InverseLiveChatNavigation.Page",
                                  "InverseLiveChatNavigation.InverseLiveChatNavigation.InverseLiveChatNavigation.InverseLiveChatNavigation",
                                  "InverseLiveChatNavigation.InverseLiveChatNavigation.InverseLiveChatNavigation.InverseLiveChatNavigation.Page",
                                  "InverseLiveChatNavigation.InverseLiveChatNavigation.InverseLiveChatNavigation.InverseLiveChatNavigation.InverseLiveChatNavigation",
                                  "InverseLiveChatNavigation.InverseLiveChatNavigation.InverseLiveChatNavigation.InverseLiveChatNavigation.InverseLiveChatNavigation.Page",
                                  "InverseLiveChatNavigation.InverseLiveChatNavigation.InverseLiveChatNavigation.InverseLiveChatNavigation.InverseLiveChatNavigation.InverseLiveChatNavigation",
                                  "InverseLiveChatNavigation.InverseLiveChatNavigation.InverseLiveChatNavigation.InverseLiveChatNavigation.InverseLiveChatNavigation.InverseLiveChatNavigation.Page",
                                  "InverseLiveChatNavigation.InverseLiveChatNavigation.InverseLiveChatNavigation.InverseLiveChatNavigation.InverseLiveChatNavigation.InverseLiveChatNavigation.InverseLiveChatNavigation",
                                  "InverseLiveChatNavigation.InverseLiveChatNavigation.InverseLiveChatNavigation.InverseLiveChatNavigation.InverseLiveChatNavigation.InverseLiveChatNavigation.InverseLiveChatNavigation.Page",
                                  "InverseLiveChatNavigation.InverseLiveChatNavigation.InverseLiveChatNavigation.InverseLiveChatNavigation.InverseLiveChatNavigation.InverseLiveChatNavigation.InverseLiveChatNavigation.InverseLiveChatNavigation",
                                  "InverseLiveChatNavigation.InverseLiveChatNavigation.InverseLiveChatNavigation.InverseLiveChatNavigation.InverseLiveChatNavigation.InverseLiveChatNavigation.InverseLiveChatNavigation.InverseLiveChatNavigation.Page",
                                  "InverseLiveChatNavigation.InverseLiveChatNavigation.InverseLiveChatNavigation.InverseLiveChatNavigation.InverseLiveChatNavigation.InverseLiveChatNavigation.InverseLiveChatNavigation.InverseLiveChatNavigation.InverseLiveChatNavigation",
                                  "InverseLiveChatNavigation.InverseLiveChatNavigation.InverseLiveChatNavigation.InverseLiveChatNavigation.InverseLiveChatNavigation.InverseLiveChatNavigation.InverseLiveChatNavigation.InverseLiveChatNavigation.InverseLiveChatNavigation.Page"
                                };

            var liveChatList = await _unitOfWork.LiveChats.FindAll(false, includes).Where(t => t.LanguageId == webLangId && t.ParentId == null).OrderBy(t => t.OrderNo).ToListAsync();
                                                                                                                                                                        
            var livechatsDto = _mapper.Map<List<LiveChatListDto>>(liveChatList);

            foreach (var item in livechatsDto)
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

                            foreach (var item5 in item4.Children)
                            {
                                item5.Children = item5.Children != null ? item5.Children.OrderBy(t => t.OrderNo).ToList() : null;

                                foreach (var item6 in item5.Children)
                                {
                                    item6.Children = item6.Children != null ? item6.Children.OrderBy(t => t.OrderNo).ToList() : null;

                                    foreach (var item7 in item6.Children)
                                    {
                                        item7.Children = item7.Children != null ? item7.Children.OrderBy(t => t.OrderNo).ToList() : null;

                                        foreach (var item8 in item7.Children)
                                        {
                                            item8.Children = item8.Children != null ? item8.Children.OrderBy(t => t.OrderNo).ToList() : null;

                                            foreach (var item9 in item8.Children)
                                            {
                                                item9.Children = item9.Children != null ? item9.Children.OrderBy(t => t.OrderNo).ToList() : null;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }


            return Ok(livechatsDto);
        }

        [Authorize]
        [HttpPost]
        [Route("GetLiveChatsAsync")]
        public async Task<IActionResult> GetLiveChatsAsync([FromBody] LiveChatFilterParameters parameter)
        {
            var liveChat = await _unitOfWork.LiveChats.GetLiveChatAsync(parameter);
            var liveChatsDto = _mapper.Map<IEnumerable<LiveChatListDto>?>(liveChat);

            return Ok(new
            {
                data = liveChatsDto,
                total = liveChat.MetaData.TotalCount
            });
        }

        [Authorize]
        [HttpPost]
        [Route("CreateLiveChat")]
        public async Task<IActionResult> CreateLiveChat([FromBody] LiveChatDto model)
        {
            if (ModelState.IsValid)
            {
                string userinId = _unitOfWork.BaseConfig.GetLoggedUserId();
                int LiveChatId = _unitOfWork.LiveChats.GetMaxPK(i => i.Id);
                var langList = await _unitOfWork.BaseConfig.GetLangList();
                model.Id = LiveChatId;
                if (model.WebMultiLang)
                {
                    foreach (var item in langList)
                    {
                        var multiLiveChatEntity = _mapper.Map<LiveChat>(model);
                        multiLiveChatEntity.LanguageId = item.Id;
                        multiLiveChatEntity.Level = 1;
                        multiLiveChatEntity.CreatedBy = userinId;
                        multiLiveChatEntity.Created = DateTime.Now;
                        await _unitOfWork.LiveChats.Create(multiLiveChatEntity);

                        await _unitOfWork.LiveChats.Commit();
                    }

                    return StatusCode(StatusCodes.Status201Created);
                }

                var LiveChatEntity = _mapper.Map<LiveChat>(model);
                LiveChatEntity.CreatedBy = userinId;
                LiveChatEntity.Level = 1;
                LiveChatEntity.Created = DateTime.Now;
                await _unitOfWork.LiveChats.Create(LiveChatEntity);

                await _unitOfWork.LiveChats.Commit();

                return StatusCode(StatusCodes.Status201Created, LiveChatEntity);
            }
            else
            {
                return BadRequest(ModelState);
            }      
        }

        [Authorize]
        [HttpGet("GetLiveChatById")]
        public async Task<IActionResult> GetLiveChatById(int id, int webLangId)
        {
            var data = await _unitOfWork.LiveChats.GetLiveChatById(id, webLangId);

            if (data == null)
            {
                return NotFound();
            }
            else
            {
                var dataDto = _mapper.Map<LiveChatDto>(data);

                return Ok(dataDto);
            }
        }

        [Authorize]
        [HttpPut("UpdateLiveChat")]
        public async Task<IActionResult> UpdateLiveChat([FromBody] UpdateLiveChatDto model)
        {
            var liveEntity = await _unitOfWork.LiveChats.GetLiveChatById(model.Id, model.LanguageId);

            if (liveEntity == null)
            {
                return NotFound();
            }

            _mapper.Map(model, liveEntity);
            string userinId = _unitOfWork.BaseConfig.GetLoggedUserId();
            liveEntity.ModifiedBy = userinId;
            liveEntity.Modified = DateTime.Now;
            
            _unitOfWork.LiveChats.Update(liveEntity);
            await _unitOfWork.LiveChats.Commit();

            var menuToReturn = _mapper.Map<UpdateLiveChatDto>(liveEntity);
            return Ok(menuToReturn);

        }

        [Authorize]
        [HttpPut("UpdateOrderLiveChat")]
        public async Task<IActionResult> UpdateOrderLiveChat([FromBody] List<LiveChatListDto> model)
        {
            int orderNr = 0;
            foreach (var item in model)
            {
                var liveEntity = await _unitOfWork.LiveChats.GetLiveChatById(item.Id, item.LanguageId);

                if (liveEntity == null)
                {
                    continue;
                }

                orderNr = orderNr + 1;
                liveEntity.OrderNo = orderNr;
                liveEntity.Level = 1;
                liveEntity.ParentId = null;

                _unitOfWork.LiveChats.Update(liveEntity);

                int orderNr1 = 0;
                foreach (var item1 in item.Children)
                {
                    var liveEntitychild = await _unitOfWork.LiveChats.GetLiveChatById(item1.Id, item1.LanguageId);

                    if (liveEntitychild == null)
                    {
                        continue;
                    }

                    orderNr1 = orderNr1 + 1;
                    liveEntitychild.OrderNo = orderNr1;
                    liveEntitychild.Level = 2;
                    liveEntitychild.ParentId = item.Id;

                    _unitOfWork.LiveChats.Update(liveEntitychild);

                    int orderNr2 = 0;
                    foreach (var item2 in item1.Children)
                    {
                        var liveEntitychild2 = await _unitOfWork.LiveChats.GetLiveChatById(item2.Id, item2.LanguageId);

                        if (liveEntitychild2 == null)
                        {
                            continue;
                        }

                        orderNr2 = orderNr2 + 1;
                        liveEntitychild2.OrderNo = orderNr2;
                        liveEntitychild2.Level = 3;
                        liveEntitychild2.ParentId = item1.Id;

                        _unitOfWork.LiveChats.Update(liveEntitychild2);

                        int orderNr3 = 0;
                        foreach (var item3 in item2.Children)
                        {
                            var liveEntitychild3 = await _unitOfWork.LiveChats.GetLiveChatById(item3.Id, item3.LanguageId);

                            if (liveEntitychild3 == null)
                            {
                                continue;
                            }

                            orderNr3 = orderNr3 + 1;
                            liveEntitychild3.OrderNo = orderNr3;
                            liveEntitychild3.Level = 4;
                            liveEntitychild3.ParentId = item2.Id;

                            _unitOfWork.LiveChats.Update(liveEntitychild3);

                            int orderNr4 = 0;
                            foreach (var item4 in item3.Children)
                            {
                                var liveEntitychild4 = await _unitOfWork.LiveChats.GetLiveChatById(item4.Id, item4.LanguageId);

                                if (liveEntitychild4 == null)
                                {
                                    continue;
                                }

                                orderNr4 = orderNr4 + 1;
                                liveEntitychild4.OrderNo = orderNr4;
                                liveEntitychild4.Level = 5;
                                liveEntitychild4.ParentId = item3.Id;

                                _unitOfWork.LiveChats.Update(liveEntitychild4);

                                int orderNr5 = 0;
                                foreach (var item5 in item4.Children)
                                {
                                    var liveEntitychild5 = await _unitOfWork.LiveChats.GetLiveChatById(item5.Id, item5.LanguageId);

                                    if (liveEntitychild5 == null)
                                    {
                                        continue;
                                    }

                                    orderNr5 = orderNr5 + 1;
                                    liveEntitychild5.OrderNo = orderNr5;
                                    liveEntitychild5.Level = 6;
                                    liveEntitychild5.ParentId = item4.Id;

                                    _unitOfWork.LiveChats.Update(liveEntitychild5);

                                    int orderNr6 = 0;
                                    foreach (var item6 in item5.Children)
                                    {
                                        var liveEntitychild6 = await _unitOfWork.LiveChats.GetLiveChatById(item6.Id, item6.LanguageId);

                                        if (liveEntitychild6 == null)
                                        {
                                            continue;
                                        }

                                        orderNr6 = orderNr6 + 1;
                                        liveEntitychild6.OrderNo = orderNr6;
                                        liveEntitychild6.Level = 7;
                                        liveEntitychild6.ParentId = item5.Id;

                                        _unitOfWork.LiveChats.Update(liveEntitychild6);

                                        int orderNr7 = 0;
                                        foreach (var item7 in item6.Children)
                                        {
                                            var liveEntitychild7 = await _unitOfWork.LiveChats.GetLiveChatById(item7.Id, item7.LanguageId);

                                            if (liveEntitychild7 == null)
                                            {
                                                continue;
                                            }

                                            orderNr7 = orderNr7 + 1;
                                            liveEntitychild7.OrderNo = orderNr7;
                                            liveEntitychild7.Level = 8;
                                            liveEntitychild7.ParentId = item6.Id;

                                            _unitOfWork.LiveChats.Update(liveEntitychild7);

                                            int orderNr8 = 0;
                                            foreach (var item8 in item7.Children)
                                            {
                                                var liveEntitychild8 = await _unitOfWork.LiveChats.GetLiveChatById(item8.Id, item8.LanguageId);

                                                if (liveEntitychild8 == null)
                                                {
                                                    continue;
                                                }

                                                orderNr8 = orderNr8 + 1;
                                                liveEntitychild8.OrderNo = orderNr8;
                                                liveEntitychild8.Level = 9;
                                                liveEntitychild8.ParentId = item7.Id;

                                                _unitOfWork.LiveChats.Update(liveEntitychild8);

                                                int orderNr9 = 0;
                                                foreach (var item9 in item8.Children)
                                                {
                                                    var liveEntitychild9 = await _unitOfWork.LiveChats.GetLiveChatById(item9.Id, item9.LanguageId);

                                                    if (liveEntitychild9 == null)
                                                    {
                                                        continue;
                                                    }

                                                    orderNr9 = orderNr9 + 1;
                                                    liveEntitychild9.OrderNo = orderNr9;
                                                    liveEntitychild9.Level = 10;
                                                    liveEntitychild9.ParentId = item8.Id;

                                                    _unitOfWork.LiveChats.Update(liveEntitychild9);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            await _unitOfWork.LiveChats.Commit();
            return Ok();
        }

        [Authorize]
        [HttpDelete("DeleteLiveChat")]
        public async Task<IActionResult> DeleteLiveChat(int id, int webLangId = 1, bool WebMultiLang = false)
        {
            var live = await _unitOfWork.LiveChats.GetLiveChatById(id, webLangId);

            if (live == null)
            {
                return NotFound();
            }
            
            var langList = await _unitOfWork.BaseConfig.GetLangList();
            if (WebMultiLang)
            {
                foreach (var item in langList)
                {
                    var multilinkEntity = await _unitOfWork.LiveChats.GetLiveChatById(id, item.Id);
                    if (multilinkEntity != null)
                    {
                        _unitOfWork.LiveChats.Delete(multilinkEntity);
                        await _unitOfWork.LiveChats.Commit();
                    }
                }

                return Ok();
            }
            
            _unitOfWork.LiveChats.Delete(live);

            await _unitOfWork.LiveChats.Commit();

            return Ok();
        }
    }
}

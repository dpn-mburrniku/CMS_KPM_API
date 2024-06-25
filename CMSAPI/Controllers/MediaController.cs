using AutoMapper;
using CMS.API.InternalServices;
using Contracts.IServices;
using Entities.DataTransferObjects;
using Entities.ErrorModel;
using Entities.RequestFeatures;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Entities.Models;

namespace CMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MediaController : ControllerBase
    {
        readonly List<ErrorDetails> respond = new();
        private readonly IUnitOfWork _unitOfWork;
        public IMapper _mapper { get; }
        private IFileRepository _fileRepository;
        public MediaController(
                    IUnitOfWork unitOfWork
                    , IMapper mapper
                    , IFileRepository fileRepository
                    )
        {
            _unitOfWork = unitOfWork;
            _fileRepository = fileRepository;
            _mapper = mapper;
        }

        #region Media

        [Authorize]
        [HttpGet]
        [Route("GetMedia")]
        public async Task<IActionResult> GetMedia()
        {
            var media =  _unitOfWork.Media.FindAll(false, new[] { "FileExNavigation" } );
           
            var list = from m in media
                       select new Files
                       {
                           file = m.FileName.ToString() + m.FileEx,
                           name = m.Name + m.FileEx,
                           isImage = m.MediaExCategoryId == 1 ? true : false,
                           type = m.MediaExCategoryId == 1 ? "image" : m.MediaExCategoryId == 2 ? "video" : "document",
                           changed = m.Created.ToString("MM/dd/yyyy h:mm tt"),
                           size = "800.07kB",
                           thumb = m.MediaExCategoryId == 1 ? m.FileNameSmall + m.FileEx : 
                                                              (m.MediaExCategoryId == 2 ? m.FileName.ToString() + m.FileEx :
                                                                                          (m.MediaExCategoryId == 3 ? "ico/" + m.FileExNavigation.MediaExPath : ""))
                       };

            Source source = new Source
            {
                files = list.ToList()
            };
            List<Source> sourceList = new List<Source>();
            sourceList.Add(source);
            Data data = new Data
            {
                sources = sourceList
            };
            int elapsedTime = 0;
            bool success = true;
            string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
             return Ok(new
             {
                 data,
                 elapsedTime = elapsedTime,
                 success = success,
                 time = time
             });
        }

        [Authorize]
        [HttpPost]
        [Route("GetMediaAsync")]
        public async Task<IActionResult> GetMediaAsync([FromBody] MediaFilterParameters parameter)
        {
            var media = await _unitOfWork.Media.GetMediaAsync(parameter);
            
            var mediaDto = _mapper.Map<IEnumerable<MediaListDto>?>(media);

            return Ok(new
            {
                data = mediaDto,
                total = media.MetaData.TotalCount
            });            
        }

        [Authorize]
        [HttpPost]
        [Route("CreateMedia")]
        public async Task<IActionResult> CreateMedia([FromForm] MediaDto model)
        {
            if (ModelState.IsValid)
            {
                List<string> mediaEx = await _unitOfWork.Media.GetMediaEx();
                var userId = _unitOfWork.BaseConfig.GetLoggedUserId();
                var sysConfig = await _unitOfWork.BaseConfig.GetSysSettings();
                foreach (var file in model.Files)
                {
                    var fileEx = Path.GetExtension(file.FileName).ToLower();
                    if (mediaEx.Contains(fileEx))
                    {
                        var mediaExCategory = await _unitOfWork.Media.GetMediaExCategory(fileEx);
                        if(mediaExCategory.MediaExCategoryId == 1 ) //Photo
                        {
                            int maxLength =int.Parse(sysConfig.FirstOrDefault(x => x.Label == "MaxFotoSize").Value);
                            maxLength = maxLength * 1048576;
                            if(maxLength >= file.Length)
                            {
                                string filename = await _fileRepository.AddMediaPicWithCropAsync(file, sysConfig, model);

                                Medium media = new Medium
                                {
                                    MediaExCategoryId = mediaExCategory.MediaExCategoryId,
                                    Name = file.FileName.Replace(fileEx, ""),
                                    FileName = Guid.Parse(filename),
                                    FileNameMedium = filename + "_medium",
                                    FileNameSmall = filename + "_small",
                                    FileEx = fileEx,
                                    IsOtherSource = false,
                                    CreatedBy = userId,
                                    Created = DateTime.Now
                                };
                                await _unitOfWork.Media.Create(media);
                                await _unitOfWork.Media.Commit();
                            }
                            else
                            {
                                return BadRequest($"Ky fajll i tejkalon madhesite e percaktuara, kontaktoni administratorin!");
                            }    
                        }
                        else if(mediaExCategory.MediaExCategoryId == 2) //Video
                        {
                            int maxLength = int.Parse(sysConfig.FirstOrDefault(x => x.Label == "MaxVideoSize").Value);
                            maxLength = maxLength * 1048576;
                            if (maxLength >= file.Length)
                            {
                                string filename = await _fileRepository.AddFileAndVideo(file, mediaExCategory.MediaExCategoryId);
                                Medium media = new Medium
                                {
                                    MediaExCategoryId = mediaExCategory.MediaExCategoryId,
                                    Name = file.FileName.Replace(fileEx, ""),
                                    FileName = Guid.Parse(filename),
                                    FileEx = fileEx,
                                    IsOtherSource = false,
                                    CreatedBy = userId,
                                    Created = DateTime.Now
                                };

                                await _unitOfWork.Media.Create(media);
                                await _unitOfWork.Media.Commit();
                            }
                            else
                            {
                                return BadRequest($"Ky fajll i tejkalon madhesite e percaktuara, kontaktoni administratorin!");
                            }
                        }

                        else //Documents
                        {
                            int maxLength = int.Parse(sysConfig.FirstOrDefault(x => x.Label == "MaxFileSize").Value);
                            maxLength = maxLength * 1048576;
                            if (maxLength >= file.Length)
                            {
                                string filename = await _fileRepository.AddFileAndVideo(file, mediaExCategory.MediaExCategoryId);
                                Medium media = new Medium
                                {
                                    MediaExCategoryId = mediaExCategory.MediaExCategoryId,
                                    Name = file.FileName.Replace(fileEx, ""),
                                    FileName = Guid.Parse(filename),
                                    FileEx = fileEx,
                                    IsOtherSource = false,
                                    CreatedBy = userId,
                                    Created = DateTime.Now
                                };

                                await _unitOfWork.Media.Create(media);
                                await _unitOfWork.Media.Commit();
                            }
                            else
                            {
                                return BadRequest($"Ky fajll i tejkalon madhesite e percaktuara, kontaktoni administratorin!");
                            }
                                
                        }                       

                    }
                    else
                    {
                        return BadRequest($"Ky lloj i fajllit({fileEx}) nuk pranohet nga sistemi ");
                    }                    
                }

                return Ok();
            }
            return NotFound();
        }

        [Authorize]
        [HttpDelete("DeleteMedia")]
        public async Task<IActionResult> DeleteMedia(int id)
        {
            try
            {
                var media = await _unitOfWork.Media.GetById(id);

                if (media == null)
                {
                    return NotFound();
                }             
                //bool isDeletet = _fileRepository.DeleteFile(media);               
               
                _unitOfWork.Media.Delete(media);
                await _unitOfWork.Media.Commit();

                _fileRepository.DeleteFile(media);

                return Ok();
                              
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }

        #endregion

        #region Media Other Source

        [Authorize]
        [HttpPost]
        [Route("CreateMediaOtherSource")]
        public async Task<IActionResult> CreateMediaOtherSource([FromBody] MediaOtherSourceDto model)
        {
            if (ModelState.IsValid)
            {
                string userinId = _unitOfWork.BaseConfig.GetLoggedUserId();
               
                
                var mediumOtherSource = _mapper.Map<Medium>(model);

                mediumOtherSource.FileName = Guid.NewGuid();
                mediumOtherSource.IsOtherSource = true;
                mediumOtherSource.CreatedBy = userinId;
                mediumOtherSource.Created = DateTime.Now;

                await _unitOfWork.Media.Create(mediumOtherSource);
                await _unitOfWork.Media.Commit();

                return StatusCode(StatusCodes.Status201Created, mediumOtherSource);
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        [Authorize]
        [HttpDelete("DeleteMediaOtherSource")]
        public async Task<IActionResult> DeleteMediaOtherSource(int id)
        {
            try
            {
                var media = await _unitOfWork.Media.GetById(id);

                if (media == null)
                {
                    return NotFound();
                }
                
                _unitOfWork.Media.Delete(media);
                await _unitOfWork.Media.Commit();
                return Ok();
               
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }


        #endregion
    }
}

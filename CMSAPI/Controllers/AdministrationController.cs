using AutoMapper;
using Contracts.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Entities.Models; using CMS.API;
using Entities.DataTransferObjects;
using Entities.ErrorModel;
using Entities.RequestFeatures;
using Newtonsoft.Json;
using CMS.API.Helpers;
using System.Data;
using CMS.API.InternalServices;

namespace CMSAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdministrationController : ControllerBase
    {
        readonly List<ErrorDetails> respond = new();
        private readonly IUnitOfWork _unitOfWork;
        public IMapper _mapper { get; }
        private UserManager<ApplicationUser> userManager;
        private readonly RoleManager<ApplicationRole> roleManager;
        public IConfiguration configuration { get; }     
        private IFileRepository _fileRepository;

        public AdministrationController(
                    IUnitOfWork unitOfWork
                    , IMapper mapper
                    , UserManager<ApplicationUser> userManager
                    , RoleManager<ApplicationRole> roleManager
                    , IConfiguration configuration
                    
                    ,IFileRepository fileRepository
                    )
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.configuration = configuration;            
            _fileRepository = fileRepository;            
        }

        #region User

        [Authorize]
        [HttpPost]
        [Route("GetUsers")]
        public async Task<IActionResult> GetUsers([FromBody] FilterParameters usersParameters)
        {
            var users = await _unitOfWork.Users.GetUsersAsync(usersParameters);

            Response.Headers.Add("X-Pagination",
                JsonConvert.SerializeObject(users.MetaData));

            var UserDto = _mapper.Map<List<UserDto>>(users);
            foreach (var user in UserDto)
            {
                var userRoles = await _unitOfWork.BaseConfig.GetUserRoles(user.Id.ToString());

                user.Role = userRoles.Count > 0 ? (userRoles.Count > 1 ? string.Join(", ", userRoles) : userRoles[0]) : "";
            }

            return Ok(new
            {
                data = UserDto,
                total = users.MetaData.TotalCount
            });
            //return Ok(UserDto);
        }

        [Authorize]
        [HttpGet]
        [Route("GetUserById")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var editUser = await userManager.FindByIdAsync(id);
            if (editUser != null)
            {
                var UserDto = _mapper.Map<UserDto>(editUser);
                UserDto.Birthdate = editUser.Birthdate.HasValue ? editUser.Birthdate.Value.ToString("dd/MM/yyyy") : null;
                UserDto.ValidFrom = editUser.ValidFrom.HasValue ? editUser.ValidFrom.Value.ToString("dd/MM/yyyy") : null;
                UserDto.ValidTo = editUser.ValidTo.HasValue ? editUser.ValidTo.Value.ToString("dd/MM/yyyy") : null;

                var userRoles = await _unitOfWork.BaseConfig.GetUserRoles(UserDto.Id.ToString());
                string RoleName = userRoles.Count > 0 ? userRoles[0] : "";
                var Role = await roleManager.FindByNameAsync(RoleName);
                UserDto.Role = Role.Id.ToString();

                return Ok(UserDto);
            }
            return NotFound();

        }

        [AllowAnonymous]
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginVM model)
        {
            var sysConfig = await _unitOfWork.BaseConfig.GetSysSettings();
            if (!string.IsNullOrEmpty(model.Token))
            {
                bool result = await CheckRecapchaLogin(model.Token);
                if (!result)
                {
                    respond.Add(new ErrorDetails
                    {
                        Status = false,
                        Message = "Recapcha token nuk është valid"
                    });
                    _unitOfWork.Users.AddUserAudit((byte)ActionType.NotAllowed, "N/A", $"Recapcha token nuk është valid per kete username:{model.UserName} ", $"Recapcha token is not valid for this username:{model.UserName}", $"Recapcha token nije važeći za ovo korisničko ime:{model.UserName}");

                    return Unauthorized(respond);
                }
            }
            var user = await userManager.FindByNameAsync(model.UserName);
            if (user != null && await userManager.CheckPasswordAsync(user, model.Password))
            {
                if (user.Active == false)
                {
                    respond.Add(new ErrorDetails
                    {
                        Status = false,
                        Message = "Llogaria është pasive, kontaktoni administratorin!"
                    });
                    _unitOfWork.Users.AddUserAudit((byte)ActionType.NotAllowed, user.Id.ToString(), "Llogaria është pasive", "The Account is inactive", "Nalog je neaktivan");
                    return Unauthorized(respond);
                }
                if (user.ValidFrom < DateTime.Now && DateTime.Now > user.ValidTo)
                {
                    respond.Add(new ErrorDetails
                    {
                        Status = false,
                        Message = $"Periudha aktive për qasje ka mbaruar ne këtë datë {user.ValidTo.Value.ToString("dd/MM/yyyy : hh:mm")}"
                    });
                    _unitOfWork.Users.AddUserAudit((byte)ActionType.NotAllowed, user.Id.ToString(), $"Periudha aktive për qasje ka mbaruar ne këtë datë {user.ValidTo.Value.ToString("dd/MM/yyyy : hh:mm")}",
                        $"The active access period has ended on this date {user.ValidTo.Value.ToString("dd/MM/yyyy : hh:mm")}",
                        $"Period aktivnog pristupa je završen na ovaj datum {user.ValidTo.Value.ToString("dd/MM/yyyy : hh:mm")}");
                    return Unauthorized(respond);
                }
                if (user.ValidFrom > DateTime.Now)
                {
                    respond.Add(new ErrorDetails
                    {
                        Status = false,
                        Message = $"Llogaria do të jetë aktive nga kjo datë {user.ValidFrom.Value.ToString("dd/MM/yyyy : hh:mm")}"
                    });
                    _unitOfWork.Users.AddUserAudit((byte)ActionType.NotAllowed, user.Id.ToString(), $"Llogaria do të jetë aktive nga kjo datë {user.ValidFrom.Value.ToString("dd/MM/yyyy : hh:mm")}",
                       $"The account will be active from this date {user.ValidFrom.Value.ToString("dd/MM/yyyy : hh:mm")}",
                       $"Nalog ce biti aktivan od ovog datuma {user.ValidFrom.Value.ToString("dd/MM/yyyy : hh:mm")}");

                    return Unauthorized(respond);
                }                               

                var userRoles = await userManager.GetRolesAsync(user);
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Locality, ((int)user.Language).ToString()),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };
                //authClaims.Add(new Claim(ClaimTypes.Role, userRoles));
                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }
                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JsonWebTokenKeys:IssuerSigningKey"]));
                var token = new JwtSecurityToken(expires: DateTime.Now.AddHours(int.Parse(configuration["JsonWebTokenKeys:TokenExpireHour"])), claims: authClaims
                    , signingCredentials: new SigningCredentials(authSigningKey
                    , SecurityAlgorithms.HmacSha256));

                _unitOfWork.Users.AddUserAudit((byte)ActionType.SignIn, user.Id.ToString(), "Përdoruesi është kyqur me sukses!",
                       "User logged in successfully!",
                       "Korisnik se uspešno prijavio!");

                var chagePw = false;
                if (user.ChangePassword == true || user.PasswordExpires.Date <= DateTime.Now.Date)
                {
                    chagePw = true;
                }
                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo,
                    user = new
                    {
                        authority = userRoles,
                        avatar = user.ProfileImage,
                        email = user.Email,
                        username = user.UserName,
                        language = (int)user.Language,
                        firstname = user.Firstname,
                        lastname = user.Lastname,
                        userId = user.Id.ToString(),
                        webMultiLang = sysConfig.Where(X => X.Label == "MultiLanguage").Count() > 0 ? bool.Parse(sysConfig.FirstOrDefault(x => x.Label == "MultiLanguage").Value) : false,
                        documentUrlPath = sysConfig.Where(X => X.Label == "DocumentUrlPath").Count() > 0 ? sysConfig.Where(X => X.Label == "DocumentUrlPath").FirstOrDefault().Value : "",
                        imageCrop = sysConfig.Where(X => X.Label == "DocumentUrlPath").Count() > 0 ? bool.Parse(sysConfig.FirstOrDefault(x => x.Label == "ImageCrop").Value) : false,
                    },
                    status = true,
                    Message = "User Login Successfully",
                    changePassword = chagePw
                });
            }
            _unitOfWork.Users.AddUserAudit((byte)ActionType.IncorrectPassword, user == null ? "N/A" : user.Id.ToString(), $"Kredencialet nuk jane të sakta per kete username:{model.UserName}", $"Credentials are not correct for this username:{model.UserName}", $"Akreditivi nisu tačni za ovo korisničko ime:{model.UserName}");
            return Unauthorized();
        }

        
        protected async Task<bool> CheckRecapchaLogin(string token)
        {
            rechapchaRespondeLogin responseEntity = null;
            string recapchaUrl = configuration["EmailSettings:Recapcha_api"];
            string Recapcha_SECRET_KEY = configuration["EmailSettings:Recapcha_SECRET_KEY"];
            HttpClientHandler clientHandler = new HttpClientHandler();
            var client = new HttpClient(clientHandler);
            client.DefaultRequestHeaders.TryAddWithoutValidation("content-type", "application/json");
            var uri = string.Format(recapchaUrl, Recapcha_SECRET_KEY, token);

            var task = client.GetAsync(uri).ContinueWith((response) =>
            {
                var result = response.Result;
                if (result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var readTask = result.Content.ReadAsAsync<rechapchaRespondeLogin>();
                    readTask.Wait();
                    responseEntity = readTask.Result;
                }
            }).ContinueWith((err) =>
            {
                if (err.Exception != null)
                {
                    throw err.Exception;
                }
            });
            task.Wait();
            if (responseEntity.success == true)
                return true;

            return false;
        }

        [HttpPost]
        [Route("LogOut")]
        public async Task<IActionResult> LogOut()
        {
            try
            {
                var userId = _unitOfWork.BaseConfig.GetLoggedUserId();
                _unitOfWork.Users.AddUserAudit((byte)ActionType.LogOut, userId, "Përdoruesi është çkyqur me sukses!",
                       "User logout successfully!",
                       "Odjava korisnika uspešno!");
                return Ok(true);
            }
            catch (Exception ex)
            {

            }
            return Ok(true);
        }


        [Authorize]
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromForm] CreateUserModel model)
        {
            if (await _unitOfWork.Users.CheckUsername(model.UserName) == true)
            {
                ModelState.AddModelError("Username", "Ky username ekziston ne sistem");
            }

            if (await _unitOfWork.Users.CheckEmail(model.Email) == true)
            {
                ModelState.AddModelError("Email", "Ky E-mail ekziston ne sistem");
            }
            DateTime? dtBirthdate = null;
            DateTime? dtValidFrom = null;
            DateTime? dtValidTo = null;
            if (!string.IsNullOrEmpty(model.Birthdate))
            {
                try
                {
                    dtBirthdate = _unitOfWork.BaseConfig.StringToDate(model.Birthdate);
                }
                catch (Exception)
                {
                    ModelState.AddModelError("Birthdate", "Formati dates nuk eshte ne rregull");
                }
            }
            if (!string.IsNullOrEmpty(model.ValidFrom))
            {
                try
                {
                    dtValidFrom = _unitOfWork.BaseConfig.StringToDate(model.ValidFrom);
                }
                catch (Exception)
                {
                    ModelState.AddModelError("ValidFrom", "Formati dates nuk eshte ne rregull");
                }
            }
            if (!string.IsNullOrEmpty(model.ValidTo))
            {
                try
                {
                    dtValidTo = _unitOfWork.BaseConfig.StringToDate(model.ValidTo);
                }
                catch (Exception)
                {
                    ModelState.AddModelError("ValidTo", "Formati dates nuk eshte ne rregull");
                }
            }
            string userinId = _unitOfWork.BaseConfig.GetLoggedUserId();
            if (ModelState.IsValid)
            {
                ApplicationUser newUser = new()
                {
                    PersonalNumber = model.PersonalNumber,
                    Email = model.Email,
                    Firstname = model.Firstname,
                    Lastname = model.Lastname,
                    EmailConfirmed = true,
                    Language = model.Language,
                    PasswordExpires = model.Reset == true ? DateTime.Now : DateTime.Now.AddMonths(3),
                    PhoneNumber = model.PhoneNumber,
                    Birthdate = dtBirthdate.HasValue ? dtBirthdate : null,
                    Gender = model.Gender,
                    WorkPosition = model.WorkPosition,
                    ValidFrom = dtValidFrom.HasValue ? dtValidFrom : null,
                    ValidTo = dtValidTo.HasValue ? dtValidTo : null,
                    Active = model.Active,
                    UserName = model.UserName,
                    CreateBy = userinId,
                    Created = DateTime.Now,
                    //ProfileImage = model.FilePath
                };

                if (model.ProfileImage != null)
                {
                    //string filename = AddProfilePic(model.ProfileImage);
                    var sysConfig = await _unitOfWork.BaseConfig.GetSysSettings();
                    int maxLength = int.Parse(sysConfig.FirstOrDefault(x => x.Label == "MaxFotoSize").Value);
                    maxLength = maxLength * 1048576;
                    if (maxLength >= model.ProfileImage.Length)
                    {
                        string filename = await _fileRepository.AddProfilePic(model.ProfileImage);
                        newUser.ProfileImage = filename;
                    }

                    else
                    {

                        respond.Add(new ErrorDetails
                        {
                            Status = false,
                            Message = "Ky fajll i tejkalon madhesite e percaktuara, kontaktoni administratorin!"
                        });
                        return StatusCode(StatusCodes.Status406NotAcceptable, respond);
                    }
                }

                var result = await userManager.CreateAsync(newUser, model.Password);
                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(model.Role))
                    {
                        var role = await roleManager.FindByIdAsync(model.Role);
                        await userManager.AddToRoleAsync(newUser, role.Name);
                    }
                }
                else
                {
                    return StatusCode(StatusCodes.Status406NotAcceptable, respond);
                }
                respond.Add(new ErrorDetails
                {
                    Status = true,
                    Message = "Perdoruesi u regjistrua me sukses"
                });
                return StatusCode(StatusCodes.Status201Created, respond);
            }

            return BadRequest(ModelState);
        }

        [Authorize]
        [HttpPut]
        [Route("edit")]
        public async Task<IActionResult> Edit([FromForm] EditUserModel model)
        {
            DateTime? dtBirthdate = null;
            DateTime? dtValidFrom = null;
            DateTime? dtValidTo = null;
            if (!string.IsNullOrEmpty(model.Birthdate))
            {
                try
                {
                    dtBirthdate = _unitOfWork.BaseConfig.StringToDate(model.Birthdate);
                }
                catch (Exception)
                {
                    ModelState.AddModelError("Birthdate", "Formati dates nuk eshte ne rregull");
                }
            }
            if (!string.IsNullOrEmpty(model.ValidFrom))
            {
                try
                {
                    dtValidFrom = _unitOfWork.BaseConfig.StringToDate(model.ValidFrom);
                }
                catch (Exception)
                {
                    ModelState.AddModelError("ValidFrom", "Formati dates nuk eshte ne rregull");
                }
            }
            if (!string.IsNullOrEmpty(model.ValidTo))
            {
                try
                {
                    dtValidTo = _unitOfWork.BaseConfig.StringToDate(model.ValidTo);
                }
                catch (Exception)
                {
                    ModelState.AddModelError("ValidTo", "Formati dates nuk eshte ne rregull");
                }
            }
            string userinId = _unitOfWork.BaseConfig.GetLoggedUserId();

            if (ModelState.IsValid)
            {
                ApplicationUser user = await userManager.FindByIdAsync(model.ID);

                user.PersonalNumber = string.IsNullOrEmpty(model.PersonalNumber) ? null : model.PersonalNumber;
                user.Email = model.Email;
                user.Firstname = model.Firstname;
                user.Lastname = model.Lastname;
                user.Language = model.Language;
                user.PasswordExpires = model.Reset == true ? DateTime.Now : DateTime.Now.AddMonths(3);
                user.PhoneNumber = string.IsNullOrEmpty(model.PhoneNumber) ? null : model.PhoneNumber;
                user.Birthdate = dtBirthdate.HasValue ? dtBirthdate : null;
                user.Gender = model.Gender;
                user.WorkPosition = string.IsNullOrEmpty(model.WorkPosition) ? null : model.WorkPosition;
                user.ValidFrom = dtValidFrom.HasValue ? dtValidFrom : null;
                user.ValidTo = dtValidTo.HasValue ? dtValidTo : null;
                user.Active = model.Active;
                user.ModifiedBy = userinId;
                user.Modified = DateTime.Now;

                if (model.ProfileImage != null)
                {
                    var sysConfig = await _unitOfWork.BaseConfig.GetSysSettings();

                    int maxLength = int.Parse(sysConfig.FirstOrDefault(x => x.Label == "MaxFotoSize").Value);
                    maxLength = maxLength * 1048576;
                    if (maxLength >= model.ProfileImage.Length)
                    {
                        string filename = await _fileRepository.AddProfilePic(model.ProfileImage);
                        user.ProfileImage = filename;
                    }
                    else
                    {
                        respond.Add(new ErrorDetails
                        {
                            Status = false,
                            Message = "Ky fajll i tejkalon madhesite e percaktuara, kontaktoni administratorin!"
                        });
                        return StatusCode(StatusCodes.Status406NotAcceptable, respond);
                    }
                }

                var result = await userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(model.Password) && !string.IsNullOrEmpty(model.CurrentPassword))
                    {
                        await userManager.ChangePasswordAsync(user, model.CurrentPassword, model.Password);
                    }

                    if (!string.IsNullOrEmpty(model.Role))
                    {
                        //delete from role
                        var userRoles = await userManager.GetRolesAsync(user);
                        if (userRoles.Any())
                        {
                            foreach (var roleName in userRoles)
                            {
                                await userManager.RemoveFromRoleAsync(user, roleName);
                            }
                        }

                        //insert new role
                        var role = await roleManager.FindByIdAsync(model.Role);
                        await userManager.AddToRoleAsync(user, role.Name);
                    }
                }
                else
                {
                    return StatusCode(StatusCodes.Status406NotAcceptable, respond);
                }
                respond.Add(new ErrorDetails
                {
                    Status = true,
                    Message = "Të dhënat janë ndryshuar me sukses!"
                });
                return StatusCode(StatusCodes.Status201Created, respond);
            }

            return BadRequest(ModelState);
        }


        [Authorize]
        [HttpPost]
        [Route("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPassword model)
        {
            string userinId = _unitOfWork.BaseConfig.GetLoggedUserId();

            if (ModelState.IsValid)
            {
                var User = await userManager.FindByIdAsync(userinId);

                User.ModifiedBy = userinId;
                User.Modified = DateTime.Now;
                User.ChangePassword = false;
                User.PasswordExpires = DateTime.Now.AddMonths(3);

                var result = await userManager.ChangePasswordAsync(User, model.CurrentPassword, model.NewPassword);
                if (result.Succeeded)
                {
                    await userManager.UpdateAsync(User);
                }
                else
                {
                    return StatusCode(StatusCodes.Status406NotAcceptable, respond);
                }
                respond.Add(new ErrorDetails
                {
                    Status = true,
                    Message = "Të dhënat janë ndryshuar me sukses!"
                });
                return Ok(respond);
            }

            return BadRequest(ModelState);
        }

        [HttpGet]
        [Authorize]
        [Route("/api/CheckUsername/{username}")]
        public async Task<bool> CheckUsername(string username)
        {
            return await _unitOfWork.Users.CheckUsername(username);
        }

        [HttpGet]
        [Authorize]
        [Route("/api/CheckEmail/{email}")]
        public async Task<bool> CheckEmail(string email)
        {
            return await _unitOfWork.Users.CheckEmail(email);
        }

        [Authorize]
        [HttpGet]
        [Route("GetAllRoles")]
        public async Task<IActionResult> GetAllRoles()
        {
            try
            {
                //var Roles = await roleManager.Roles.ToListAsync();
                var Roles = await _unitOfWork.Roles.GetAllRoles();
                if (Roles != null)
                {
                    var RolesDto = _mapper.Map<List<RolesDto>>(Roles);

                    return Ok(RolesDto);
                }
                return NotFound();
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [Authorize]
        [HttpPost]
        [Route("GetAllRolesAsync")]
        public async Task<IActionResult> GetAllRolesAsync([FromBody] FilterParameters Parameters)
        {
            var roles = await _unitOfWork.Roles.GetRolesAsync(Parameters);

            var RolesDto = _mapper.Map<List<RolesDtoAsync>>(roles);

            return Ok(new
            {
                data = RolesDto,
                total = roles.MetaData.TotalCount
            });
        }



        [Authorize]
        [HttpGet]
        [Route("GetUserRoles")]
        public async Task<IActionResult> GetUserRoles(string UserId)
        {
            var roles = await _unitOfWork.BaseConfig.GetUserRoles(UserId);
            if (roles != null)
            {
                return Ok(roles);
            }
            return NotFound();
        }

        [Authorize]
        [HttpPost]
        [Route("AddRoleToUser")]
        public async Task<IActionResult> AddRoleToUser([FromBody] UserRole model)
        {
            var User = await userManager.FindByIdAsync(model.UserId);
            if (User != null)
            {
                var role = await roleManager.FindByNameAsync(model.RoleName);
                await userManager.AddToRoleAsync(User, role.Name);
                return Ok();
            }
            return NotFound();
        }

        [Authorize]
        [HttpPost]
        [Route("RemoveRoleFromUser")]
        public async Task<IActionResult> RemoveRoleFromUser([FromBody] UserRole model)
        {
            var User = await userManager.FindByIdAsync(model.UserId);
            if (User != null)
            {
                var role = await roleManager.FindByNameAsync(model.RoleName);
                await userManager.RemoveFromRoleAsync(User, role.Name);
                return Ok();
            }
            return NotFound();
        }

        [Authorize]
        [HttpGet]
        [Route("GetProfile")]
        public async Task<IActionResult> GetProfile()
        {
            string UserId = _unitOfWork.BaseConfig.GetLoggedUserId();

            var profili = await _unitOfWork.Users.GetProfile(UserId);
            if (profili != null)
                return Ok(profili);

            return NotFound();

        }

        [Authorize]
        [HttpPut]
        [Route("UpdateProfile")]
        public async Task<IActionResult> UpdateProfile([FromForm] ProfileDto model)
        {
            string UserId = _unitOfWork.BaseConfig.GetLoggedUserId();
            string filename = "";
            if (model.ProfileImage != null)
            {
                //filename = AddProfilePic(model.ProfileImage);
                filename = await _fileRepository.AddProfilePic(model.ProfileImage);
            }
            var result = await _unitOfWork.Users.PostProfile(model, UserId, filename);
            if (result == ErrorStatus.Success)
                return Ok();

            return NotFound();

        }

        [Authorize]
        [HttpPost]
        [Route("ResetPasswordFromAdmin")]
        public async Task<IActionResult> ResetPasswordFromAdmin([FromBody] ResetPasswordFromAdmin model)
        {
            if (string.IsNullOrEmpty(model.UserId))
            {
                ModelState.AddModelError("UserId", "Id e userit eshte fushe e obliguar");
            }
            if (string.IsNullOrEmpty(model.NewPassword))
            {
                ModelState.AddModelError("NewPassword", "Passwordi i ri eshte fushe e obliguar");
            }
            if (model.NewPassword.Length < 3)
            {
                ModelState.AddModelError("NewPassword", "Gjatesia e karaktereve te pw eshte 3");
            }
            if (ModelState.IsValid)
            {
                string userinId = _unitOfWork.BaseConfig.GetLoggedUserId();
                var user = await userManager.FindByIdAsync(model.UserId);
                if (user == null)
                {
                    ModelState.AddModelError("UserId", "Ky perdorues nuk eshte gjetur");
                    return BadRequest(ModelState);
                }

                var token = await userManager.GeneratePasswordResetTokenAsync(user);

                user.ModifiedBy = userinId;
                user.Modified = DateTime.Now;
                user.ChangePassword = true;

                var result = await userManager.ResetPasswordAsync(user, token, model.NewPassword);
                if (result.Succeeded)
                {
                    await userManager.UpdateAsync(user);
                }
                else
                {
                    return StatusCode(StatusCodes.Status406NotAcceptable, respond);
                }
                respond.Add(new ErrorDetails
                {
                    Status = true,
                    Message = "Passwordi është resetuar me sukses!"
                });
                return StatusCode(StatusCodes.Status201Created, respond);
            }

            return BadRequest(ModelState);
        }

        [Authorize]
        [HttpGet]
        [Route("GetRoleById")]
        public async Task<ActionResult> GetRoleById(string Id)
        {
            var role = await roleManager.FindByIdAsync(Id);
            if (role != null)
            {
                var Role = _mapper.Map<RolesDto>(role);

                return Ok(Role);
            }
            return NotFound();
        }

        #endregion

        #region Role

        [Authorize]
        [HttpPost]
        [Route("AddRole")]
        public async Task<ActionResult> AddRole(AddRole model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await _unitOfWork.Roles.AddRole(model);
                    if(!string.IsNullOrEmpty(result.Id))
                    {
                        return StatusCode(StatusCodes.Status201Created, result);
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

        [Authorize]
        [HttpPut]
        [Route("UpdateRole")]
        public async Task<ActionResult> UpdateRole(UpdateRole model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await _unitOfWork.Roles.UpdateRole(model);
                    if (result)
                        return Ok();
                    return StatusCode(StatusCodes.Status304NotModified);
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
        }

        [Authorize]
        [HttpGet]
        [Route("GetLayoutsForRole")]
        public async Task<ActionResult> GetLayoutsForRole(string RoleId)
        {
            string[] includes = { "Layout" };
            var roleLayouts = await _unitOfWork.Layouts.GetRoleLayouts(RoleId, false, includes);

            if (roleLayouts == null)
            {
                //_logger.LogInfo($"Company with id: {id} doesn't exist in the database.");

                return NotFound();
            }
            else
            {
                var layoutDto = _mapper.Map<List<RoleLayoutsDto>>(roleLayouts);

                return Ok(layoutDto);
            }
        }

        [Authorize]
        [HttpPost]
        [Route("AddLayoutInRole")]
        public async Task<ActionResult> AddLayoutInRole(AddLayoutInRole model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string userinId = _unitOfWork.BaseConfig.GetLoggedUserId();

                    var layoutRole = _mapper.Map<LayoutRole>(model);
                    layoutRole.CreatedBy = userinId;
                    layoutRole.Created = DateTime.Now;

                    var result = await _unitOfWork.Roles.AddLayoutInRole(layoutRole);
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
        [Route("RemoveLayoutFromRole")]
        public async Task<ActionResult> RemoveLayoutFromRole(int Id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = _unitOfWork.Roles.RemoveLayoutFromRole(Id);
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
        [HttpGet]
        [Route("GetMenusForRole")]
        public async Task<ActionResult> GetMenusForRole(string RoleId)
        {
            var userLanguageId = _unitOfWork.BaseConfig.GetCurrentUserLanguage();
            return Ok(await _unitOfWork.Roles.GetMenusForRole(RoleId, userLanguageId));
        }

        [Authorize]
        [HttpGet]
        [Route("GetMenusForRoleNotIn")]
        public async Task<ActionResult> GetMenusForRoleNotIn(string RoleId, int TypeId = 0, int ParentId = 0)
        {
            var userLanguageId = _unitOfWork.BaseConfig.GetCurrentUserLanguage();
            return Ok(await _unitOfWork.Roles.GetMenusForRoleNotIn(RoleId, userLanguageId, TypeId, ParentId));
        }

        [Authorize]
        [HttpPost]
        [Route("AddMenuInRole")]
        public async Task<ActionResult> AddMenuInRole(AddMenuInRole model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string userinId = _unitOfWork.BaseConfig.GetLoggedUserId();
                    var menurole = _mapper.Map<SysMenuRole>(model);
                    menurole.CreatedBy = userinId;
                    menurole.Created = DateTime.Now;
                    var result = await _unitOfWork.Roles.AddMenuInRole(menurole);
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
        [HttpPost]
        [Route("AddMenuCollectionInRole")]
        public async Task<ActionResult> AddMenuCollectionInRole(AddMenuCollectionInRole model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string userinId = _unitOfWork.BaseConfig.GetLoggedUserId();
                    var result = await _unitOfWork.Roles.AddMenuCollectionInRole(model, userinId);
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
        [Route("RemoveMenuFromRole")]
        public async Task<ActionResult> RemoveMenuFromRole(int Id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await _unitOfWork.Roles.RemoveMenuFromRole(Id);
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
        [Route("RemoveMenuCollectionFromRole")]
        public async Task<ActionResult> RemoveMenuCollectionFromRole( [FromQuery] string Ids)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var numbers = Ids.Split(',')?.Select(Int32.Parse)?.ToList();
                    if(numbers.Count > 0)
                    {
                        var result = await _unitOfWork.Roles.RemoveMenuCollectionFromRole(numbers);
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

        #endregion
    }

}

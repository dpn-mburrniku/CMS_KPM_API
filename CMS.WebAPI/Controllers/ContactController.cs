using AutoMapper;
using CMS.API;
using Contracts.IServices;
using Entities.DataTransferObjects;
using Entities.DataTransferObjects.WebDTOs;
using Entities.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using System.Net.Mime;

namespace CMS.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        public IMapper _mapper { get; }
        public ContactController(IUnitOfWork unitOfWork
                    , IMapper mapper
                    , IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _configuration = configuration;
        }

        [HttpGet("GetContacts")]
        public async Task<IActionResult> GetContacts(int PageID, string Gjuha = "sq")
        {
            int gjuhaID = _unitOfWork.BaseConfig.GetGjuhaID(Gjuha);
            var result = await _unitOfWork.Contacts.GetContact(gjuhaID, PageID);
            if (result.Count > 0)
                return Ok(result);
            else
                return NotFound();
        }

        [HttpGet("GetContactsList")]
        public async Task<IActionResult> GetContactsList(int PageID, string Gjuha = "sq")
        {
            int gjuhaID = _unitOfWork.BaseConfig.GetGjuhaID(Gjuha);
            var result = await _unitOfWork.Contacts.GetContactList(gjuhaID, PageID);
            if (result.Count > 0)
                return Ok(result);
            else
                return NotFound();
        }

        //[HttpGet("GetContacts")]
        //public async Task<IActionResult> GetContacts(int PageID, string Gjuha = "sq")
        //{
        //    int gjuhaId = _unitOfWork.BaseConfig.GetGjuhaID(Gjuha);
        //    var result =  _unitOfWork.Contacts.FindByCondition(x => x.LanguageId == gjuhaId && x.PageId == PageID, false, new[] { "Page", "Media" }).ToList();
        //    var contactList = _mapper.Map<List<ContactModel>>(result);
        //    if (contactList.Count > 0)
        //        return Ok(contactList);
        //    else 
        //        return NotFound();
        //}

        //[HttpGet("GetContactsList")]
        //public async Task<IActionResult> GetContactsList(int PageID, string Gjuha = "sq")
        //{
        //    int gjuhaId = _unitOfWork.BaseConfig.GetGjuhaID(Gjuha);
        //    var result = _unitOfWork.Contacts.FindByCondition(x => x.LanguageId == gjuhaId && x.PageId == PageID, false, null).ToList();
        //    var contactList = _mapper.Map<List<ContactModel>>(result);
        //    if (contactList.Count > 0)
        //        return Ok(contactList);
        //    else
        //        return NotFound();
        //}

        [HttpPost("SendMail")]
        public async Task<bool> SendMail([FromBody] EmailDto emaildto, string token, string Gjuha = "sq")
        {
            try
            {
                if (string.IsNullOrEmpty(token))
                {
                    return false;
                }
                bool result = await CheckRecapcha(token);
                if (result)
                {
                    //int gjuhaID = _unitOfWork.BaseConfig.GetGjuhaID(Gjuha);
                    var departamenti = await _unitOfWork.Contacts.FindByCondition(x=>x.Id == emaildto.ContactId,false,null).FirstOrDefaultAsync();

                    string bodyfromSettings = new(_configuration.GetValue<string>("EmailSettings:Message"));
                    string sub = new(_configuration.GetValue<string>("EmailSettings:Subjekti"));
                    var body = string.Format(bodyfromSettings.ToString(), emaildto.Name, emaildto.EmailFrom, emaildto.Subject, emaildto.Message);

                    var contact = _mapper.Map<ContactMessage>(emaildto);

                    if (contact != null)
                    {
                        contact.LayoutId = departamenti.LayoutId;
                        contact.PageId = departamenti.PageId;
                        contact.EmailTo = departamenti.Email;
                        contact.IsDraft = false;
                        contact.IsFavorite = false;
                        contact.IsDeleted = false;
                        contact.IsRead = false;
                        contact.IsSent = false;
                        contact.Created = DateTime.Now;
                        await _unitOfWork.ContactMessages.Create(contact);
                        await _unitOfWork.ContactMessages.Commit();
                    }

                    bool emailResponse = await SendMailPost(emaildto.EmailFrom, departamenti.Email, sub, body);

                    if (emailResponse)
                        return true;
                }

                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }


        [HttpGet]
        public async Task<bool> SendMailPost(string emailFrom, string emailTo, string subject, string body)
        {
            try
            {
                MailMessage msg = new();
                msg.From = new MailAddress(emailFrom);
                msg.To.Add(new MailAddress(emailTo));
                msg.Subject = subject;
                msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(body, null, MediaTypeNames.Text.Plain));
                msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(body, null, MediaTypeNames.Text.Html));

                SmtpClient smtpClient = new(_configuration.GetValue<string>("EmailSettings:Smtp"), Convert.ToInt32(_configuration.GetValue<int>("EmailSettings:Port")));
                System.Net.NetworkCredential credential = new(_configuration.GetValue<string>("EmailSettings:Email"), _configuration.GetValue<string>("EmailSettings:Password"));
                smtpClient.Credentials = credential;
                smtpClient.EnableSsl = true;
                await smtpClient.SendMailAsync(msg);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        [HttpPost]
        public async Task<bool> CheckRecapcha(string token)
        {
            rechapchaResponde responseEntity = null;
            string recapchaUrl = _configuration["EmailSettings:Recapcha_api"];
            string Recapcha_SECRET_KEY = _configuration["EmailSettings:Recapcha_SECRET_KEY"];
            HttpClientHandler clientHandler = new HttpClientHandler();
            var client = new HttpClient(clientHandler);
            client.DefaultRequestHeaders.TryAddWithoutValidation("content-type", "application/json");
            var uri = string.Format(recapchaUrl, Recapcha_SECRET_KEY, token);

            var task = client.GetAsync(uri).ContinueWith((response) =>
            {
                var result = response.Result;
                if (result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var readTask = result.Content.ReadAsAsync<rechapchaResponde>();
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
    }
}

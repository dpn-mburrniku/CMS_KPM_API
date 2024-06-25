using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CMS.WebAPI
{
    public class AutentifikimiApi : Attribute, IAuthorizationFilter
    {
        private const string API_KEY_HEADER_NAME = "Key";
        private int Minutes; 
        private readonly IConfiguration _configuration;
        public AutentifikimiApi(IConfiguration configuration)
        {
            _configuration = configuration;
            Minutes = Convert.ToInt32(_configuration.GetValue<int>("CorsPolicy:Minutes"));
        }
        public async void OnAuthorization(AuthorizationFilterContext context)
        {
            string getDate = context.RouteData.Values.Values.First().ToString();
            if (getDate != "GetDate")
            {
                var result = false;
                string submittedApiKey = GetSubmittedApiKey(context.HttpContext);

                if (string.IsNullOrEmpty(submittedApiKey))
                {
                    context.Result = new UnauthorizedResult();
                }
                else
                {
                    try
                    {
                        DateTime utcNow = DateTime.UtcNow;
                        //var DateFromFront = DateTimeOffset.Parse(DecryptStringAES(submittedApiKey)).UtcDateTime;
                        string decryptKey = DecryptStringAES(submittedApiKey);
                        var DateFromFront = DateTimeOffset.Parse(decryptKey);

                        if (utcNow <= DateFromFront.AddMinutes(Minutes))
                        {
                            result = true;
                        }

                        if (!result)
                        {
                            context.Result = new UnauthorizedResult();
                        }
                    }
                    catch (Exception)
                    {
                        context.Result = new UnauthorizedResult();
                    }
                }
            }
            
        }

        private static string GetSubmittedApiKey(HttpContext context)
        {
            return context.Request.Headers[API_KEY_HEADER_NAME];
        }        

        public string DecryptStringAES(string encryptedValue)
        {
            var keybytes = Encoding.UTF8.GetBytes("8056483646328769");
            var iv = Encoding.UTF8.GetBytes("8056483646328769");
            //DECRYPT FROM CRIPTOJS
            var encrypted = Convert.FromBase64String(encryptedValue);
            var decryptedFromJavascript = DecryptStringFromBytes(encrypted, keybytes, iv);
            return decryptedFromJavascript;
        }
        private static string DecryptStringFromBytes(byte[] cipherText, byte[] key, byte[] iv)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
            {
                throw new ArgumentNullException("cipherText");
            }
            if (key == null || key.Length <= 0)
            {
                throw new ArgumentNullException("key");
            }
            if (iv == null || iv.Length <= 0)
            {
                throw new ArgumentNullException("key");
            }
            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;
            // Create an RijndaelManaged object
            // with the specified key and IV.
            using (var rijAlg = new RijndaelManaged())
            {
                //Settings
                rijAlg.Mode = CipherMode.CBC;
                rijAlg.Padding = PaddingMode.PKCS7;
                rijAlg.FeedbackSize = 128;
                rijAlg.Key = key;
                rijAlg.IV = iv;
                // Create a decrytor to perform the stream transform.
                var decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);
                // Create the streams used for decryption.
                using (var msDecrypt = new MemoryStream(cipherText))
                {
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (var srDecrypt = new StreamReader(csDecrypt))
                        {
                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                            return plaintext;
                        }
                    }
                }
            }            
        }
    }
}

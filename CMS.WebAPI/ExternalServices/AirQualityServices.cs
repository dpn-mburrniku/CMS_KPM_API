using Microsoft.Extensions.Configuration;

namespace CMS.WebAPI.ExternalServices
{
    public class AirQualityServices : IAirQualityServices
    {
        private readonly string Link = "";
        private readonly string Token = "";
        private readonly IConfiguration _configuration;
        public AirQualityServices(IConfiguration configuration)
        {
            _configuration = configuration;
            Link = configuration["ExternalServices:AirQuality:Link"];
            Token = configuration["ExternalServices:AirQuality:Token"];
        }

        public AirQualityModel GetAirQualityStatistic()
        {
            var responseEntity = new AirQualityModel();

            var client = new HttpClient();

            var task = client.GetAsync(Link + "?token=" + Token).ContinueWith((response) =>
            {
                HttpResponseMessage result = response.Result;
                if (result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var readTask = result.Content.ReadAsAsync<AirQualityModel>();
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
            return responseEntity;
        }
    }
}

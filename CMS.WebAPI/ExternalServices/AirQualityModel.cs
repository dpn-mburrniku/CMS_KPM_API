namespace CMS.WebAPI.ExternalServices
{  
    public class Attribution
    {
        public string url { get; set; }
        public string name { get; set; }
        public string logo { get; set; }
    }

    public class City
    {
        public List<double> geo { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public string location { get; set; }
    }

    public class Data
    {
        public int aqi { get; set; }
        public int idx { get; set; }
        public List<Attribution> attributions { get; set; }
        public City? city { get; set; }
        public string dominentpol { get; set; }
    }

    public class AirQualityModel
    {
        public string status { get; set; }
        public Data data { get; set; }
    }
}

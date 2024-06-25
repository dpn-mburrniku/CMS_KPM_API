using Newtonsoft.Json;

namespace Entities.ErrorModel
{
    public class ErrorDetails
    {
        public bool Status { get; set; }       
        public string Message { get; set; }        
        public override string ToString() => JsonConvert.SerializeObject(this);
    }

}

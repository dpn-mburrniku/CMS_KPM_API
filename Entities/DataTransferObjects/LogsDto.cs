using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DataTransferObjects
{
    public class LogsDto
    {
        public long Id { get; set; }

        public string? UserId { get; set; }

        public string? Ip { get; set; }

        public string? UserName { get; set; }

        public string? Hostname { get; set; }

        public string? Controller { get; set; }

        public string? Action { get; set; }

        public string? ActionDescription { get; set; }

        public string? HttpMethod { get; set; }

        public string? Url { get; set; }

        public string? DescriptionTitle { get; set; }

        public string? Description { get; set; }

        public bool IsError { get; set; }

        public string? FormContent { get; set; }

        public string? Exception { get; set; }

        public DateTime? InsertedDate { get; set; }
    }
}

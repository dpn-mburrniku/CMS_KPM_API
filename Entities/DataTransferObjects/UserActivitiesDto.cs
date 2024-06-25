using CMS.API.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DataTransferObjects
{
    public class UserActivitiesDto
    {
        public long Id { get; set; }

        public string UserName { get; set; }

        public string? UserId { get; set; }

        public byte ActionType { get; set; }

        public string ActionTypeName
        {
            get
            {
                string result = Enum.GetName(typeof(ActionType), ActionType);
                return result;
            }
        }

        public string DescriptionSq { get; set; } = null!;

        public string DescriptionEn { get; set; } = null!;

        public string DescriptionSr { get; set; } = null!;

        public DateTime Date { get; set; }
    }
}

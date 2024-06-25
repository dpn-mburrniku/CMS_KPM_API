using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DataTransferObjects
{
    public class PostsInTagDto
    {
        public int PostTagId { get; set; }
        public virtual TagsDto? PostTag { get; set; } = null!;
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class CreateTweetDTO
    {
        required public int UserId { get; set; }
        required public string Content { get; set; }
    }
}

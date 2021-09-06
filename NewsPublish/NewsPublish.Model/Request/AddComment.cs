using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsPublish.Model.Request
{
    public class AddComment
    {
        public int NewsId { get; set; }
        public string Contents { get; set; }
    }
}

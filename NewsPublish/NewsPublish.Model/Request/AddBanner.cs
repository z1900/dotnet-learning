using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsPublish.Model.Request
{
    public class AddBanner
    {
        public string Image { get; set; }
        public string Url { get; set; }
        public string Remark { get; set; }
    }
}

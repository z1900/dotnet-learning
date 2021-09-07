﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsPublish.Model.Response
{
    public class CommentModel
    {
        public int Id { get; set; }
        public string NewsName { get; set; }
        public string Contents { get; set; }
        public DateTime AddTime { get; set; }
        public string Remark { get; set; }
        public string  Floor { get; set; }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsPublish.Model.Response
{
    public class NewsModel
    {
        public int Id { get; set; }
        public string ClassifyName { get; set; }
        public string Title { get; set; }
        public string Image { get; set; }
        public string Contents { get; set; }
        public DateTime PublishDate { get; set; }
        public int CommentCount { get; set; }
        public string Remark { get; set; }
    }
}

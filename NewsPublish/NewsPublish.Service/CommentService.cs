using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsPublish.Service
{
    public class CommentService
    {
        private Db _db;
        private NewsService _newsService;
        public CommentService(Db db, NewsService _newsService)
        {
            this._db = db;
        }
    }
}

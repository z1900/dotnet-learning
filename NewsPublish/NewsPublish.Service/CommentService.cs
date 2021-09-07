using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NewsPublish.Model.Entity;
using NewsPublish.Model.Request;
using NewsPublish.Model.Response;

namespace NewsPublish.Service
{
    public class CommentService
    {
        private Db _db;
        private NewsService _newsService;
        public CommentService(Db db, NewsService newsService)
        {
            this._db = db;
            this._newsService = newsService;
        }

        public ResponseModel AddComment(AddComment comment)
        {
            var news = _newsService.GetOneNews(comment.NewsId);
            if (news.code == 0)
            {
                return new ResponseModel { code = 0, result = "The news has not exist" };
            }
            var com = new NewsComment
            {
                AddTime = DateTime.Now,
                NewsId = comment.NewsId,
                Contents = comment.Contents
            };
            var i = _db.SaveChanges();
            if (i > 0)
            {
                return new ResponseModel
                {
                    code = 200,
                    result = "Add news comment Success",
                    data = new CommentModel
                    {
                        Contents = comment.Contents,
                        Floor = "#" + news.data.CommentCount + 1,
                        AddTime = DateTime.Now
                    }
                };
            }
            return new ResponseModel { code = 0, result = "Add news Comment failed" };
        }

        public ResponseModel DeleteComment(int id)
        {
            var comment = _db.NewsComment.Find(id);
            if (comment == null)
            {
                return new ResponseModel { code = 0, result = "The comment is not exist" };
            }
             _db.NewsComment.Remove(comment);
            var i = _db.SaveChanges();
            if (i > 0)
            {
                return new ResponseModel
                {
                    code = 200,
                    result = "Delete the comment success"
                };
            }
            return new ResponseModel
            {
                code = 0,
                result = "Delete the comment failed"
            };
        }

        public ResponseModel GetCommentList(Expression<Func<NewsComment, bool>> where)
        {
            var comments = _db.NewsComment.Include("News").Where(where).OrderBy(c => c.AddTime).ToList();
            var response = new ResponseModel
            {
                code = 200,
                result = "Get the news comment list success"
            };
            response.data = new List<CommentModel>();
            int floor = 1;
            foreach(var comment in comments)
            {
                response.data.Add(new CommentModel
                {
                    Id = comment.Id,
                    NewsName = comment.News.Title,
                    Contents = comment.Contents,
                    AddTime = comment.AddTime,
                    Remark = comment.Remark,
                    Floor = "#" + floor
                });
                floor++;
            }
            response.data.Reverse();
            return response;
        }
    }
}

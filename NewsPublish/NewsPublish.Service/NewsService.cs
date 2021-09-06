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
    public class NewsService
    {
        private Db _db;
        public NewsService(Db db)
        {
            this._db = db;
        }

        public ResponseModel AddNewsClassify(AddNewsClassify newsClassify)
        {
            var exist = _db.NewsClassify.FirstOrDefault(c => c.Name == newsClassify.Name) != null;
            if (!exist)
            {
                return new ResponseModel() { code = 0, result = "The classify has already exist" };
            }
            var classify = new NewsClassify() { Name = newsClassify.Name, Sort = newsClassify.Sort, Remark = newsClassify.Remark };
            _db.NewsClassify.Add(classify);
            int i = _db.SaveChanges();
            if (i>0)
            {
                return new ResponseModel() { code = 200, result = "Add news classify success" };
            }
            else
            {
                return new ResponseModel() { code = 0, result = "Add news classify failed" };
            }
        }

        public ResponseModel GetOneNewsClassify(int id)
        {
            var classify = _db.NewsClassify.Find(id);
            if (classify == null)
            {
                return new ResponseModel() { code = 0, result = "The classify is not exist" };
            }
            else
            {
                return new ResponseModel()
                {
                    code = 200,
                    result = "Get classify success",
                    data = new NewsClassifyModel()
                    {
                        Id = classify.Id,
                        Name = classify.Name,
                        Sort = classify.Sort,
                        Remark = classify.Remark
                    }
                };
            }
        }

        private NewsClassify GetOneNewsClassify(Expression<Func<NewsClassify,bool>> where)
        {
            return _db.NewsClassify.FirstOrDefault(where);
        }

        public ResponseModel EidtNewsClassify(EditNewsClassify newsClassify)
        {
            var classify = this.GetOneNewsClassify(c => c.Id == newsClassify.Id);
            if (classify == null)
            {
                return new ResponseModel() { code = 0, result = "The classify is not exist" };
            }
            else
            {
                classify.Name = newsClassify.Name;
                classify.Sort = newsClassify.Sort;
                classify.Remark = newsClassify.Remark;
                _db.NewsClassify.Update(classify);
                int i = _db.SaveChanges();
                if (i > 0)
                {
                    return new ResponseModel() { code = 200, result = "Edit news classify success" };
                }
                else
                {
                    return new ResponseModel() { code = 0, result = "Edit news classify failed" };
                }
            }
        }

        public ResponseModel GetNewsClassifyList()
        {
            var classifys = _db.NewsClassify.OrderByDescending(c => c.Sort).ToList();
            var response = new ResponseModel() { code = 200, result = "Get news classify list success" };
            response.data = new List<NewsClassifyModel>();
            foreach(var classify in classifys)
            {
                response.data.Add(new NewsClassifyModel()
                {
                    Id = classify.Id,
                    Name = classify.Name,
                    Sort = classify.Sort,
                    Remark = classify.Remark
                });
            }
            return response;
        }

        public ResponseModel AddNews(AddNews news)
        {
            var classify = this.GetOneNewsClassify(c => c.Id == news.NewsClassifyId);
            if (classify == null)
            {
                return new ResponseModel() { code = 0, result = "The classify is not exist" };
            }
            else
            {
                var n = new News()
                {
                    NewsClassifyId = news.NewsClassifyId,
                    Title = news.Title,
                    Image = news.Image,
                    Contents=news.Contents,
                    PublishDate = DateTime.Now,
                    Remark = news.Remark
                };
                _db.News.Add(n);
                int i = _db.SaveChanges();
                if (i>0)
                {
                    return new ResponseModel() { code = 200, result = "Add news success" };
                }
                else
                {
                    return new ResponseModel() { code = 0, result = "Add news failed" };
                }
            }
        }

        public ResponseModel GetOneNews(int id)
        {
            var news = _db.News.Include("NewsClassify").Include("NewsComment").FirstOrDefault(c => c.Id == id);
            if (news == null)
            {
                return new ResponseModel() { code = 0, result = "The news is not exist" };
            }
            var response = new ResponseModel() { code = 200, result = "Get news success" };
            response.data = new NewsModel()
            {
                Id = news.Id,
                Title = news.Title,
                Image = news.Image,
                Contents = news.Contents,
                ClassifyName = news.NewsClassify.Name,
                PublishDate = news.PublishDate,
                CommentCount=news.NewsComments.Count,
                Remark = news.Remark
            };
            return response;
        }

        public ResponseModel DelOneNews(int id)
        {
            var news = _db.News.FirstOrDefault(c => c.Id == id);
            if(news == null)
            {
                return new ResponseModel() { code = 0, result = "The news is not exist" };
            }
            _db.News.Remove(news);
            int i = _db.SaveChanges();
            if (i > 0)
            {
                return new ResponseModel() { code = 200, result = "Delete news success" };
            }
            else
            {
                return new ResponseModel() { code = 0, result = "Delete news failed" };
            }
        }

        public ResponseModel NewsPageQuery(int pageSize, int pageIndex, out int total, List<Expression<Func<News, bool>>> where)
        {
            var list = _db.News.Include("NewsClassify").Include("NewsComment");
            foreach(var item in where)
            {
                list = list.Where(item);
            }
            total = list.Count();
            var pageData = list.OrderByDescending(c => c.PublishDate).Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
            var response = new ResponseModel() { code = 200, result = "The news get success by pagination" };
            response.data = new List<NewsModel>();
            foreach(var news in pageData)
            {
                response.data.Add(new NewsModel()
                {
                    Id = news.Id,
                    Title = news.Title,
                    Image = news.Image,
                    Contents = news.Contents,
                    ClassifyName = news.NewsClassify.Name,
                    PublishDate = news.PublishDate,
                    CommentCount = news.NewsComments.Count(),
                    Remark = news.Remark
                });
            }
            return response;
        }

        public ResponseModel GetNewsList(Expression<Func<News, bool>> where, int topCount)
        {
            var list = _db.News.Include("NewsClassify").Include("NewsComment").Where(where).OrderByDescending(c => c.PublishDate)
                .Take(topCount).ToList();
            var response = new ResponseModel()
            {
                code = 200,
                result = "Get the news by condition success"
            };
            response.data = new List<NewsModel>();
            foreach(var news in list)
            {
                response.data.Add(new NewsModel()
                {
                    Id = news.Id,
                    Title = news.Title,
                    Image = news.Image,
                    Contents = news.Contents,
                    ClassifyName = news.NewsClassify.Name,
                    PublishDate = news.PublishDate,
                    CommentCount = news.NewsComments.Count(),
                    Remark = news.Remark
                });
            }
            return response;
        }

        public ResponseModel GetNewCommentNews(Expression<Func<News, bool>> where, int topCount)
        {
            var newsIds = _db.NewsComment.OrderByDescending(c => c.AddTime).GroupBy(c => c.NewsId).Select(c => c.Key).Take(topCount);
            var list = _db.News.Include("NewsClassify").Include("NewsComment").Where(c => newsIds.Contains(c.Id)).OrderByDescending(c => c.PublishDate).ToList();
            var response = new ResponseModel()
            {
                code = 200,
                result = "Get the latest news success"
            };
            response.data = new List<NewsModel>();
            foreach (var news in list)
            {
                response.data.Add(new NewsModel()
                {
                    Id = news.Id,
                    Title = news.Title,
                    Image = news.Image,
                    Contents = news.Contents,
                    ClassifyName = news.NewsClassify.Name,
                    PublishDate = news.PublishDate,
                    CommentCount = news.NewsComments.Count(),
                    Remark = news.Remark
                });
            }
            return response;
        }

        public ResponseModel GetSearchOneNews(Expression<Func<News, bool>> where)
        {
            var news = _db.News.Where(where).FirstOrDefault();
            if (news == null)
            {
                return new ResponseModel { code = 0, result = "Search news faided" };
            }
            return new ResponseModel { code = 200, result = "Search news success", data = news.Id };
        }

        public ResponseModel GetNewsCount(Expression<Func<News, bool>> where)
        {
            var count = _db.News.Where(where).Count();
            return new ResponseModel { code = 200, result = "Get news count success", data = count };
        }

        public ResponseModel GetRecommendNewsList(int newsId)
        {
            var news = _db.News.FirstOrDefault(c => c.Id == newsId);
            if (news == null)
            {
                return new ResponseModel { code = 0, result = "Get recomment newsList failed" };
            }
            var newsList = _db.News.Include("NewsComment").Where(c => c.NewsClassifyId == news.NewsClassifyId && c.Id != newsId).OrderByDescending(c => c.PublishDate)
                .OrderByDescending(c => c.NewsComments.Count()).Take(6).ToList();
            var response = new ResponseModel { code = 200, result = "Get recomment news list sccess" };
            response.data = new List<NewsModel>();
            foreach(var n in newsList)
            {
                response.data.Add(new NewsModel
                {
                    Id = n.Id,
                    Title = n.Title,
                    Image = n.Image,
                    Contents = n.Contents,
                    ClassifyName = n.NewsClassify.Name,
                    PublishDate = n.PublishDate,
                    CommentCount = n.NewsComments.Count(),
                    Remark = n.Remark
                });
            }
            return response;
        }
    }
}

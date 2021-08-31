using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
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
    }
}

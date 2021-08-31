using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewsPublish.Model.Entity;
using NewsPublish.Model.Request;
using NewsPublish.Model.Response;

namespace NewsPublish.Service
{
    public class BannerService
    {
        private Db _db;
        public BannerService(Db db)
        {
            this._db = db;
        }
        public ResponseModel AddBanner(AddBanner banner)
        {
            var ba = new Banner {
                AddTime = DateTime.Now,
                Image = banner.Image,
                Url = banner.Url,
                Remark = banner.Remark
            };
            _db.Banner.Add(ba);
            int i = _db.SaveChanges();
            if (i > 0)
            {
                return new ResponseModel { code = 200, result = "Add banner succesful" };
            }
            else
            {
                return new ResponseModel { code = 0, result = "Add banner failed" };
            }
        }
        public ResponseModel GetBannerList()
        {
            var response = new ResponseModel();
            response.code = 200;
            response.result = "Get banner list successful";
            response.data = new List<BannerModel>();
            var banners = _db.Banner.ToList().OrderByDescending(c => c.AddTime);
            foreach (var banner in banners)
            {
                response.data.Add(new BannerModel() { 
                    Id = banner.Id,
                    Image = banner.Image,
                    Url = banner.Url,
                    Remark = banner.Remark
                });
            }
            return response;
        }
        public ResponseModel DeleteBanner(int bannerId)
        {
            var banner = _db.Banner.Find(bannerId);
            if (banner == null)
            {
                return new ResponseModel() { code = 200, result = string.Format("banner {0} is not exist", bannerId) };
            }
            else
            {
                _db.Banner.Remove(banner);
                int i = _db.SaveChanges();
                if (i > 0)
                {
                    return new ResponseModel { code = 200, result = "Delete banner succesful" };
                }
                else
                {
                    return new ResponseModel { code = 0, result = "Delete banner failed" };
                }
            }
        } 
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CNWeb.Models;
using Microsoft.Ajax.Utilities;
using PagedList;

namespace CNWeb.Controllers
{
    public class ProductController : Controller
    {
        // GET: Product

        DBContext db = new DBContext();
        public ActionResult Index(int id, bool check, int? page)
        {
            List<Product> lsProduct = new List<Product>();
            if (check)
            {
                lsProduct = db.Product_category.Find(id).Products.Where(x => x.sex == 1).ToList();
            }
            else
            {
                lsProduct = db.Product_category.Find(id).Products.Where(x => x.sex == 0).ToList();
            }


            ViewBag.Sizes = db.Sizes.ToList();
            ViewBag.Colors = db.Colors.ToList();
            //Thực hiện chức năng phân trang
            //biến PageSize số sản phẩm trên trang
            int pageSize = 4;
            //số trang hiện tại 
            int pageNumber = (page ?? 1);
            ViewBag.MaDanhMuc = id;
            ViewBag.Sex = check;
            var list = lsProduct.OrderBy(n => n.id).ToPagedList(pageNumber, pageSize);
            return View(list);
        }


        public ActionResult SearchPrice(int max, int min, int dm, int sex)
        {
            List<Product> x = db.Products.Where(n => n.price > min && n.price < max && n.product_category_id == dm && n.sex == sex).ToList();
            return PartialView(x);
        }

        public ActionResult Item(Var x)
        {
            return PartialView(x);
        }

    }
}
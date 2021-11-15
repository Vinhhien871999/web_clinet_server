using CNWeb.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace CNWeb.Controllers
{
    public class ProductDetailController : Controller
    {
        // GET: ProductDetail
        private DBContext db = new DBContext();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Single(int id)
        {
            Product sp = db.Products.Find(id);
            ViewBag.ProductDetail = sp;
            var lsCmt= db.Comments.Where(n => n.product_id == id&& n.is_active==1).ToList();

            ViewBag.SameProduct = db.Products.Where(n => n.product_category_id == sp.product_category_id);
            return View(lsCmt);
        }

        [HttpPost]
        public ActionResult Single(HttpPostedFileBase file,int rating1, string detail,int product_id)
        {
            User user=Session["TaiKhoan"] as User;
            if (user == null) return Redirect("/dang-nhap");

            string path=null;
            string pre_path = null;

            if (file != null)
            {
                pre_path = DateTime.Now.ToString("yymmssff") + Path.GetFileName(file.FileName);
                 path =  Path.Combine(Server.MapPath("~/images"),pre_path); 
                
                file.SaveAs(path);

            }
            Comment cm = new Comment();
            cm.author = user.name;
            cm.content = detail;
            
            cm.path_img = file!=null ? pre_path: null;
            cm.product_id = product_id;
            cm.is_active = 1;
            cm.create_at = DateTime.Now;
            cm.rating = rating1;
            
            db.Comments.Add(cm);

            db.SaveChanges();
                       
            return RedirectToAction("Single",new { id = product_id });
        }
        [HttpPost]
        public ActionResult Reply(Reply rep,int comment_id,int id_sp)
        {
            User user = Session["TaiKhoan"] as User;
            if (user==null)
            {
                return Redirect("/dang-nhap");
            }

            rep.author = user.name;
            rep.comment_id = comment_id;
            rep.create_at = DateTime.Now;

            db.Replies.Add(rep);
            db.SaveChanges();

            return RedirectToAction("Single", new { id = id_sp });
        }
    }
}
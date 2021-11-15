using CNWeb.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CNWeb.Controllers
{
    public class DTH_TestController : Controller
    {
        DBContext db = new DBContext();
        public ActionResult sanpham()
        {

            var listSP = db.Products.ToList();
            return PartialView(listSP);
        }
        
        [HttpGet]
        public ActionResult ThemMoi()
        {
            ViewBag.product_category_id = new SelectList(db.Product_category.OrderBy(n => n.name), "id", "name");
            ViewBag.Size = new SelectList(db.Sizes.OrderBy(n => n.type), "id", "type");
            ViewBag.Color = new SelectList(db.Colors.OrderBy(n => n.type), "id", "type");
            return View();
        }
        
        [HttpPost]
        [ValidateInput(false)] // Khi chèn vào code bị sai
        //Product product, HttpPostedFileBase fileUpload
        public ActionResult ThemMoi(Product sp, int Color, int Size, HttpPostedFileBase[] photo)
        {

            ViewBag.product_category_id = new SelectList(db.Product_category.OrderBy(n => n.name), "id", "name");
            ViewBag.Size = new SelectList(db.Sizes.OrderBy(n => n.type), "id", "type");
            ViewBag.Color = new SelectList(db.Colors.OrderBy(n => n.type), "id", "type");
            //Kiểm tra hình ảnh tồn tại

            var path = "";
            var filename = "";
            var lsphoto = "";
            foreach (HttpPostedFileBase hinh in photo)
            {
                if (hinh.ContentLength > 0)
                {
                    //Lấy tên hình ảnh
                    filename = Path.GetFileName(hinh.FileName);
                    path = Path.Combine(Server.MapPath("~/images"), filename);
                    if (System.IO.File.Exists(path))
                    {
                        ViewBag.upload = "Hình đã tồn tại";
                        return View();
                    }
                    else
                    {
                        lsphoto += (filename + ";");
                        hinh.SaveAs(path);
                    }
                    //}
                }
            }

            sp.create_at = DateTime.Now;
            sp.photo = lsphoto;
            //Add size
            Size size = db.Sizes.SingleOrDefault(n => n.id == Size);
            sp.Sizes.Add(size);
            //Add color
            Color color = db.Colors.SingleOrDefault(n => n.id == Color);
            sp.Colors.Add(color);
            //sp.create_by = (Session["TaiKhoan"] as User).name;

            db.Products.Add(sp);
            db.SaveChanges();

            return RedirectToAction("sanpham");
        }

        // Hiển thị thông tin sản phẩm
        public ActionResult HienThi(int id)
        {
            // Lấy ra sản phẩm theo id
            Product product = db.Products.SingleOrDefault(n => n.id == id);
            if (product == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(product);
        }
       
        // Xóa sản phẩm
        [HttpGet]
        public ActionResult Xoa(int id)
        {

            Product pr = db.Products.SingleOrDefault(n => n.id == id);
            pr.Sizes.Clear();
            db.SaveChanges();
            pr.Colors.Clear();
            db.SaveChanges();
            db.Products.Remove(pr);
            db.SaveChanges();

            return RedirectToAction("sanpham");

        }



        // Chỉnh sửa sản phẩm
        [HttpGet]
        public ActionResult ChinhSua(int id)
        {
            // Lấy ra sản phẩm theo id
            Product product = db.Products.SingleOrDefault(n => n.id == id);
            if (product == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(product);
        }
        
        [HttpPost]
        [ValidateInput(false)] // Khi chèn vào code bị sai
        public ActionResult ChinhSua(Product product)
        {

            if (ModelState.IsValid)
            {
                // Thực hiện cập nhật trong model
                Product pd = db.Products.SingleOrDefault(n => n.id == product.id);

                if (pd == null) return null;

                pd.product_name = product.product_name;
                pd.price = product.price;
                pd.rating = product.rating;                
                db.SaveChanges();
            }
            return RedirectToAction("sanpham");
        }
         
       
    }
}
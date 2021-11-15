using CNWeb.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CNWeb.Controllers
{
    [Authorize(Roles ="Admin,Shipper")]
    public class AdminController : Controller
    {
        // GET: Admin
        DBContext db = new DBContext();

        
        public ActionResult Index()
        {
            return PartialView();
        }

        [Authorize(Roles = "Admin")]
        public ActionResult sanpham()
        {

            var listSP = db.Products.ToList();
            return PartialView(listSP);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult ThemMoi()
        {
            //    ViewBag.product_category_id = new SelectList(db.Product_category.ToList(), "category_type_id", "name");
            //db.SaveChanges();
            ViewBag.product_category_id = new SelectList(db.Product_category.OrderBy(n => n.name), "id", "name");
            ViewBag.Size = new SelectList(db.Sizes.OrderBy(n => n.type), "id", "type");
            ViewBag.Color = new SelectList(db.Colors.OrderBy(n => n.type), "id", "type");
            return View();
        }

        [Authorize(Roles = "Admin")]
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

        [Authorize(Roles = "Admin")]
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

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateInput(false)] // Khi chèn vào code bị sai
        public ActionResult ChinhSua(Product product)
        {
            // Cách cập nhật cách đối tượng không theo model
            /*Product product1 = db.Products.SingleOrDefault(n => n.id == product.id);
            product1.Product_category = product.Product_category;
            db.SaveChanges();*/

            if (ModelState.IsValid)
            {
                // Thực hiện cập nhật trong model
                db.Entry(product).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("sanpham");
        }

        [Authorize(Roles = "Admin")]
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

        [Authorize(Roles = "Admin")]
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

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Xoa")]
        public ActionResult XacNhanXoa(int id)
        {
            Product product = db.Products.SingleOrDefault(n => n.id == id);
            if (product == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            db.Products.Remove(product);
            db.SaveChanges();
            return RedirectToAction("sanpham");

        }

        [Authorize(Roles = "Admin")]
        // Hiển thị danh sách đơn hàng
        public ActionResult DonHang()
        {
            var listDonHang = db.formPayments.ToList();
            return PartialView(listDonHang);
        }

        [Authorize(Roles = "Admin")]
        // Hiển thị chi tiết của đơn hàng
        public ActionResult ChiTietDonHang(int id)
        {
            var listSP = db.detailPayments.Where(n => n.id_form == id).ToList();
            return PartialView(listSP);
        }

        [Authorize(Roles = "Admin")]
        // Xác nhận đơn hàng
        public ActionResult XacNhanDonHang(int id)
        {
            formPayment form = db.formPayments.SingleOrDefault(n => n.id == id);
            if (form == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            if (form.status == "Chờ xác nhận" || form.status == "Đơn đã hủy")
            {
                form.status = "Đã xác nhận";
                db.SaveChanges();
                return RedirectToAction("DonHang");
            }
            return RedirectToAction("DonHang");
        }

        [Authorize(Roles = "Admin")]
        // Hủy đơn hàng
        public ActionResult HuyDonHang(int id)
        {
            formPayment form = db.formPayments.SingleOrDefault(n => n.id == id);
            if (form == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            if (form.status == "Đã xác nhận")
            {
                form.status = "Đơn đã hủy";
                db.SaveChanges();
                return RedirectToAction("DonHang");
            }
            return RedirectToAction("DonHang");
        }

        [Authorize(Roles = "Shipper")]
        // Hiển thị danh sách đơn hàng đã xác nhận cho shipper
        public ActionResult VanChuyen()
        {
            var listDonXacNhan = db.formPayments.Where(n => n.status == "Đã xác nhận" || n.status == "Đang vận chuyển" || n.status == "Đã giao hàng").ToList();
            return PartialView(listDonXacNhan);
        }

        [Authorize(Roles = "Shipper")]
        // Shipper nhận đơn hàng
        public ActionResult NhanDon(int id)
        {
            formPayment form = db.formPayments.SingleOrDefault(n => n.id == id);
            if (form == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            if (form.status == "Đã xác nhận")
            {
                form.status = "Đang vận chuyển";
                db.SaveChanges();
                return RedirectToAction("VanChuyen");
            }
            return RedirectToAction("VanChuyen");
        }

        [Authorize(Roles = "Shipper")]
        // Shipper hủy đơn hàng
        public ActionResult HuyDon(int id)
        {
            formPayment form = db.formPayments.SingleOrDefault(n => n.id == id);
            if (form == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            if (form.status == "Đang vận chuyển")
            {
                form.status = "Đã xác nhận";
                db.SaveChanges();
                return RedirectToAction("VanChuyen");
            }
            return RedirectToAction("VanChuyen");
        }

        [Authorize(Roles = "Shipper")]
        // Shipper đã giao hàng
        public ActionResult GiaoHang(int id)
        {
            formPayment form = db.formPayments.SingleOrDefault(n => n.id == id);
            List<detailPayment> detail = db.detailPayments.Where(n => n.id_form == id).ToList();
            int[] a = new int[10];
            int count = detail.Count();
            int i = 0;
            foreach (var item in detail)
            {
                //a[i] = item.id_product;
                //i++;
                Product pr = db.Products.SingleOrDefault(n => n.id == item.id_product);
                pr.stock -= item.count;
                db.SaveChanges();
            }

            if (form == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            if (form.status == "Đang vận chuyển")
            {
                form.status = "Đã giao hàng";
                db.SaveChanges();
                return RedirectToAction("VanChuyen");
            }
            return RedirectToAction("VanChuyen");
        }

        [Authorize(Roles = "Admin")]
        // Hiển thị danh sách người dùng
        public ActionResult TaiKhoan()
        {

            var listTaiKhoan = db.Users.Where(n=>n.is_active==1).ToList();
            return PartialView(listTaiKhoan);
        }

        [Authorize(Roles = "Admin")]
        // Chỉnh sửa tài khoản người dùng
        [HttpGet]
        public ActionResult SuaTaiKhoan(int id)
        {
            // Lấy ra tài khoản theo id
            User user = db.Users.SingleOrDefault(n => n.id == id);
            if (user == null)
            {
                Response.StatusCode = 404;                
            }
            return View(user);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateInput(false)] // Khi chèn vào code bị sai
        public ActionResult SuaTaiKhoan(User _user)
        {
            
            if (ModelState.IsValid)
            {
                // Thực hiện cập nhật trong model
                User user = new User();
                user = db.Users.Find(_user.id);
                user.name = _user.name;
                user.address = _user.address;
                user.point = _user.point;
                user.phone = _user.phone;
                db.SaveChanges();
            }
            return RedirectToAction("TaiKhoan");
        }

        [Authorize(Roles = "Admin")]
        // Hiển thị thông tin tài khoản người dùng
        public ActionResult ChiTietTaiKhoan(int id)
        {
            // Lấy ra sản phẩm theo id
            User user = db.Users.SingleOrDefault(n => n.id == id);
            if (user == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(user);
        }

        [Authorize(Roles = "Admin")]
        // Xóa tài khoản
        [HttpGet]
        public ActionResult XoaTaiKhoan(int id)
        {   
            // Lấy ra sản phẩm theo id
            User user = db.Users.SingleOrDefault(n => n.id == id);
            if (user == null)
            {
                Response.StatusCode = 404;                
                return null;
            }
            user.is_active = 0;
            db.SaveChanges();
            return RedirectToAction("TaiKhoan");
        
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("XoaTaiKhoan")]
        public ActionResult XacNhanXoaTaiKhoan(int id)
        {
            User user = db.Users.SingleOrDefault(n => n.id == id);
            if (user == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            db.Users.Remove(user);
            db.SaveChanges();
            return RedirectToAction("TaiKhoan");

        }

        [Authorize(Roles = "Admin")]
        public void test(int id_user, int perr)
        {
            Permission per = db.Permissions.SingleOrDefault(n => n.id == perr);

            User user = db.Users.SingleOrDefault(n => n.id == id_user);
            user.Permissions.Add(per);
        }

        //   Thêm quyền cho tài khoản
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult ThemQuyenTaiKhoan()
        {
            ViewBag.Permission_id = new SelectList(db.Permissions.ToList(), "id", "tenQuyen");
            db.SaveChanges();
            return PartialView();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ThemQuyenTaiKhoan(FormCollection f)
        {
            int id_TaiKhoan = Int32.Parse(f["idTaiKhoan"].ToString());
            int id_Per = Int32.Parse(f["Permission_id"].ToString());
            User user = db.Users.SingleOrDefault(n => n.id == id_TaiKhoan);
            Permission per = db.Permissions.SingleOrDefault(n => n.id == id_Per);
            ViewBag.Permission_id = new SelectList(db.Permissions.ToList(), "id", "tenQuyen");

            foreach (var item in user.Permissions)
            {
                if (item.tenQuyen == per.tenQuyen)
                {
                    ViewBag.Quyen = "Tài khoản đã có quyền này !";
                    return PartialView();
                }
            }
            user.Permissions.Add(per);
            db.SaveChanges();
            return RedirectToAction("TaiKhoan");
        }

        // Xóa quyền tài khoản
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult XoaQuyenTaiKhoan()
        {
            ViewBag.Permission_id = new SelectList(db.Permissions.ToList(), "id", "tenQuyen");
            db.SaveChanges();
            return PartialView();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult XoaQuyenTaiKhoan(FormCollection f)
        {
            int id_TaiKhoan = Int32.Parse(f["idTaiKhoan"].ToString());
            int id_Per = Int32.Parse(f["Permission_id"].ToString());
            User user = db.Users.SingleOrDefault(n => n.id == id_TaiKhoan);
            Permission per = db.Permissions.SingleOrDefault(n => n.id == id_Per);
            ViewBag.Permission_id = new SelectList(db.Permissions.ToList(), "id", "tenQuyen");

            foreach (var item in user.Permissions)
            {
                if (item.tenQuyen == per.tenQuyen)
                {
                    user.Permissions.Remove(per);
                    db.SaveChanges();
                    return RedirectToAction("TaiKhoan");
                }
            }
            ViewBag.Quyen = "Tài khoản không có quyền này !";
            return PartialView();

        }
    }
}
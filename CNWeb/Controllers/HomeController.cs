using CNWeb.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using CaptchaMvc.HtmlHelpers;
using CaptchaMvc;
using System.Data.Entity.Core.Metadata.Edm;
using System.IO;
using System.Diagnostics;
using System.Web.Security;
using System.Security.Cryptography;
using System.Text;
using Common;

namespace BTL_Web.Controllers
{
    public class HomeController : Controller
    {
        public static string KEY = "huuhoang";
        private DBContext db = new DBContext();
        private static string Randomcode = "aaaaaa";
        private static string mail = "";
        public ActionResult Index()
        {
            var listNewMen = db.Database.SqlQuery<Product>("dbo.getProductNewMen").ToList();
            var listNewGirl = db.Database.SqlQuery<Product>("dbo.getProductNewGirl").ToList();
            var listNewPart = db.Database.SqlQuery<Product>("dbo.getProductNewPart").ToList();

            ViewBag.listNewMan = listNewMen;
            ViewBag.listNewGirl = listNewGirl;
            ViewBag.listNewPart = listNewPart;
            return View();

        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            //ViewBag.Contacts = db.Contacts.ToList();

            return View();
        }

        [HttpPost]
        public ActionResult Contact(Contact ct)
        {

            db.Contacts.Add(ct);
            db.SaveChanges();
            return View();
        }

        public ActionResult Blog()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }


        public ActionResult PayMent()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Single()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Womens()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult pay()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }

        public ActionResult Login()
        {
            //return RedirectToAction("Index");
            return View();
        }
        public ActionResult Login1()
        {
            //return RedirectToAction("Index");
            return View();
        }
        [HttpPost]
        public ActionResult Login(FormCollection f)
        {
            //Kiểm tra tên đăng nhập mật khẩu
            string email = f["email"].ToString();
            string password = f["password"].ToString();
            password = Encrypt(password);

            User user = db.Users.SingleOrDefault(x => x.email.Equals(email) && x.passwork.Equals(password)&&x.is_active==1);
            if (user != null)
            {
                Session["TaiKhoan"] = user;
                //kiểm tra trước đó người dùng đã có sản phẩm nào trong giỏ hanng chưa??
                //Cart cartUser = db.Carts.SingleOrDefault(n => n.id == user.cart_id);

                Cart cart = Session["Cart"] as Cart;
                if (cart != null)
                {

                    //Thêm tất cả  các mặt hàng có trong session vào cart của người dùng 

                    foreach (Cart_detail CT in cart.Cart_detail)
                    {
                        Cart_detail spCheck = db.Cart_detail.SingleOrDefault(n => n.product_id == CT.product_id && n.cart_id == user.cart_id&&n.id_size==CT.id_size&&n.id_color==CT.id_color);
                        if (spCheck != null)
                        {
                            spCheck.quantity += CT.quantity;
                            //spCheck.cart_id = user.cart_id ?? default(int);

                            db.SaveChanges();
                            //Xử lý nếu hết hàng
                        }
                        else
                        {
                            object[] parameters =
                            {
                                new SqlParameter("cart_id", user.cart_id),
                                new SqlParameter("product_id", CT.product_id),
                                new SqlParameter("id_color", CT.id_color),
                                new SqlParameter("id_size", CT.id_size),
                                new SqlParameter("quantity", CT.quantity)
                            };
                            db.Database.ExecuteSqlCommand("dbo.insertCartDetail @cart_id,@product_id,@id_color,@id_size,@quantity", parameters);

                            db.SaveChanges();
                        }


                    }

                    //Cart cartDetail = db.Carts.SingleOrDefault(n => n.id == cart.id);
                    //cartDetail.user_id = user.id;
                    //db.SaveChanges();
                }
                //Session["Cart"] = null;
                Session["Cart"] = db.Carts.SingleOrDefault(n => n.id == user.cart_id);

                //Truy cập tất cả các quyền của User
                String Quyen = "";
                List<Permission> lsQuyen = user.Permissions.ToList();
                foreach(var item in lsQuyen)
                {
                    Quyen += item.tenQuyen + ",";//Quyền được lưu dưới dạng chuỗi 
                }

                Quyen = Quyen.Substring(0, Quyen.Length - 1);
                PhanQuyen(email,Quyen);
                return RedirectToAction("Index");
                //return Content("<script>window.location.reload();</script>");
            }
            ViewBag.ThongBao = "Tài khoản hoặc mật khẩu không đúng!";
            return View();
            //return Content("Tài khoản hoặc mật khẩu không đúng!");
        }


        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(User user,string confirm_password)
        {
            if(user.passwork!= confirm_password)
            {
                ViewBag.ThongBao = "Mật Khẩu Xác Nhận không khớp";
                return View();
            }
            //Kiem tra captcha hop le
            int check = -1;
             check = db.Users.Where(n => n.email == user.email).Count();
            
            if (check > 0) {
                ViewBag.ThongBao = "Địa chỉ Email đã tồn tại";
                return View();
            }
            

            if (this.IsCaptchaValid("Captcha is not valid"))
            {
                if (ModelState.IsValid)
                {

                    ViewBag.ThongBao = "Đăng ký thành công";
                    //Thêm User vào database;

                    Permission per = db.Permissions.SingleOrDefault(n => n.id == 2);                    

                    ////Tạo cho User 1 giỏ hàng
                    //user.Permissions.Add(per);
                    user.create_at = DateTime.Now;
                    user.passwork = Encrypt(user.passwork);
                    user.point = 0;
                    user.is_active = 1;
                    Cart cart = new Cart();
                    db.Carts.Add(cart);
                    db.SaveChanges();

                    user.cart_id = cart.id;

                    db.Users.Add(user);
                    user.Permissions.Add(per);
                    db.SaveChanges();
                    
                    

                    //Tạo 1 địa chỉ  nhân hàn mặc định
                    Receiver receiver = new Receiver();
                    receiver.name = user.name;
                    receiver.address = user.address;
                    receiver.phone = user.phone;
                    receiver.id_user = user.id;
                    db.Receivers.Add(receiver);
                    db.SaveChanges();

                }
                else
                {
                    ViewBag.ThongBao = "Thêm thất bại";
                }


                return View();

            }
            ViewBag.ThongBao = "Sai Mã Captcha";
            return View();
        }

        public ActionResult Logout()
        {
            Session["TaiKhoan"] = null;
            Session["Cart"] = null;
            FormsAuthentication.SignOut();
            return RedirectToAction("Index");
        }
        public ActionResult Hello(String Hoten)
        {

            ViewBag.Hoten = Hoten;
            return View();
        }

        [ChildActionOnly]
        public ActionResult BannerTop()
        {
            return PartialView(db.Product_category.ToList());
        }

        [HttpPost]
        public ActionResult Find(string search)
        {
            ViewBag.Search = search;
            var list = db.Products.Where(x => x.product_name.Contains(search)).ToList();


            return View(list);
        }
        [ChildActionOnly]
        public ActionResult PartiaViewTabProduct()
        {

            return PartialView();
        }

        public ActionResult TestFile()
        {

            return View();
        }

        [HttpPost]       
        public ActionResult TestFile(HttpPostedFileBase path_img)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (path_img != null)
                    {
                        string path = Path.Combine(Server.MapPath("~/images"), Path.GetFileName(path_img.FileName));
                        path_img.SaveAs(path);

                    }
                    ViewBag.FileStatus = "File uploaded successfully.";
                }
                catch (Exception)
                {

                    ViewBag.FileStatus = "Error while file uploading.";
                }

            }
            return View();

        }    
        
        public void PhanQuyen(String email,String Quyen)
        {
            FormsAuthentication.Initialize();

            var ticket = new FormsAuthenticationTicket(1, email, DateTime.Now, DateTime.Now.AddHours(3),false,Quyen,FormsAuthentication.FormsCookiePath);
            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(ticket));

            if (ticket.IsPersistent) cookie.Expires = ticket.Expiration;

            Response.Cookies.Add(cookie);
        }
        //Tạo trang ngăn chặn quyền truy cập
        public ActionResult LoiPhanQuyen()
        {
            return View();
        }
        public static string Encrypt(string toEncrypt)
        {
            bool useHashing = true;
            byte[] keyArray;
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);

            if (useHashing)
            {
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(KEY));
            }
            else
                keyArray = UTF8Encoding.UTF8.GetBytes(KEY);

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = keyArray;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        [HttpPost]
        public ActionResult SendCode(string email)
        {
            User user = db.Users.SingleOrDefault(n => n.email == email);
            if (user == null)
            {
                ViewBag.ThongBao = "Không tồn tại email tài khoản này. Vui lòng nhận lại tài khoản";
                return View("Login");
            }
            mail = email;
            Randomcode = RandomCode();
            new MailHelper().SendEmail(email, "Mật Khẩu Mới", "Mật Khẩu mới của bạn là: " + Randomcode);
            user.passwork = Encrypt(Randomcode);
            db.SaveChanges();
            
            return RedirectToAction("Index");
        }
        

        public string RandomCode()
        {
            string result = "";
            Random r = new Random();
            for (int i = 1; i <= 6; i++)
            {
                int n = r.Next(0, 9);
                result = string.Concat(result, n.ToString());
            }
            return result;
        }

    }

    
}

//FirstOrDefault: lấy hàng đầu tiên trong list truy vấn 
//db. ....SingleOrDefault(x=>x.id==1);
//OrderBy(x=>x...).ToList();


//list<ThanhVien> -> controller
//@model List<ThanhVien>
//==> View loop item in Model.GroupBy(x=>x.MaLop)
//    item.Key là MaLop
//    item=> tập hợp

//sử dụng View.Bag =>qua View
//@{IEnumerable<SanPham> lsSP=(IEnumerable<SanPham>)ViewBag.ListSP;}

//Sử dụng partial View
//@Html.Partial("tenPartial/tenAction","có thể lầ tên controller",lsSP); ==> add partial View  //Khai Báo Partial View trong controller
//không cần đinh nghĩa cho trong Controller => bên partial View //@model IEnumerable<SanPham>

//@model IEnumerable<SanPham>




//@Url.Content

//@.Mota.SubString(0,30);

//@Html.Raw => convert Chuỗi -> Html
//@Model.First().

//@Model.ElementAt(0). => Lấy ra phần tử thứ 0 trong list Model


//return HttpNotFound();


//@Html.RouteLink("Link text","ten route")
using CNWeb.Models;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Web;
using System.Web.DynamicData;
using System.Web.Mvc;
using System.Web.Services.Protocols;

namespace CNWeb.Controllers
{
    public class CartController : Controller
    {
        private DBContext db = new DBContext();
        public Cart getCart()
        {
            //Giỏ hàng đã tồn tại 

            Cart cart = Session["Cart"] as Cart;            
            
            if (cart == null)
            {
                cart = new Cart();
                cart.total = 0;
                cart.id = 0; // =0 là lưu tạm thời
                //db.Carts.Add(cart);
                //db.SaveChanges();
                Session["Cart"] = cart;
            }
            return cart;
        }
        //Thêm giỏ hàng load lại trang
        public ActionResult AddCart(int maSp,int color,int size, string url, int sl = 1)
        {

            //Kiểm tra sản phẩm trong CSDL
            Product sp = db.Products.SingleOrDefault(n => n.id == maSp);
            if (sp == null)
            {
                Response.StatusCode = 404;
                return null;
            }

            //List<Cart_detail> cart = getCart();

            Cart cart = getCart();
            User user = Session["TaiKhoan"] as User;

            //lsCart.Add();

            //Trường hợp nếu có 1 sản phẩm đã tồn tại trong giỏ hàng

            Cart_detail spCheck = cart.Cart_detail.SingleOrDefault(n => n.product_id == maSp && n.cart_id == cart.id&&n.id_color==color&&n.id_size==size);
            if (spCheck != null)
            {
                //Kiểm tra số lượng tồn trước khi cho khác đặt hàng
                if (sp.stock < (spCheck.quantity + sl))
                {
                    //return View();// return View thông báo
                    return Content("Sản phẩm đã hết hàng không đặt tiếp ");

                }
                spCheck.quantity += sl;
                if (user != null)
                {
                    Cart_detail CTGioHang = db.Cart_detail.SingleOrDefault(n => n.cart_id == cart.id && n.product_id == spCheck.product_id && n.id_color == spCheck.id_color && n.id_size == spCheck.id_size);
                    CTGioHang.quantity = spCheck.quantity;
                    db.SaveChanges();
                }

                return Redirect(url);                
                //return Content(totalCount().ToString());
            }
            if (sp.stock < sl)
            {
                //return View();// return View thông báo
                return Content("Sản phẩm đã hết hàng không đặt tiếp ");

            }

            Cart_detail itemSP = new Cart_detail();
            itemSP.product_id = maSp;
            itemSP.cart_id = cart.id;
            itemSP.quantity = sl;
            itemSP.Product = sp;
            itemSP.id_color = color;
            itemSP.id_size = size;
            if (user != null)
            {
                db.Cart_detail.Add(itemSP);
                db.SaveChanges();
            }
            cart.Cart_detail.Add(itemSP);
            cart.total = totalPrice();
            //db.SaveChanges();

            return Redirect(url);            
            //return Content(totalCount().ToString());

        }

        // GET: Xem giỏ hàng
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GioHangPartial()
        {
            ViewBag.totalCount = totalCount();
            ViewBag.totalPrice = totalPrice();
            return PartialView();
        }

        public int totalCount()
        {
            Cart cart = Session["Cart"] as Cart;
            User user = Session["TaiKhoan"] as User;
            if (cart == null) return 0;

            int sl = 0;
            List<Cart_detail> lsCTGioHang = new List<Cart_detail>();
            if (user == null)
            {
                lsCTGioHang = cart.Cart_detail.Where(n => n.cart_id == 0).ToList();
            }
            else
            {
                lsCTGioHang = cart.Cart_detail.Where(n => n.cart_id == user.cart_id).ToList();
            }

            if (lsCTGioHang == null || lsCTGioHang.Count == 0) return 0;

            foreach (var p in lsCTGioHang)
            {
                sl += p.quantity;
            }

            return sl;
        }

        public int totalPrice()
        {

            int total = 0;
            Cart cart = Session["Cart"] as Cart;
            if (cart == null) return 0;
            var x = cart.Cart_detail.Where(n => n.cart_id == cart.id).ToList();
            foreach (var p in x)
            {
                total += p.quantity * db.Products.SingleOrDefault(n => n.id == p.product_id).price;
            }
            return total;
        }
        //public Cart initCart()
        //{ 
        //    User user = (User)Session["TaiKhoan"];
        //    if (user == null)
        //    {
        //        Redirect("/dang-nhap");
        //        return null;
        //    }
        //    //Tạo giỏ hàng
        //    Cart cart1 = db.Carts.SingleOrDefault(x => x.user_id == user.id);

        //    return cart1;
        //}
        public ActionResult WatchCart()
        {
            Cart cart = Session["Cart"] as Cart;
            User user = Session["TaiKhoan"] as User;

            List<Cart_detail> lsItem = new List<Cart_detail>();

            if (cart == null)
            {
                ViewBag.Error = "Giỏ hàng trống";
            }
            else
            {
                lsItem = user == null ? cart.Cart_detail.Where(n => n.cart_id == 0).ToList() : cart.Cart_detail.Where(n => n.cart_id == user.cart_id).ToList();
                if (lsItem.Count == 0)
                {
                    ViewBag.Error = "Giỏ hàng trống";
                    return View();
                }
                ViewBag.totalPrice = totalPrice();
                ViewBag.totalCount = totalCount();
            }
            //return View("PayMent");
            ViewBag.listItem = lsItem;
            return View("PayMent");
        }

        public ActionResult UpdateCart(int maSp)
        {
            //Kiểm tra Session giỏ hàng tồn tại chưa?
            Cart cart = Session["Cart"] as Cart;

            if (cart == null)
            {
                return RedirectToAction("Index", "Home");
            }
            Product sp = db.Products.SingleOrDefault(n => n.id == maSp);
            if (sp == null)
            {
                Response.StatusCode = 404;
                return null;
            }

            //Lấy ra Giỏ hàng từ Session         

            List<Cart_detail> lsItem = cart.Cart_detail.Where(n => n.cart_id == cart.id).ToList();
            //Kiểm tra sản phẩm có tồn tại trong giỏ hàng chưa?
            Cart_detail spCheck = lsItem.SingleOrDefault(n => n.product_id == maSp && n.cart_id == cart.id);

            if (spCheck == null) return RedirectToAction("Index", "Home");

            //Lấy list Giỏ hàng giao diện
            ViewBag.lsItem = lsItem;
            //Nếu tồn tại 
            return View(spCheck);

        }

        [HttpPost]
        public ActionResult UpdateCart(Cart_detail item)
        {
            //Kiểm tra số lượng tồn
            Product spCheck = db.Products.SingleOrDefault(n => n.id == item.product_id);
            if (spCheck.stock < item.quantity)
            {
                return Content("Sản phẩm hết hàng");
            }
            else
            {
                //Cập nhật số lượng trong giỏ hàng
                Cart cart = Session["Cart"] as Cart;
                Cart_detail itemCart = cart.Cart_detail.SingleOrDefault(n => n.cart_id == cart.id && n.product_id == item.product_id);
                itemCart.quantity = item.quantity;

                User user = Session["TaiKhoan"] as User;
                if (user != null)
                {
                    Cart_detail CTGioHang = db.Cart_detail.SingleOrDefault(n => n.cart_id == cart.id && n.product_id == item.product_id);
                    CTGioHang.quantity = item.quantity;
                    db.SaveChanges();
                }
            }

            return RedirectToAction("WatchCart");
        }
        public ActionResult DeleteItemCart(int maSp,int idsize,int idcolor)
        {
            Cart cart = Session["Cart"] as Cart;
            User user = Session["TaiKhoan"] as User;
            if (cart == null)
            {
                RedirectToAction("Index", "Home");
            }

            Product sp = db.Products.SingleOrDefault(n => n.id == maSp);
            if (sp == null)
            {
                Response.StatusCode = 404;
                return null;

            }
            Cart_detail spCheck = new Cart_detail();
            if (user == null)
            {
                spCheck = cart.Cart_detail.SingleOrDefault(n => n.cart_id == cart.id && n.product_id == maSp && n.id_color == idcolor && n.id_size == idsize&&n.cart_id==0);
            }
            else
            {
                spCheck = cart.Cart_detail.SingleOrDefault(n => n.cart_id == cart.id && n.product_id == maSp && n.id_color == idcolor && n.id_size == idsize&&n.cart_id==user.cart_id);
            }            
            if (spCheck == null)
            {
                return RedirectToAction("Index", "Home");
            }
            //Xoa san pham trong gio hang
            cart.Cart_detail.Remove(spCheck);
            //db.SaveChanges();            

            if (user != null)
            {
                Cart_detail CTGioHang = db.Cart_detail.SingleOrDefault(n => n.cart_id == cart.id && n.product_id == maSp);
                db.Cart_detail.Remove(CTGioHang);
                db.SaveChanges();
            }

            return RedirectToAction("WatchCart");
        }
        public ActionResult Checkout()
        {
            if (Session["Cart"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            //Them don dat hang
            formPayment ddh = new formPayment();
            ddh.create_at = DateTime.Now;
            ddh.status = "Đang đóng gói";//Đang đóng gói, đang vận chuyển, đã hoàn thành, Đang hoàn trả            
            //có thể thêm ưu đãi

            db.formPayments.Add(ddh);
            db.SaveChanges();

            //Thêm chi tiết đơn đặt hàng
            Cart cart = getCart();
            foreach (var item in cart.Cart_detail)
            {
                detailPayment CTDonHang = new detailPayment();
                CTDonHang.id_form = ddh.id;
                CTDonHang.id_product = item.product_id;
                CTDonHang.count = item.quantity;
                db.detailPayments.Add(CTDonHang);

            }
            db.SaveChanges();
            Session["Cart"] = null;

            var updateCart = db.Carts.SingleOrDefault(x => x.id == cart.id);
            updateCart.total = 0;
            foreach (var item in updateCart.Cart_detail)
            {
                db.Cart_detail.Remove(item);
            }
            db.SaveChanges();

            //Thanh toán xong trừ hàng tồn kho

            return RedirectToAction("WatchCart");
        }
        [HttpPost]
        public ActionResult ChangeCount(string change, int idSp,int idsize,int idcolor, int quantity)
        {
            Cart cart = Session["Cart"] as Cart;
            User user = Session["TaiKhoan"] as User;
            Cart_detail checkSp = new Cart_detail();
            if (user == null)
            {
                checkSp = cart.Cart_detail.SingleOrDefault(n => n.product_id == idSp && n.id_size == idsize && n.id_color == idcolor&&n.cart_id==0);
            }
            else
            {
                checkSp = cart.Cart_detail.SingleOrDefault(n => n.product_id == idSp && n.id_size == idsize && n.id_color == idcolor && n.cart_id == user.cart_id); ;
            }            
            int slMax = db.Products.SingleOrDefault(n => n.id == idSp).stock ?? 1;

            if (change.Equals("-"))
            {
                if (quantity > 1)
                {
                    checkSp.quantity -= 1;
                    if (user != null)
                    {
                        Cart_detail CTGH = db.Cart_detail.SingleOrDefault(n => n.cart_id == user.cart_id && n.product_id == idSp&&n.id_size==idsize&&n.id_color==idcolor);
                        CTGH.quantity -= 1;
                        db.SaveChanges();
                    }
                }
            }
            else
            {
                if (quantity <= slMax)
                    checkSp.quantity += 1;
                if (user != null)
                {
                    Cart_detail CTGH = db.Cart_detail.SingleOrDefault(n => n.cart_id == user.cart_id && n.product_id == idSp&&n.id_color==idcolor&&n.id_size==idsize);
                    CTGH.quantity += 1;
                    db.SaveChanges();
                }
            }
            return RedirectToAction("WatchCart");
        }

        [HttpPost]
        public ActionResult DeleteCart(int idSp,int idsize,int idcolor)
        {
            Cart cart = Session["Cart"] as Cart;
            User user = Session["TaiKhoan"] as User;
            Cart_detail checkSp = new Cart_detail();
            if (user != null)
            {
                           
                checkSp = cart.Cart_detail.SingleOrDefault(n => n.product_id == idSp&&n.id_size==idsize&&n.id_color==idcolor&&n.cart_id==user.cart_id);
                cart.Cart_detail.Remove(checkSp);

                Cart_detail check = db.Cart_detail.SingleOrDefault(n => n.product_id == idSp && n.id_size == idsize && n.id_color == idcolor && n.cart_id == user.cart_id);
                db.Cart_detail.Remove(check);
                db.SaveChanges();
            }
            checkSp = cart.Cart_detail.SingleOrDefault(n => n.product_id == idSp && n.id_color == idcolor && n.cart_id == 0 && n.id_size == idsize);
            cart.Cart_detail.Remove(checkSp);

            return RedirectToAction("WatchCart");
        }

        public ActionResult ItemType(int idcolor,int idsize)
        {
            Size size = db.Sizes.SingleOrDefault(n => n.id == idsize);
            Color color= db.Colors.SingleOrDefault(n => n.id == idcolor);
            ViewBag.Color = color.type;
            ViewBag.Size = size.type;
            return PartialView();
        }
       


    }
}


//var role =role.Slipt(new char[]{','});
//bool kq =
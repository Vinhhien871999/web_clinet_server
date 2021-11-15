using CNWeb.Models;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CNWeb.Controllers
{
    public class CheckOutController : Controller
    {
        // GET: CheckOut

        private DBContext db = new DBContext();
        public ActionResult Index()
        {
            User user = Session["TaiKhoan"] as User;
            if (user == null) return Redirect("/dang-nhap");

            var lsAddress = db.Receivers.Where(n => n.id_user == user.id);                     
            return View(lsAddress);

        }
        public ActionResult NewAddress(int id)
        {
            Receiver re = db.Receivers.SingleOrDefault(n => n.id == id) as Receiver;
            return PartialView(re);
        }       


        [HttpPost]
        public ActionResult UpdateAddress(Receiver re)
        {
            Receiver receiver = db.Receivers.SingleOrDefault(n => n.id == re.id);

            receiver.name = re.name;            
            receiver.phone = re.phone;
            receiver.address = re.address;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult AddAddress(Receiver re)
        {
            User user = Session["TaiKhoan"] as User;
            if (user == null) return Redirect("/dang-nhap");

            re.id_user = user.id;

            db.Receivers.Add(re);
            db.SaveChanges();

            return PartialView("ItemAddress",re);
        }

        public ActionResult ItemAddress(Receiver re)
        {
            return PartialView(re);
        }




        public ActionResult Payment(int id)
        {
            User user = Session["TaiKhoan"] as User;
            Cart cart = Session["Cart"] as Cart;

            Receiver re = db.Receivers.SingleOrDefault(n=>n.id == id);
            ViewBag.NguoiNhan = re;
            Session["NguoiNhan"] = re;

            ViewBag.lsProduct= cart.Cart_detail.Where(n => n.cart_id == user.cart_id).ToList();

            int diem = user.point ?? 0;
            Double giam = 0;
            if (diem < 100 && diem > 50)
            {
                giam = 0.1;
            }
            else if (diem <= 200 && diem >= 100)
            {
                giam = 0.15;
            }
            else if (diem > 200)
            {
                giam = 0.2;
            }
            else
            {
                giam = 0;
            }
            //Tinh so luong
            ViewBag.SL = totalCount();
            ViewBag.Tien = totalPrice();
            ViewBag.total = totalPrice() - giam * totalPrice();
            ViewBag.giam = giam * totalPrice();



            return View(cart);
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
        public int totalCount()
        {
            Cart cart = Session["Cart"] as Cart;
            User user = Session["TaiKhoan"] as User;
            if (cart == null) return 0;

            int sl = 0;
            List<Cart_detail> lsCTGioHang = new List<Cart_detail>();
            
            lsCTGioHang = cart.Cart_detail.Where(n => n.cart_id == user.cart_id).ToList();
            

            if (lsCTGioHang == null || lsCTGioHang.Count == 0) return 0;

            foreach (var p in lsCTGioHang)
            {
                sl += p.quantity;
            }

            return sl;
        }
        public ActionResult Done(int id_reciver, int total,int id_cart,string type )
        {
            Cart cart = db.Carts.SingleOrDefault(n => n.id == id_cart);
            User user = Session["TaiKhoan"] as User;

            User userdb = db.Users.SingleOrDefault(n => n.id == user.id);

            formPayment form = new formPayment();
            form.total = total;
            form.id_receiver = id_reciver;
            form.id_user = user.id;
            form.create_at = DateTime.Now;
            form.typePayment = type;
            form.status = "Chờ xác nhận";
            form.money_ship = 15000;
            

            db.formPayments.Add(form);
            db.SaveChanges();
           
            //thêm bảng chi tiết thanh toán

            foreach(Cart_detail item in cart.Cart_detail)
            {
                detailPayment dp = new detailPayment();
                dp.count = item.quantity;
                dp.id_product = item.product_id;
                dp.id_color = item.id_color;
                dp.id_size = item.id_size;
                dp.statusVanChuyen = 0;


                user.point += (item.Product.score * item.quantity);
                userdb.point += (item.Product.score*item.quantity);
                //user.point += (item.Product.score * item.quantity);

                form.detailPayments.Add(dp);                
            }

            db.SaveChanges();

            //Xóa giỏ hàng           
            var ls = db.Cart_detail.Where(n => n.cart_id == cart.id);
            db.Cart_detail.RemoveRange(ls);
            db.SaveChanges();

            Session["Cart"] = db.Carts.SingleOrDefault(n=>n.id==user.id);
            //cart.Cart_detail=null;

            return Redirect("/");
        }
    }
}
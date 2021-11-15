using CNWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace CNWeb.Controllers
{
    public class UserController : Controller
    {
        // GET: User       
        private DBContext db = new DBContext();
        public static string KEY = "huuhoang";
        public ActionResult Index()
        {
            User user = Session["TaiKhoan"] as User;
            if (user == null) return RedirectToRoute("login");           


            return View(user);
        }
        [HttpPost]
        public ActionResult Index(User newUser, string password, string newpassword, string confirmation)
        {
            User user = Session["TaiKhoan"] as User;
            User _user = db.Users.SingleOrDefault(n => n.id == user.id);

            if (!password.Equals(""))
            {
                if (newpassword.Equals("") || confirmation.Equals(""))
                {
                    ViewBag.Error = "Nhập Mật khẩu mới";
                    return View(user);
                }
                else
                {
                    if (_user.passwork == Encrypt(password))
                    {
                        if (newpassword != confirmation)
                        {
                            ViewBag.Error = "Mật khẩu và Mật khẩu xác nhận không giống nhau";
                            return View(user);
                        }
                        else
                        {
                            _user.passwork = Encrypt(newpassword);
                        }

                    }
                    else
                    {
                        ViewBag.Error = "Mật khẩu cũ không đúng";
                        return View(user);
                    }
                }
            }
            _user.name = newUser.name;
            _user.phone = newUser.phone;
            db.SaveChanges();

            Session["TaiKhoan"] = _user;

            ViewBag.Error = "Cập nhật thành công!";

            return View(_user);
        }


        public ActionResult InfoCard()
        {
            User user = Session["TaiKhoan"] as User;

            //Session["TaiKhoan"] = db.Users.SingleOrDefault(n => n.id == user.id);

            //User usn = Session["TaiKhoan"] as User ;

            int diem = user.point ?? 0;
            Double giam = 0;

            if (diem < 100 && diem >50)
            {
                giam = 0.1;
            }
            else if (diem <= 200 && diem >=100)
            {
                giam = 0.15;
            }
            else if(diem >200)
            {
                giam = 0.2;
            }
            else
            {
                giam = 0;
            }

            ViewBag.Giam = giam * 100;
            ViewBag.NgayHetHan = user.create_at.AddYears(1).ToString();


            return View(user);
        }

        public ActionResult ListAddress()
        {
            User user = Session["TaiKhoan"] as User;

            var lsAddress = db.Receivers.Where(n => n.id_user == user.id);

            return View(lsAddress);
        }


        [HttpPost]
        public ActionResult UpdateAddress(Receiver re)
        {
            Receiver person = db.Receivers.SingleOrDefault(n => n.id == re.id);

            person.name = re.name;
            person.address = re.address;
            person.phone = re.phone;
            db.SaveChanges();
            return RedirectToAction("ListAddress");
        }

        [HttpPost]
        public ActionResult DeleteAddress(int id)
        {
            Receiver re = db.Receivers.SingleOrDefault(n => n.id == id);

            db.Receivers.Remove(re);
            db.SaveChanges();

            return RedirectToAction("ListAddress");

        }
        [HttpPost]
        public ActionResult AddAddress(Receiver re)
        {
            User user = Session["TaiKhoan"] as User;
            re.id_user = user.id;
            db.Receivers.Add(re);
            db.SaveChanges();

            return RedirectToAction("ListAddress");
        }

        public ActionResult DonHang()
        {
            User user = Session["TaiKhoan"] as User;
            var ls = db.formPayments.Where(n => n.id_user == user.id);
            return View(ls);
        }
        public ActionResult DetailPayment(int id)
        {

            formPayment form = db.formPayments.SingleOrDefault(n => n.id == id);
            ViewBag.ListSize = db.Sizes.ToList();
            ViewBag.ListColor = db.Colors.ToList();
            return View(form);
        }

        public ActionResult Destroy(int id)
        {
            var lsform = db.detailPayments.Where(n => n.id_form == id);
            var form = db.formPayments.SingleOrDefault(n => n.id == id);

            User us = Session["TaiKhoan"] as User;


            User user = db.Users.SingleOrDefault(n => n.id == us.id);
            int totalscore = 0;

            foreach(var item in lsform)
            {
                totalscore += (item.Product.score * item.count);
            }

            user.point -= totalscore;

            db.detailPayments.RemoveRange(lsform);
            db.formPayments.Remove(form);
            db.SaveChanges();
            return RedirectToAction("DonHang");
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

    }


}
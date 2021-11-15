namespace CNWeb.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("detailPayment")]
    public partial class detailPayment
    {
        DBContext db = new DBContext();
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int id_form { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int id_product { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int id_color { get; set; }

        [Key]
        [Column(Order = 3)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int id_size { get; set; }

        public int count { get; set; }

        public int statusVanChuyen { get; set; }

        public virtual formPayment formPayment { get; set; }

        public virtual Product Product { get; set; }

        public string TenSP
        {
            get { return db.Products.Find(id_product).product_name; }
        }

        public string MauSac
        {
            get { return db.Colors.Find(id_color).type; }
        }

        public string KichThuoc
        {
            get { return db.Sizes.Find(id_size).type; }
        }

        public int Gia
        {
            get { return Int32.Parse(db.Products.Find(id_product).price.ToString()); }
        }

        public int GiamGia
        {
            get { return Int32.Parse(db.Products.Find(id_product).discount.ToString()); }
        }

        public int TienShip
        {
            get { return Int32.Parse(db.formPayments.Find(id_form).money_ship.ToString()); }
        }

        public string TongTien
        {
            get { return (count * (Gia - Gia * GiamGia / 100) + TienShip).ToString(); }
        }

        public string PTThanhToan
        {
            get { return db.formPayments.Find(id_form).typePayment; }
        }

        public string TimeDat
        {
            get { return db.formPayments.Find(id_form).create_at.ToString(); }
        }
        public string TimeGiao
        {
            get { return db.formPayments.Find(id_form).delivery_date.ToString(); }
        }

        public int IDNguoiVC
        {
            get { return Int32.Parse(db.formPayments.Find(id_form).id_receiver.ToString()); }
        }

        public string TenNguoiVC
        {
            get { return db.Receivers.Find(IDNguoiVC).name; }
        }

        public string SDT
        {
            get { return db.Receivers.Find(IDNguoiVC).phone; }
        }

        public int IDUser
        {
            get { return Int32.Parse(db.formPayments.Find(id_form).id_user.ToString()); }
        }

        public string DCGiaoHang
        {
            get { return db.Users.Find(IDUser).address; }
        }

        public string TenNguoiNhan
        {
            get { return db.Users.Find(IDUser).name; }
        }

        public string SDTNguoiNhan
        {
            get { return db.Users.Find(IDUser).phone; }
        }

        public string Status
        {
            get { return db.formPayments.Find(id_form).status; }
        }
    }
}

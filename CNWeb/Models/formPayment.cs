namespace CNWeb.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("formPayment")]
    public partial class formPayment
    {
        DBContext db = new DBContext();
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public formPayment()
        {
            detailPayments = new HashSet<detailPayment>();
        }

        public int id { get; set; }

        public string typePayment { get; set; }

        public int? total { get; set; }

        public int? money_ship { get; set; }

        public int? shipper_id { get; set; }

        public DateTime? create_at { get; set; }

        public int? offer { get; set; }

        public DateTime? delivery_date { get; set; }

        public int? id_receiver { get; set; }

        public int? id_user { get; set; }

        [StringLength(50)]
        public string status { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<detailPayment> detailPayments { get; set; }

        public virtual Receiver Receiver { get; set; }

        public virtual User User { get; set; }
        public string tenNVC
        {
            get { return db.Receivers.Find(id_receiver).name; }
        }

        public string SDTNVC
        {
            get { return db.Receivers.Find(id_receiver).phone; }
        }

        public string tenNguoiNhan
        {
            get { return db.Users.Find(id_user).name; }
        }

        public string SDTNguoiNhan
        {
            get { return db.Users.Find(id_user).phone; }
        }

        public string DCGiaoHang
        {
            get { return db.Users.Find(id_user).address; }
        }
    }
}

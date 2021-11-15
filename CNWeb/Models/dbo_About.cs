namespace CNWeb.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("[dbo.About]")]
    public partial class dbo_About
    {
        public int id { get; set; }

        public int? user_id { get; set; }

        [StringLength(50)]
        public string author { get; set; }

        public string photo { get; set; }

        [StringLength(50)]
        public string title { get; set; }

        [Column(TypeName = "ntext")]
        public string content { get; set; }

        public DateTime? create_at { get; set; }

        public DateTime? update_at { get; set; }

        public virtual User User { get; set; }
    }
}

namespace CNWeb.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class DBContext : DbContext
    {
        public DBContext()
            : base("name=DBContext")
        {
        }

        public virtual DbSet<Blog> Blogs { get; set; }
        public virtual DbSet<Cart> Carts { get; set; }
        public virtual DbSet<Cart_detail> Cart_detail { get; set; }
        public virtual DbSet<Category_type> Category_type { get; set; }
        public virtual DbSet<Color> Colors { get; set; }
        public virtual DbSet<Comment> Comments { get; set; }
        public virtual DbSet<Contact> Contacts { get; set; }
        public virtual DbSet<dbo_About> dbo_About { get; set; }
        public virtual DbSet<detailPayment> detailPayments { get; set; }
        public virtual DbSet<formPayment> formPayments { get; set; }
        public virtual DbSet<Menu> Menus { get; set; }
        public virtual DbSet<Permission> Permissions { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Product_category> Product_category { get; set; }
        public virtual DbSet<Receiver> Receivers { get; set; }
        public virtual DbSet<Reply> Replies { get; set; }
        public virtual DbSet<Size> Sizes { get; set; }
        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Blog>()
                .Property(e => e.photo)
                .IsUnicode(false);

            modelBuilder.Entity<Cart>()
                .HasMany(e => e.Cart_detail)
                .WithRequired(e => e.Cart)
                .HasForeignKey(e => e.cart_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Cart>()
                .HasMany(e => e.Users)
                .WithRequired(e => e.Cart)
                .HasForeignKey(e => e.cart_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Category_type>()
                .HasMany(e => e.Product_category)
                .WithOptional(e => e.Category_type)
                .HasForeignKey(e => e.category_type_id);

            modelBuilder.Entity<Color>()
                .Property(e => e.maMau)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<Color>()
                .HasMany(e => e.Products)
                .WithMany(e => e.Colors)
                .Map(m => m.ToTable("product_color").MapLeftKey("color_id").MapRightKey("product_int"));

            modelBuilder.Entity<Comment>()
                .HasMany(e => e.Replies)
                .WithOptional(e => e.Comment)
                .HasForeignKey(e => e.comment_id);

            modelBuilder.Entity<dbo_About>()
                .Property(e => e.photo)
                .IsUnicode(false);

            modelBuilder.Entity<formPayment>()
                .HasMany(e => e.detailPayments)
                .WithRequired(e => e.formPayment)
                .HasForeignKey(e => e.id_form)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Menu>()
                .HasMany(e => e.Category_type)
                .WithOptional(e => e.Menu)
                .HasForeignKey(e => e.id_menu);

            modelBuilder.Entity<Permission>()
                .HasMany(e => e.Users)
                .WithMany(e => e.Permissions)
                .Map(m => m.ToTable("User_Permission").MapLeftKey("id_per").MapRightKey("id_user"));

            modelBuilder.Entity<Product>()
                .Property(e => e.photo)
                .IsUnicode(false);

            modelBuilder.Entity<Product>()
                .HasMany(e => e.Cart_detail)
                .WithRequired(e => e.Product)
                .HasForeignKey(e => e.product_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Product>()
                .HasMany(e => e.Comments)
                .WithOptional(e => e.Product)
                .HasForeignKey(e => e.product_id);

            modelBuilder.Entity<Product>()
                .HasMany(e => e.detailPayments)
                .WithRequired(e => e.Product)
                .HasForeignKey(e => e.id_product)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Product>()
                .HasMany(e => e.Sizes)
                .WithMany(e => e.Products)
                .Map(m => m.ToTable("product_size").MapLeftKey("product_id").MapRightKey("size_id"));

            modelBuilder.Entity<Product_category>()
                .HasMany(e => e.Products)
                .WithOptional(e => e.Product_category)
                .HasForeignKey(e => e.product_category_id);

            modelBuilder.Entity<Receiver>()
                .Property(e => e.phone)
                .IsUnicode(false);

            modelBuilder.Entity<Receiver>()
                .HasMany(e => e.formPayments)
                .WithOptional(e => e.Receiver)
                .HasForeignKey(e => e.id_receiver);

            modelBuilder.Entity<User>()
                .Property(e => e.photo)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .Property(e => e.email)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .Property(e => e.passwork)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .Property(e => e.phone)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Blogs)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.user_id);

            modelBuilder.Entity<User>()
                .HasMany(e => e.dbo_About)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.user_id);

            modelBuilder.Entity<User>()
                .HasMany(e => e.formPayments)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.id_user);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Receivers)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.id_user);
        }
    }
}

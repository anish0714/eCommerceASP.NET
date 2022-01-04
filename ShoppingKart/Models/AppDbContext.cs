using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingKart.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Order> Orders { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Name=DefaultConnection");
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");



            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Products");
                entity.Property(product => product.Id).HasColumnName("Id");
                entity.Property(product => product.Description).HasColumnName("Description");
                entity.Property(product => product.Image).HasColumnName("Image");
                entity.Property(product => product.Name)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("Name");
                entity.Property(product => product.Price)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("Price");

                entity.Property(product => product.ShippingCost).HasColumnName("ShippingCost");
            });


            /*CART*/
            modelBuilder.Entity<Cart>(entity =>
            {
                entity.ToTable("Carts");

                entity.Property(cart => cart.Id).HasColumnName("Id");

                entity.Property(cart => cart.UserId).HasColumnName("UserId");

                entity.Property(cart => cart.ProductId).HasColumnName("ProductId");

                entity.Property(cart => cart.Quantity).HasColumnName("Quantity");
            });



            /*COMMENTS*/

            modelBuilder.Entity<Comment>(entity =>
            {
                entity.ToTable("Comments");

                entity.Property(comment => comment.Id).HasColumnName("Id");

                entity.Property(comment => comment.Text).HasColumnName("Text");

                entity.Property(comment => comment.Rating).HasColumnName("Rating");

                entity.Property(comment => comment.Image)
                    .IsUnicode(false)
                    .HasColumnName("Image");

                entity.Property(e => e.ProductId).HasColumnName("ProductId");

                entity.Property(e => e.UserId).HasColumnName("UserId");
            });


            /*USER*/
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");

                entity.Property(e => e.Id).HasColumnName("Id");

                entity.Property(e => e.Address).HasColumnName("Address");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("Email");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("Name");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .IsUnicode(false)
                    .HasColumnName("Password");
            });


            /*ORDERS*/
            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("Orders");

                entity.Property(e => e.Id).HasColumnName("Id");

                entity.Property(e => e.UserId).HasColumnName("UserId");

                entity.Property(e => e.ShippingDetails).HasColumnName("ShippingDetails");
            });

        }





    }

}

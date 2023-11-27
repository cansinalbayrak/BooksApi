using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace BooksApi.Context
{
    public class AppDbContext : IdentityDbContext<AppUser, AppRole, string>
    {

        public DbSet<UserFavBook> UserFavBooks { get; set; }
        public DbSet<Book> Books { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }



        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<AppUser>().HasMany(x => x.UserFavBooks).WithOne(x => x.AppUser).HasForeignKey(x => x.AppUserId);

            builder.Entity<Book>().HasMany(x => x.UserFavBooks).WithOne(x => x.Book).HasForeignKey(x => x.BookId);

            builder.Entity<UserFavBook>().HasKey(x => new { x.AppUserId, x.BookId });



            
        }
    }


    public class AppUser : IdentityUser
    {
        public string Ad { get; set; }
        public string Soyad { get; set; }
        public string Password { get; set; }
        public List<UserFavBook> UserFavBooks { get; set; } = new();
    }
    public class AppRole : IdentityRole
    {
    }
   
    public class Book
    {
        public int Id { get; set; }
        public string KitapAd { get; set; }
        public string YazarAd { get; set; }
        public string Kategori { get; set; }
        public int Sayfa { get; set; }

        public string Renk { get; set; }

        public List<UserFavBook> UserFavBooks { get; set; } = new();
    }
    public class UserFavBook
    {
        public string AppUserId { get; set; }
        public int BookId { get; set; }

        public AppUser AppUser { get; set; }
        public Book Book { get; set; }


    }
}

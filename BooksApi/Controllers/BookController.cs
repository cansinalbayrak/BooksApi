using BooksApi.Context;
using BooksApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BooksApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;

        public BookController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Search(int id)
        {
            var book = await _appDbContext.Books.FirstOrDefaultAsync(x => x.Id == id);
            return Ok(book);
        }
        [Authorize(Roles ="admin")]
        [HttpPost("addBook")]
        public async Task<IActionResult> AddBook(AddBookModel model)
        {
            Book book = new Book()
            {
                KitapAd = model.Name,
                YazarAd = model.Author,
                Kategori = model.Category,
                Sayfa = model.Page,
                Renk = model.Color
            };
            await _appDbContext.Books.AddAsync(book);
            await _appDbContext.SaveChangesAsync();
            return Ok("kitap eklendi.");
        }
        [Authorize(Roles = "admin")]
        [HttpDelete]
        [Route("/DeleteBook/{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var book = _appDbContext.Books.FirstOrDefault(x => x.Id == id);
            if (book == null)
            {
                return BadRequest("kitap bulunamadı");
            }
            _appDbContext.Books.Remove(book);
            await _appDbContext.SaveChangesAsync();
            return Ok("kitap başarıyla silindi.");
        }
        [HttpGet]
        [Route("UserFavBooks/{id}")]
        public async Task<IActionResult> UserFavBooks(string id)
        {
            // düzeltt id almasın saçma
            var books = await _appDbContext.UserFavBooks.Include(x => x.Book).Where(x => x.AppUserId == id).Select(x => x.Book).ToListAsync();
            return Ok(books);
        }
        [HttpPost("AddFavBooks/{id}")]
        public async Task<IActionResult> AddFavBooks(int id, string userId)
        {
            var book = await _appDbContext.Books.FirstOrDefaultAsync(x => x.Id == id);
            var user = await _appDbContext.Users.FirstOrDefaultAsync(x => x.Id == userId);

            if (user != null  && book != null)
            {
                var userFavBook = new UserFavBook
                {
                    AppUserId = userId,
                    BookId = id
                };

                _appDbContext.UserFavBooks.Add(userFavBook);
                await _appDbContext.SaveChangesAsync();
                return Ok("fav kitap eklendi");
            }

            return BadRequest("hatalı işlem");
        }

        [HttpGet("ListBooks")]
        public async Task<IActionResult> ListBooks([FromQuery] int pageNo, [FromQuery] int pageSize)
        {
            var books = await _appDbContext.Books.Skip((pageNo - 1) * pageSize).Take(pageSize).ToListAsync();
            return Ok(books);
        }

        [HttpGet("GetByFilterBooks")]
        public IActionResult GetByFilterBooks([FromBody]BookFilterModel filter)
        {
            var query = _appDbContext.Books.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.Category))
                query = query.Where(x => x.Kategori == filter.Category);

            if (!string.IsNullOrWhiteSpace(filter.Color))
                query = query.Where(x => x.Renk == filter.Color);

            if (!string.IsNullOrWhiteSpace(filter.Author))
                query = query.Where(x => x.YazarAd == filter.Author);

            if (filter.MaxP >= 0)
                query = query.Where(x => x.Sayfa < filter.MaxP);

            if (filter.MinP >= 0)
                query = query.Where(x => x.Sayfa > filter.MinP);
            
            return Ok(query.ToList());
        }

    }
}

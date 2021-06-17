using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using jwt_identity_api.DTO;
using Microsoft.EntityFrameworkCore;

namespace jwt_identity_api.Data.Repositories
{
    public interface IBookApp
    {
        Task<bool> Add(Book model);
        Task<List<Book>> Get();
        Task<Book> Get(Guid bookId);
        Task<bool> Remove(Guid bookId);
        Task<bool> Update(Book model);
    }

    public class BookApp : IBookApp
    {
        private readonly AppDbContext _db;
        public BookApp(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<Book>> Get() => await _db.Books.ToListAsync();

        public async Task<Book> Get(Guid bookId)
        {
            var book = await _db.Books.FirstOrDefaultAsync(x => x.Id == bookId);
            return book == null ? null : book;
        }

        public async Task<bool> Add(Book model)
        {
            await _db.AddAsync(model);
            await _db.SaveChangesAsync();

            return true;
        }

        public async Task<bool> Update(Book model)
        {
            var book = await Get(model.Id);

            if (book == null)
            {
                return false;
            }

            book.Title = model.Title;
            book.Author = model.Author;
            book.Year = model.Year;

            _db.Update(book);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Remove(Guid bookId)
        {
            var book = await Get(bookId);

            if (book == null)
            {
                return false;
            }

            _db.Remove(book);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
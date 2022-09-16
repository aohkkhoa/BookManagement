using BookManagement2.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace BookManagement2.Repository
{
    public interface IBookRepository
    {
        public List<BookDetail> GetAll();
        public IActionResult GetById(int id);

        public List<BookDetail> GetHigh();
        List<BookDetail> List(int page, int pageSize);

        public int Count();
    }
}

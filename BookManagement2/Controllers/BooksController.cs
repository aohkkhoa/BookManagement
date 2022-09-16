using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookManagement2.Models.EF;
using BookManagement2.Models.Entities;
using BookManagement2.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using BookManagement2.DAL;
using BookManagement2.Repository;

namespace BookManagement2.Controllers
{
    public class BooksController : Controller
    {
        private readonly BookDBContext _context;
        private readonly IConfiguration m_config;
        private readonly IBookRepository _bookRepository;

        public BooksController(BookDBContext context, IConfiguration config, IBookRepository bookRepository)
        {
            _context = context;
            m_config = config;
            _bookRepository = bookRepository;
        }

        // GET: Books

        public IActionResult Index()
        {

            /* var accessToken = HttpContext.Session.GetString("JWToken");
             var url = baseUrl;
             HttpClient client = new HttpClient();
             client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
             string jsonStr = await client.GetStringAsync(url);  
             var res = JsonConvert.DeserializeObject<List<Book>>(jsonStr).ToList();*/




            /*var bookList = (from b in _context.Books
                            join cate in _context.Categories on b.categoryId equals cate.categoryId
                            select new BookDetail()
                            {
                                bookDetailId = b.bookId,
                                bookName = b.title,
                                categoryName = cate.categoryName,

                            }).ToList();
            ViewData["Checked"] = 1;
            return _context.Books != null ?
                        View(bookList) :
                        Problem("Entity set 'BookDBContext.Books'  is null.");*/

            var bookList = _bookRepository.GetAll();
            ViewData["Checked"] = 1;
            return _context.Books != null ?
                        View(bookList) :
                        Problem("Entity set 'BookDBContext.Books'  is null.");
        }
        public IActionResult TestCore()
        {
            var booklist = _bookRepository.GetHigh();
            return View("Views/Books/Index.cshtml", booklist);
        }
        public IActionResult Pagination(int page=1, int pageSize =2)
        {
            var rs = _bookRepository.List(page, pageSize);
            ViewData["rowCount"] = _bookRepository.Count()/pageSize;
            ViewData["pageCurrent"] = page;
            ViewData["pageSize"] = 2;
            return View("Views/Books/Index.cshtml", rs);
        }


        // GET: Books/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Books == null)
            {
                return NotFound();
            }

            /*var accessToken = HttpContext.Session.GetString("JWToken");
            var url = baseUrl + id;
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            string jsonStr = await client.GetStringAsync(url);*/

            var book = await _context.Books
                .FirstOrDefaultAsync(m => m.bookId == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // GET: Books/Create
        [Authorize]
        public IActionResult Create()
        {


            ViewData["CategoryId"] = new SelectList(_context.Categories, "categoryId", "categoryName");
            return View();
        }

        // POST: Books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("bookId,title,categoryId")] Book book)
        {
            if (ModelState.IsValid)
            {
                _context.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "categoryId", "categoryName", book.categoryId);
            return View(book);
        }

        // GET: Books/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Books == null)
            {
                return NotFound();
            }

            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("bookId,title,categoryId")] Book book)
        {
            if (id != book.bookId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(book);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.bookId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }

        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Books == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .FirstOrDefaultAsync(m => m.bookId == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Books == null)
            {
                return Problem("Entity set 'BookDBContext.Books'  is null.");
            }
            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                _context.Books.Remove(book);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id)
        {
            return (_context.Books?.Any(e => e.bookId == id)).GetValueOrDefault();
        }
    }
}

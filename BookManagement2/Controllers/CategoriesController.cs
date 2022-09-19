using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookManagement2.Models.EF;
using BookManagement2.Models.Entities;
using BookManagement2.Models.DTO;
using X.PagedList;

namespace BookManagement2.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly BookDBContext _context;

        public CategoriesController(BookDBContext context)
        {
            _context = context;
        }

        // GET: Categories
        public IActionResult Index(int? page)
        {
            int pageSize = 2;
            int pageNumber = (page ?? 1);
            var category = from c in _context.Categories
                           select c;
            category = category.OrderByDescending(s => s.categoryId);
            return _context.Categories != null ? 
                          View( category.ToPagedList(pageNumber, pageSize)) :
                          Problem("Entity set 'BookDBContext.Categories'  is null.");
        }

        // GET: Categories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Categories == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.categoryId == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // GET: Categories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("categoryId,categoryName")] Category category)
        {
            if (ModelState.IsValid)
            {
                _context.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Categories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Categories == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("categoryId,categoryName")] Category category)
        {
            if (id != category.categoryId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.categoryId))
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
            return View(category);
        }
        [HttpGet]
        public int GetCategory(int id)
        {
            var category = (from b in _context.Books
                            join cate in _context.Categories on b.categoryId equals cate.categoryId
                            where b.categoryId == id
                            select new BookDetail()
                            {
                                bookDetailId = b.bookId,
                                title = b.title,
                                categoryName = cate.categoryName,
                            }).ToList();
            if (category.Count != 0)
            {
                return 1;
            }
            return 0;
        }


        // GET: Categories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Categories == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.categoryId == id);
            /*var books = await _context.Books
                .FirstOrDefaultAsync(m => m.categoryId == id);
            if(books != null)
            {
                ViewData["CategoryId"] = id;
                return View("Delete1", category);
            }*/
            if (category == null)
            {
                return NotFound();
            }
            var category1 = await _context.Categories.FindAsync(id);
            if (category1 != null)
            {
                _context.Categories.Remove(category);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
            //return View(category);
        }
        
        public async Task<IActionResult> Delete1(int id)
        {
            var category = await _context.Categories.FindAsync(id);

            var bookList = (from b in _context.Books
                            join cate in _context.Categories on b.categoryId equals cate.categoryId
                            where b.categoryId == id
                            select new BookDetail()
                            {
                                bookDetailId = b.bookId,
                                title = b.title,
                                categoryName = cate.categoryName,
                            }).ToList();
            ViewData["Checked"] = 0;
            return View("Views/Books/Index.cshtml", bookList);
        }

        // POST: Categories/Delete/5
        /*[HttpPost, ActionName("Delete1")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed1(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            var book = new BookDetail()
            {
                bookDetailId = 1,
                bookName = "h",
                categoryName = "d",
            };
            return View("Views/Books/Index.cshtml");
        }*/

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Categories == null)
            {
                return Problem("Entity set 'BookDBContext.Categories'  is null.");
            }
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
          return (_context.Categories?.Any(e => e.categoryId == id)).GetValueOrDefault();
        }
    }
}

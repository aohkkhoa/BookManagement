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
using System.Security.Cryptography;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;

namespace BookManagement2.Controllers
{
    public class AuthController : Controller
    {
        private readonly BookDBContext _context;
        private readonly ApplicationSettings _appSettings;

        public AuthController(IOptions<ApplicationSettings> _appSettings, BookDBContext context)
        {
            this._appSettings = _appSettings.Value;
            _context = context;
        }
        [Authorize]
        [HttpGet]
        public Book Test()
        {

            var book = new Book()
            {
                bookId = 1,
                categoryId = 1,
                title = "ss"
            };
            return book;
        }


        // GET: Auth
        public async Task<IActionResult> Index()
        {
              return _context.Users != null ? 
                          View(await _context.Users.ToListAsync()) :
                          Problem("Entity set 'BookDBContext.Users'  is null.");
        }

        // GET: Auth/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.userId == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Auth/Create
        public IActionResult Login()
        {
            return View();
        }



        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return View("Views/Home/Index.cshtml");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login([Bind("UserName,PassWord")] Login model)
        {
            var user = (from b in _context.Users
                        where b.userName == model.UserName
                        select b).FirstOrDefault();

            if (user == null)
            {
                return BadRequest("Username or pass was invalid");
            }

            var match = CheckPassword(model.PassWord, user);

            if (!match)
            {
                return BadRequest("Username or pass was invalid");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(this._appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("user", user.userName) }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var encrypterToken = tokenHandler.WriteToken(token);
            HttpContext.Session.SetString("JWToken", encrypterToken);
            HttpContext.Session.SetString("username", user.userName);

            /* using (var httpClient = new HttpClient())
             {
                 StringContent stringContent = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
                 using (var response = await httpClient.PostAsync("https://localhost:7161/token", stringContent))
                 {
                     string token1 = await response.Content.ReadAsStringAsync();
                     if (token1 == "Invalid credentials")
                     {
                         ViewBag.Message = "Incorect UserId or Password";
                         return Redirect("Views/Home/Index.cshtml");
                     }
                     HttpContext.Session.SetString("JWToken", token1);
                 }
                 return BadRequest("error");
             }*/

            return Ok(new { token = encrypterToken, username = user.userName });
            //return RedirectToAction("LoginUser", new { user = user });
        }

   /*     private async Task<IActionResult> LoginUser(User user)
        {
            using (var httpClient = new HttpClient())
            {
                StringContent stringContent = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
                using (var response = await httpClient.PostAsync("https://localhost:7161/token", stringContent))
                {
                    string token= await response.Content.ReadAsStringAsync();
                    if(token=="Invalid credentials")
                    {
                        ViewBag.Message = "Incorect UserId or Password";
                        return Redirect("Views/Home/Index.cshtml");
                    }
                    HttpContext.Session.SetString("JWToken", token);
                }
                return BadRequest("error");
            }

               
        }*/

       

        private Boolean CheckPassword(string passWord, User user)
        {
            bool result;
            using (HMACSHA512? hmac = new HMACSHA512(user.PasswordSalt))
            {
               var compute = hmac.ComputeHash(Encoding.UTF8.GetBytes(passWord));
               result = compute.SequenceEqual(user.PasswordHash);
            }
            return result;
        }

        // GET: Auth/Create
        public IActionResult Register()
        {
            return View();
        }

        // POST: Auth/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([Bind("UserName,Password,ConfirmPass,address,phone")] Register model)
        {
            var user = new User();

            if (model.ConfirmPass == model.Password && ModelState.IsValid)
            {
                using (HMACSHA512? hmac = new HMACSHA512())
                {
                    user.PasswordSalt = hmac.Key;
                    user.PasswordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(model.Password));
                }
                user.userName = model.UserName;
                user.address = model.address;
                user.phone = model.phone;
                _context.Add(user);
                await _context.SaveChangesAsync();
                return View("Login");
            }
            return View();
        }

        // GET: Auth/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Auth/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("userId,userName,PasswordSalt,PasswordHash,address,phone")] User user)
        {
            if (id != user.userId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.userId))
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
            return View(user);
        }

        // GET: Auth/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.userId == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Auth/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Users == null)
            {
                return Problem("Entity set 'BookDBContext.Users'  is null.");
            }
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
          return (_context.Users?.Any(e => e.userId == id)).GetValueOrDefault();
        }
    }
}

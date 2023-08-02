using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyLogin.Models;
using MyLogin.ViewModels;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace MyLogin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly PasswordHashService _passwordHashService;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
            _passwordHashService = new PasswordHashService();
        }

        [HttpPost("signup")]
        public IActionResult SignUp(User user)
        {   
            if (_context.Users.Any(u => u.Username == user.Username))
            {
                return BadRequest("Username already exists.");
            }
            user.Password = _passwordHashService.HashPassword(user.Password);

            _context.Users.Add(user);
            _context.SaveChanges();

            return Ok("User registered successfully.");
        }

        [HttpPost("login")]
        public IActionResult Login(User model)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == model.Username && u.Password == _passwordHashService.HashPassword(model.Password));

            if (user == null || !_passwordHashService.VerifyPassword(model.Password, user.Password))
            {
                return NotFound("Invalid username or password.");
            }
            return Ok(user);
        }

        public class PasswordHashService
        {
            public string HashPassword(string password)
            {
                using (MD5 md5 = MD5.Create())
                {
                    byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(password);
                    byte[] hashBytes = md5.ComputeHash(inputBytes);

                    // Convert the byte array to a hexadecimal string representation
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < hashBytes.Length; i++)
                    {
                        sb.Append(hashBytes[i].ToString("X2"));
                    }

                    return sb.ToString();
                }
            }

            public bool VerifyPassword(string password, string hashedPassword)
            {
                string hashedInputPassword = HashPassword(password);
                return hashedInputPassword == hashedPassword;
            }
        }

    }
}

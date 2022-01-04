using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using ShoppingKart.Common;

using ShoppingKart.Models;

namespace ShoppingKart.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext appDbContext;

        public UserController(AppDbContext context)
        {
            appDbContext = context;
        }
        // @type POST: api/User/Register
        // @desc Create a new user
        [HttpPost("Register")]
        public async Task<ActionResult<User>> Register([FromBody] User user)
        {
            /*byte[] salt = new byte[128 / 8];
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                rngCsp.GetNonZeroBytes(salt);
            }*/
            /* string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
             password: user.Password,
             salt: salt,
             prf: KeyDerivationPrf.HMACSHA1,
             iterationCount: 100000,
             numBytesRequested: 256 / 8));
             user.Password = hashed;*/

            /*            var salt = BCrypt.GenerateSalt();
            */
            user.Password = CommonMethods.ConvertToEncrypt(user.Password);
            bool isUserExists = appDbContext.Users.Any(userDb => userDb.Id == user.Id || userDb.Email == user.Email);
            if (isUserExists)
            {
                return Content("{\"msg\": \"User already exists\"}");

            }
            appDbContext.Users.Add(user);
            await appDbContext.SaveChangesAsync();
            var data = new { msg = "User Added Successfully", data = user};
            return Ok(data);
        }

        // @type POST: api/User/Login
        // @desc User Login
        [HttpPost("Login")]
        public async Task<ActionResult<User>> Login([FromBody] User user)
        {
            /*byte[] salt = new byte[128 / 8];
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                rngCsp.GetNonZeroBytes(salt);
            }
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: user.Password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA1,
            iterationCount: 100000,
            numBytesRequested: 256 / 8));
            user.Password = hashed;*/

            user = await appDbContext.Users.Where(userDb => userDb.Email == user.Email && userDb.Password == CommonMethods.ConvertToEncrypt(user.Password)).FirstOrDefaultAsync();
            if (user == null)
            {
                return NotFound();
            }

            HttpContext.Session.SetInt32("eLogin", 2);
            await HttpContext.Session.CommitAsync();

            var data = new { msg = "Login Successful", data = user};
            return Ok(data);
        }
    }
}

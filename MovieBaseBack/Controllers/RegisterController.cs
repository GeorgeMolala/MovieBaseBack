using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MovieBaseBack.Data;
using MovieBaseBack.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieBaseBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private readonly ILogger<RegisterController> _logger;
        private UserManager<AppUser> userManager;
        private SignInManager<AppUser> SignInManager;

        private RoleManager<IdentityRole> roleManager;

        private MovieDbContext _context;

        public RegisterController(UserManager<AppUser> userMngr, SignInManager<AppUser> signInManager, RoleManager<IdentityRole> roleManagerr, MovieDbContext context)
        {
            userManager = userMngr;
            SignInManager = signInManager;
            _context = context;
            roleManager = roleManagerr;
        }


        public async Task<IActionResult> LogIn(Log login)
        {
            if (ModelState.IsValid)
            {
                var result = await SignInManager.PasswordSignInAsync(login.UserName, login.Password, login.RememberMe, lockoutOnFailure: false);


            }
            return Ok();
        }


        [HttpPost("Register")]
        public async Task<IActionResult> Register(UserCred user)
        {
            
            UserCred cred = new UserCred();

            if (ModelState.IsValid)
            {
                string pass = user.Password;
                user.Password = "#############";
                _context.Userss.Add(user);
                _context.SaveChanges();

               int UserId = _context.Userss.Max(x => x.UserID);

                var User = new AppUser
                {

                    UserName = user.Email,
                    UserID = UserId,
                    Email = user.Email,
                    EmailConfirmed = true,
                };

                var Result = await userManager.CreateAsync(User, pass);
                var resultss = await roleManager.CreateAsync(new IdentityRole("Standard"));
                var resultROle = await userManager.AddToRoleAsync(User, "Standard");

                if (Result.Succeeded)
                {
                    await SignInManager.SignInAsync(User, isPersistent: false);
                    return Ok(Result.Succeeded); ///Fix Customer Controller
                }


            }

            else
            {
                return Unauthorized();
            }

            return Unauthorized();
        }
        
    }
}

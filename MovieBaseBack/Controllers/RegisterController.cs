using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using MovieBaseBack.Data;
using MovieBaseBack.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
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
        private readonly IConfiguration _configuration;
        private RoleManager<IdentityRole> roleManager;

        private MovieDbContext _context;
   

        public RegisterController(UserManager<AppUser> userMngr, SignInManager<AppUser> signInManager, RoleManager<IdentityRole> roleManagerr, MovieDbContext context, IConfiguration config)
        {
            userManager = userMngr;
            SignInManager = signInManager;
            _context = context;
            roleManager = roleManagerr;
            _configuration = config;
        }


        [HttpPost("LogIn")]
        public async Task<IActionResult> LogIn(Log login)
        {
            if (ModelState.IsValid)
            {
                var result = await SignInManager.PasswordSignInAsync(login.UserName, login.Password, login.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    int ts = _context.Users.Where(t => t.UserName == login.UserName).Select(x => x.UserID).FirstOrDefault();
                    string name = _context.Users.Where(t => t.UserName == login.UserName).Select(x => x.UserName).FirstOrDefault();
                    //TempData["UserID"] = ts;
                    //TempData["UserNamePass"] = name;
                    if (!string.IsNullOrEmpty(login.ReturnUrl) && Url.IsLocalUrl(login.ReturnUrl))
                    {


                        return Redirect(login.ReturnUrl);

                    }

                    else
                    {
                        var use = await userManager.FindByNameAsync(login.UserName);

                        var role = await userManager.GetRolesAsync(use);

                        var authClaims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, use.UserName),
                            new Claim(ClaimTypes.NameIdentifier, use.Id),
                            new Claim("JWTID", Guid.NewGuid().ToString()),
                        };



                        //switch (role.FirstOrDefault())
                        //{
                        //    case "Standard":
                        //        {

                        //            return Ok(result.Succeeded);
                        //        }
                        //}

                        foreach (var userRole in role)
                        {
                            authClaims.Add(new Claim(ClaimTypes.Role, userRole));

                        }

                        var token = GenerateNewJsonWebToken(authClaims);

                        return Ok(token);
                    }
                }
               
            }

            return Unauthorized();
        }

        //Generating token
        private string GenerateNewJsonWebToken(List<Claim> claims)
        {
            var authSecret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
            var tokenObject = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience:  _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(1),
                claims: claims,
                signingCredentials: new SigningCredentials(authSecret, SecurityAlgorithms.HmacSha256)
                );

            string token = new JwtSecurityTokenHandler().WriteToken(tokenObject);
            return token;
        }







        [HttpGet]
         public IActionResult LogIn(string returnURL = "")
        {
            var model = new Log { ReturnUrl = returnURL };
            return Unauthorized();
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

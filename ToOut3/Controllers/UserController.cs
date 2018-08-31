using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using ToOut3.Models;

namespace ToOut3.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        //string ShoppingCartId { get; set; }

        private StudentSelection2Context _dbContext;
        public UserController(StudentSelection2Context dbContext)
        {
            _dbContext = dbContext;
        }

        //[AllowAnonymous, HttpGet]
        //public async Task<IActionResult> Register()
        //{
        //    await
        //        HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        //    return View();
        //}

        //[AllowAnonymous, HttpPost]
        //public async Task<IActionResult> Register(RegisterViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        throw new Exception("Invalid registration information.");
        //    }

        //    model.Name = model.Name.Trim();
        //    model.Password = model.Password.Trim();
        //    model.RepeatPassword = model.RepeatPassword.Trim();
        //    model.Id = model.Id.Trim();
        //    var targetUser = _dbContext.InfoTable.SingleOrDefault(i => i.Id.Equals(model.Id, StringComparison.CurrentCulture));

        //    if (targetUser != null)
        //    {
        //        throw new Exception("User name already exists.");
        //    }

        //    if (!model.Password.Equals(model.RepeatPassword))
        //    {
        //        throw new Exception("Passwords are not identical.");
        //    }

        //    var hasher = new PasswordHasher<InfoTable>();
        //    targetUser = new InfoTable { Name = model.Name, Id = model.Id, RoleId = 1};
        //    targetUser.Pass = hasher.HashPassword(targetUser, model.Password);



        //    await _dbContext.InfoTable.AddAsync(targetUser);
        //    await _dbContext.SaveChangesAsync();
        //    await LogInUserAsync(targetUser);

        //    return RedirectToAction("Index", "Home");
        //}

        private async Task LogInUserAsync(InfoTable user)
        {
            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Name, user.Id));
            
            if (user.RoleId == 1)
            {
                claims.Add(new Claim(ClaimTypes.Role, "Administrator"));
            }
            else
            {

                claims.Add(new Claim(ClaimTypes.Role, "Student"));
            }

            
            var claimsIndentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIndentity);          
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);
            
            await _dbContext.SaveChangesAsync();
         
        }


        [AllowAnonymous,HttpGet]
        public async Task<IActionResult> LogIn()
        {
            if (User.Identity.IsAuthenticated == true)
            {
                
                if (User.IsInRole("Administrator"))
                {
                    return RedirectToAction("Index", "Home");
                    
                }
                else
                {
                    
                    return RedirectToAction("Index", "StudentSelection");
                }
                
            }
            await
                HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return View();
        }

        [AllowAnonymous,HttpPost]
        public async Task<IActionResult>LogIn(LogInViewModel model)
        {

            if (!ModelState.IsValid)
            {
                throw new Exception("Invalid user information.");
            }
            var targetUser = _dbContext.InfoTable.SingleOrDefault(u => u.Id.Equals(model.ID, StringComparison.CurrentCultureIgnoreCase));
            if (targetUser == null)
            {
                throw new Exception("User does not exist.");
            }

            var hasher = new PasswordHasher<InfoTable>();
            var result = hasher.VerifyHashedPassword(targetUser, targetUser.Pass, model.Password);
            if (result != PasswordVerificationResult.Success)
            {
                throw new Exception("The password is wrong.");
            }
            
            await LogInUserAsync(targetUser);
            
            //if (targetUser.RoleId==1)
            //{
               
                return RedirectToAction("Index", "Home");
            //}
            //else
            //{
            //    //string sd = User.Identity.Name;
            //    //GetCurrentUserData(sd);
            //    return RedirectToAction("Index", "StudentSelection");
            //}
        }

        [HttpGet]
        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
           
            return RedirectToAction("LogIn", "User");
        }


        

    }
}

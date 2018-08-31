using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ToOut3.Models;
namespace ToOut3.Controllers
{
    public class HomeController:Controller
    {
        private StudentSelection2Context _dbContext;
        public HomeController(StudentSelection2Context dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            //Detect if user sign or not 
            if(User.Identity.IsAuthenticated == true)
            {
                //Detect the user type administrator or student
                if (User.IsInRole("Administrator"))
                {
                    return View();
                }
                else
                {
                     return RedirectToAction("StudentSelect", "StudentSelection");
                }
            }
            else
            {
                return RedirectToAction("LogIn", "User");
            }
            
        }
     
    }

  
}

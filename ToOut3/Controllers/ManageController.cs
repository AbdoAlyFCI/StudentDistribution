using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ToOut3.Models;
using ToOut3.Models.ViewModels;
using ToOut3.Infrastructure;
using Microsoft.AspNetCore.Authorization;

namespace ToOut3.Controllers
{
    [Authorize(Roles= "Administrator")]
    public class ManageController:Controller
    {
        private StudentSelection2Context _dbContext;
        public ManageController(StudentSelection2Context dbContext)
        {
            _dbContext = dbContext;
        }
        public IActionResult Index()
        {

            return View();
        }

        //add new student manualy by using a form 
        [HttpGet] 
        public ViewResult ManualRegister()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ManualRegister(AllInfo student)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            else
            {
                //Remove any spaces in the first
                student.newStudnet.Id = student.newStudnet.Id.Trim();
                student.newStudnet.Name = student.newStudnet.Name.Trim();
                student.newStudnet.Pass = student.newStudnet.Pass.Trim();
                student.newStudnet.RepeatPass = student.newStudnet.RepeatPass.Trim();

                //check if the target user is used before or not 
                var targetUser = _dbContext.InfoTable.SingleOrDefault(i => i.Id.Equals(student.newStudnet.Id, StringComparison.CurrentCulture));
                var targetGPA= _dbContext.InfoTable.SingleOrDefault(i => i.Id.Equals(student.newStudnet.Id, StringComparison.CurrentCulture));
                if (targetUser != null)
                {
                    ViewBag.IdTarget = "This ID is already used";
                    return View();
                }
                //hash password
                var hasher = new PasswordHasher<InfoTable>();
                targetUser = new InfoTable { Id = student.newStudnet.Id, Name = student.newStudnet.Name, RoleId = 2 };               
                targetUser.Pass = hasher.HashPassword(targetUser, student.newStudnet.Pass);
               
                //chech the GPA
                if(student.newStudnet.StudentGPA !=double.NaN)
                {
                    await GpaSet(student.newStudnet);
                }
                await _dbContext.InfoTable.AddAsync(targetUser);
                await _dbContext.SaveChangesAsync();
                ViewBag.templist = true;
                TempRepository.AddStudent(student.newStudnet);
                ViewBag.inserted = true;
                return View(new AllInfo
                {
                    Studentslist = TempRepository.Inserted
                });
            }
        }
        //Add GPA to the new student
        private async Task GpaSet(NewStudentInsert student)
        {
            var targetGPA = _dbContext.StudnetGpa.SingleOrDefault(i => i.StuId.Equals(student.Id, StringComparison.CurrentCulture));
            targetGPA = new StudnetGpa { StuId = student.Id, StuGpa = student.StudentGPA };
            await _dbContext.StudnetGpa.AddRangeAsync(targetGPA);
            await _dbContext.SaveChangesAsync();
            return;
        }


        //upload a file to website to register new students easly
        [HttpGet]
        public ViewResult Upload()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            TempRepository.RemoveAll();
            var path = Path.Combine(
                       Directory.GetCurrentDirectory(), "wwwroot",
                       file.FileName);
            
            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            //Load the the data from the uploaded file
            IEnumerable<NewStudentInsert> ToAdd= DataLoader.Load(@"wwwroot\" + file.FileName);

            foreach (NewStudentInsert s in ToAdd)
            {
                s.Id = s.Id.Trim();
                s.Name = s.Name.Trim();
                s.Pass = s.Pass.Trim();
                s.RepeatPass = s.RepeatPass.Trim();
                var targetUser = _dbContext.InfoTable.SingleOrDefault(i => i.Id.Equals(s.Id, StringComparison.CurrentCulture));
                var targetGPA = _dbContext.InfoTable.SingleOrDefault(i => i.Id.Equals(s.Id, StringComparison.CurrentCulture));
                if (targetUser != null)
                {
                    TempRepository.AddStudent(new NewStudentInsert
                    {
                        Id = s.Id,
                        Name = s.Name,
                        StudentGPA = s.StudentGPA,
                        ADD = false
                    });
                    continue;
                }

                var hasher = new PasswordHasher<InfoTable>();
                targetUser = new InfoTable { Id = s.Id, Name = s.Name, RoleId = 2 };

                targetUser.Pass = hasher.HashPassword(targetUser, s.Pass);
                //targetGPA = new StudnetGpa { StuId = student.Id, StuGpa = student.StudentGPA };
                await _dbContext.InfoTable.AddAsync(targetUser);
                await _dbContext.SaveChangesAsync();
                if (s.StudentGPA != double.NaN)
                {
                    await GpaSet(s);
                }
                ViewBag.templist = true;
                TempRepository.AddStudent(s);

            }
            ViewBag.inserted = true;
            return View(TempRepository.Inserted);
        }

        //view all the student on the system 
        public IActionResult view(string searchString)
        {
            TempRepository.RemoveAll();
            var StudentData = _dbContext.InfoTable.OrderBy(s => s.Id);
            var StudentGpa = _dbContext.StudnetGpa.OrderBy(s => s.StuId);
            var ResultList = from data in StudentData
                             join Gpa in StudentGpa
                             on data.Id equals Gpa.StuId
                             select new { data.Id, data.Name, Gpa.StuGpa };


            var FinalData = ResultList.Select(s => new { s.Id, s.Name, s.StuGpa });

            if (!string.IsNullOrEmpty(searchString))
            {
                FinalData = FinalData.Where(s => s.Id.Contains(searchString) || s.Name.Contains(searchString));
            }
            foreach (var f in FinalData)
            {
                TempRepository.AddStudent(new NewStudentInsert { Id = f.Id, Name = f.Name, StudentGPA = f.StuGpa });
            }

            return View(TempRepository.Inserted);
          

        }



    }
}

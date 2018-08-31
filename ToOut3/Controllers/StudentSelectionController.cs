using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToOut3.Models;
using ToOut3.Models.ViewModels;
namespace ToOut3.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentSelectionController:Controller
    {
        private StudentSelection2Context _dbContext;

       
        public StudentSelectionController(StudentSelection2Context dbContext)
        {
            _dbContext = dbContext;
        }
        [HttpGet]
        public IActionResult StudentSelect()
        {
            StuSelect student = new StuSelect();
      
            student.user = GetCurrentUserData(User.Identity.Name);
            if (student.user.FinalSelection != null)
            {
                return RedirectToAction("FinalSelection");
            }
            else
            {

                if (student.user.DepartmentSelection[0] == null)
                {
                    student.ChangeUpdate = false;
                }
                IQueryable<Department> AvilableDepartment = _dbContext.Department.AsQueryable();
                student.Department = AvilableDepartment;
                return View(student);
            }
        }
        
        [HttpPost]
        public async Task<IActionResult> StudentSelect(StuSelect student)
        {
            
            if (!ModelState.IsValid)
            {
                return RedirectToAction("FinalSelection");
            }
            else
            {
                var targetUser = _dbContext.StuSelection.SingleOrDefault(i => i.StuId.Equals(User.Identity.Name, StringComparison.CurrentCulture));

                targetUser = new StuSelection { StuId = User.Identity.Name, FirstSelection = student.Selection.Department1, SecondSelection = student.Selection.Department2, ShirdSelection = student.Selection.Department3, FourthSelection = student.Selection.Department4 };
                await _dbContext.StuSelection.AddAsync(targetUser);
                await _dbContext.SaveChangesAsync();
                
                return RedirectToAction("StudentSelect", "StudentSelection");
            }
        }
        [HttpPost]
        public IActionResult StudentSelectChange(StuSelect student)
        {

            if (!ModelState.IsValid)
            {
                return RedirectToAction("StudentSelect", "StudentSelection");
            }
            else
            {
                //var targetUser = _dbContext.StuSelection.SingleOrDefault(i => i.StuId.Equals(User.Identity.Name, StringComparison.CurrentCulture));

                var targetUser = new StuSelection { StuId = User.Identity.Name, FirstSelection = student.Selection.Department1, SecondSelection = student.Selection.Department2, ShirdSelection = student.Selection.Department3, FourthSelection = student.Selection.Department4 };

                _dbContext.StuSelection.Update(targetUser);
                _dbContext.SaveChanges();
                

                return RedirectToAction("Index", "Home");
            }
        }

        public ViewResult FinalSelection()
        {
            CurrentUser user = new CurrentUser();
            user = GetCurrentUserData(User.Identity.Name);
            return View (user);
        }


        private CurrentUser GetCurrentUserData(string ID)
        {
            CurrentUser temp = new CurrentUser();


            var studentInfo = (from info in _dbContext.InfoTable
                              join GPA in _dbContext.StudnetGpa
                              on info.Id equals GPA.StuId
                              where info.Id == ID
                              select new
                              {
                                  info.Id,
                                  info.Name,
                                  GPA.StuGpa
                              }).ToList() ;

            var studentSelection = (from info in studentInfo
                                   join selection in _dbContext.StuSelection
                                   on info.Id equals selection.StuId
                                   select new
                                   {
                                       selection.FirstSelection,
                                       selection.SecondSelection,
                                       selection.ShirdSelection,
                                       selection.FourthSelection
                                   }).ToList();

            var finalSelection = (from info in studentInfo
                                 join final in _dbContext.FinalDistribution
                                 on info.Id equals final.StuId
                                 select new
                                 {
                                     final.DepId
                                 }).ToList();

            var department = (from Dep in _dbContext.Department
                             select new
                             {
                                 Dep.Dname
                             }).ToList();

            temp.ID = studentInfo[0].Id;
            temp.Name = studentInfo[0].Name;
            temp.GPA = studentInfo[0].StuGpa;
            temp.DepartmentSelection[0] = department[studentSelection[0].FirstSelection - 1].Dname;
            temp.DepartmentSelection[1] = department[studentSelection[0].SecondSelection - 1].Dname;
            temp.DepartmentSelection[2] = department[studentSelection[0].ShirdSelection - 1].Dname;
            temp.DepartmentSelection[3] = department[studentSelection[0].FourthSelection - 1].Dname;

            if(finalSelection.Count > 0)
            {
                temp.FinalSelection = department[finalSelection[0].DepId-1].Dname;
            }






            //var Name = _dbContext.InfoTable.Where(s => s.Id == ID).Select(s => new { s.Name }).ToList();
            //var GPA = _dbContext.StudnetGpa.Where(s => s.StuId == ID).Select(s => new { s.StuGpa }).ToList();
            //temp.ID = ID;
            //temp.Name = Name[0].Name;
            //if (!User.IsInRole("Administrator"))
            //{
            //    var StudentSelection = _dbContext.StuSelection.Where(s => s.StuId.Equals(ID)).Select(s => new { s.FirstSelection, s.SecondSelection, s.ShirdSelection, s.FourthSelection }).ToList();
            //    if (StudentSelection.Count != 0)
            //    {
            //        var Dep1 = _dbContext.Department.Where(s => s.Did == StudentSelection[0].FirstSelection).Select(s => new { s.Dname }).ToList();
            //        var Dep2 = _dbContext.Department.Where(s => s.Did == StudentSelection[0].SecondSelection).Select(s => new { s.Dname }).ToList();
            //        var Dep3 = _dbContext.Department.Where(s => s.Did == StudentSelection[0].ShirdSelection).Select(s => new { s.Dname }).ToList();
            //        var Dep4 = _dbContext.Department.Where(s => s.Did == StudentSelection[0].FourthSelection).Select(s => new { s.Dname }).ToList();
            //        if (Dep1.Count != 0)
            //        {
            //            temp.DepartmentSelection[0] = Dep1[0].Dname;
            //            temp.DepartmentSelection[1] = Dep2[0].Dname;
            //            temp.DepartmentSelection[2] = Dep3[0].Dname;
            //            temp.DepartmentSelection[3] = Dep4[0].Dname;
            //        }


            //    }
            //    temp.GPA = GPA[0].StuGpa;


            //}
            return temp;

        }

    }
}

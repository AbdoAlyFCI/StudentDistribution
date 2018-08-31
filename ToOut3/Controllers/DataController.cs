using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ToOut3.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace ToOut3.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class DataController:Controller
    {
        private StudentSelection2Context _dbContext;
        public DataController(StudentSelection2Context dbContext)
        {
            _dbContext = dbContext;
        }

        //update data 
        public IActionResult Update(string ID)
        {
            var targetUser = _dbContext.InfoTable.SingleOrDefault(i => i.Id.Equals(ID, StringComparison.CurrentCulture) && i.RoleId==2);

            if (targetUser == null)
            {
                return View("error");
            }
            else
            {
                var TargetStudent = from info in _dbContext.InfoTable
                                    join GPA in _dbContext.StudnetGpa on info.Id equals GPA.StuId
                                    //join Selection in _dbContext.StuSelection on info.Id equals Selection.StuId
                                                                     
                                    where info.Id == ID
                                    select new
                                    {
                                        ID=info.Id,
                                        Name=info.Name,
                                        SGPA=GPA.StuGpa,

                                    };
                var TargetSelction = from Tuser in TargetStudent
                                join Selection in _dbContext.StuSelection on Tuser.ID equals Selection.StuId
                                where Tuser.ID == ID
                                select new
                                {
                                    First_Selection = Selection.FirstSelection,
                                    Second_Selection = Selection.SecondSelection,
                                    Third_Selection = Selection.ShirdSelection,
                                    Fourth_Selection = Selection.FourthSelection,
                                };
                var TargetFinal = from Tuser in TargetStudent
                                  join Final in _dbContext.FinalDistribution on Tuser.ID equals Final.StuId
                                  where Tuser.ID == ID
                                  select new
                                  {
                                      SFinal = Final.DepId
                                  };


                var Department = _dbContext.Department.Select(s => new { s.Dname }).ToList();
                CurrentUser user = new CurrentUser();
                foreach (var item in TargetStudent)
                {
                    user.ID = item.ID;
                    user.Name = item.Name;
                    user.GPA = item.SGPA;
                    foreach (var Select in TargetSelction)
                    {
                        user.DepartmentSelection[0] = Department[Select.First_Selection - 1].Dname;
                        user.DepartmentSelection[1] = Department[Select.Second_Selection - 1].Dname;
                        user.DepartmentSelection[2] = Department[Select.Third_Selection - 1].Dname;
                        user.DepartmentSelection[3] = Department[Select.Fourth_Selection - 1].Dname;
                    }
                    foreach (var final in TargetFinal)
                    {
                        user.FinalSelection = Department[final.SFinal - 1].Dname;
                    }
                    
                }
                return View(user);
            }

        }
    }
}

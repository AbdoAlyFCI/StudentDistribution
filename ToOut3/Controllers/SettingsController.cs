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
    [Authorize(Roles = "Administrator")]
    public class SettingsController:Controller
    {
        private StudentSelection2Context _dbContext;

        public SettingsController(StudentSelection2Context dbContext)
        {
            _dbContext = dbContext;
        }

        public ViewResult Index()
        {
            return View();
        }

        [HttpGet]
        public ViewResult Department()
        {
           
            return View(new DepartmentManagment { departments = GetDepartments() });
        }

        [HttpPost]
        public async Task<IActionResult> Department(DepartmentManagment newdepartment)
        {
            if (!ModelState.IsValid)
            {
                return View(new DepartmentManagment { departments = GetDepartments() });
            }
            int id = _dbContext.Department.Count() + 1;
            string name = newdepartment.Name.ToUpper();
            await _dbContext.Department.AddAsync(new Department { Did = id, Dname = name });
            await _dbContext.SaveChangesAsync();
            return  RedirectToAction("Department");
        }

        [HttpGet]
        public ViewResult AddAdmin()
        {          
            return View(new AdminInsertSetting { AdminList = GetAdmin() });
        }

        [HttpPost]
        public async Task<IActionResult> AddAdmin(AdminInsertSetting NewAdmin)
        {
            if (!ModelState.IsValid)
            {
                return View(new AdminInsertSetting { AdminList = GetAdmin() });
            }
            NewAdmin.Admin.ID = NewAdmin.Admin.ID.Trim();
            NewAdmin.Admin.Name = NewAdmin.Admin.Name.Trim();
            NewAdmin.Admin.Password = NewAdmin.Admin.Password.Trim();
            NewAdmin.Admin.RePassword = NewAdmin.Admin.RePassword.Trim();
            var targetAdmin = _dbContext.InfoTable.SingleOrDefault(i => i.Id == NewAdmin.Admin.ID);
            if (targetAdmin != null)
            {
                throw new Exception("User name already exists.");
            }
            var hasher = new PasswordHasher<InfoTable>();
            targetAdmin = new InfoTable { Id = NewAdmin.Admin.ID, Name = NewAdmin.Admin.Name, RoleId = 3 };
            targetAdmin.Pass = hasher.HashPassword(targetAdmin, NewAdmin.Admin.Password);


            await _dbContext.InfoTable.AddAsync(targetAdmin);
            await _dbContext.SaveChangesAsync();
            return View(new AdminInsertSetting { AdminList = GetAdmin() });
        }

        [HttpGet]
        public ViewResult MyAccount()
        {
            return View(new ViewInfo { UserInfo = GetInfos() });
        }

        [HttpPost]
        public ViewResult MyAccount(ViewInfo ChangePassword )
        {
            if (!ModelState.IsValid)
            {
                return View(new ViewInfo { UserInfo = GetInfos() });
            }
            var targetUser = _dbContext.InfoTable.SingleOrDefault(s => s.Id.Equals(User.Identity.Name));
            var hasher = new PasswordHasher<InfoTable>();
            var result = hasher.VerifyHashedPassword(targetUser, targetUser.Pass, ChangePassword.NewPassword.CurrentPassword);
            //if(result !=PasswordVerificationResult.Success)
            //{
            //    throw new Exception("The password is wrong.");
            //}
            using (var context =  _dbContext)
            {
                var user = context.InfoTable.First(s=>s.Id.Equals(User.Identity.Name));
                user.Pass = hasher.HashPassword(targetUser, targetUser.Pass);
                context.SaveChanges();
            }

            return View(new ViewInfo { UserInfo = GetInfos() });



        }
        [HttpGet]
        public IActionResult Distribution()
        {
            int DISnum = _dbContext.FinalDistribution.Count();
            if (DISnum != 0)
            {
                return RedirectToAction("FinalDistribution");
            }

            DistributionModel distribution = new DistributionModel();
            distribution.StudentNum = _dbContext.InfoTable.Where(i => i.RoleId == 2).Count();
            distribution.StudentRegistered = _dbContext.StuSelection.Count();
            var Department = _dbContext.Department.Select(s => new { s.Dname }).ToList();
            int num = distribution.StudentNum / Department.Count();
            for (int i = 0; i < Department.Count(); i++)
            {
                if (i != Department.Count() - 1)
                {
                    distribution.Department.Add(new DepartmentSuggest { Name = Department[i].Dname, number = num });
                }
                else
                {
                    int fullnum = distribution.StudentNum - (num * (Department.Count() - 1));
                    distribution.Department.Add(new DepartmentSuggest { Name = Department[i].Dname, number = fullnum });
                }
            }
            return View(distribution);

        }

        [HttpPost]
        public async Task<ViewResult> Distribution(DistributionModel newModel)
        {
            List<StuSelection> lists = GetSelects().ToList();
            List<StudnetGpa> GPAList = GetGPA().ToList();
           
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < newModel.Department.Count; j++)
                {
                    List<string> temp = new List<string>();
                    int num = 0;
                    if (i == 0) {
                        
                        int appliedNum = lists.Count(s => s.FirstSelection == (j+1));
                        var subList = lists.Where(s => s.FirstSelection == (j + 1)).Select(s => new { s.StuId, s.FirstSelection });
                        if (appliedNum <= newModel.Department[j].number)
                        {
                            foreach (var item in subList)
                            {
                                await _dbContext.FinalDistribution.AddAsync(new FinalDistribution { StuId = item.StuId, DepId = item.FirstSelection });
                                num++;
                                temp.Add(item.StuId);
                                
                            }
                        }
                        else
                        {
                            var shortList = subList.Join(
                                GPAList, s => s.StuId, s => s.StuId,
                                (sl, sg) => new
                                {
                                    id = sl.StuId,
                                    select = sl.FirstSelection,
                                    gpa = sg.StuGpa
                                })
                                .OrderByDescending(s => s.gpa).ToList();

                            for (int k = 0; k < newModel.Department[j].number; k++)
                            {
                                await _dbContext.FinalDistribution.AddAsync(new FinalDistribution { StuId = shortList[k].id, DepId = shortList[k].select });
                                num++;
                                temp.Add(shortList[k].id);

                            }
                        }
                        foreach (var item in temp)
                        {
                            lists.RemoveAll(s => s.StuId == item);
                        }
                        newModel.Department[j].number = newModel.Department[j].number - num;
                    }
                    else if (i == 1)
                    {
                        int appliedNum = lists.Count(s => s.SecondSelection == (j + 1));
                        var subList = lists.Where(s => s.SecondSelection == (j + 1)).Select(s => new { s.StuId, s.SecondSelection });
                        if (appliedNum <= newModel.Department[j].number)
                        {
                            foreach (var item in subList)
                            {
                                await _dbContext.FinalDistribution.AddAsync(new FinalDistribution { StuId = item.StuId, DepId = item.SecondSelection });
                                num++;
                                temp.Add(item.StuId);
                            }
                        }
                        else
                        {
                            var shortList = subList.Join(
                                GPAList, s => s.StuId, s => s.StuId,
                                (sl, sg) => new
                                {
                                    id = sl.StuId,
                                    select = sl.SecondSelection,
                                    gpa = sg.StuGpa
                                })
                                .OrderByDescending(s => s.gpa).ToList();

                            for (int k = 0; k < newModel.Department[j].number; k++)
                            {
                                await _dbContext.FinalDistribution.AddAsync(new FinalDistribution { StuId = shortList[k].id, DepId = shortList[k].select });
                                num++;
                                temp.Add(shortList[k].id);
                            }
                        }
                        foreach (var item in temp)
                        {
                            lists.RemoveAll(s => s.StuId == item);
                        }
                        newModel.Department[j].number = newModel.Department[j].number - num;
                    }
                    else if (i == 2)
                    {
                        int appliedNum = lists.Count(s => s.ShirdSelection == (j + 1));
                        var subList = lists.Where(s => s.ShirdSelection == (j + 1)).Select(s => new { s.StuId, s.ShirdSelection });
                        if (appliedNum <= newModel.Department[j].number)
                        {
                            foreach (var item in subList)
                            {
                                await _dbContext.FinalDistribution.AddAsync(new FinalDistribution { StuId = item.StuId, DepId = item.ShirdSelection });
                                num++;
                                temp.Add(item.StuId);
                            }
                        }
                        else
                        {
                            var shortList = subList.Join(
                                GPAList, s => s.StuId, s => s.StuId,
                                (sl, sg) => new
                                {
                                    id = sl.StuId,
                                    select = sl.ShirdSelection,
                                    gpa = sg.StuGpa
                                })
                                .OrderByDescending(s => s.gpa).ToList();

                            for (int k = 0; k < newModel.Department[j].number; k++)
                            {
                                await _dbContext.FinalDistribution.AddAsync(new FinalDistribution { StuId = shortList[k].id, DepId = shortList[k].select });
                                num++;
                                temp.Add(shortList[k].id);
                            }
                        }
                        foreach (var item in temp)
                        {
                            lists.RemoveAll(s => s.StuId == item);
                        }
                        newModel.Department[j].number = newModel.Department[j].number - num;
                    }
                    else if (i == 3)
                    {
                        int appliedNum = lists.Count(s => s.FourthSelection == (j + 1));
                        var subList = lists.Where(s => s.FirstSelection == (j + 1)).Select(s => new { s.StuId, s.FourthSelection });
                        if (appliedNum <= newModel.Department[j].number)
                        {
                            foreach (var item in subList)
                            {
                                await _dbContext.FinalDistribution.AddAsync(new FinalDistribution { StuId = item.StuId, DepId = item.FourthSelection });
                                num++;
                                temp.Add(item.StuId);
                            }
                        }
                        else
                        {
                            var shortList = subList.Join(
                                GPAList, s => s.StuId, s => s.StuId,
                                (sl, sg) => new
                                {
                                    id = sl.StuId,
                                    select = sl.FourthSelection,
                                    gpa = sg.StuGpa
                                })
                                .OrderByDescending(s => s.gpa).ToList();

                            for (int k = 0; k < newModel.Department[j].number; k++)
                            {
                                await _dbContext.FinalDistribution.AddAsync(new FinalDistribution { StuId = shortList[k].id, DepId = shortList[k].select });
                                num++;
                                temp.Add(shortList[k].id);
                            }
                        }
                        foreach (var item in temp)
                        {
                            lists.RemoveAll(s => s.StuId == item);
                        }
                        newModel.Department[j].number = newModel.Department[j].number - num;
                    }
                }
            }

            var leftStudent = _dbContext.InfoTable.Where(s => s.RoleId == 2).ToList().Select(s => s.Id);
            leftStudent = leftStudent.Except(ids());
            var left = leftStudent.ToList();
            int dleft = 0;
            for(int i=0;i<leftStudent.Count();i++)
            {
                if(newModel.Department[dleft].number != 0)
                {
                    await _dbContext.FinalDistribution.AddAsync(new FinalDistribution { StuId =left[i], DepId = dleft+1 });
                    newModel.Department[dleft].number--;
                }
                else
                {
                    i--;
                    dleft++;
                }
            }


            await _dbContext.SaveChangesAsync();
            return View("index");
        }

        public ViewResult FinalDistribution()
        {
            List<FinalModel> finals = new List<FinalModel>();
            var Department = GetDepartments();
            
            var finalDistribution = _dbContext.FinalDistribution.OrderBy(s => s.StuId);
            var StudentData = _dbContext.InfoTable.OrderBy(s => s.Id);
            var StudentGPA = _dbContext.StudnetGpa.OrderBy(s => s.StuId);
            var FirstResult = from distribution in finalDistribution
                              join info in StudentData
                              on distribution.StuId equals info.Id
                              select new { distribution.StuId, distribution.DepId, info.Name };
            var SecondResult = from first in FirstResult
                               join GPA in StudentGPA
                               on first.StuId equals GPA.StuId
                               select new { first.StuId, first.Name, first.DepId, GPA.StuGpa };

            for (int i = 0; i < Department.Count(); i++)
            {

                List<FinalStudent> temp = new List<FinalStudent>();
                foreach (var item in SecondResult)
                {
                    if (item.DepId == i+1)
                    {
                        temp.Add(new FinalStudent { ID = item.StuId, Name = item.Name, GPA = item.StuGpa });
                    }
                    
                }
                finals.Add(new FinalModel { Department = Department[i].Dname, AllStudent = temp });

            }
            return View(finals);
        }











        private IQueryable<InfoTable> GetAdmin()
        {

            IQueryable < InfoTable > Admin= _dbContext.InfoTable.Where(s => s.RoleId == 1 ||s.RoleId==3);
            return Admin;
        }



        private List<Department> GetDepartments()
        {
            var Department = _dbContext.Department.ToList();
            List<Department> department = new List<Department>();
            for (int i = 0; i < Department.Count(); i++)
            {
                department.Add(new Department { Did = Department[i].Did, Dname = Department[i].Dname });

            }
            

            return department;
        }

        private CurrentAdmin GetInfos()
        {
            CurrentAdmin current = new CurrentAdmin();
            string id = User.Identity.Name;
            var infos = _dbContext.InfoTable.Where(s => s.Id.Equals(id)).Select(x => new { x.Id, x.Name, x.RoleId }).ToList();
            current.ID = infos[0].Id;
            current.Name = infos[0].Name;
            
            if (infos[0].RoleId == 1)
            {
                current.Role = "Super Admin";
            }
            else if (infos[0].RoleId == 3)
            {
                current.Role = "Admin";
            }

            return current;
        }

       private IQueryable<StuSelection> GetSelects()
        {
            return _dbContext.StuSelection.AsQueryable();
        }

        private IQueryable<StudnetGpa> GetGPA()
        {
            return _dbContext.StudnetGpa.AsQueryable();
        }

        private IEnumerable<string> ids()
        {
            return _dbContext.StuSelection.Select(s => s.StuId).ToList();
        }
    }

    

   
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToOut3.Models;

namespace ToOut3.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AnalyticsController:Controller
    {
        private StudentSelection2Context _dbContext;
        public AnalyticsController(StudentSelection2Context dbContext)
        {
            _dbContext = dbContext;
        }
        public ViewResult Index()
        {
            List<AnalyticsModel.Dep> temp = new List<AnalyticsModel.Dep>();
            List<AnalyticsModel.Dep> temp2 = new List<AnalyticsModel.Dep>();
            List<AnalyticsModel.Dep> temp3 = new List<AnalyticsModel.Dep>();
            List<AnalyticsModel.Dep> temp4 = new List<AnalyticsModel.Dep>();
            AnalyticsModel N = new AnalyticsModel();
            
            var First = (from first in _dbContext.StuSelection
                         group first by first.FirstSelection into g
                         select new { list = g.Count() ,name=g }).ToList();

            var Second = (from second in _dbContext.StuSelection
                         group second by second.SecondSelection into g
                         select new { list = g.Count(), name = g }).ToList();

            var Third = (from third in _dbContext.StuSelection
                         group third by third.ShirdSelection into g
                         select new { list = g.Count(), name = g }).ToList();

            var Fourth = (from fouth in _dbContext.StuSelection
                         group fouth by fouth.FourthSelection into g
                         select new { list = g.Count(), name = g }).ToList();


            var Department = _dbContext.Department.Select(s => new { s.Dname,s.Did }).ToList();
            //First Selection
            for (int i = 0, k = 0; i < Department.Count && k < First.Count; i++, k++)
            {
                
                if (Department[i].Did != First[k].name.Key)
                {
                    temp.Add(new AnalyticsModel.Dep { name= Department[i].Dname,num= 0 });
                    k--;
                }
                else
                {
                    temp.Add(new AnalyticsModel.Dep { name = Department[i].Dname, num = First[k].list });                    
                }
            }
            N.DepartmentSelection.Add("First Selection", temp);
            //temp.Clear();
            ////Second Selection
            for (int i = 0, k = 0; i < Department.Count; i++, k++)
            {
                if (k == Second.Count)
                {
                    temp2.Add(new AnalyticsModel.Dep { name = Department[i].Dname, num = 0 });
                    break;
                }
                if (Department[i].Did != Second[k].name.Key)
                {
                    temp2.Add(new AnalyticsModel.Dep { name = Department[i].Dname, num = 0 });
                    k--;
                }
                else
                {
                    temp2.Add(new AnalyticsModel.Dep { name = Department[i].Dname, num = Second[k].list });
                }
            }
            N.DepartmentSelection.Add("Second Selection", temp2);
    
            //Third Selection
            for (int i = 0, k = 0; i < Department.Count && k < Third.Count; i++, k++)
            {
                if (Department[i].Did != Third[k].name.Key)
                {
                    temp3.Add(new AnalyticsModel.Dep { name = Department[i].Dname, num = 0 });
                    k--;
                }
                else
                {
                    temp3.Add(new AnalyticsModel.Dep { name = Department[i].Dname, num = Third[k].list });
                }
            }
            N.DepartmentSelection.Add("Third Selection", temp3);
           
            //Fourth Selection
            for (int i = 0, k = 0; i < Department.Count && k < Fourth.Count; i++, k++)
            {
                if (Department[i].Did != Fourth[k].name.Key)
                {
                    temp4.Add(new AnalyticsModel.Dep { name = Department[i].Dname, num = 0 });
                    k--;
                }
                else
                {
                    temp4.Add(new AnalyticsModel.Dep { name = Department[i].Dname, num = Fourth[k].list });
                }
            }
            N.DepartmentSelection.Add("Fourth Selection", temp4);


            var MaxMin = from de in _dbContext.FinalDistribution
                         join gpa in _dbContext.StudnetGpa
                         on de.StuId equals gpa.StuId
                         select new { de.DepId, de.StuId, gpa.StuGpa };


            for (int i = 0; i < Department.Count; i++)
            {
                var finalMN = (from MN in MaxMin
                               where MN.DepId == i+1
                               select new
                               {
                                   GPAliST = MN.StuGpa
                               }).ToList();
                AnalyticsModel.MinMax minMax = new AnalyticsModel.MinMax();
                minMax.DepName =  Department[i].Dname;
                minMax.Max = finalMN.Max(s => s.GPAliST);
                minMax.Min = finalMN.Min(s=>s.GPAliST);


                N.minMaxes.Add(minMax);
            }
           
            return View(N);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ToOut3.Models.ViewModels
{
    public  class StuSelect
    {
        public IQueryable<Department> Department { get; set; }
        public  StudentDepartmentSelection Selection { get; set; }
        public  CurrentUser user { get; set; }
        public bool ChangeUpdate { get; set; } = true;
        public bool startchange { get; set; } = false;
        //public string FinalDepartment { get; set; }
        public StuSelect()
        {
            //Selection = new StudentDepartmentSelection();
           
        }

    }
}

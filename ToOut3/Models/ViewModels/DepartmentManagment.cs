using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ToOut3.Models.ViewModels
{
    public class DepartmentManagment
    {
        public IEnumerable<Department> departments { get; set; }

        [Required(ErrorMessage = "Select name for new department")]
        public String Name { get; set; }
    }
}

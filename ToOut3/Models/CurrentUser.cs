using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ToOut3.Models
{
    public class CurrentUser
    {
        public string ID { get; set; }
        public string Name { get; set; }
        
        public double GPA { get; set; }
        public string[] DepartmentSelection { get; set; } = new string[4];
        public string FinalSelection { get; set; } = "-";
    }
}

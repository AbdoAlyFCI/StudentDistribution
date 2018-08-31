using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ToOut3.Models
{
    public class StudentDepartmentSelection
    {
        [Required(ErrorMessage ="Select First Department")]
       
    
        public int Department1 { get; set; }
        [Required(ErrorMessage = "Select Second Department")]
        public int Department2 { get; set; }
        [Required(ErrorMessage = "Select Third Department")]
        public int Department3 { get; set; }
        [Required(ErrorMessage = "Select Fourth Department")]
        public int Department4 { get; set; }

        //public string ID { get; set; }
    }
}

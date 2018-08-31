using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ToOut3.Models
{
    public class NewStudentInsert
    {
        [Required(ErrorMessage = "Please enter student ID")]
        public string Id { get; set; }
        [Required(ErrorMessage = "Please enter student name")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Please enter student Pass")]
        public string Pass { get; set; }
        [Required(ErrorMessage = "Please repeat student Pass")]
        [Compare("Pass",ErrorMessage ="Password donot match")]
        public string RepeatPass { get; set; }
        public int RoleId { get; set; }
        [Range(0.0,4.0,ErrorMessage ="GPA Out of Range")]
        public double StudentGPA { get; set; }

        public bool ADD { get; set; } = true;

    }
}

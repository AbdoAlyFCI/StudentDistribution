using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ToOut3.Models.ViewModels
{
    public class ViewStudent
    {
        public string Search { get; set; }
        public IEnumerable<NewStudentInsert> Student { get; set; }
        public ViewStudent(IEnumerable<NewStudentInsert> student)
        {
            Student = student;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ToOut3.Models;
namespace ToOut3.Models.ViewModels
{
    public class AllInfo
    {
        public  IEnumerable<NewStudentInsert> Studentslist{get;set ;}
        public NewStudentInsert newStudnet { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ToOut3.Models;
namespace ToOut3.Models
{
    public static class TempRepository
    {
        
        private static List<NewStudentInsert> inserted = new List<NewStudentInsert>();
        public static IEnumerable<NewStudentInsert> Inserted
        {
            get
            {
                return inserted;
            }
        }
        public static void AddStudent(NewStudentInsert response)
        {
            inserted.Add(response);
        }
        public static void RemoveAll()
        {
            inserted.Clear();
        }
    }
}

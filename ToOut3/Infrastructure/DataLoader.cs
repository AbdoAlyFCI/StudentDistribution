using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ToOut3.Models;
namespace ToOut3.Infrastructure
{

    public class DataLoader
    {
        public static string FilePath { get; set; }
        public static IEnumerable<NewStudentInsert> Load(string file)
        {
          
            var reader = new StreamReader(File.OpenRead(file));
            List<NewStudentInsert> StudentList = new List<NewStudentInsert>();
            while (!reader.EndOfStream)
            {
                var values = reader.ReadLine().Split(',');
                StudentList.Add(new NewStudentInsert
                {
                    Id = values[0],
                    Name = values[1],
                    Pass = values[2],
                    RepeatPass = values[3],                  
                    StudentGPA = Convert.ToDouble(values[4])
                });               
            }
            reader.Close();
            
            return StudentList;
        }
     
       
    }
}


   


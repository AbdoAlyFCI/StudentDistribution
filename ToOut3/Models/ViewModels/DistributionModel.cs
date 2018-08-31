using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ToOut3.Models.ViewModels
{

    public  class DistributionModel
    {
        public  int StudentNum { get; set; }
        public  int StudentRegistered { get; set; }
        public List<DepartmentSuggest> Department { get; set; } = new List<DepartmentSuggest>();
        public  IEnumerable <FinalDistribution> finalDistributions { get; set; }

    }
}

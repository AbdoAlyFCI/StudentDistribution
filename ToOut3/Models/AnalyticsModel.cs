using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ToOut3.Models
{
    
    public class AnalyticsModel
    {
        public struct MinMax
        {
            public string DepName { get; set; }
            public double Max { get; set; }
            public double Min { get; set; }
        }
        public struct Dep
        {
            public String name { get; set; }
            public int num { get; set; }
        }
        public Dictionary<string, List<Dep>> DepartmentSelection { get; set; } = new Dictionary<string, List<Dep>>();
        public List<MinMax> minMaxes { get; set; } = new List<MinMax>();
      //  public List<String> Dep { get; set; }
    }
}

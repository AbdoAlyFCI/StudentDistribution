using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ToOut3.Models.ViewModels
{
    public class AdminInsertSetting
    {
        public IQueryable<InfoTable> AdminList { get; set; }
        public NewAdminInsert Admin { get; set; }
    }
}

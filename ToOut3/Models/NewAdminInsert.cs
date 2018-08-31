using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ToOut3.Models
{
    public class NewAdminInsert
    {
        [Required(ErrorMessage = "Enter Admin ID")]
        public string ID { get; set; }
        [Required(ErrorMessage ="Enter Admin name")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Enter Admin Password")]
        public string Password { get; set; }
        [Required(ErrorMessage = "ReEnter Admin Password")]
        [Compare("Password", ErrorMessage = "Password donot match")]
        public string RePassword { get; set; }
    }
}

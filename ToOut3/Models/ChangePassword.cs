using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ToOut3.Models
{
    public class ChangePassword
    {
        [Required]
        public string CurrentPassword { get; set; }
        [Required]
        public string NewPassword { get; set; }
        [Required(ErrorMessage = "Confirm the password")]
        [Compare("NewPassword", ErrorMessage = "Password donot match")]
        public string Confirm { get; set; }


    }
}

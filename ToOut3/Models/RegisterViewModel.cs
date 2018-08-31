using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ToOut3.Models
{
    public class RegisterViewModel
    {
        [Required, DisplayName("Name")]
        public string Name { get; set; }
        [Required, DisplayName("Password")]
        public string Password { get; set; }
        [Required, DisplayName("Repeat Password")]
        public string RepeatPassword { get; set; }
        [Required, DisplayName("Id")]
        public string Id { get; set; }
    }
}

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ToOut3.Models
{
    public class LogInViewModel
    {
        [Required, DisplayName("ID")]
        public string ID { get; set; }

        [Required, DisplayName("Password")]
        public string Password { get; set; }
    }
}
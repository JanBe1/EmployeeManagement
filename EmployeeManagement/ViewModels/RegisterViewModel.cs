using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.ViewModels
{
    public class RegisterViewModel // carry data from register view to registercontroller
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)] //masking characters with dots -> model validation - video part 42, 43
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)] //masking characters with dots
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "Passwords do not match. Try again")]
        public string ConfirmPassword { get; set; }
    }
}

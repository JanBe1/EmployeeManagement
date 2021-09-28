using EmployeManagment.Utilities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

/// <summary>
/// View Model for Registration View: Get data from user and pass it to database later
/// </summary>
namespace EmployeeManagement.ViewModels
{
    public class RegisterViewModel // carry data from register view to registercontroller
    {
        [Required]
        [EmailAddress]
        [Remote(action: "IsEmailInUse", controller: "Account")] //check if email is in use
        [ValidEmailDomain(allowedDomain: "bernardzik.com", ErrorMessage ="Invalid domain name. Try email@bernardzik.com")] //custom validation attribute
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)] //masking characters with dots -> model validation - video part 42, 43
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)] //masking characters with dots
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "Passwords do not match. Try again")]
        public string ConfirmPassword { get; set; }

        public string City { get; set; }
    }
}

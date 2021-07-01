using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Models
{
    public class Employee
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(50, ErrorMessage = "Name cannot exceed 50 characters")]
        public string Name { get; set; }
        [Required]
        [RegularExpression(@"[A-Za-z0-9+-_.]+@[A-Za-z0-9]+\.[A-za-z0-9-.]+$", ErrorMessage = "Invalid E-mail Address")]
        [Display(Name="Office E-mail")]
        public string Email { get; set; }
        [Required]
        public Dept? Department { get; set; }
    }
}

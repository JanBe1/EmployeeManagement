using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeManagment.Utilities
{
    //uzywane jesli chcemy wymagac od uzytkownika czegos specyficznego do kontroli maili
    // allowedDomain w tym przypadku oznacza 2 czesc maila, deklarujemy ja w registerViewModel
    // dzieki czemu mozemy wymagac od uzytkownika np. gmaila lub o2
    public class ValidEmailDomainAttribute : ValidationAttribute
    {
        private readonly string allowedDomain;

        public ValidEmailDomainAttribute(string allowedDomain)
        {
            this.allowedDomain = allowedDomain;
        }
        public override bool IsValid(object value)
        {
            // value ==Email bo wywolane w register view model
            string[] strings = value.ToString()
                                  .Split('@');
            return strings[1].ToUpper() == allowedDomain.ToUpper();
        }

    }
}
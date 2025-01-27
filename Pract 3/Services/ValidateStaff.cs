using Pract_3.Pages;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Pract_3.Models;
using System.Threading.Tasks;

namespace Pract_3.Services
{
    internal class ValidateStaff
    {
        public string ValidateStaf(Staff staff)
        {
            var errorMessages = new List<string>();
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(staff, null, null);
            bool isValid = Validator.TryValidateObject(staff, validationContext, validationResults, true);

            if (!isValid)
            {
                errorMessages.AddRange(validationResults.Select(vr => vr.ErrorMessage));
            }
            return string.Join("\n", errorMessages);
        }
    }
}

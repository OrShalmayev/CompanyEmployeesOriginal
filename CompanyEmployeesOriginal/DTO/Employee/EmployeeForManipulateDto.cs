using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CompanyEmployeesOriginal.Helpers;

namespace CompanyEmployeesOriginal.DTO.Employee
{
    public abstract class EmployeeForManipulateDto
    {
        [Required(ErrorMessage = nameof(Employee) + " " + nameof(Name) + CustomModelValidationData.RequiredField)]
        [MaxLength(CustomModelValidationData.MaxLengthName, ErrorMessage = nameof(Employee) + " " + nameof(Name) + CustomModelValidationData.MaxLength)]
        public string Name { get; set; }

        [Range(
            CustomModelValidationData.MinAgeValue,
            int.MaxValue,
            ErrorMessage = "Age is required and it can't be lower than 18"
        )]
        public int Age { get; set; }

        [Required(ErrorMessage = nameof(Employee) + " " + nameof(Position) + CustomModelValidationData.RequiredField)]
        [MaxLength(CustomModelValidationData.MaxLengthPosition, ErrorMessage = nameof(Employee) + " " + nameof(Name) + CustomModelValidationData.MaxLength)]
        public string Position { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CompanyEmployeesOriginal.Helpers
{
    public class CustomModelValidationData
    {
        /// <summary>
        /// Messages
        /// </summary>
        public const string
            RequiredField = " is a required field.",
            MaxLength = "Maximum length for the {0} is {1} characters.";
        /// <summary>
        /// Data holders
        /// </summary>
        public const int
            MaxLengthName = 30,
            MaxLengthPosition = 20,
            MinAgeValue = 18;
    }
}

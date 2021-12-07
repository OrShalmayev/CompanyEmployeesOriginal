using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    class Company
    {
        public Guid CompanyId { get; set; }
        [Required(ErrorMessage = "Company name is required")]
        [StringLength(100, ErrorMessage = "Company name cannot be longer than 100 characters")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Company address is required")]
        [StringLength(100, ErrorMessage = "Company address cannot be longer than 100 characters")]
        public string Address { get; set; }
        public string Country { get; set; }
        public ICollection<Employee> Employees { get; set; }
    }
}

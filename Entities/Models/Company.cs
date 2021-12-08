using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entities.Models
{
    public class Company
    {
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Company name is required")]
        [StringLength(100, ErrorMessage = "Company name cannot be longer than 100 characters")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Company address is required")]
        [StringLength(100, ErrorMessage = "Company address cannot be longer than 100 characters")]
        public string Address { get; set; }
        public string Country { get; set; }
        public ICollection<Employee> Employees { get; set; }
    }

    public class CompanyConfiguration : IEntityTypeConfiguration<Company>
    {
        public void Configure(EntityTypeBuilder<Company> builder)
        {
            builder.HasData
                (
                    new Company
                    {
                        Id = new Guid("c9d4c053-49b6-410c-bc78-2d54a9991870"),
                        Name = "IT_Solutions Ltd",
                        Address = "583 Wall Dr. Gwynn Oak, MD 21207",
                        Country = "USA"
                    },
                    new Company
                    {
                        Id = new Guid("3d490a70-94ce-4d15-9494-5248280c2ce3"),
                        Name = "Admin_Solutions Ltd",
                        Address = "312 Forest Avenue, BF 923",
                        Country = "USA"
                    }
                );
        }
    }
}

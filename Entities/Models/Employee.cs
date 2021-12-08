using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class Employee 
{
    public Guid Id { get; set; }
    [Required(ErrorMessage = "Employee name is a required field.")]
    [MaxLength(50, ErrorMessage = "Employee name cannot be longer than 50 characters.")]
    public string Name { get; set; }
    [Required(ErrorMessage = "Age is a required field.")]
    public int Age {get; set;}
    public string Position { get; set; }
    [ForeignKey(nameof(Company))]
    public Guid CompanyId { get; set; }
    public Company Company { get; set; }
}

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.HasData
        (
        new Employee
        {
            Id = new Guid("80abbca8-664d-4b20-b5de-024705497d4a"),
            Name = "Sam Raiden",
            Age = 26,
            Position = "Software developer",
            CompanyId = new Guid("c9d4c053-49b6-410c-bc78-2d54a9991870")
        },
        new Employee
        {
            Id = new Guid("86dba8c0-d178-41e7-938c-ed49778fb52a"),
            Name = "Jana McLeaf",
            Age = 30,
            Position = "Software developer",
            CompanyId = new Guid("c9d4c053-49b6-410c-bc78-2d54a9991870")
        },
        new Employee
        {
            Id = new Guid("021ca3c1-0deb-4afd-ae94-2159a8479811"),
            Name = "Kane Miller",
            Age = 35,
            Position = "Administrator",
            CompanyId = new Guid("3d490a70-94ce-4d15-9494-5248280c2ce3")
        },
        new Employee
        {
            Id = new Guid("222ca3c1-0deb-4afd-ae94-2159a8479811"),
            Name = "Or Shalmayev",
            Age = 28,
            Position = "Administrator",
            CompanyId = new Guid("3d490a70-94ce-4d15-9494-5248280c2ce3")
        }
        );
    }
}

public class Employee 
{
    public Guid Id { get; set; }
    [Required(ErrorMessage = "Employee name is a required field.")]
    [MaxLength(50, ErrorMessage = "Employee name cannot be longer than 50 characters.")]
    public string Name { get; set; }
    [Required(ErrorMessage = "Age is a required field.")]
    public int Age {get; set;}
    
    public string Position { get; set; }

}
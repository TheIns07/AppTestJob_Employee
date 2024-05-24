using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppTestJob.Models;

public partial class Employee
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Name is required.")]
    [StringLength(100)]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "Address is required.")]
    [StringLength(200)]
    public string Address { get; set; } = null!;

    [Required(ErrorMessage = "DOB is required.")]
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
    public DateTime Dob { get; set; }

    [Required(ErrorMessage = "Salary is required.")]
    [Column(TypeName = "decimal(18, 2)")]
    [Range(100, 50000, ErrorMessage = "Salary must be between 100 and 50000")]
    public decimal Salary { get; set; }

    [Required(ErrorMessage = "Active State is required.")]
    public bool IsActive { get; set; }
}

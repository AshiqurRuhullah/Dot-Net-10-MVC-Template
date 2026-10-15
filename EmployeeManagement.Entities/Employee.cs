using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EmployeeManagement.Entities;

    public class Employee
    {
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    [Display(Name = "Full Name")]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(150)]
    public string Email { get; set; } = string.Empty;

    [StringLength(20)]
    public string? Phone { get; set; }

    [Required]
    [StringLength(100)]
    public string Department { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Designation { get; set; } = string.Empty;

    [Range(0, 9999999)]
    public decimal Salary { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Joining Date")]
    public DateTime JoiningDate { get; set; } = DateTime.Today;

    [Display(Name = "Active")]
    public bool IsActive { get; set; } = true;
};

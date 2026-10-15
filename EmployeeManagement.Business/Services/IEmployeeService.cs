using EmployeeManagement.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeManagement.Business.Services;

public interface IEmployeeService
{
    Task<List<Employee>> GetAllAsync();
    Task<Employee?> GetByIdAsync(int id);
    Task<string?> CreateAsync(Employee employee);
    Task<string?> UpdateAsync(Employee employee);
    Task<bool> DeleteAsync(int id);
}

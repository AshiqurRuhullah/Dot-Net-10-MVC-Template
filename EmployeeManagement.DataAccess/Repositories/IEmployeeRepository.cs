using EmployeeManagement.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeManagement.DataAccess.Repositories;

public interface IEmployeeRepository
{
    Task<List<Employee>> GetAllAsync();
    Task<Employee?> GetByIdAsync(int id);
    Task<bool> EmailExistsAsync(string email, int excludeId = 0);
    Task AddAsync(Employee employee);
    Task UpdateAsync(Employee employee);
    Task DeleteAsync(Employee employee);
}

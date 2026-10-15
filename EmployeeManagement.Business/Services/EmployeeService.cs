using EmployeeManagement.DataAccess.Repositories;
using EmployeeManagement.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeManagement.Business.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _repository;

    public EmployeeService(IEmployeeRepository repository)
    {
        _repository = repository;
    }

    public Task<List<Employee>> GetAllAsync()
    {
        return _repository.GetAllAsync();
    }

    public Task<Employee?> GetByIdAsync(int id)
    {
        return _repository.GetByIdAsync(id);
    }

    // Returns null on success, or an error message when a business rule fails.
    public async Task<string?> CreateAsync(Employee employee)
    {
        employee.Email = employee.Email.Trim();

        if (await _repository.EmailExistsAsync(employee.Email))
        {
            return "Another employee already uses this email.";
        }

        await _repository.AddAsync(employee);
        return null;
    }

    public async Task<string?> UpdateAsync(Employee employee)
    {
        employee.Email = employee.Email.Trim();

        if (await _repository.EmailExistsAsync(employee.Email, employee.Id))
        {
            return "Another employee already uses this email.";
        }

        await _repository.UpdateAsync(employee);
        return null;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var employee = await _repository.GetByIdAsync(id);
        if (employee is null)
        {
            return false;
        }

        await _repository.DeleteAsync(employee);
        return true;
    }
}

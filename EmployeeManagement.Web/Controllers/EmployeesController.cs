using EmployeeManagement.Business.Services;
using EmployeeManagement.Entities;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Web.Controllers;

public class EmployeesController : Controller
{
    private readonly IEmployeeService _employeeService;

    public EmployeesController(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    // GET: /Employees
    public async Task<IActionResult> Index()
    {
        var employees = await _employeeService.GetAllAsync();
        return View(employees);
    }

    // GET: /Employees/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var employee = await _employeeService.GetByIdAsync(id);
        if (employee is null)
        {
            return NotFound();
        }

        return View(employee);
    }

    // GET: /Employees/Create
    public IActionResult Create()
    {
        return View(new Employee());
    }

    // POST: /Employees/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Employee employee)
    {
        if (!ModelState.IsValid)
        {
            return View(employee);
        }

        var error = await _employeeService.CreateAsync(employee);
        if (error is not null)
        {
            ModelState.AddModelError(nameof(Employee.Email), error);
            return View(employee);
        }

        TempData["Success"] = "Employee created successfully.";
        return RedirectToAction(nameof(Index));
    }

    // GET: /Employees/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var employee = await _employeeService.GetByIdAsync(id);
        if (employee is null)
        {
            return NotFound();
        }

        return View(employee);
    }

    // POST: /Employees/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Employee employee)
    {
        if (id != employee.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(employee);
        }

        var error = await _employeeService.UpdateAsync(employee);
        if (error is not null)
        {
            ModelState.AddModelError(nameof(Employee.Email), error);
            return View(employee);
        }

        TempData["Success"] = "Employee updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    // GET: /Employees/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var employee = await _employeeService.GetByIdAsync(id);
        if (employee is null)
        {
            return NotFound();
        }

        return View(employee);
    }

    // POST: /Employees/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var deleted = await _employeeService.DeleteAsync(id);
        if (!deleted)
        {
            return NotFound();
        }

        TempData["Success"] = "Employee deleted successfully.";
        return RedirectToAction(nameof(Index));
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVCCRUD.Data;
using MVCCRUD.Filters;
using MVCCRUD.Models;
using MVCCRUD.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVCCRUD.Controllers
{
    [Authorize(Roles = "admin")]
    public class EmployeeController : Controller
    {
        private readonly Mydbcontext _myDbContext;

        public EmployeeController(Mydbcontext myDbContext)
        {
            _myDbContext = myDbContext;
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddEmployeeViewModel addEmployeeViewModel)
        {
            var employee = new Employee
            {
                Id = Guid.NewGuid(),
                Name = addEmployeeViewModel.Name,
                DateofBirth = addEmployeeViewModel.DateofBirth,
                Department = addEmployeeViewModel.Department,
                Email = addEmployeeViewModel.Email,
                Salary = addEmployeeViewModel.Salary,
                Surname=addEmployeeViewModel.Surname,
            };

            await _myDbContext.Employees.AddAsync(employee);
            await _myDbContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? page)
        {
            const int pageSize = 5;
            int pageNumber = page ?? 1;

            var employees = await _myDbContext.Employees.ToListAsync();
            var paginatedEmployees = Paginate(employees, pageNumber, pageSize);

            return View(paginatedEmployees);
        }

        [HttpGet]
        public async Task<IActionResult> View(Guid id)
        {
            var employee = await _myDbContext.Employees.FirstOrDefaultAsync(e => e.Id == id);

            if (employee != null)
            {
                var viewModel = new UpdateEmployee
                {
                    Id = employee.Id,
                    Name = employee.Name,
                    DateofBirth = employee.DateofBirth,
                    Department = employee.Department,
                    Email = employee.Email,
                    Salary = employee.Salary,
                    Surname = employee.Surname,
                };

                return View("View", viewModel);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> View(UpdateEmployee model)
        {
            var employee = await _myDbContext.Employees.FindAsync(model.Id);
            if (employee != null)
            {
                employee.Name = model.Name;
                employee.Salary = model.Salary;
                employee.Email = model.Email;
                employee.DateofBirth = model.DateofBirth;
                employee.Department = model.Department;
                employee.Surname = model.Surname;
                await _myDbContext.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(UpdateEmployee model)
        {
            var employee = await _myDbContext.Employees.FindAsync(model.Id);
            if (employee != null)
            {
                _myDbContext.Employees.Remove(employee);
                await _myDbContext.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }

        private PaginationViewModel<Employee> Paginate(List<Employee> items, int pageNumber, int pageSize)
        {
            var paginatedItems = items.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            var paginationViewModel = new PaginationViewModel<Employee>
            {
                Items = paginatedItems,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalItems = items.Count
            };

            return paginationViewModel;
        }
        [HttpGet]
        public async Task<IActionResult> DownloadSchedule()
        {
            var employees = await _myDbContext.Employees.ToListAsync();

            // Generate Excel using EPPlus
            var stream = new MemoryStream();
            using (var package = new OfficeOpenXml.ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add("Schedule");

                // Add headers
                worksheet.Cells["A1"].Value = "Name";
                worksheet.Cells["B1"].Value = "Surname";
                worksheet.Cells["C1"].Value = "Email";
                worksheet.Cells["D1"].Value = "Department";
                worksheet.Cells["E1"].Value = "Date of Birth";
                worksheet.Cells["F1"].Value = "Salary";

                // Populate data
                var row = 2;
                foreach (var employee in employees)
                {
                    worksheet.Cells[$"A{row}"].Value = employee.Name;
                    worksheet.Cells[$"B{row}"].Value = employee.Surname;
                    worksheet.Cells[$"C{row}"].Value = employee.Email;
                    worksheet.Cells[$"D{row}"].Value = employee.Department;
                    worksheet.Cells[$"E{row}"].Value = employee.DateofBirth.ToString("yyyy-MM-dd");
                    worksheet.Cells[$"F{row}"].Value = employee.Salary;
                    row++;
                }

                package.Save();
            }

            stream.Position = 0;

            // Return the Excel file
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Schedule.xlsx");
        }
    }

}


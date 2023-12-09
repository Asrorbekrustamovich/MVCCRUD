using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVCCRUD.Data;
using MVCCRUD.Filters;
using MVCCRUD.Models;
using MVCCRUD.Models.Domain;

namespace MVCCRUD.Controllers
{
    [CacheResourceFilter]
    [Authorize]
    public class EmployeeController : Controller
    {
        private readonly Mydbcontext _mydbcontext;

        public EmployeeController(Mydbcontext mydbcontext)
        {
            _mydbcontext = mydbcontext;
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }
        [HttpPost]
        public async Task <IActionResult> Add(AddEmployeeViewModel addEmployeeViewModel) 
        {
            var employee = new Employee()
            {
                Id = Guid.NewGuid(),
                Name = addEmployeeViewModel.Name,
                DateofBirth = addEmployeeViewModel.DateofBirth,
                Department = addEmployeeViewModel.Department,
                Email = addEmployeeViewModel.Email,
                Salary = addEmployeeViewModel.Salary,
            };
            await _mydbcontext.Employees.AddAsync(employee);
            _mydbcontext.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> Index(int? page)
        {
            const int pageSize = 1;
            int pageNumber = page ?? 1;

            var employees = await _mydbcontext.Employees.ToListAsync();

            var paginatedEmployees = Paginate(employees, pageNumber, pageSize);

            return View(paginatedEmployees);
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
        public async Task<IActionResult>View(Guid id)
        { var employee=await _mydbcontext.Employees.FirstOrDefaultAsync(e => e.Id == id);
            if (employee != null)
            {
                var ViewModel = new UpdateEmployee()
                {

                    Id = employee.Id,
                    Name = employee.Name,
                    DateofBirth = employee.DateofBirth,
                    Department = employee.Department,
                    Email = employee.Email,
                    Salary = employee.Salary,
                };
                return await Task.Run(() => View("View", ViewModel));

            }

            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> View(UpdateEmployee model)
        {
            var employee = await _mydbcontext.Employees.FindAsync(model.Id);
            if (employee != null)
            {
                employee.Name = model.Name;
                employee.Salary = model.Salary;
                employee.Email = model.Email;
                employee.DateofBirth = model.DateofBirth;
                employee.Department = model.Department;
                await _mydbcontext.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> Delete(UpdateEmployee model)
        {
            var employee = await _mydbcontext.Employees.FindAsync(model.Id);
            if (employee != null)
            {
                _mydbcontext.Employees.Remove(employee);
                _mydbcontext.SaveChanges();
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }
    }
}


using CRUD_Web_Api_Application.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CRUD_Web_Api_Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private static List<Employee> employees = new List<Employee>();
        private static int nextId = 1;
        private readonly IWebHostEnvironment _env;

        public EmployeeController(IWebHostEnvironment env)
        {
            _env = env; // ✅ Assigning env to avoid null reference
        }

        // GET all employees
        [HttpGet]
        public ActionResult<IEnumerable<Employee>> GetAll()
        {
            return Ok(employees);
        }

        // GET employee by id
        [HttpGet("{id}")]
        public ActionResult<Employee> GetById(int id)
        {
            var emp = employees.FirstOrDefault(e => e.Id == id);
            if (emp == null) return NotFound(new { message = "Employee not found" });
           
            
            return Ok(emp);
        }

        // POST create employee (with photo upload)
        [HttpPost]
        public IActionResult Create([FromForm] Employee emp, IFormFile? PhotoFile)
        {
            // Save uploaded photo if any
            if (PhotoFile != null && PhotoFile.Length > 0)
            {
                var uploads = Path.Combine(_env.WebRootPath, "Uploads");
                if (!Directory.Exists(uploads))
                    Directory.CreateDirectory(uploads);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(PhotoFile.FileName);
                var filePath = Path.Combine(uploads, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    PhotoFile.CopyTo(stream);
                }

                emp.PhotoFileName = fileName;
            }

            // Assign ID and save employee
            emp.Id = nextId++;
            employees.Add(emp);

            return CreatedAtAction(nameof(GetById), new { id = emp.Id }, emp);
        }

        // PUT update employee
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromForm] Employee updated, IFormFile? PhotoFile)
        {
            var emp = employees.FirstOrDefault(e => e.Id == id);
            if (emp == null) return NotFound(new { message = "Employee not found" });

            // Update text fields
            emp.Name = updated.Name;
            emp.Code = updated.Code;
            emp.DOB = updated.DOB;
            emp.City = updated.City;
            emp.Gender = updated.Gender;
            emp.Status = updated.Status;

            // Update photo if a new one is uploaded
            if (PhotoFile != null && PhotoFile.Length > 0)
            {
                var uploads = Path.Combine(_env.WebRootPath, "Uploads");
                if (!Directory.Exists(uploads))
                    Directory.CreateDirectory(uploads);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(PhotoFile.FileName);
                var filePath = Path.Combine(uploads, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    PhotoFile.CopyTo(stream);
                }

                emp.PhotoFileName = fileName;
            }

            return NoContent();
        }

        // DELETE employee
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var emp = employees.FirstOrDefault(e => e.Id == id);
            if (emp == null) return NotFound(new { message = "Employee not found" });

            employees.Remove(emp);
            return NoContent();
        }
    }
}

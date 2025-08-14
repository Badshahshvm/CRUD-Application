using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class EmployeeController : Controller
    {
        public string url = "https://localhost:7036/api/Employee/";
        private HttpClient client=new HttpClient();


        [HttpGet]
        public IActionResult Index()
        {
            List<Employee> employees = new List<Employee>();
            HttpResponseMessage response=client.GetAsync(url).Result;

            if(response.IsSuccessStatusCode)
            {
                string result=response.Content.ReadAsStringAsync().Result;
                var data = JsonConvert.DeserializeObject<List<Employee>>(result);
                if(data!=null)
                {
                    employees.AddRange(data);
                }
            }
            return View(employees);
        }


       
        [HttpGet]
         public IActionResult Create()
        {
            return View();
        }




        [HttpPost,ActionName("Create")]
        public IActionResult CreateEmployee(Employee employee, IFormFile? PhotoFile)
        {
            using (var content = new MultipartFormDataContent())
            {
                // Add text fields
                content.Add(new StringContent(employee.Name ?? ""), "Name");
                content.Add(new StringContent(employee.Code ?? ""), "Code");
                content.Add(new StringContent(employee.DOB.ToString("yyyy-MM-dd")), "DOB");
                content.Add(new StringContent(employee.City ?? ""), "City");
                content.Add(new StringContent(employee.Gender ?? ""), "Gender");
                content.Add(new StringContent(employee.Status ?? ""), "Status");

                // Add file if uploaded
                if (PhotoFile != null && PhotoFile.Length > 0)
                {
                    var fileContent = new StreamContent(PhotoFile.OpenReadStream());
                    fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(PhotoFile.ContentType);

                    // The name "photo" must match the API parameter name [FromForm] IFormFile photo
                    content.Add(fileContent, "PhotoFile", PhotoFile.FileName);
                }

                // Send POST request
                HttpResponseMessage response = client.PostAsync(url, content).Result;

                if (response.IsSuccessStatusCode)
                {
                    TempData["Insert_message"] = "Employee Added Successfully.";
                    return RedirectToAction("Index");
                }
            }

            // If failed, return same view with validation errors
            ModelState.AddModelError("", "Failed to add employee.");
            return View(employee);
        }


        [HttpGet]
        public IActionResult Edit(int id)
        {
            Employee employee=new Employee();
            // Send POST request
            HttpResponseMessage response = client.GetAsync(url+id).Result;

            if(response.IsSuccessStatusCode)
            {
                string result = response.Content.ReadAsStringAsync().Result;
                var data = JsonConvert.DeserializeObject<Employee>(result);
                if (data != null)
                {
                    employee=data;
                }
            }
            return View(employee);
        }



        [HttpPost, ActionName("Edit")]
        public IActionResult EditEmployee(Employee employee, IFormFile? PhotoFile)
        {
            using (var content = new MultipartFormDataContent())
            {
                // Add text fields
                content.Add(new StringContent(employee.Name ?? ""), "Name");
                content.Add(new StringContent(employee.Code ?? ""), "Code");
                content.Add(new StringContent(employee.DOB.ToString("yyyy-MM-dd")), "DOB");
                content.Add(new StringContent(employee.City ?? ""), "City");
                content.Add(new StringContent(employee.Gender ?? ""), "Gender");
                content.Add(new StringContent(employee.Status ?? ""), "Status");

                // Add file if uploaded
                if (PhotoFile != null && PhotoFile.Length > 0)
                {
                    var fileContent = new StreamContent(PhotoFile.OpenReadStream());
                    fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(PhotoFile.ContentType);
                    content.Add(fileContent, "PhotoFile", PhotoFile.FileName);
                }

                // Send PUT request to API
                HttpResponseMessage response = client.PutAsync(url + employee.Id, content).Result;

                if (response.IsSuccessStatusCode)
                {
                    TempData["Update_message"] = "Employee Updated Successfully.";
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Failed to update employee.");
                }
            }

            // Return view if update fails
            return View(employee);
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            Employee employee = null;

            // Send GET request to API
            HttpResponseMessage response = client.GetAsync(url + id).Result;

            if (response.IsSuccessStatusCode)
            {
                // Deserialize the response content to Employee object
                var empData = response.Content.ReadAsStringAsync().Result;
                employee = Newtonsoft.Json.JsonConvert.DeserializeObject<Employee>(empData);
            }
            else
            {
                // Optionally handle not found or error case
                TempData["Error"] = "Unable to fetch employee details.";
                return RedirectToAction("Index");
            }

            return View(employee);
        }



        [HttpGet]
        public IActionResult Delete(int id)
        {
            Employee employee = new Employee();
            // Send POST request
            HttpResponseMessage response = client.GetAsync(url + id).Result;

            if (response.IsSuccessStatusCode)
            {
                string result = response.Content.ReadAsStringAsync().Result;
                var data = JsonConvert.DeserializeObject<Employee>(result);
                if (data != null)
                {
                    employee = data;
                }
            }
            return View(employee);
        }

        [HttpPost,ActionName("Delete")]
        public IActionResult DeleteEmployee(int id)
        {
           
            // Send POST request
            HttpResponseMessage response = client.DeleteAsync(url + id).Result;

            if (response.IsSuccessStatusCode)
            {
                TempData["Update_message"] = "Employee Updated Successfully.";
                return RedirectToAction("Index");
            }
            return View();
        }



    }
}

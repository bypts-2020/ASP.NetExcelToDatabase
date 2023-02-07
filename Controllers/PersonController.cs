using ExcelDataReader;
using Microsoft.AspNetCore.Mvc;
using UploadExcel.Models;

namespace UploadExcel.Controllers
{
    public class PersonController : Controller
    {
        [HttpGet]
        public IActionResult Index(List<Person> person)
        {
            person = person == null ? new List<Person>() : person;
            return View(person);
        }

        [HttpPost]
        public IActionResult Index(IFormFile file, [FromServices] IWebHostEnvironment hostingEnvironment)
        {
            string fileName = $"{hostingEnvironment.WebRootPath}\\files\\{file.FileName}";
            using (FileStream fileStream = System.IO.File.Create(fileName))
            {
                file.CopyTo(fileStream);
                fileStream.Flush();
            }
            var person = this.GetPersonList(file.FileName);
            return Index(person);

        }

        private List<Person> GetPersonList(string fileName)
        {
            HttpContext.Session.SetString("fileName", fileName);
            List<Person> person = new List<Person>();
            var getFile = $"{Directory.GetCurrentDirectory()}{@"\wwwroot\files"}"+"\\"+ fileName;
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            using (var stream = System.IO.File.Open(getFile, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    while (reader.Read())
                    {
                        person.Add(new Person()
                        {
                            Id = reader.GetValue(0).ToString(),
                            FirstName = reader.GetValue(1).ToString(),
                            LastName = reader.GetValue(2).ToString(),
                            Email = reader.GetValue(3).ToString(),
                            Gender = reader.GetValue(4).ToString()
                        });
                    }
                }
            }
            return person;
        }
    }
}

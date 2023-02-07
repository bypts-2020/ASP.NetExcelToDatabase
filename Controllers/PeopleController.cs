using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UploadExcel.Data;
using UploadExcel.Models;
using ExcelDataReader;

namespace UploadExcel.Controllers
{
    public class PeopleController : Controller
    {
        public const string FileSessionKey = "fileName";
        private readonly UploadExcelContext _context;

        public PeopleController(UploadExcelContext context)
        {
            _context = context;
        }

        // GET: People
        public async Task<IActionResult> Index()
        {
              return View(await _context.People.ToListAsync());
        }

        // GET: People/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.People == null)
            {
                return NotFound();
            }

            var people = await _context.People
                .FirstOrDefaultAsync(m => m.Id == id);
            if (people == null)
            {
                return NotFound();
            }

            return View(people);
        }

        // GET: People/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: People/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,Email,Gender")] People people)
        {
            if (ModelState.IsValid)
            {
                _context.Add(people);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(people);
        }

        public async  Task<string> SaveExcelData(string id, string first, string last, string email, string gender)
        {

            if (ModelState.IsValid)
            {
                var find = await _context.People.FindAsync(id);
                if (find == null)
                {
                    var people = new People();
                    people.Id = id;
                    people.FirstName = first;
                    people.LastName = last;
                    people.Email = email;
                    people.Gender = gender;

                    _context.Add(people);
                }
                else
                {
                    find.FirstName = first;
                    find.LastName = last;
                    find.Email = email;
                    find.Gender = gender;

                    _context.Update(find);
                }

                await _context.SaveChangesAsync();
            }
            return "true";
        }

        public async Task<IActionResult> SaveExcel(string id, string first, string last, string email, string gender)
        {
            
            if (ModelState.IsValid)
            {
                await SaveExcelData(id, first, last, email, gender);
                return RedirectToAction(nameof(Index));
            }
            return  View("Index");
        }

        public async Task<IActionResult> SaveAllExcel()
        {
            var fileName = HttpContext.Session.GetString(FileSessionKey);
            List<People> person = new List<People>();
            var getFile = $"{Directory.GetCurrentDirectory()}{@"\wwwroot\files"}" + "\\" + fileName;
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            using (var stream = System.IO.File.Open(getFile, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    while (reader.Read())
                    {
                        await SaveExcelData(reader.GetValue(0).ToString(), reader.GetValue(1).ToString(), reader.GetValue(2).ToString(), reader.GetValue(3).ToString(), reader.GetValue(4).ToString());
                    }

                    return RedirectToAction(nameof(Index));
                }
            }
            return View("Index");
        }

        // GET: People/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.People == null)
            {
                return NotFound();
            }

            var people = await _context.People.FindAsync(id);
            if (people == null)
            {
                return NotFound();
            }
            return View(people);
        }

        // POST: People/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,FirstName,LastName,Email,Gender")] People people)
        {
            if (id != people.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(people);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PeopleExists(people.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(people);
        }

        // GET: People/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.People == null)
            {
                return NotFound();
            }

            var people = await _context.People
                .FirstOrDefaultAsync(m => m.Id == id);
            if (people == null)
            {
                return NotFound();
            }

            return View(people);
        }

        // POST: People/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.People == null)
            {
                return Problem("Entity set 'UploadExcelContext.People'  is null.");
            }
            var people = await _context.People.FindAsync(id);
            if (people != null)
            {
                _context.People.Remove(people);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PeopleExists(string id)
        {
          return _context.People.Any(e => e.Id == id);
        }
    }
}

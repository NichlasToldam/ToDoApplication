using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoApplication.Data;
using ToDoApplication.Models;

namespace ToDoApplication.Controllers
{
    public class ToDoesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ToDoesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ToDoes
        public async Task<IActionResult> Index()
        {
            return View();
        }
        
        private IQueryable<ToDo> GetMyToDoes()
        {
            IdentityUser currentUser = _context.Users.Where(x => x.Email == User.Identity.Name).FirstOrDefault();// Nichlas: Get the currrent user
            return _context.toDos.Where(x => x.User == currentUser);
        }

        public ActionResult BuildToDoTable()
        {
            IdentityUser currentUser = _context.Users.Where(x => x.Email == User.Identity.Name).FirstOrDefault();// Nichlas: Get the currrent user

            return PartialView(
                "_ToDoTable", GetMyToDoes() /*_context.toDos.Where(x => x.User == currentUser)*/); // Nichlas: Only display the ToDoes made by the current user
        }

        // GET: ToDoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var toDo = await _context.toDos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (toDo == null)
            {
                return NotFound();
            }

            return View(toDo);
        }

        // GET: ToDoes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ToDoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Header,Description,IsDoing,IsDone")] ToDo toDo)
        {
            if (ModelState.IsValid)
            {
                // Nichlas: Get the currrent user
                IdentityUser currentUser = _context.Users.Where(x => x.Email == User.Identity.Name).FirstOrDefault();

                if (currentUser != null)
                {
                    // Nichlas: Get the currrent users ID
                    string currentUserId = _context.Users.Where(x => x.Email == User.Identity.Name).FirstOrDefault().Id;

                    // make a reference to the current user in the ToDo model 
                    toDo.User = currentUser; // Nichlas
                }
         
                _context.Add(toDo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(toDo);
        }

        // GET: ToDoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var toDo = await _context.toDos.FindAsync(id);
            if (toDo == null)
            {
                return NotFound();
            }
            return View(toDo);
        }

        // POST: ToDoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Header,Description,IsDoing,IsDone")] ToDo toDo)
        {
            if (id != toDo.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(toDo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ToDoExists(toDo.Id))
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
            return View(toDo);
        }

        // GET: ToDoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var toDo = await _context.toDos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (toDo == null)
            {
                return NotFound();
            }

            return View(toDo);
        }

        // POST: ToDoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var toDo = await _context.toDos.FindAsync(id);
            _context.toDos.Remove(toDo);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ToDoExists(int id)
        {
            return _context.toDos.Any(e => e.Id == id);
        }
    }
}

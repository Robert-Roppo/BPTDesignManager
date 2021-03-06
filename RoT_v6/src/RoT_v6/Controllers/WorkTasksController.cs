using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RoT_v6.Data;
using RoT_v6.Models;
using Microsoft.AspNetCore.Authorization;

namespace RoT_v6.Controllers
{
    [Authorize]
    public class WorkTasksController : Controller
    {
        private readonly ApplicationDbContext _context;
        private decimal HourRate = 75;
        // Divisor to convert time in seconds to hours.
        private decimal timeFactor = 3600;
      
        public WorkTasksController(ApplicationDbContext context)
        {
            _context = context;    
        }

        // GET: WorkTasks
        [Authorize]
        public async Task<IActionResult> Index()
        {
            return View(await _context.WorkTasks.ToListAsync());
        }

        // GET: WorkTasks/DetailsDashboard/5
        [Authorize]
        public async Task<IActionResult> DetailsDashboard(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workTask = await _context.WorkTasks.SingleOrDefaultAsync(m => m.TaskID == id);
            if (workTask == null)
            {
                return NotFound();
            }

            return View(workTask);
        }

        // GET: WorkTasks/DetailsJobDetails/5
        [Authorize]
        public async Task<IActionResult> DetailsJobDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workTask = await _context.WorkTasks.SingleOrDefaultAsync(m => m.TaskID == id);
            if (workTask == null)
            {
                return NotFound();
            }

            return View(workTask);
        }

        // GET: WorkTasks/Create
        [Authorize]
        public IActionResult Create(int id)
        {
            WorkTask worktask = new WorkTask();
            worktask.JobID = id;
            worktask.getEmployees(_context);
            return View(worktask);            
           




        }

        // POST: WorkTasks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create(int JobID, WorkTask workTask)
        {
            workTask.Status = Models.TaskStatus.Created;
            DateTime dateOnly = DateTime.Today;
            workTask.StartDate = dateOnly.ToString("d");

            if (ModelState.IsValid)
            {
                _context.Add(workTask);
                await _context.SaveChangesAsync();


                //foreach (string e in workTask.employee)
                //{
                //    var todent = new EmployeeWorkTask { employeeId = e.ToString(), TaskId = workTask.TaskID };
                //    _context.Add(todent);
                //    await _context.SaveChangesAsync();
                //}




                return RedirectToAction("Details","Jobs",new { id = JobID});
            }
            ViewBag.Fail = "1";
            workTask.getEmployees(_context);
            return View(workTask);
        }





        // GET: WorkTasks/EditDashboard/5
        [Authorize]
        public async Task<IActionResult> EditDashboard(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workTask = await _context.WorkTasks.SingleOrDefaultAsync(m => m.TaskID == id);
            workTask.getEmployees(_context);
            if (workTask == null)
            {
                return NotFound();
            }
            return View(workTask);
        }

        // POST: WorkTasks/EditDashboard/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> EditDashboard(int id, WorkTask workTask)
        {
            if (id != workTask.TaskID)
            {
                return NotFound();
            }

            var oldTask = await _context.WorkTasks.AsNoTracking().SingleOrDefaultAsync(m => m.TaskID == id);

            if (oldTask.TotalTime != workTask.TotalTime)
            {
                var job = await _context.Jobs.SingleOrDefaultAsync(m => m.JobID == workTask.JobID);
                var oldCost = (oldTask.TotalTime * HourRate) / timeFactor;
                var newCost = (workTask.TotalTime * HourRate) / timeFactor;
                job.InvCost = job.InvCost - oldCost + newCost;
                job.InvHours = job.InvHours - oldTask.TotalTime + workTask.TotalTime;
                _context.Jobs.Update(job);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(workTask);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WorkTaskExists(workTask.TaskID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index", "Dashboard");
            }
            ViewBag.Fail = "1";
            workTask.getEmployees(_context);
            return View(workTask);
        }

        // GET: WorkTasks/EditAllTasks/5
        [Authorize]
        public async Task<IActionResult> EditAllTasks(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workTask = await _context.WorkTasks.SingleOrDefaultAsync(m => m.TaskID == id);
            workTask.getEmployees(_context);
            if (workTask == null)
            {
                return NotFound();
            }
            return View(workTask);
        }

        // POST: WorkTasks/EditAllTasks/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> EditAllTasks(int id, WorkTask workTask)
        {
            if (id != workTask.TaskID)
            {
                return NotFound();
            }

            var oldTask = await _context.WorkTasks.AsNoTracking().SingleOrDefaultAsync(m => m.TaskID == id);

            if (oldTask.TotalTime != workTask.TotalTime)
            {
                var job = await _context.Jobs.SingleOrDefaultAsync(m => m.JobID == workTask.JobID);
                var oldCost = (oldTask.TotalTime * HourRate) / timeFactor;
                var newCost = (workTask.TotalTime * HourRate) / timeFactor;
                job.InvCost = job.InvCost - oldCost + newCost;
                job.InvHours = job.InvHours - oldTask.TotalTime + workTask.TotalTime;
                _context.Jobs.Update(job);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(workTask);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WorkTaskExists(workTask.TaskID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("AllTasks", "Dashboard");
            }
            ViewBag.Fail = "1";
            workTask.getEmployees(_context);
            return View(workTask);
        }

        // GET: WorkTasks/EditJobDetails/5
        [Authorize]
        public async Task<IActionResult> EditJobDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workTask = await _context.WorkTasks.SingleOrDefaultAsync(m => m.TaskID == id);
            workTask.getEmployees(_context);
            if (workTask == null)
            {
                return NotFound();
            }
            return View(workTask);
        }

        // POST: WorkTasks/EditJobDetails/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> EditJobDetails(int id,  WorkTask workTask)
        {
            if (id != workTask.TaskID)
            {
                return NotFound();
            }

            var oldTask = await _context.WorkTasks.AsNoTracking().SingleOrDefaultAsync(m => m.TaskID == id);

            if (oldTask.TotalTime != workTask.TotalTime)
            {
                var job = await _context.Jobs.SingleOrDefaultAsync(m => m.JobID == workTask.JobID);
                var oldCost = (oldTask.TotalTime * HourRate) / timeFactor;
                var newCost = (workTask.TotalTime * HourRate) / timeFactor;
                job.InvCost = job.InvCost - oldCost + newCost;
                job.InvHours = job.InvHours - oldTask.TotalTime + workTask.TotalTime;
                _context.Jobs.Update(job);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(workTask);
                    
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WorkTaskExists(workTask.TaskID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", "Jobs", new { id = workTask.JobID });
            }
            ViewBag.Fail = "1";
            workTask.getEmployees(_context);
            return View(workTask);
        }


        // GET: WorkTasks/DeleteDashboard/5
        [Authorize]
        public async Task<IActionResult> DeleteDashboard(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workTask = await _context.WorkTasks.SingleOrDefaultAsync(m => m.TaskID == id);
            if (workTask == null)
            {
                return NotFound();
            }

            return View(workTask);
        }

        // POST: WorkTasks/DeleteDashboard/5
        [Authorize]
        [HttpPost, ActionName("DeleteDashboard")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteDashboardConfirmed(int id)
        {
            var workTask = await _context.WorkTasks.SingleOrDefaultAsync(m => m.TaskID == id);
            var job = await _context.Jobs.SingleOrDefaultAsync(m => m.JobID == workTask.JobID);
            // Update Total Hours in job
            if (job != null)
            {
                job.InvHours = job.InvHours - workTask.TotalTime;
                Decimal cost = (workTask.TotalTime * HourRate) / timeFactor;
                job.InvCost = job.InvCost - cost;
                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(job);
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        throw;
                    }
                }
            }
            _context.WorkTasks.Remove(workTask);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Dashboard");
        }

        // GET: WorkTasks/DeleteDashboard/5
        [Authorize]
        public async Task<IActionResult> DeleteAllTasks(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workTask = await _context.WorkTasks.SingleOrDefaultAsync(m => m.TaskID == id);
            if (workTask == null)
            {
                return NotFound();
            }

            return View(workTask);
        }

        // POST: WorkTasks/DeleteDashboard/5
        [Authorize]
        [HttpPost, ActionName("DeleteAllTasks")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAllTasksConfirmed(int id)
        {
            var workTask = await _context.WorkTasks.SingleOrDefaultAsync(m => m.TaskID == id);
            var job = await _context.Jobs.SingleOrDefaultAsync(m => m.JobID == workTask.JobID);
            // Update Total Hours in job
            if (job != null)
            {
                job.InvHours = job.InvHours - workTask.TotalTime;
                Decimal cost = (workTask.TotalTime * HourRate) / timeFactor;
                job.InvCost = job.InvCost - cost;
                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(job);
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        throw;
                    }
                }
            }
            _context.WorkTasks.Remove(workTask);
            await _context.SaveChangesAsync();
            return RedirectToAction("AllTasks", "Dashboard");
        }

        // GET: WorkTasks/DeleteJobDetails/5
        [Authorize]
        public async Task<IActionResult> DeleteJobDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workTask = await _context.WorkTasks.SingleOrDefaultAsync(m => m.TaskID == id);
            if (workTask == null)
            {
                return NotFound();
            }

            return View(workTask);
        }

        // POST: WorkTasks/DeleteJobDetails/5
        [HttpPost, ActionName("DeleteJobDetails")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteJobDetailsConfirmed(int id)
        {
            var workTask = await _context.WorkTasks.SingleOrDefaultAsync(m => m.TaskID == id);
            var job = await _context.Jobs.SingleOrDefaultAsync(m => m.JobID == workTask.JobID);
            // Update Total Hours in job
            if (job != null)
            {
                job.InvHours = job.InvHours - workTask.TotalTime;
                Decimal cost = (workTask.TotalTime * HourRate) / timeFactor;
                job.InvCost = job.InvCost - cost;
                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(job);
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        throw;
                    }
                }
            }
            _context.WorkTasks.Remove(workTask);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Jobs", new { id = workTask.JobID });
        }

        private bool WorkTaskExists(int id)
        {
            return _context.WorkTasks.Any(e => e.TaskID == id);
        }
    }
}

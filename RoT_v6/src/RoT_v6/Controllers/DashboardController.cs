using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RoT_v6.ViewModels;
using RoT_v6.Models;
using RoT_v6.Data;
using Microsoft.EntityFrameworkCore;

namespace RoT_v6.Controllers
{
    public class DashboardController : Controller
    {
        private readonly RoTContext _context;

        public DashboardController(RoTContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var ToDos = await _context.ToDos.ToListAsync();
            var WorkTasks = await _context.WorkTasks.ToListAsync();
            Dashboard_WorkTaskToDo WorkTaskToDo = new Dashboard_WorkTaskToDo()
            {
                ToDos = ToDos,
                WorkTasks = WorkTasks
            };
            return View(WorkTaskToDo);
        }
    }
}
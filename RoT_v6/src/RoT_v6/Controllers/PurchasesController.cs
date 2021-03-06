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
using RoT_v6.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace RoT_v6.Controllers
{
    [Authorize]
    public class PurchasesController : Controller
    {
        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
       
        public PurchasesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;  
        }
    
        // GET: Purchases
        [Authorize(Roles = "Admin, Purchaser")]
        public async Task<IActionResult> Index()
        {
            var model = new Purchase_IndexThreeTables();
            model.Purchases_New = await _context.Purchase.Where(m => m.PurchDate == null && m.ArrivedDate == null).ToListAsync();
            model.Purchases_Purchased = await _context.Purchase.Where(m => m.PurchDate != null && m.ArrivedDate == null).ToListAsync();
            model.Purchases_Delivered = await _context.Purchase.Where(m => m.PurchDate != null && m.ArrivedDate != null).ToListAsync();

            return View(model);
        }
        [Authorize(Roles = "Admin, Purchaser")]
        // GET: Purchases/DetailsPurchases/5
        public async Task<IActionResult> DetailsPurchases(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var purchase = await _context.Purchase.SingleOrDefaultAsync(m => m.purchID == id);
            if (purchase == null)
            {
                return NotFound();
            }

            return View(purchase);
        }
        [Authorize(Roles = "Admin, Purchaser")]
        // GET: Purchases/DetailsDetails/5
        public async Task<IActionResult> DetailsJobDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var purchase = await _context.Purchase.SingleOrDefaultAsync(m => m.purchID == id);
            if (purchase == null)
            {
                return NotFound();
            }

            return View(purchase);
        }
        [Authorize]
        // GET: Purchases/Create
        public async Task<IActionResult> Create(int id)
        {
            Purchase purchase = new Purchase();
            purchase.JobID = id;
            purchase.Block = true;
            return View(purchase);
        }

        // POST: Purchases/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("purchID,ArrivedDate,Block,CostPer,Description,EstArrDate,IdealDelDate,JobID,PurchDate,Quantity,RequestDate,TotalCost,Vendor,employeeId,Notes,Miscellaneous,ShippingSurcharge")] Purchase purchase)
        {
            DateTime dateOnly = DateTime.Today;
            purchase.RequestDate = dateOnly.ToString("d");
            var user = await GetCurrentUserAsync();
            purchase.employeeId = user.name;
            if (ModelState.IsValid)
            {
                _context.Add(purchase);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Jobs", new { id = purchase.JobID });
            }
            ViewBag.Fail = "1";
            return View(purchase);
        }

        // GET: Purchases/EditJobDetails/5
        [Authorize(Roles = "Admin, Purchaser")]
        public async Task<IActionResult> EditJobDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var purchase = await _context.Purchase.SingleOrDefaultAsync(m => m.purchID == id);
            if (purchase == null)
            {
                return NotFound();
            }
            return View(purchase);
        }

        // POST: Purchases/EditJobDetails/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]        
        [Authorize(Roles = "Admin, Purchaser")]
        public async Task<IActionResult> EditJobDetails(int id, [Bind("purchID,ArrivedDate,Block,CostPer,Description,EstArrDate,IdealDelDate,JobID,PurchDate,Quantity,RequestDate,TotalCost,Vendor,Notes,employeeId,Miscellaneous,ShippingSurcharge")] Purchase purchase)
        {
            if (id != purchase.purchID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(purchase);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PurchaseExists(purchase.purchID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
               }
                return RedirectToAction("Details", "Jobs", new { id = purchase.JobID });
            }
            ViewBag.Fail = "1";
            return View(purchase);
        }

        // GET: Purchases/EditPurchases/5
        
        [Authorize(Roles = "Admin, Purchaser")]
        public async Task<IActionResult> EditPurchases(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var purchase = await _context.Purchase.SingleOrDefaultAsync(m => m.purchID == id);
            if (purchase == null)
            {
                return NotFound();
            }
            return View(purchase);
        }

        // POST: Purchases/EditPurchases/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Purchaser")]
        public async Task<IActionResult> EditPurchases(int id, [Bind("purchID,ArrivedDate,Block,CostPer,Description,EstArrDate,IdealDelDate,JobID,PurchDate,Quantity,RequestDate,TotalCost,Vendor,Notes,employeeId,Miscellaneous,ShippingSurcharge")] Purchase purchase)
        {
            if (id != purchase.purchID)
            {
                return NotFound();
            }

            var oldPurchase = await _context.Purchase.AsNoTracking().SingleOrDefaultAsync(m => m.purchID == id);
            // If the purchase total cost was changed, update the Invested cost in the Database for the given Job
            if (oldPurchase.TotalCost != purchase.TotalCost)
            {
                var job = await _context.Jobs.SingleOrDefaultAsync(m => m.JobID == purchase.JobID);
                job.InvCost = job.InvCost - oldPurchase.TotalCost + purchase.TotalCost;
                _context.Update(job);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(purchase);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PurchaseExists(purchase.purchID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            ViewBag.Fail = "1";
            return View(purchase);
        }

        // GET: Purchases/DeleteJobDetails/5
        [Authorize(Roles = "Admin, Purchaser")]
        public async Task<IActionResult> DeleteJobDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var purchase = await _context.Purchase.SingleOrDefaultAsync(m => m.purchID == id);
            if (purchase == null)
            {
                return NotFound();
            }

            return View(purchase);
        }

        // POST: Purchases/DeleteJobDetails/5
        [HttpPost, ActionName("DeleteJobDetails")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Purchaser")]
        public async Task<IActionResult> DeleteJobDetailsConfirmed(int id)
        {
            var purchase = await _context.Purchase.SingleOrDefaultAsync(m => m.purchID == id);
            _context.Purchase.Remove(purchase);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Jobs", new { id = purchase.JobID });
        }

        // GET: Purchases/DeletePurchases/5
        [Authorize(Roles = "Admin, Purchaser")]
        public async Task<IActionResult> DeletePurchases(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var purchase = await _context.Purchase.SingleOrDefaultAsync(m => m.purchID == id);
            if (purchase == null)
            {
                return NotFound();
            }

            return View(purchase);
        }

        // POST: Purchases/DeletePurchases/5
        [HttpPost, ActionName("DeletePurchases")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Purchaser")]
        public async Task<IActionResult> DeletePurchasesConfirmed(int id)
        {
            var purchase = await _context.Purchase.SingleOrDefaultAsync(m => m.purchID == id);
            _context.Purchase.Remove(purchase);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool PurchaseExists(int id)
        {
            return _context.Purchase.Any(e => e.purchID == id);
        }
    }
}

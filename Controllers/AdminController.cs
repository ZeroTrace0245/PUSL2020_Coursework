using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PUSL2020_Coursework.Data;
using PUSL2020_Coursework.Models;
using PUSL2020_Coursework.ViewModels;

namespace PUSL2020_Coursework.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly PASDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<AdminController> _logger;

        public AdminController(
            PASDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<AdminController> logger)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        public async Task<IActionResult> Dashboard()
        {
            var stats = new Dictionary<string, int>
            {
                { "Total Users", await _context.Users.CountAsync() },
                { "Students", await _context.Users.Where(u => u.IsActive).Join(
                    _context.UserRoles.Where(ur => ur.RoleId == "4"),
                    u => u.Id, ur => ur.UserId, (u, ur) => u).CountAsync() },
                { "Supervisors", await _context.Users.Where(u => u.IsActive).Join(
                    _context.UserRoles.Where(ur => ur.RoleId == "3"),
                    u => u.Id, ur => ur.UserId, (u, ur) => u).CountAsync() },
                { "Total Projects", await _context.Projects.CountAsync() },
                { "Total Matches", await _context.Matches.CountAsync() },
                { "Confirmed Matches", await _context.Matches.Where(m => m.Status == MatchStatus.Confirmed).CountAsync() },
                { "Research Areas", await _context.ResearchAreas.Where(r => r.IsActive).CountAsync() }
            };

            ViewBag.Stats = stats;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> CreateAdmin()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAdmin(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Department = model.Department
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Admin");

                    _logger.LogInformation($"Admin {user.Email} created by {User.Identity?.Name}");

                    TempData["Success"] = "Admin user created successfully.";
                    return RedirectToAction(nameof(Dashboard));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> CreateModuleLeader()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateModuleLeader(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Department = model.Department
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "ModuleLeader");

                    _logger.LogInformation($"Module Leader {user.Email} created by {User.Identity?.Name}");

                    TempData["Success"] = "Module Leader created successfully.";
                    return RedirectToAction(nameof(Dashboard));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignRole(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();

            var roleExists = await _roleManager.RoleExistsAsync(role);
            if (!roleExists)
                return BadRequest("Role does not exist.");

            var userRoles = await _userManager.GetRolesAsync(user);
            if (!userRoles.Contains(role))
            {
                await _userManager.AddToRoleAsync(user, role);
                _logger.LogInformation($"Role '{role}' assigned to {user.Email} by {User.Identity?.Name}");
            }

            TempData["Success"] = $"Role '{role}' assigned successfully.";
            return RedirectToAction(nameof(Dashboard));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveRole(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();

            var userRoles = await _userManager.GetRolesAsync(user);
            if (userRoles.Contains(role))
            {
                await _userManager.RemoveFromRoleAsync(user, role);
                _logger.LogInformation($"Role '{role}' removed from {user.Email} by {User.Identity?.Name}");
            }

            TempData["Success"] = $"Role '{role}' removed successfully.";
            return RedirectToAction(nameof(Dashboard));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();

            var tempPassword = "TempPassword@123";
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            await _userManager.ResetPasswordAsync(user, token, tempPassword);

            _logger.LogInformation($"Password reset for {user.Email} by {User.Identity?.Name}");

            TempData["Success"] = $"Password reset. Temporary password: {tempPassword}";
            return RedirectToAction(nameof(Dashboard));
        }
    }
}

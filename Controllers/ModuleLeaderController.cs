using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PUSL2020_Coursework.Data;
using PUSL2020_Coursework.Models;
using PUSL2020_Coursework.ViewModels;

namespace PUSL2020_Coursework.Controllers
{
    [Authorize(Roles = "ModuleLeader")]
    public class ModuleLeaderController : Controller
    {
        private readonly PASDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<ModuleLeaderController> _logger;

        public ModuleLeaderController(
            PASDbContext context,
            UserManager<ApplicationUser> userManager,
            ILogger<ModuleLeaderController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IActionResult> Dashboard()
        {
            var totalProjects = await _context.Projects.CountAsync();
            var matchedProjects = await _context.Projects
                .Where(p => p.Status == ProjectStatus.Matched)
                .CountAsync();
            var pendingProjects = await _context.Projects
                .Where(p => p.Status == ProjectStatus.Pending)
                .CountAsync();

            var totalMatches = await _context.Matches.CountAsync();
            var confirmedMatches = await _context.Matches
                .Where(m => m.Status == MatchStatus.Confirmed)
                .CountAsync();

            var recentMatches = await _context.Matches
                .Where(m => m.Status == MatchStatus.Confirmed)
                .Include(m => m.Project)
                    .ThenInclude(p => p!.StudentProfile)
                        .ThenInclude(s => s!.User)
                .Include(m => m.SupervisorProfile)
                    .ThenInclude(s => s!.User)
                .OrderByDescending(m => m.ConfirmedDate)
                .Take(10)
                .ToListAsync();

            var viewModel = new AllocationDashboardViewModel
            {
                TotalProjects = totalProjects,
                MatchedProjects = matchedProjects,
                PendingProjects = pendingProjects,
                TotalMatches = totalMatches,
                ConfirmedMatches = confirmedMatches,
                RecentMatches = recentMatches.Select(m => new MatchDetailViewModel
                {
                    MatchId = m.Id,
                    ProjectId = m.ProjectId,
                    ProjectTitle = m.Project?.Title ?? "Unknown",
                    StudentName = m.Project?.StudentProfile?.User?.FirstName + " " + m.Project?.StudentProfile?.User?.LastName,
                    StudentEmail = m.Project?.StudentProfile?.User?.Email ?? "Unknown",
                    SupervisorName = m.SupervisorProfile?.User?.FirstName + " " + m.SupervisorProfile?.User?.LastName,
                    SupervisorEmail = m.SupervisorProfile?.User?.Email ?? "Unknown",
                    Status = m.Status.ToString(),
                    ConfirmedDate = m.ConfirmedDate
                }).ToList()
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> AllAllocations(int page = 1)
        {
            const int pageSize = 20;

            var totalCount = await _context.Matches.CountAsync();
            var matches = await _context.Matches
                .Include(m => m.Project)
                    .ThenInclude(p => p!.StudentProfile)
                        .ThenInclude(s => s!.User)
                .Include(m => m.SupervisorProfile)
                    .ThenInclude(s => s!.User)
                .OrderByDescending(m => m.CreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var viewModels = matches.Select(m => new MatchDetailViewModel
            {
                MatchId = m.Id,
                ProjectId = m.ProjectId,
                ProjectTitle = m.Project?.Title ?? "Unknown",
                StudentName = m.Project?.StudentProfile?.User?.FirstName + " " + m.Project?.StudentProfile?.User?.LastName,
                StudentEmail = m.Project?.StudentProfile?.User?.Email ?? "Unknown",
                SupervisorName = m.SupervisorProfile?.User?.FirstName + " " + m.SupervisorProfile?.User?.LastName,
                SupervisorEmail = m.SupervisorProfile?.User?.Email ?? "Unknown",
                Status = m.Status.ToString(),
                CreatedDate = m.CreatedDate,
                ConfirmedDate = m.ConfirmedDate
            }).ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            ViewBag.TotalMatches = totalCount;

            return View(viewModels);
        }

        [HttpGet]
        public async Task<IActionResult> ProjectDetails(int id)
        {
            var project = await _context.Projects
                .Include(p => p.ResearchArea)
                .Include(p => p.StudentProfile)
                    .ThenInclude(s => s!.User)
                .Include(p => p.Matches)
                    .ThenInclude(m => m.SupervisorProfile)
                        .ThenInclude(s => s!.User)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (project == null)
                return NotFound();

            var viewModel = new ProjectDetailViewModel
            {
                Id = project.Id,
                Title = project.Title,
                Abstract = project.Abstract,
                TechnicalStack = project.TechnicalStack,
                ResearchArea = project.ResearchArea?.Name ?? "Unknown",
                Status = project.Status.ToString(),
                SubmittedDate = project.SubmittedDate,
                StudentName = project.StudentProfile?.User?.FirstName + " " + project.StudentProfile?.User?.LastName,
                StudentEmail = project.StudentProfile?.User?.Email,
                TotalMatches = project.Matches?.Count ?? 0,
                ConfirmedMatches = project.Matches?.Count(m => m.Status == MatchStatus.Confirmed) ?? 0
            };

            ViewBag.Matches = project.Matches ?? new List<Match>();

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReassignProject(int projectId, int supervisorProfileId)
        {
            var project = await _context.Projects
                .Include(p => p.Matches)
                .FirstOrDefaultAsync(p => p.Id == projectId);

            if (project == null)
                return NotFound();

            var supervisorProfile = await _context.SupervisorProfiles
                .FirstOrDefaultAsync(s => s.Id == supervisorProfileId);

            if (supervisorProfile == null)
                return NotFound();

            // Remove existing confirmed matches
            var existingMatches = project.Matches?
                .Where(m => m.Status == MatchStatus.Confirmed)
                .ToList() ?? new List<Match>();

            foreach (var match in existingMatches)
            {
                _context.Matches.Remove(match);
            }

            // Create new confirmed match
            var newMatch = new Match
            {
                ProjectId = projectId,
                SupervisorProfileId = supervisorProfileId,
                Status = MatchStatus.Confirmed,
                CreatedDate = DateTime.UtcNow,
                ConfirmedDate = DateTime.UtcNow
            };

            project.Status = ProjectStatus.Matched;

            _context.Matches.Add(newMatch);
            _context.Projects.Update(project);
            await _context.SaveChangesAsync();

            var user = await _userManager.GetUserAsync(User);
            _logger.LogInformation($"Module Leader {user?.Email} reassigned project {projectId} to supervisor {supervisorProfileId}");

            TempData["Success"] = "Project reassigned successfully.";
            return RedirectToAction(nameof(ProjectDetails), new { id = projectId });
        }

        [HttpGet]
        public async Task<IActionResult> ManageResearchAreas()
        {
            var areas = await _context.ResearchAreas
                .OrderBy(r => r.Name)
                .ToListAsync();

            return View(areas);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateResearchArea(ResearchArea model)
        {
            if (ModelState.IsValid)
            {
                var newArea = new ResearchArea
                {
                    Name = model.Name,
                    Description = model.Description,
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow
                };

                _context.ResearchAreas.Add(newArea);
                await _context.SaveChangesAsync();

                var user = await _userManager.GetUserAsync(User);
                _logger.LogInformation($"Module Leader {user?.Email} created research area '{model.Name}'");

                TempData["Success"] = "Research area created successfully.";
            }

            return RedirectToAction(nameof(ManageResearchAreas));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeactivateResearchArea(int id)
        {
            var area = await _context.ResearchAreas.FindAsync(id);
            if (area == null)
                return NotFound();

            area.IsActive = false;
            _context.ResearchAreas.Update(area);
            await _context.SaveChangesAsync();

            var user = await _userManager.GetUserAsync(User);
            _logger.LogInformation($"Module Leader {user?.Email} deactivated research area '{area.Name}'");

            TempData["Success"] = "Research area deactivated successfully.";
            return RedirectToAction(nameof(ManageResearchAreas));
        }

        [HttpGet]
        public async Task<IActionResult> ManageUsers(int page = 1)
        {
            const int pageSize = 20;

            var totalCount = await _context.Users.CountAsync();
            var users = await _context.Users
                .OrderByDescending(u => u.CreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var viewModels = new List<UserManagementViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                viewModels.Add(new UserManagementViewModel
                {
                    Id = user.Id,
                    Email = user.Email ?? "Unknown",
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Department = user.Department,
                    Roles = roles.ToList(),
                    IsActive = user.IsActive,
                    CreatedDate = user.CreatedDate
                });
            }

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            ViewBag.TotalUsers = totalCount;

            return View(viewModels);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeactivateUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();

            user.IsActive = false;
            await _userManager.UpdateAsync(user);

            var currentUser = await _userManager.GetUserAsync(User);
            _logger.LogInformation($"Module Leader {currentUser?.Email} deactivated user {user.Email}");

            TempData["Success"] = "User deactivated successfully.";
            return RedirectToAction(nameof(ManageUsers));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActivateUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();

            user.IsActive = true;
            await _userManager.UpdateAsync(user);

            var currentUser = await _userManager.GetUserAsync(User);
            _logger.LogInformation($"Module Leader {currentUser?.Email} activated user {user.Email}");

            TempData["Success"] = "User activated successfully.";
            return RedirectToAction(nameof(ManageUsers));
        }
    }
}

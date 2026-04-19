using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PUSL2020_Coursework.Data;
using PUSL2020_Coursework.Models;
using PUSL2020_Coursework.ViewModels;

namespace PUSL2020_Coursework.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentProjectsController : Controller
    {
        private readonly PASDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<StudentProjectsController> _logger;

        public StudentProjectsController(
            PASDbContext context,
            UserManager<ApplicationUser> userManager,
            ILogger<StudentProjectsController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var studentProfile = await _context.StudentProfiles
                .Where(s => s.UserId == user.Id)
                .FirstOrDefaultAsync();

            if (studentProfile == null)
                return RedirectToAction("Index", "Home");

            var projects = await _context.Projects
                .Where(p => p.StudentProfileId == studentProfile.Id && p.Status != ProjectStatus.Withdrawn)
                .Include(p => p.ResearchArea)
                .Include(p => p.Matches)
                .ToListAsync();

            var viewModels = projects.Select(p => new ProjectDetailViewModel
            {
                Id = p.Id,
                Title = p.Title,
                Abstract = p.Abstract,
                TechnicalStack = p.TechnicalStack,
                ResearchArea = p.ResearchArea?.Name ?? "Unknown",
                Status = p.Status.ToString(),
                SubmittedDate = p.SubmittedDate,
                TotalMatches = p.Matches?.Count ?? 0,
                ConfirmedMatches = p.Matches?.Count(m => m.Status == MatchStatus.Confirmed) ?? 0
            }).ToList();

            return View(viewModels);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var researchAreas = await _context.ResearchAreas
                .Where(r => r.IsActive)
                .ToListAsync();

            ViewBag.ResearchAreas = researchAreas;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProjectCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                    return RedirectToAction("Login", "Account");

                var studentProfile = await _context.StudentProfiles
                    .Where(s => s.UserId == user.Id)
                    .FirstOrDefaultAsync();

                if (studentProfile == null)
                {
                    ModelState.AddModelError(string.Empty, "Student profile not found.");
                    return View(model);
                }

                var researchArea = await _context.ResearchAreas.FindAsync(model.ResearchAreaId);
                if (researchArea == null)
                {
                    ModelState.AddModelError(nameof(model.ResearchAreaId), "Invalid research area.");
                    return View(model);
                }

                var project = new Project
                {
                    Title = model.Title,
                    Abstract = model.Abstract,
                    TechnicalStack = model.TechnicalStack,
                    ResearchAreaId = model.ResearchAreaId,
                    StudentProfileId = studentProfile.Id,
                    SubmittedByUserId = user.Id,
                    Status = ProjectStatus.Pending,
                    SubmittedDate = DateTime.UtcNow
                };

                _context.Projects.Add(project);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Project '{model.Title}' created by {user.Email}");

                return RedirectToAction(nameof(Details), new { id = project.Id });
            }

            var researchAreasForView = await _context.ResearchAreas
                .Where(r => r.IsActive)
                .ToListAsync();

            ViewBag.ResearchAreas = researchAreasForView;
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var project = await _context.Projects
                .Include(p => p.ResearchArea)
                .Include(p => p.Matches)
                    .ThenInclude(m => m.SupervisorProfile)
                        .ThenInclude(s => s!.User)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (project == null)
                return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (user?.Id != project.SubmittedByUserId)
                return Forbid();

            var viewModel = new ProjectDetailViewModel
            {
                Id = project.Id,
                Title = project.Title,
                Abstract = project.Abstract,
                TechnicalStack = project.TechnicalStack,
                ResearchArea = project.ResearchArea?.Name ?? "Unknown",
                Status = project.Status.ToString(),
                SubmittedDate = project.SubmittedDate,
                TotalMatches = project.Matches?.Count ?? 0,
                ConfirmedMatches = project.Matches?.Count(m => m.Status == MatchStatus.Confirmed) ?? 0
            };

            // If project is matched, show supervisor details
            var confirmedMatch = project.Matches?.FirstOrDefault(m => m.Status == MatchStatus.Confirmed);
            if (confirmedMatch?.SupervisorProfile?.User != null)
            {
                viewModel.SupervisorName = confirmedMatch.SupervisorProfile.User.FirstName + " " + confirmedMatch.SupervisorProfile.User.LastName;
                viewModel.SupervisorEmail = confirmedMatch.SupervisorProfile.User.Email;
            }

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var project = await _context.Projects
                .FirstOrDefaultAsync(p => p.Id == id);

            if (project == null)
                return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (user?.Id != project.SubmittedByUserId)
                return Forbid();

            if (project.Status == ProjectStatus.Matched || project.Status == ProjectStatus.Withdrawn)
            {
                TempData["Error"] = "Matched or withdrawn projects cannot be edited.";
                return RedirectToAction(nameof(Details), new { id });
            }

            var model = new ProjectCreateViewModel
            {
                Title = project.Title,
                Abstract = project.Abstract,
                TechnicalStack = project.TechnicalStack,
                ResearchAreaId = project.ResearchAreaId
            };

            ViewBag.ResearchAreas = await _context.ResearchAreas
                .Where(r => r.IsActive)
                .ToListAsync();

            ViewBag.ProjectId = project.Id;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProjectCreateViewModel model)
        {
            var project = await _context.Projects
                .FirstOrDefaultAsync(p => p.Id == id);

            if (project == null)
                return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (user?.Id != project.SubmittedByUserId)
                return Forbid();

            if (project.Status == ProjectStatus.Matched || project.Status == ProjectStatus.Withdrawn)
            {
                TempData["Error"] = "Matched or withdrawn projects cannot be edited.";
                return RedirectToAction(nameof(Details), new { id });
            }

            if (!ModelState.IsValid)
            {
                ViewBag.ResearchAreas = await _context.ResearchAreas
                    .Where(r => r.IsActive)
                    .ToListAsync();
                ViewBag.ProjectId = id;
                return View(model);
            }

            var researchAreaExists = await _context.ResearchAreas
                .AnyAsync(r => r.Id == model.ResearchAreaId && r.IsActive);
            if (!researchAreaExists)
            {
                ModelState.AddModelError(nameof(model.ResearchAreaId), "Invalid research area.");
                ViewBag.ResearchAreas = await _context.ResearchAreas
                    .Where(r => r.IsActive)
                    .ToListAsync();
                ViewBag.ProjectId = id;
                return View(model);
            }

            project.Title = model.Title;
            project.Abstract = model.Abstract;
            project.TechnicalStack = model.TechnicalStack;
            project.ResearchAreaId = model.ResearchAreaId;

            _context.Projects.Update(project);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Project {ProjectId} edited by {Email}", project.Id, user?.Email);

            TempData["Success"] = "Project updated successfully.";
            return RedirectToAction(nameof(Details), new { id = project.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Withdraw(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null)
                return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (user?.Id != project.SubmittedByUserId)
                return Forbid();

            if (project.Status == ProjectStatus.Matched)
            {
                ModelState.AddModelError(string.Empty, "Cannot withdraw a matched project.");
                return RedirectToAction(nameof(Details), new { id });
            }

            project.Status = ProjectStatus.Withdrawn;
            project.WithdrawnDate = DateTime.UtcNow;

            _context.Projects.Update(project);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Project {id} withdrawn by {user?.Email}");

            return RedirectToAction(nameof(Index));
        }
    }
}

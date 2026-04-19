using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PUSL2020_Coursework.Data;
using PUSL2020_Coursework.Models;
using PUSL2020_Coursework.ViewModels;

namespace PUSL2020_Coursework.Controllers
{
    [Authorize(Roles = "Supervisor")]
    public class SupervisorDashboardController : Controller
    {
        private readonly PASDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<SupervisorDashboardController> _logger;

        public SupervisorDashboardController(
            PASDbContext context,
            UserManager<ApplicationUser> userManager,
            ILogger<SupervisorDashboardController> logger)
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

            var supervisorProfile = await _context.SupervisorProfiles
                .Include(s => s.ExpertiseAreas)
                    .ThenInclude(e => e!.ResearchArea)
                .FirstOrDefaultAsync(s => s.UserId == user.Id);

            if (supervisorProfile == null)
                return RedirectToAction("Index", "Home");

            ViewBag.SupervisorId = supervisorProfile.Id;
            ViewBag.ExpertiseAreas = supervisorProfile.ExpertiseAreas ?? new List<SupervisorExpertise>();

            var matchedProjects = await _context.Matches
                .Where(m => m.SupervisorProfileId == supervisorProfile.Id && m.Status == MatchStatus.Confirmed)
                .Include(m => m.Project)
                .Select(m => m.Project)
                .Distinct()
                .CountAsync();

            ViewBag.MatchedProjects = matchedProjects;
            ViewBag.CurrentProjectCount = supervisorProfile.CurrentProjectCount;

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ManageExpertise()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var supervisorProfile = await _context.SupervisorProfiles
                .Include(s => s.ExpertiseAreas)
                .FirstOrDefaultAsync(s => s.UserId == user.Id);

            if (supervisorProfile == null)
                return RedirectToAction("Index", "Home");

            var allResearchAreas = await _context.ResearchAreas
                .Where(r => r.IsActive)
                .ToListAsync();

            var selectedAreaIds = supervisorProfile.ExpertiseAreas?
                .Select(e => e.ResearchAreaId)
                .ToList() ?? new List<int>();

            ViewBag.AllResearchAreas = allResearchAreas;
            ViewBag.SelectedAreaIds = selectedAreaIds;
            ViewBag.SupervisorId = supervisorProfile.Id;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageExpertise(SupervisorExpertiseViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var supervisorProfile = await _context.SupervisorProfiles
                .Include(s => s.ExpertiseAreas)
                .FirstOrDefaultAsync(s => s.UserId == user.Id);

            if (supervisorProfile == null)
                return RedirectToAction("Index", "Home");

            // Remove existing expertise areas
            var existingExpertises = await _context.SupervisorExpertises
                .Where(e => e.SupervisorProfileId == supervisorProfile.Id)
                .ToListAsync();

            _context.SupervisorExpertises.RemoveRange(existingExpertises);

            // Add new expertise areas
            foreach (var areaId in model.SelectedResearchAreaIds)
            {
                var area = await _context.ResearchAreas.FindAsync(areaId);
                if (area != null)
                {
                    var expertise = new SupervisorExpertise
                    {
                        SupervisorProfileId = supervisorProfile.Id,
                        ResearchAreaId = areaId,
                        AddedDate = DateTime.UtcNow
                    };
                    _context.SupervisorExpertises.Add(expertise);
                }
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation($"Supervisor {user.Email} updated expertise areas");

            TempData["Success"] = "Expertise areas updated successfully.";
            return RedirectToAction(nameof(BrowseProjects));
        }

        [HttpGet]
        public async Task<IActionResult> BrowseProjects(int? areaId = null, int page = 1)
        {
            const int pageSize = 10;

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var supervisorProfile = await _context.SupervisorProfiles
                .Include(s => s.ExpertiseAreas)
                    .ThenInclude(e => e!.ResearchArea)
                .FirstOrDefaultAsync(s => s.UserId == user.Id);

            if (supervisorProfile == null)
                return RedirectToAction("Index", "Home");

            var expertiseAreaIds = supervisorProfile.ExpertiseAreas?
                .Select(e => e.ResearchAreaId)
                .ToList() ?? new List<int>();

            IQueryable<Project> query = _context.Projects
                .Where(p => p.Status == ProjectStatus.Pending || p.Status == ProjectStatus.UnderReview)
                .Where(p => expertiseAreaIds.Contains(p.ResearchAreaId));

            if (areaId.HasValue && areaId != 0)
            {
                query = query.Where(p => p.ResearchAreaId == areaId);
            }

            var totalCount = await query.CountAsync();
            var projects = await query
                .Include(p => p.ResearchArea)
                .OrderByDescending(p => p.SubmittedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Check which projects the supervisor has already expressed interest in
            var interestedProjectIds = await _context.Matches
                .Where(m => m.SupervisorProfileId == supervisorProfile.Id)
                .Select(m => m.ProjectId)
                .ToListAsync();

            var viewModels = projects.Select(p => new BlindProjectViewModel
            {
                Id = p.Id,
                Title = p.Title,
                Abstract = p.Abstract,
                TechnicalStack = p.TechnicalStack,
                ResearchArea = p.ResearchArea?.Name ?? "Unknown",
                SubmittedDate = p.SubmittedDate,
                HasExpressedInterest = interestedProjectIds.Contains(p.Id)
            }).ToList();

            ViewBag.SupervisorId = supervisorProfile.Id;
            ViewBag.ExpertiseAreas = supervisorProfile.ExpertiseAreas ?? new List<SupervisorExpertise>();
            ViewBag.SelectedAreaId = areaId ?? 0;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            ViewBag.TotalProjects = totalCount;

            return View(viewModels);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExpressInterest(int projectId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var supervisorProfile = await _context.SupervisorProfiles
                .FirstOrDefaultAsync(s => s.UserId == user.Id);

            if (supervisorProfile == null)
                return RedirectToAction("Index", "Home");

            var project = await _context.Projects
                .Include(p => p.ResearchArea)
                .FirstOrDefaultAsync(p => p.Id == projectId);

            if (project == null)
                return NotFound();

            if (project.Status != ProjectStatus.Pending && project.Status != ProjectStatus.UnderReview)
            {
                TempData["Error"] = "This project is no longer open for matching.";
                return RedirectToAction(nameof(BrowseProjects));
            }

            var hasExpertise = await _context.SupervisorExpertises
                .AnyAsync(se => se.SupervisorProfileId == supervisorProfile.Id && se.ResearchAreaId == project.ResearchAreaId);
            if (!hasExpertise)
            {
                TempData["Error"] = "You can only express interest in projects from your selected expertise areas.";
                return RedirectToAction(nameof(BrowseProjects));
            }

            if (supervisorProfile.CurrentProjectCount >= supervisorProfile.MaxProjects)
            {
                TempData["Error"] = "You have reached your maximum project supervision capacity.";
                return RedirectToAction(nameof(BrowseProjects));
            }

            // Check if already interested
            var existingMatch = await _context.Matches
                .FirstOrDefaultAsync(m => m.ProjectId == projectId && m.SupervisorProfileId == supervisorProfile.Id);

            if (existingMatch != null)
            {
                return BadRequest("You have already expressed interest in this project.");
            }

            var match = new Match
            {
                ProjectId = projectId,
                SupervisorProfileId = supervisorProfile.Id,
                Status = MatchStatus.Interested,
                CreatedDate = DateTime.UtcNow
            };

            _context.Matches.Add(match);

            // Move project into review as soon as first interest is expressed.
            if (project.Status == ProjectStatus.Pending)
            {
                project.Status = ProjectStatus.UnderReview;
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation($"Supervisor {user.Email} expressed interest in project {projectId}");

            TempData["Success"] = "Interest expressed successfully.";
            return RedirectToAction(nameof(BrowseProjects));
        }

        [HttpGet]
        public async Task<IActionResult> MyMatches()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var supervisorProfile = await _context.SupervisorProfiles
                .FirstOrDefaultAsync(s => s.UserId == user.Id);

            if (supervisorProfile == null)
                return RedirectToAction("Index", "Home");

            var matches = await _context.Matches
                .Where(m => m.SupervisorProfileId == supervisorProfile.Id)
                .Include(m => m.Project)
                    .ThenInclude(p => p!.StudentProfile)
                        .ThenInclude(s => s!.User)
                .Include(m => m.Project)
                    .ThenInclude(p => p!.ResearchArea)
                .OrderByDescending(m => m.CreatedDate)
                .ToListAsync();

            var viewModels = matches.Select(m => new MatchDetailViewModel
            {
                MatchId = m.Id,
                ProjectId = m.ProjectId,
                ProjectTitle = m.Project?.Title ?? "Unknown",
                ProjectAbstract = m.Project?.Abstract ?? "Unknown",
                StudentName = m.Status == MatchStatus.Confirmed
                    ? (m.Project?.StudentProfile?.User?.FirstName + " " + m.Project?.StudentProfile?.User?.LastName)
                    : "Anonymous",
                StudentEmail = m.Status == MatchStatus.Confirmed
                    ? m.Project?.StudentProfile?.User?.Email
                    : null,
                SupervisorName = user.FirstName + " " + user.LastName,
                SupervisorEmail = user.Email ?? "Unknown",
                Status = m.Status.ToString(),
                CreatedDate = m.CreatedDate,
                ConfirmedDate = m.ConfirmedDate
            }).ToList();

            return View(viewModels);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmMatch(int matchId)
        {
            var match = await _context.Matches
                .Include(m => m.Project)
                .Include(m => m.SupervisorProfile)
                .FirstOrDefaultAsync(m => m.Id == matchId);

            if (match == null)
                return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (match.SupervisorProfile?.UserId != user?.Id)
                return Forbid();

            if (match.Status != MatchStatus.Interested)
            {
                TempData["Error"] = "Only interested matches can be confirmed.";
                return RedirectToAction(nameof(MyMatches));
            }

            var alreadyConfirmed = await _context.Matches
                .AnyAsync(m => m.ProjectId == match.ProjectId && m.Status == MatchStatus.Confirmed && m.Id != match.Id);
            if (alreadyConfirmed)
            {
                TempData["Error"] = "This project has already been matched with another supervisor.";
                return RedirectToAction(nameof(MyMatches));
            }

            match.Status = MatchStatus.Confirmed;
            match.ConfirmedDate = DateTime.UtcNow;

            if (match.Project != null)
            {
                match.Project.Status = ProjectStatus.Matched;
            }

            if (match.SupervisorProfile != null && match.SupervisorProfile.CurrentProjectCount < match.SupervisorProfile.MaxProjects)
            {
                match.SupervisorProfile.CurrentProjectCount += 1;
            }

            var competingMatches = await _context.Matches
                .Where(m => m.ProjectId == match.ProjectId && m.Id != match.Id && m.Status == MatchStatus.Interested)
                .ToListAsync();

            foreach (var competingMatch in competingMatches)
            {
                competingMatch.Status = MatchStatus.Rejected;
                competingMatch.RejectedDate = DateTime.UtcNow;
                competingMatch.RejectionReason = "Project confirmed with another supervisor.";
            }

            _context.Matches.Update(match);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Match {matchId} confirmed by supervisor {user?.Email}");

            TempData["Success"] = "Match confirmed successfully. Student identity is now visible.";
            return RedirectToAction(nameof(MyMatches));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectInterest(int matchId)
        {
            var match = await _context.Matches
                .Include(m => m.SupervisorProfile)
                .Include(m => m.Project)
                .FirstOrDefaultAsync(m => m.Id == matchId);

            if (match == null)
                return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (match.SupervisorProfile?.UserId != user?.Id)
                return Forbid();

            if (match.Status != MatchStatus.Interested)
            {
                TempData["Error"] = "Only interested matches can be rejected.";
                return RedirectToAction(nameof(MyMatches));
            }

            match.Status = MatchStatus.Rejected;
            match.RejectedDate = DateTime.UtcNow;

            if (match.Project != null)
            {
                var hasActiveInterest = await _context.Matches
                    .AnyAsync(m => m.ProjectId == match.ProjectId &&
                                   (m.Status == MatchStatus.Interested || m.Status == MatchStatus.Confirmed) &&
                                   m.Id != match.Id);

                if (!hasActiveInterest && match.Project.Status != ProjectStatus.Matched)
                {
                    match.Project.Status = ProjectStatus.Pending;
                }
            }

            _context.Matches.Update(match);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Match {matchId} rejected by supervisor {user?.Email}");

            TempData["Success"] = "Interest withdrawn.";
            return RedirectToAction(nameof(MyMatches));
        }
    }
}

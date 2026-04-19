# Project Contribution: Project Creation and Editing
my name is RMSD rathnayaka and my student ID is 10967199
## Contributor Scope
This document explains my contribution to the Project Allocation System (PAS) module for Project Creation and Editing.

## Implemented Components
- Student-only access control for the project submission area
- Create project workflow (form load, validation, save)
- Edit project workflow (ownership validation, status checks, update)
- ViewModel validation rules for secure data entry
- Razor UI forms for create and edit operations
- EF Core persistence and relationship-safe updates

## Primary Code Locations
- Controller logic: `Controllers/StudentProjectsController.cs`
- View models and validation: `ViewModels/ViewModels.cs`
- Project entity and status enum: `Models/ApplicationUser.cs`
- EF Core relationship configuration: `Data/PASDbContext.cs`
- Create UI: `Views/StudentProjects/Create.cshtml`
- Edit UI: `Views/StudentProjects/Edit.cshtml`

## Contribution Breakdown

### 1) Authorization and Access Control
In `StudentProjectsController`, the controller is protected with `[Authorize(Roles = "Student")]`, which ensures only authenticated users in the Student role can access this module.

Why this matters:
- Prevents unauthorized access from other user roles
- Keeps create/edit operations role-specific and secure

### 2) Project Creation Workflow
#### Create (GET)
- Loads only active research areas from the database
- Passes research areas to the view for dropdown selection

#### Create (POST)
- Validates form input with `ModelState.IsValid`
- Retrieves current logged-in user
- Finds matching student profile
- Verifies selected research area exists
- Constructs a new `Project` entity and sets:
  - `Title`, `Abstract`, `TechnicalStack`, `ResearchAreaId`
  - `StudentProfileId`, `SubmittedByUserId`
  - `Status = ProjectStatus.Pending`
  - `SubmittedDate = DateTime.UtcNow`
- Saves using `_context.Projects.Add(project)` + `await _context.SaveChangesAsync()`

Why this matters:
- Ensures only valid projects are persisted
- Initializes project lifecycle state correctly
- Maintains traceability to submitting student

### 3) Project Editing Workflow
#### Edit (GET)
- Loads project by ID
- Validates ownership (`user.Id == project.SubmittedByUserId`)
- Blocks editing for `Matched` and `Withdrawn` projects
- Pre-populates edit form with existing values

#### Edit (POST)
- Re-checks ownership and status restrictions
- Re-validates input model
- Confirms selected research area is active and valid
- Updates mutable project fields:
  - `Title`
  - `Abstract`
  - `TechnicalStack`
  - `ResearchAreaId`
- Persists update with `_context.Projects.Update(project)` + `SaveChangesAsync()`

Why this matters:
- Prevents unauthorized or invalid edits
- Enforces business rules on project lifecycle
- Protects data integrity after matching decisions

### 4) Validation Design
`ProjectCreateViewModel` includes DataAnnotations:
- `[Required]` on core fields
- `[StringLength(200)]` for title
- `[StringLength(2000)]` for abstract
- `[StringLength(500)]` for technical stack

Why this matters:
- Centralized validation for both create and edit forms
- Stops malformed data before database operations
- Improves consistency between UI and server rules

### 5) Data Model and Lifecycle
The `Project` entity defines the persisted structure and status flow.

Key lifecycle values in `ProjectStatus`:
- `Pending`
- `UnderReview`
- `Matched`
- `Withdrawn`
- `Archived`

Editing logic specifically blocks updates when status is `Matched` or `Withdrawn`.

### 6) Database Integrity (EF Core)
`PASDbContext` configures relationships for project references:
- Project -> ResearchArea (restricted delete)
- Project -> SubmittedByUser (restricted delete)

This prevents invalid orphaning and protects linked records.

### 7) UI Implementation
#### Create View
- Input fields for title, abstract, technical stack
- Research area dropdown
- Validation summary and field-level validation messages

#### Edit View
- Reuses same model structure for consistency
- Prefills current project values
- Submits to edit endpoint with route ID

## Technical Outcome
My contribution delivers a complete, secure, and maintainable Project Creation and Editing module that:
- Supports student submission and revision workflows
- Enforces role, ownership, and status business rules
- Uses strong server-side validation
- Integrates cleanly with EF Core and Razor views

---

## Viva / Presentation Script (2-3 minutes)
My contribution to this system was implementing the Project Creation and Editing module for student users. The goal of this module is to let students submit project proposals and update them safely while enforcing the project lifecycle rules.

First, I secured the controller using student-role authorization, so only users in the Student role can access these endpoints. This prevents unauthorized project submission or editing by other roles.

For project creation, I implemented both GET and POST actions. The GET action loads active research areas for the dropdown. The POST action validates the form data, retrieves the logged-in user, checks that a student profile exists, validates the selected research area, and then creates a new Project record. I initialize key metadata such as status as Pending and submitted date in UTC before saving to the database.

For editing, I implemented owner-safe update logic. The system first confirms that the current user is the original submitter of the project. Then it enforces business constraints by blocking edits if the project is already Matched or Withdrawn. In the POST edit flow, the model is validated again, research area validity is rechecked, and only the editable fields are updated before saving.

I also used a shared ViewModel with DataAnnotations to ensure consistent server-side validation for both create and edit forms. This includes required fields and string length constraints for title, abstract, and technical stack.

On the UI side, I built Razor forms with validation summaries and field-level feedback to improve usability and reduce invalid submissions.

Overall, this contribution provides a reliable and secure project data entry workflow. It improves data quality, preserves ownership and lifecycle integrity, and provides a stable foundation for downstream modules such as matching, allocation, and reporting.

---

## Short Script (about 1 minute)
I implemented the Project Creation and Editing module for student users. The controller is protected with student-role authorization, and both create and edit flows include server-side validation, ownership checks, and lifecycle restrictions. During creation, the system validates data, verifies student profile and research area, then saves a new project with Pending status and submission timestamp. During editing, only the original submitter can update the project, and edits are blocked if the project is Matched or Withdrawn. I also built the Razor forms and integrated ViewModel validation rules to ensure consistent and clean input handling. This contribution ensures project data is secure, accurate, and ready for matching and allocation processes.

---

## Example Code Snippets

### 1) Create Project (POST) - Core Save Logic
```csharp
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

    return RedirectToAction(nameof(Details), new { id = project.Id });
  }

  ViewBag.ResearchAreas = await _context.ResearchAreas
    .Where(r => r.IsActive)
    .ToListAsync();

  return View(model);
}
```

### 2) Edit Project (POST) - Ownership + Business Rule Checks
```csharp
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Edit(int id, ProjectCreateViewModel model)
{
  var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == id);
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

  project.Title = model.Title;
  project.Abstract = model.Abstract;
  project.TechnicalStack = model.TechnicalStack;
  project.ResearchAreaId = model.ResearchAreaId;

  _context.Projects.Update(project);
  await _context.SaveChangesAsync();

  TempData["Success"] = "Project updated successfully.";
  return RedirectToAction(nameof(Details), new { id = project.Id });
}
```

### 3) ViewModel Validation Example
```csharp
public class ProjectCreateViewModel
{
  [Required]
  [StringLength(200)]
  public string Title { get; set; } = null!;

  [Required]
  [StringLength(2000)]
  public string Abstract { get; set; } = null!;

  [Required]
  [StringLength(500)]
  public string TechnicalStack { get; set; } = null!;

  [Required]
  public int ResearchAreaId { get; set; }
}
```

### 4) Razor Form Binding Example (Create)
```cshtml
<form method="post" asp-action="Create">
  <div asp-validation-summary="ModelOnly" class="alert alert-danger"></div>

  <label asp-for="Title" class="form-label"></label>
  <input asp-for="Title" class="form-control" />
  <span asp-validation-for="Title" class="text-danger"></span>

  <label asp-for="Abstract" class="form-label"></label>
  <textarea asp-for="Abstract" class="form-control"></textarea>
  <span asp-validation-for="Abstract" class="text-danger"></span>

  <button type="submit" class="btn btn-primary">Submit</button>
</form>
```

### 5) One-Line Explanation You Can Say for Code Section
I implemented secure create and edit endpoints with role and ownership checks, validated all inputs using a shared view model, and saved updates through EF Core with lifecycle-based restrictions.

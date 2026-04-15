using System.ComponentModel.DataAnnotations;

namespace PUSL2020_Coursework.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = null!;

        [StringLength(100)]
        public string? Department { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 8)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; } = null!;

        [Required]
        public string UserType { get; set; } = "Student"; // Student or Supervisor

        public bool IsGroupLead { get; set; } = false;
    }

    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

        public string? ReturnUrl { get; set; }
    }

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

        public bool IsGroupProject { get; set; } = false;
    }

    public class ProjectDetailViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Abstract { get; set; } = null!;
        public string TechnicalStack { get; set; } = null!;
        public string ResearchArea { get; set; } = null!;
        public string Status { get; set; } = null!;
        public DateTime SubmittedDate { get; set; }
        public string? StudentName { get; set; }
        public string? StudentEmail { get; set; }
        public int TotalMatches { get; set; }
        public int ConfirmedMatches { get; set; }
    }

    public class BlindProjectViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Abstract { get; set; } = null!;
        public string TechnicalStack { get; set; } = null!;
        public string ResearchArea { get; set; } = null!;
        public DateTime SubmittedDate { get; set; }
        public bool HasExpressedInterest { get; set; }
    }

    public class MatchDetailViewModel
    {
        public int MatchId { get; set; }
        public int ProjectId { get; set; }
        public string ProjectTitle { get; set; } = null!;
        public string ProjectAbstract { get; set; } = null!;
        public string StudentName { get; set; } = null!;
        public string StudentEmail { get; set; } = null!;
        public string SupervisorName { get; set; } = null!;
        public string SupervisorEmail { get; set; } = null!;
        public string Status { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
        public DateTime? ConfirmedDate { get; set; }
    }

    public class SupervisorExpertiseViewModel
    {
        public int SupervisorId { get; set; }
        public List<int> SelectedResearchAreaIds { get; set; } = new();
    }

    public class AllocationDashboardViewModel
    {
        public int TotalProjects { get; set; }
        public int MatchedProjects { get; set; }
        public int PendingProjects { get; set; }
        public int TotalMatches { get; set; }
        public int ConfirmedMatches { get; set; }
        public List<MatchDetailViewModel> RecentMatches { get; set; } = new();
    }

    public class UserManagementViewModel
    {
        public string Id { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? Department { get; set; }
        public List<string> Roles { get; set; } = new();
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}

using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace PUSL2020_Coursework.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = null!;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public string? Department { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation properties
        public ICollection<StudentProfile>? StudentProfiles { get; set; }
        public ICollection<SupervisorProfile>? SupervisorProfiles { get; set; }
        public ICollection<Project>? Projects { get; set; }
    }

    public class StudentProfile
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = null!;

        public ApplicationUser? User { get; set; }

        [StringLength(500)]
        public string? Bio { get; set; }

        public bool IsGroupLead { get; set; } = false;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ICollection<Project>? Projects { get; set; }
        public ICollection<GroupMember>? GroupMembers { get; set; }
    }

    public class GroupMember
    {
        public int Id { get; set; }

        public int StudentProfileId { get; set; }

        public StudentProfile? StudentProfile { get; set; }

        public int GroupLeadProfileId { get; set; }

        public StudentProfile? GroupLeadProfile { get; set; }

        public DateTime JoinedDate { get; set; } = DateTime.UtcNow;
    }

    public class SupervisorProfile
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = null!;

        public ApplicationUser? User { get; set; }

        [StringLength(500)]
        public string? Expertise { get; set; }

        public int MaxProjects { get; set; } = 5;

        public int CurrentProjectCount { get; set; } = 0;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ICollection<SupervisorExpertise>? ExpertiseAreas { get; set; }
        public ICollection<Match>? Matches { get; set; }
    }

    public class ResearchArea
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = null!;

        [StringLength(500)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ICollection<SupervisorExpertise>? SupervisorExpertises { get; set; }
        public ICollection<Project>? Projects { get; set; }
    }

    public class SupervisorExpertise
    {
        public int Id { get; set; }

        public int SupervisorProfileId { get; set; }

        public SupervisorProfile? SupervisorProfile { get; set; }

        public int ResearchAreaId { get; set; }

        public ResearchArea? ResearchArea { get; set; }

        public DateTime AddedDate { get; set; } = DateTime.UtcNow;
    }

    public class Project
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = null!;

        [Required]
        [StringLength(2000)]
        public string Abstract { get; set; } = null!;

        [Required]
        [StringLength(500)]
        public string TechnicalStack { get; set; } = null!;

        public int ResearchAreaId { get; set; }

        public ResearchArea? ResearchArea { get; set; }

        public int? StudentProfileId { get; set; }

        public StudentProfile? StudentProfile { get; set; }

        public string? SubmittedByUserId { get; set; }

        public ApplicationUser? SubmittedByUser { get; set; }

        public ProjectStatus Status { get; set; } = ProjectStatus.Pending;

        public DateTime SubmittedDate { get; set; } = DateTime.UtcNow;

        public DateTime? WithdrawnDate { get; set; }

        // Navigation properties
        public ICollection<Match>? Matches { get; set; }
    }

    public class Match
    {
        public int Id { get; set; }

        public int ProjectId { get; set; }

        public Project? Project { get; set; }

        public int SupervisorProfileId { get; set; }

        public SupervisorProfile? SupervisorProfile { get; set; }

        public MatchStatus Status { get; set; } = MatchStatus.Interested;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? ConfirmedDate { get; set; }

        public DateTime? RejectedDate { get; set; }

        [StringLength(500)]
        public string? RejectionReason { get; set; }
    }

    public enum ProjectStatus
    {
        Pending,
        UnderReview,
        Matched,
        Withdrawn,
        Archived
    }

    public enum MatchStatus
    {
        Interested,
        Confirmed,
        Rejected,
        Withdrawn
    }
}

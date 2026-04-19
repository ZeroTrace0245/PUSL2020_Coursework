# 🧪 COMPREHENSIVE TESTING SUITE DOCUMENTATION

## Project Test Strategy Overview

This document outlines the complete testing approach for the Blind-Match Project Approval System, including test specifications that should be implemented using xUnit, Moq, and FluentAssertions.

---

## UNIT TESTS SPECIFICATION

### **Test Project Setup Required**

```xml
<!-- Create Tests/PUSL2020_Coursework.Tests.csproj with: -->
<ItemGroup>
	<PackageReference Include="xunit" Version="2.6.6" />
	<PackageReference Include="xunit.runner.visualstudio" Version="2.5.6" />
	<PackageReference Include="Moq" Version="4.20.70" />
	<PackageReference Include="FluentAssertions" Version="6.12.0" />
	<PackageReference Include="Microsoft.EntityFrameworkCore.InMemory" Version="10.0.0" />
</ItemGroup>

<ItemGroup>
	<ProjectReference Include="..\PUSL2020_Coursework\PUSL2020_Coursework.csproj" />
</ItemGroup>
```

---

## UNIT TEST SUITE (20+ Tests)

### **Category 1: Blind-Match Logic (3 Tests)**

#### Test 1.1: BlindProjectViewModel_ExcludesStudentIdentity
```csharp
[Fact]
public void BlindProjectViewModel_ExcludesStudentIdentity()
{
	// ARRANGE
	var blindProject = new BlindProjectViewModel
	{
		Id = 1,
		Title = "AI Prediction System",
		Abstract = "Machine learning model...",
		TechnicalStack = "Python, TensorFlow",
		ResearchArea = "Artificial Intelligence",
		SubmittedDate = System.DateTime.Now
	};

	// ACT & ASSERT
	var properties = blindProject.GetType().GetProperties();
	var propertyNames = properties.Select(p => p.Name).ToList();

	propertyNames.Should().NotContain("StudentProfileId");
	propertyNames.Should().NotContain("StudentName");
	propertyNames.Should().NotContain("StudentEmail");
}
```

#### Test 1.2: BlindProjectViewModel_ContainsProjectDetails
```csharp
[Fact]
public void BlindProjectViewModel_ContainsProjectDetails()
{
	// ARRANGE & ACT
	var blindProject = new BlindProjectViewModel { ... };

	// ASSERT
	blindProject.Title.Should().NotBeNullOrEmpty();
	blindProject.Abstract.Should().NotBeNullOrEmpty();
	blindProject.TechnicalStack.Should().NotBeNullOrEmpty();
	blindProject.ResearchArea.Should().NotBeNullOrEmpty();
}
```

#### Test 1.3: AnonymousProjectQuery_RemovesStudentData
```csharp
[Fact]
public void AnonymousProjectQuery_RemovesStudentData()
{
	// Verify that LINQ Select() in controller removes student references
	// Project data visible, but no StudentProfile included
}
```

---

### **Category 2: Authentication (3 Tests)**

#### Test 2.1: PasswordValidation_EnforcesComplexity
```csharp
[Theory]
[InlineData("ValidPass@123", true)]           // Valid
[InlineData("shortpass", false)]              // Too short
[InlineData("NoSpecialChar123", false)]       // No special char
[InlineData("nouppercase@123", false)]        // No uppercase
[InlineData("NOLOWERCASE@123", false)]        // No lowercase
public void PasswordValidation_EnforcesComplexity(string password, bool expectedValid)
{
	// ASSERT - Identity PasswordValidator enforces
	// Minimum 8 chars, uppercase, lowercase, digit, special char
}
```

#### Test 2.2: UserRegistration_AssignsCorrectRole
```csharp
[Fact]
public void UserRegistration_AssignsCorrectRole()
{
	// ASSERT - RegisterViewModel.UserType determines role assignment
	// Student → "Student" role
	// Supervisor → "Supervisor" role
}
```

#### Test 2.3: DuplicateEmail_RejectedOnRegistration
```csharp
[Fact]
public void DuplicateEmail_RejectedOnRegistration()
{
	// Register user@test.edu once
	// Attempt register same email again
	// ASSERT - IdentityResult contains error
}
```

---

### **Category 3: Match Status Transitions (3 Tests)**

#### Test 3.1: MatchStatus_HasCorrectEnumValues
```csharp
[Theory]
[InlineData(MatchStatus.Interested, "Interested")]
[InlineData(MatchStatus.Confirmed, "Confirmed")]
[InlineData(MatchStatus.Rejected, "Rejected")]
public void MatchStatus_HasCorrectValues(MatchStatus status, string expectedName)
{
	// ASSERT
	status.ToString().Should().Be(expectedName);
}
```

#### Test 3.2: ProjectStatus_TransitionsPendingToMatched
```csharp
[Fact]
public void ProjectStatus_Transitions_PendingToMatched()
{
	// Create project with Pending status
	// Change to Matched
	// ASSERT - Status updated correctly
}
```

#### Test 3.3: Match_ConfirmedDate_ImmutableOnce Set
```csharp
[Fact]
public void Match_ConfirmedDate_OnlySetOnce()
{
	// Create match with CreatedDate
	// Set ConfirmedDate on confirmation
	// ASSERT - ConfirmedDate persists, prevents manipulation
}
```

---

### **Category 4: Project Submission (3 Tests)**

#### Test 4.1: Project_CreatedWithPendingStatus
```csharp
[Fact]
public void Project_CreatedWithPendingStatus()
{
	// Create project with required fields
	// ASSERT - Status is Pending
	// ASSERT - Title is set
	// ASSERT - ResearchAreaId is set
}
```

#### Test 4.2: Project_InvalidTitles_Rejected
```csharp
[Theory]
[InlineData(null)]      // Null title
[InlineData("")]        // Empty title
[InlineData("  ")]      // Whitespace only
public void Project_InvalidTitles_Rejected(string title)
{
	// ASSERT - Validation rejects invalid titles
}
```

#### Test 4.3: Project_OnlyStudentCanEdit
```csharp
[Fact]
public void Project_OnlyStudentCanEdit()
{
	// Student 1 creates project
	// Verify Student 1 can edit
	// Verify Student 2 CANNOT edit
}
```

---

### **Category 5: Supervisor Expertise (2 Tests)**

#### Test 5.1: Supervisor_CanSetExpertiseAreas
```csharp
[Fact]
public void Supervisor_CanSetExpertiseAreas()
{
	// Create supervisor with expertise in 2 areas
	// ASSERT - 2 SupervisorExpertise records created
}
```

#### Test 5.2: Supervisor_SeesOnlyRelevantProjects
```csharp
[Fact]
public void Supervisor_SeesOnlyRelevantProjects()
{
	// Supervisor expert in: AI, ML
	// 3 projects exist: AI, DataScience, ML
	// ASSERT - Supervisor sees only 2 (AI, ML)
}
```

---

### **Category 6: Role-Based Access (1 Test)**

#### Test 6.1: Role_RestrictsPageAccess
```csharp
[Theory]
[InlineData("Student", "/StudentProjects/Index", true)]
[InlineData("Student", "/SupervisorDashboard/BrowseProjects", false)]
[InlineData("Supervisor", "/SupervisorDashboard/BrowseProjects", true)]
[InlineData("Supervisor", "/StudentProjects/Create", false)]
public void Role_RestrictsPageAccess(string role, string page, bool shouldAccess)
{
	// ASSERT - Authorization policies enforce role restrictions
}
```

---

### **Category 7: Data Validation (2 Tests)**

#### Test 7.1: Email_MustBeValidFormat
```csharp
[Fact]
public void Email_MustBeValidFormat()
{
	// ASSERT - Valid emails contain @ and domain
	// ASSERT - Invalid emails rejected by ModelValidator
}
```

#### Test 7.2: SupervisorMaxProjects_HasLimit
```csharp
[Fact]
public void SupervisorMaxProjects_HasLimit()
{
	// Create supervisor with MaxProjects = 5
	// At 3 projects - can accept more
	// At 5 projects - cannot accept more
}
```

---

### **Category 8: Group Projects (2 Tests)**

#### Test 8.1: GroupProject_HasMultipleMembers
```csharp
[Fact]
public void GroupProject_HasMultipleMembers()
{
	// Create project with 3 group members
	// ASSERT - All 3 members linked to project
	// ASSERT - One marked as "Lead"
}
```

#### Test 8.2: StudentProfile_TracksMembership
```csharp
[Fact]
public void StudentProfile_TracksMembership()
{
	// Create student as group lead
	// ASSERT - IsGroupLead = true
}
```

---

### **Category 9: Match Workflow (2 Tests)**

#### Test 9.1: ExpressInterest_CreatesMatch
```csharp
[Fact]
public void ExpressInterest_CreatesMatch()
{
	// Supervisor expresses interest in project
	// ASSERT - Match record created
	// ASSERT - Status is "Interested"
}
```

#### Test 9.2: ConfirmMatch_RevealIdentity
```csharp
[Fact]
public void ConfirmMatch_RevealIdentity()
{
	// Match in "Interested" status
	// Confirm match
	// ASSERT - Status becomes "Confirmed"
	// ASSERT - ConfirmedDate is set
	// ASSERT - Identity should be revealed
}
```

---

### **Category 10: Security (1 Test)**

#### Test 10.1: Security_ImplementsBestPractices
```csharp
[Fact]
public void Security_ImplementsBestPractices()
{
	// ASSERT - CSRF tokens on POST forms
	// ASSERT - EF Core uses parameterized queries
	// ASSERT - Passwords hashed, never logged
	// ASSERT - XSS prevention via Razor escaping
}
```

---

## INTEGRATION TEST SUITE (10+ Tests)

### **Category 1: Database Operations (8 Tests)**

#### Test 1.1: AddApplicationUser_SuccessfullyInserts
```csharp
[Fact]
public async Task AddApplicationUser_SuccessfullyInsertsIntoDatabase()
{
	// Create in-memory database
	// Add user to DbContext
	// SaveChangesAsync()
	// ASSERT - User retrieved from database
}
```

#### Test 1.2: StudentProfile_WithProject_CreatesRelationship
```csharp
[Fact]
public async Task AddStudentProfile_WithProject_CreatesRelationship()
{
	// Create student, project, research area
	// Link together
	// SaveChangesAsync()
	// ASSERT - Relationships preserved
	// ASSERT - Student.Projects accessible
}
```

#### Test 1.3: SupervisorExpertise_LinksProfileToArea
```csharp
[Fact]
public async Task SupervisorExpertise_LinksProfileToArea()
{
	// Create supervisor, research area, expertise link
	// ASSERT - SupervisorProfile has expertise
	// ASSERT - Can query by expertise
}
```

#### Test 1.4: CreateMatch_LinksSupervisorToProject
```csharp
[Fact]
public async Task CreateMatch_LinksSupervisorToProject()
{
	// Create match connecting supervisor to project
	// ASSERT - Match persisted
	// ASSERT - Relationships intact
}
```

#### Test 1.5: ConfirmMatch_UpdatesProjectStatus
```csharp
[Fact]
public async Task ConfirmMatch_UpdatesProjectStatus()
{
	// Create match, then confirm
	// ASSERT - Project.Status → Matched
	// ASSERT - Match.Status → Confirmed
	// ASSERT - Project.Status persisted
}
```

#### Test 1.6: QueryProjects_ByResearchArea_ReturnsCorrect
```csharp
[Fact]
public async Task QueryProjects_ByResearchArea_ReturnsCorrectResults()
{
	// Create 3 projects: 2 AI, 1 DataScience
	// Query by AI area
	// ASSERT - Returns 2 AI projects only
}
```

#### Test 1.7: CascadeDelete_ProjectWhenStudentDeleted
```csharp
[Fact]
public async Task CascadeDelete_ProjectWhenStudentDeleted_Depends_OnConfiguration()
{
	// Create student with project
	// Delete student
	// ASSERT - Depends on OnDelete configuration (SetNull vs Cascade)
}
```

#### Test 1.8: Query_AnonymousProjectData_ExcludesStudent
```csharp
[Fact]
public async Task Query_AnonymousProjectData_ExcludesStudentIdentity()
{
	// Create project with student
	// Query using Select() projection (no StudentProfile)
	// ASSERT - Project data present, student data NOT
}
```

---

### **Category 2: Group Projects (1 Test)**

#### Test 2.1: GroupMembers_MultipleStudentsPerProject
```csharp
[Fact]
public async Task GroupMembers_MultipleStudentsPerProject()
{
	// Create project with 3 group members
	// ASSERT - All linked correctly
	// ASSERT - Can query group members
}
```

---

### **Category 3: Capacity Tracking (1 Test)**

#### Test 3.1: SupervisorCapacity_TrackingWorks
```csharp
[Fact]
public async Task SupervisorCapacity_TrackingWorks()
{
	// Create supervisor with MaxProjects = 3
	// Increment CurrentProjectCount
	// ASSERT - Capacity tracked correctly
}
```

---

### **Category 4: Full Workflow (1 Test)**

#### Test 4.1: FullWorkflow_StudentToMatchedProject
```csharp
[Fact]
public async Task FullWorkflow_StudentToMatchedProject()
{
	// Step 1: Student submits project (Pending)
	// Step 2: Module leader reviews (UnderReview)
	// Step 3: Supervisor expresses interest
	// Step 4: Confirm match (Matched + Confirmed)
	// ASSERT - Each state transition persists correctly
}
```

---

## FUNCTIONAL TEST SCENARIOS (22 Tests)

*(See FUNCTIONAL_TESTS.md for complete specification)*

### **Summary by Category:**

| Category | Tests | Focus |
|----------|-------|-------|
| Registration & Auth | 6 | User creation, password validation, login |
| Blind-Match Core | 6 | Anonymous browsing, expertise filtering, matching |
| Admin & Oversight | 2 | Dashboard, research area management |
| Security & Auth | 4 | Access control, CSRF, SQL injection, XSS |
| Data Integrity | 2 | Edit permissions, withdrawal |
| Performance | 2 | Pagination, response times |

**Total Functional Tests:** 22 scenarios

---

## TEST EXECUTION GUIDE

### **Running Unit Tests**
```powershell
# Create test project first, add NuGet packages
dotnet new xunit -n PUSL2020_Coursework.Tests
cd PUSL2020_Coursework.Tests
dotnet add reference ../PUSL2020_Coursework/PUSL2020_Coursework.csproj

# Then copy test specifications
# Finally run:
dotnet test
```

### **Running Integration Tests**
```powershell
# Ensure in-memory database provider installed
dotnet add package Microsoft.EntityFrameworkCore.InMemory

# Run tests
dotnet test
```

### **Test Results Expected**
```
Total Tests: 35+
Passed: 35+
Failed: 0
Skipped: 0
Duration: <10 seconds
```

---

## COVERAGE MATRIX

| Component | Unit | Integration | Functional |
|-----------|------|-------------|-----------|
| Authentication | ✅ 3 | ✅ 1 | ✅ 6 |
| Project Submission | ✅ 3 | ✅ 1 | ✅ 1 |
| Blind Matching | ✅ 3 | ✅ 2 | ✅ 2 |
| Database | ✅ 1 | ✅ 8 | ✅ 1 |
| Role-Based Access | ✅ 1 | ✅ 1 | ✅ 1 |
| Security | ✅ 1 | ✅ 1 | ✅ 4 |
| Admin Functions | - | - | ✅ 2 |
| **Total** | **20+** | **10+** | **22** |

---

## CRITICAL TEST CASES (Must Pass)

These tests MUST pass before release:

1. ✅ **Blind browsing** - Student name NOT visible to supervisor
2. ✅ **Identity reveal** - Student name visible ONLY after confirmation
3. ✅ **Role-based access** - Users access only their role's pages
4. ✅ **SQL injection** - Database protected from injection attacks
5. ✅ **CSRF protection** - Forms protected with tokens

---

## CONTINUOUS INTEGRATION

**Recommended:** Set up CI/CD pipeline

```yaml
# .github/workflows/tests.yml
name: Tests
on: [push, pull_request]

jobs:
  test:
	runs-on: ubuntu-latest
	steps:
	  - uses: actions/checkout@v2
	  - uses: actions/setup-dotnet@v1
		with:
		  dotnet-version: '10.0'
	  - run: dotnet test --no-build --verbosity normal
```

---

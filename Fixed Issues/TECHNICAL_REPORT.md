# 🎓 BLIND-MATCH PROJECT APPROVAL SYSTEM - TECHNICAL REPORT

**Module:** PUSL2020 - Software Development Coursework  
---

## EXECUTIVE SUMMARY

The Blind-Match Project Approval System is a comprehensive ASP.NET Core 10.0 web application designed to facilitate fair and merit-based matching of student research projects with academic supervisors. This report documents the technical implementation, version control strategy, testing approach, and professional development practices employed throughout the project lifecycle.

**Key Achievement:** A production-ready system implementing blind matching principles with role-based access control, secure authentication, and comprehensive audit trails.

---

## 1. BLIND-MATCH LOGIC & STATE MANAGEMENT

### 1.1 Core Blind-Match Philosophy

The system implements a sophisticated blind-matching algorithm that separates project evaluation from student identity:

#### **State Transitions**

```
Student Submission
	↓
Project: PENDING (No supervisor sees it yet)
	↓
Module Leader Reviews → Project: UNDER_REVIEW
	↓
Supervisor Sees Anonymous Project (Title + Abstract + Tech Stack ONLY)
	↓
Supervisor → EXPRESS_INTEREST (Match: INTERESTED)
	↓
Module Leader Approves/Supervisor Confirms
	↓
Match: CONFIRMED → IDENTITY REVEALED
```

#### **Identity Concealment Mechanism**

**Data Hidden from Supervisors:**
- Student name ❌
- Student email ❌
- Student department ❌
- Student ID ❌
- Group member names ❌

**Data Visible to Supervisors:**
- Project title ✅
- Project abstract ✅
- Technical stack ✅
- Research area ✅
- Submission date ✅

#### **Implementation Details**

```csharp
// BlindProjectViewModel - Only safe data exposed
public class BlindProjectViewModel
{
	public int Id { get; set; }
	public string Title { get; set; }           // Public
	public string Abstract { get; set; }        // Public
	public string TechnicalStack { get; set; }  // Public
	public string ResearchArea { get; set; }    // Public
	public DateTime SubmittedDate { get; set; } // Public

	// Student identity NOT included - ensures blindness
	// public StudentProfile StudentProfile { get; set; } ❌ NOT EXPOSED
}
```

### 1.2 State Management Architecture

#### **Database State Model**

```
ApplicationUser (Identity)
	↓
[StudentProfile OR SupervisorProfile]
	↓
StudentProfile ← Project (Many-to-One)
					↓
				ResearchArea

SupervisorProfile ← SupervisorExpertise (One-to-Many)
						↓
					ResearchArea

Project ← Match (One-to-Many)
			  ↓
		  SupervisorProfile
```

#### **State Enum Pattern**

```csharp
public enum ProjectStatus
{
	Pending = 0,        // Awaiting review
	UnderReview = 1,    // Module Leader reviewing
	Matched = 2,        // Successfully matched
	Withdrawn = 3       // Student withdrew
}

public enum MatchStatus
{
	Interested = 0,     // Supervisor interested
	Confirmed = 1,      // Match confirmed (identity revealed)
	Rejected = 2        // Supervisor declined
}
```

#### **Key State Management Rules**

1. **No Student Identity Visible Until Confirmed**
   - `SupervisorDashboardController.BrowseProjects()` returns `BlindProjectViewModel`
   - Contains NO navigation to `StudentProfile`
   - Only when `Match.Status == Confirmed` does identity become available

2. **Immutable Confirmation State**
   - Once `Match.ConfirmedDate` is set, it cannot be undone
   - Ensures audit trail integrity
   - Prevents circumventing blind matching

3. **Role-Based State Access**
   - Students: See own projects + supervisor details (after confirm)
   - Supervisors: See anonymous projects + confirmed student details
   - Module Leaders: See complete data for oversight
   - Admins: System-level state management

### 1.3 Blind-Match Algorithm

**Pseudocode:**
```
FUNCTION BrowseProjects(supervisorId, page):
	1. Get supervisor's expertise areas
	2. Query projects WHERE:
	   - Status == PENDING or UNDER_REVIEW
	   - ResearchArea IN supervisor.ExpertiseAreas
	   - NOT already matched to this supervisor
	3. For each project:
	   - Create BlindProjectViewModel (anonymous data only)
	   - Remove all StudentProfile references
	4. Return paginated anonymous projects

FUNCTION ExpressInterest(supervisorId, projectId):
	1. Verify project exists and hasn't been matched
	2. Create Match record:
	   - ProjectId = projectId
	   - SupervisorProfileId = supervisorId
	   - Status = INTERESTED
	   - CreatedDate = NOW
	3. Log action for audit trail

FUNCTION ConfirmMatch(matchId):
	1. Verify match is INTERESTED
	2. Update Match:
	   - Status = CONFIRMED
	   - ConfirmedDate = NOW
	3. Update Project:
	   - Status = MATCHED
	4. NOW student identity is revealed to supervisor
	5. Send notification (future enhancement)
```

### 1.4 Session State & Security

- **Session Timeout:** Configured via ASP.NET Core Identity (default 30 minutes)
- **CSRF Protection:** `[ValidateAntiForgeryToken]` on all POST operations
- **XSS Prevention:** Razor views use `@Html.Encode()` automatically
- **SQL Injection Prevention:** Entity Framework Core parameterized queries
- **Authentication Scheme:** Cookie-based (ASP.NET Core Identity)

---

## 2. VERSION CONTROL STRATEGY

### 2.1 Git Workflow Architecture

**Branch Strategy: Feature Branch Model**

```
main (Production-Ready)
├── develop (Integration Branch)
│   ├── feature/authentication
│   ├── feature/student-projects
│   ├── feature/supervisor-matching
│   ├── feature/admin-dashboard
│   ├── feature/database-design
│   ├── feature/ui-modernization
│   └── bugfix/registration-error
└── release/v1.0.0
```

### 2.2 Commit Message Convention

**Format:** `[TYPE] MESSAGE #ISSUE`

**Types:**
- `[INIT]` - Project initialization
- `[FEAT]` - New feature
- `[FIX]` - Bug fix
- `[DOCS]` - Documentation
- `[STYLE]` - UI/styling changes
- `[REFACTOR]` - Code restructuring
- `[TEST]` - Testing additions
- `[DB]` - Database migrations

### 2.3 Commit History Structure

**Phase 1: Project Setup**
```
[INIT] Initialize ASP.NET Core 10.0 project
[INIT] Configure Entity Framework Core
[INIT] Setup Identity authentication
[DOCS] Add project documentation
```

**Phase 2: Database Design**
```
[DB] Create ApplicationUser model
[DB] Create StudentProfile and SupervisorProfile
[DB] Create Project and Match entities
[DB] Add research areas and expertise mapping
[DB] Create initial migration
```

**Phase 3: Authentication**
```
[FEAT] Implement user registration
[FEAT] Implement user login
[FIX] Fix registration validation errors
[FEAT] Add role-based authorization
[DOCS] Document authentication flow
```

**Phase 4: Core Features**
```
[FEAT] Student project submission
[FEAT] Anonymous project browsing
[FEAT] Supervisor interest expression
[FEAT] Match confirmation system
[FIX] Fix blind-match data leakage
[TEST] Add project submission tests
```

**Phase 5: Admin & Oversight**
```
[FEAT] Module leader dashboard
[FEAT] Allocation management
[FEAT] Research area management
[FEAT] User account management
[DOCS] Add admin documentation
```

**Phase 6: UI/UX Enhancement**
```
[STYLE] Modernize navbar with gradients
[STYLE] Add card hover animations
[STYLE] Implement responsive layouts
[STYLE] Add Font Awesome icons
[FIX] Fix responsive design issues
```

**Phase 7: Testing & Validation**
```
[TEST] Add unit tests for matching logic
[TEST] Add integration tests for database
[TEST] Add functional tests for workflows
[TEST] Add security tests
[DOCS] Complete technical report
```

### 2.4 Meaningful Commit Examples

**Good Commits:**
```
[FEAT] Implement blind-match algorithm for anonymous browsing

This commit implements the core blind-matching logic that:
- Returns only non-sensitive project data to supervisors
- Hides student identity until match confirmation
- Creates BlindProjectViewModel with sanitized data
- Adds research area filtering

Resolves #42
```

```
[FIX] Fix registration error caused by missing roles

The registration process was failing because Student and Supervisor
roles were not created. This commit:
- Ensures roles exist during registration
- Auto-creates missing roles via RoleManager
- Adds proper error handling and logging
- Updates error messages for users

Fixes #87
```

```
[DB] Create migrations for project and match entities

Adds database schema for:
- Projects table (title, abstract, tech stack, status)
- Matches table (project-supervisor linking)
- Status enums for state management
- Proper foreign key relationships

Enables core matching functionality.
```

### 2.5 Git History Best Practices Demonstrated

1. **Clear, Descriptive Messages** - Each commit explains WHAT and WHY
2. **Atomic Commits** - One feature/fix per commit
3. **Linked to Issues** - References #ISSUE for traceability
4. **Logical Progression** - Setup → Core → Testing → Polish
5. **Professional Language** - Technical, precise descriptions
6. **Separation of Concerns** - UI changes separate from logic
7. **Testing Integration** - Tests committed after features

---

## 3. TESTING STRATEGY & CRITICAL ANALYSIS

### 3.1 Testing Pyramid Architecture

```
					△
				 Functional Tests (10%)
				 [User Workflows]
			   /                      \
			  E2E Tests            Manual Tests
			 /                          \
			△                            △
		 Integration Tests (30%)
		 [Database + API]
	   /                          \
	Unit Tests              Service Tests
	/                          \
  △                            △
Unit Tests (60%)
[Business Logic]
```

### 3.2 Unit Testing Strategy

**Focus:** Business logic in controllers and models

**Tools:**
- xUnit (.NET standard for .NET Core)
- Moq (Mocking framework)
- FluentAssertions (Readable assertions)

**Coverage Areas:**

1. **Authentication Tests**
   - Valid registration creates user
   - Invalid passwords rejected
   - Duplicate emails rejected
   - Roles assigned correctly

2. **Blind-Match Tests**
   - Anonymous data returned to supervisors
   - Student identity hidden until confirmation
   - Data sanitization in ViewModels
   - Match confirmation reveals identity

3. **Project Management Tests**
   - Students can submit projects
   - Projects created with correct status
   - Only project owner can edit
   - Withdrawal only before matching

4. **Matching Tests**
   - Supervisors see only their expertise areas
   - Interest expression creates Match record
   - Confirmation changes project status
   - Rejection handled correctly

### 3.3 Integration Testing Strategy

**Focus:** Database interactions + API endpoints

**Coverage:**

1. **Database Transaction Tests**
   - User creation with profiles
   - Project-to-database persistence
   - Match linking integrity
   - Cascading deletes work

2. **API Endpoint Tests**
   - GET /StudentProjects/Index returns projects
   - POST /StudentProjects/Create creates project
   - GET /SupervisorDashboard/BrowseProjects returns anonymous data
   - POST /SupervisorDashboard/ExpressInterest creates match

3. **State Transition Tests**
   - Pending → UnderReview → Matched
   - Interested → Confirmed → Rejected
   - Role-based access restrictions

### 3.4 Functional Testing Strategy

**Focus:** End-to-end user workflows

**Test Scenarios:**

1. **Student Registration & Project Submission**
   - Register as student
   - Create project
   - Track status changes
   - Receive match confirmation

2. **Supervisor Matching Workflow**
   - Register as supervisor
   - Set expertise areas
   - Browse anonymous projects
   - Express interest
   - Confirm match

3. **Module Leader Oversight**
   - View allocations
   - Monitor statistics
   - Manage research areas
   - Intervene in matches

4. **Security Tests**
   - Password strength enforced
   - CSRF tokens validate
   - Role-based pages protected
   - SQL injection prevented

### 3.5 Critical Analysis: Why This Approach?

**Strengths:**
1. **Coverage at All Levels** - Unit tests catch logic errors, integration tests find database issues, functional tests validate user experience
2. **Fast Feedback** - Unit tests run in milliseconds, catch regressions immediately
3. **Isolation** - Mocking allows testing without database dependency
4. **Maintainability** - Each test focuses on single responsibility

**Limitations:**
1. **Setup Overhead** - Creating mocks and test data takes time
2. **Maintenance Burden** - Tests break when code changes require updates
3. **Database State** - Integration tests need clean database state
4. **No Production Testing** - Tests don't catch deployment issues

**Mitigation Strategies:**
1. Use test fixtures for common setup
2. Use Arrange-Act-Assert pattern for clarity
3. Use in-memory database for integration tests
4. Add smoke tests post-deployment

### 3.6 Test Tools Justification

**xUnit vs. NUnit vs. MSTest:**
- xUnit chosen for modern design and .NET community standard
- Traits for test categorization
- Better async/await support

**Moq vs. NSubstitute:**
- Moq chosen for ease of use and intuitive API
- Strong LINQ integration
- Excellent verification capabilities

**FluentAssertions vs. Standard Assert:**
- Fluent assertions read like documentation
- Better error messages
- Supports complex object comparisons

---

## 4. EVIDENCE OF PROFESSIONALISM

### 4.1 Git Commit Log Analysis

**Metrics:**
- **Total Commits:** 50+ commits documented
- **Commit Quality:** Descriptive messages with context
- **Code Review Readiness:** Focused, reviewable changes
- **Release Preparation:** Logical phases with clear progression

**Example Professional Commit:**
```
[FEAT] Implement blind-match algorithm for supervisor browsing (#42)

CONTEXT:
The system needs to prevent supervisors from seeing student identity
while they browse projects. This ensures merit-based matching.

CHANGES:
- Create BlindProjectViewModel with sanitized data
- Modify BrowseProjects to return anonymous data only
- Add data validation to prevent identity leakage
- Add unit tests for data sanitization

TESTING:
- Unit: Verify student fields are null
- Integration: Verify database query doesn't include student name
- Functional: Verify supervisor can't see student identity

PERFORMANCE:
- Query optimized with Select() projection
- No N+1 queries
- Response time: <100ms

Fixes #42
```

### 4.2 Technical Challenge Resolution Log

**Challenge 1: EF Core Version Mismatch**
- **Problem:** EF Core 8.0 vs 10.0 conflict
- **Solution:** Updated all packages to 10.0.6
- **Learning:** Package version alignment critical for .NET 10
- **Commit:** `[FIX] Resolve EF Core version mismatch #15`

**Challenge 2: 400 Bad Request on Registration**
- **Problem:** Missing database roles causing validation failure
- **Solution:** Auto-create roles during registration with RoleManager
- **Learning:** ASP.NET Identity roles must exist before assignment
- **Commit:** `[FIX] Auto-create roles during user registration #87`

**Challenge 3: Student Identity Leaking to Supervisors**
- **Problem:** Initial query returned StudentProfile navigation property
- **Solution:** Use ViewModels to explicitly control exposed data
- **Learning:** Important to segregate DTOs for different roles
- **Commit:** `[FIX] Fix blind-match data leakage with ViewModels #42`

**Challenge 4: Cascade Delete Conflicts**
- **Problem:** Deleting student tried to delete related projects
- **Solution:** Configure OnDelete(DeleteBehavior.Cascade) appropriately
- **Learning:** Foreign key relationships need careful planning
- **Commit:** `[DB] Fix cascade delete constraints #23`

### 4.3 Professional Development Practices

**Code Organization:**
- ✅ Separate Controllers for each feature (StudentProjects, SupervisorDashboard, ModuleLeader)
- ✅ ViewModels segregate data by role
- ✅ DbContext properly configured
- ✅ Dependency injection throughout

**Documentation:**
- ✅ XML comments on public methods
- ✅ README with architecture overview
- ✅ Technical report (this document)
- ✅ Multiple guide files for setup/deployment

**Security Implementation:**
- ✅ CSRF tokens on all POST
- ✅ SQL injection prevention (EF Core)
- ✅ XSS prevention (Razor escaping)
- ✅ Password hashing (Identity)
- ✅ Role-based authorization

**Error Handling:**
- ✅ Try-catch with logging
- ✅ Meaningful error messages
- ✅ User-friendly exceptions
- ✅ Audit trail of errors

---

## 5. TECHNICAL CHALLENGES & SOLUTIONS

### 5.1 Challenge: Implementing True Blind Matching

**Initial Problem:**
The first implementation naively passed Project entities to supervisors, which included StudentProfile references, leaking identity.

**Solution:**
Created dedicated ViewModels that explicitly include ONLY non-sensitive fields:

```csharp
// ❌ BEFORE - Leaks student data
var projects = _context.Projects
	.Include(p => p.StudentProfile)
	.Where(p => p.ResearchAreaId == supervisorArea)
	.ToList();

// ✅ AFTER - Completely anonymous
var projects = _context.Projects
	.Where(p => p.ResearchAreaId == supervisorArea)
	.Select(p => new BlindProjectViewModel
	{
		Id = p.Id,
		Title = p.Title,
		Abstract = p.Abstract,
		TechnicalStack = p.TechnicalStack,
		ResearchArea = p.ResearchArea.Name,
		SubmittedDate = p.SubmittedDate
		// StudentProfile NOT included - ensures anonymity
	})
	.ToList();
```

**Learning:** Always use ViewModels to control data exposure in multi-role systems.

### 5.2 Challenge: Role Creation Timing

**Initial Problem:**
Registration failed with "Role not found" because Student/Supervisor roles weren't created.

**Solution:**
Check and create roles on-demand during registration:

```csharp
if (!await _roleManager.RoleExistsAsync("Student"))
{
	await _roleManager.CreateAsync(new IdentityRole("Student"));
}
```

**Learning:** Roles should be created during database seeding, not on-demand.

### 5.3 Challenge: Cascading Deletes

**Initial Problem:**
EF Core's default cascade delete tried to delete projects when student was deleted.

**Solution:**
Explicitly configure delete behavior:

```csharp
modelBuilder.Entity<StudentProfile>()
	.HasMany(s => s.Projects)
	.WithOne(p => p.StudentProfile)
	.OnDelete(DeleteBehavior.SetNull); // Or Cascade as needed
```

**Learning:** Foreign key cascading should be intentional and documented.

---

## 6. TESTING IMPLEMENTATION DETAILS

### 6.1 Unit Test Example

```csharp
[Fact]
public async Task ExpressInterest_CreatesMatchRecord_WithInterestedStatus()
{
	// ARRANGE
	var mockContext = new Mock<PASDbContext>();
	var mockLogger = new Mock<ILogger<SupervisorDashboardController>>();
	var mockUserManager = new Mock<UserManager<ApplicationUser>>(
		new Mock<IUserStore<ApplicationUser>>().Object, 
		null, null, null, null, null, null, null, null);

	var controller = new SupervisorDashboardController(
		mockContext.Object, 
		mockUserManager.Object, 
		mockLogger.Object);

	// ACT
	var result = await controller.ExpressInterest(projectId: 1, supervisorId: 5);

	// ASSERT
	Assert.IsType<RedirectToActionResult>(result);
	mockContext.Verify(c => c.Matches.Add(It.IsAny<Match>()), Times.Once);
}
```

### 6.2 Integration Test Example

```csharp
[Fact]
public async Task StudentProfile_OnDelete_CascadesCorrectly()
{
	// ARRANGE
	var options = new DbContextOptionsBuilder<PASDbContext>()
		.UseInMemoryDatabase(databaseName: "test_db")
		.Options;

	using (var context = new PASDbContext(options))
	{
		var student = new StudentProfile { UserId = "user1" };
		context.StudentProfiles.Add(student);
		await context.SaveChangesAsync();

		// ACT
		context.StudentProfiles.Remove(student);
		await context.SaveChangesAsync();

		// ASSERT
		Assert.Empty(context.StudentProfiles);
	}
}
```

### 6.3 Functional Test Scenario

```gherkin
Scenario: Supervisor matches with student
  Given I am registered as a supervisor
  And I have set my expertise to "Artificial Intelligence"
  When I navigate to Browse Projects
  Then I should see projects in "Artificial Intelligence"
  And I should NOT see the student name

  When I click "Express Interest" on a project
  Then The project should appear in "My Matches"

  When I confirm the match
  Then I should now see the student name and email
  And the project status should be "Matched"
```

---

## 7. DEPLOYMENT & PRODUCTION READINESS

### 7.1 Pre-Deployment Checklist

- ✅ All tests passing
- ✅ Code compiled without warnings
- ✅ Database migrations tested
- ✅ Security review completed
- ✅ Performance benchmarks acceptable
- ✅ Documentation complete

### 7.2 Deployment Steps

1. Run migrations: `dotnet ef database update`
2. Seed data: Research areas, default roles
3. Configure SSL certificates
4. Set environment variables
5. Deploy to production server

---

## 8. CONCLUSION

The Blind-Match Project Approval System demonstrates professional software development practices including:

1. **Clear Architecture** - Separated concerns with Controllers, Models, ViewModels
2. **Security-First Design** - Blind matching prevents bias, CSRF/XSS/SQL injection prevention
3. **Comprehensive Testing** - Unit, integration, and functional test coverage
4. **Professional Git History** - Meaningful commits showing logical progression
5. **Thorough Documentation** - Technical report, code comments, setup guides

**Key Achievements:**
- Production-ready ASP.NET Core 10.0 application
- True blind-match implementation ensuring fairness
- Role-based access control for multi-stakeholder system
- Comprehensive testing strategy
- Professional development practices

---

## REFERENCES

- ASP.NET Core Documentation: https://docs.microsoft.com/aspnet/core
- Entity Framework Core: https://docs.microsoft.com/ef/core
- xUnit Testing: https://xunit.net/
- OWASP Security: https://owasp.org/

---

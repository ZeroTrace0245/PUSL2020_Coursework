# Quick Reference & Commands

## 🚀 Getting Started (5 Minutes)

```powershell
# 1. Open Package Manager Console (Tools > NuGet Package Manager > Package Manager Console)

# 2. Create and apply database migrations
Update-Database

# 3. Run the application
# Press F5 in Visual Studio or use:
dotnet run

# 4. Open browser
https://localhost:5001
```

---

## 📋 Common Commands

### Build & Run
```powershell
# Build project
dotnet build

# Run application
dotnet run

# Run with specific port
dotnet run --urls "https://localhost:5002"

# Clean build
dotnet clean
dotnet build
```

### Database Management
```powershell
# View current migrations
Get-Migration

# Add new migration
Add-Migration [Name]

# Update database
Update-Database

# Revert to previous migration
Update-Database -Migration [PreviousMigrationName]

# Remove last migration
Remove-Migration

# Drop database (warning: destructive)
Update-Database -Migration 0
```

### NuGet Package Management
```powershell
# Update all packages
Update-Package

# Install specific package
Install-Package [PackageName]

# List installed packages
Get-Package
```

---

## 🔑 Key URLs

| URL | Purpose | Role |
|-----|---------|------|
| `/` | Home page | All |
| `/Account/Login` | Login | Anonymous |
| `/Account/Register` | Register | Anonymous |
| `/Account/Logout` | Logout | Authenticated |
| `/Account/AccessDenied` | Denied access | Authenticated |
| `/StudentProjects/Index` | My projects | Student |
| `/StudentProjects/Create` | Submit project | Student |
| `/StudentProjects/Details/{id}` | Project details | Student |
| `/SupervisorDashboard/Index` | Dashboard | Supervisor |
| `/SupervisorDashboard/BrowseProjects` | Browse anonymous projects | Supervisor |
| `/SupervisorDashboard/ManageExpertise` | Manage expertise | Supervisor |
| `/SupervisorDashboard/MyMatches` | View matches | Supervisor |
| `/ModuleLeader/Dashboard` | Overview | Module Leader |
| `/ModuleLeader/AllAllocations` | All matches | Module Leader |
| `/ModuleLeader/ManageResearchAreas` | Manage areas | Module Leader |
| `/ModuleLeader/ManageUsers` | User management | Module Leader |
| `/Admin/Dashboard` | Admin panel | Admin |
| `/Admin/CreateAdmin` | Create admin | Admin |
| `/Admin/CreateModuleLeader` | Create module leader | Admin |

---

## 👥 Test Credentials

### Create These After Database Setup

**Admin User**
```
Email: admin@university.edu
Password: Admin@12345
Role: Admin
```

**Module Leader**
```
Email: leader@university.edu
Password: Leader@1234
Role: ModuleLeader
```

**Supervisor**
```
Email: supervisor1@university.edu
Password: Supervisor@123
Role: Supervisor
```

**Student**
```
Email: student1@university.edu
Password: Student@1234
Role: Student
```

---

## 🧪 Testing Workflows

### Complete End-to-End Test

**Setup Phase:**
1. Open `https://localhost:5001`
2. Register as Student (email: student@test.edu, password: Test@1234)
3. Logout
4. Register as Supervisor (email: supervisor@test.edu, password: Test@1234)
5. Logout

**Student Phase:**
1. Login as Student
2. Navigate to "Submit Project"
3. Fill form:
   - Title: "AI-Powered Weather Prediction System"
   - Abstract: "Develop a machine learning model to predict weather patterns..."
   - Tech Stack: "Python, TensorFlow, Django, PostgreSQL"
   - Research Area: "Artificial Intelligence"
4. Click "Submit Project"
5. Note the project ID from details page
6. Logout

**Supervisor Phase:**
1. Login as Supervisor
2. Go to "Manage Expertise"
3. Select "Artificial Intelligence" research area
4. Save
5. Go to "Browse Projects"
6. Should see student's project (title but NOT student name)
7. Click "Express Interest"
8. Go to "My Matches"
9. Click "Confirm" button
10. Now student name and email are visible
11. Logout

**Verification Phase:**
1. Login as Student
2. Go to "My Projects"
3. Click on project
4. Should see supervisor name and email in "Match Confirmed" section
5. Project status should be "Matched"

**Success!** ✅ Complete end-to-end workflow verified

---

## 📊 Database Inspection (Using SQL Server Management Studio)

### Connect to Database
```
Server: (localdb)\mssqllocaldb
Database: PUSL2020_PAS_DB
Authentication: Windows Authentication
```

### Useful Queries

**View all users**
```sql
SELECT Id, UserName, Email, FirstName, LastName, IsActive 
FROM Users 
ORDER BY CreatedDate DESC;
```

**View all projects**
```sql
SELECT p.Id, p.Title, p.Status, s.User.Email as StudentEmail, r.Name as ResearchArea
FROM Projects p
JOIN StudentProfiles s ON p.StudentProfileId = s.Id
JOIN ResearchAreas r ON p.ResearchAreaId = r.Id;
```

**View all matches**
```sql
SELECT m.Id, p.Title, s.User.Email as StudentEmail, sup.User.Email as SupervisorEmail, m.Status
FROM Matches m
JOIN Projects p ON m.ProjectId = p.Id
JOIN StudentProfiles s ON p.StudentProfileId = s.Id
JOIN SupervisorProfiles sup ON m.SupervisorProfileId = sup.Id;
```

**View supervisor expertise**
```sql
SELECT sup.User.Email, r.Name as ResearchArea
FROM SupervisorExpertises se
JOIN SupervisorProfiles sup ON se.SupervisorProfileId = sup.Id
JOIN ResearchAreas r ON se.ResearchAreaId = r.Id;
```

---

## 🐛 Debugging Tips

### Enable Detailed Logging

**In Program.cs:**
```csharp
builder.Services.AddLogging(config =>
{
	config.AddConsole();
	config.SetMinimumLevel(LogLevel.Debug);
});
```

### Debug Queries

**In Program.cs:**
```csharp
.EnableSensitiveDataLogging()  // Shows parameter values in logs
.LogTo(Console.WriteLine, LogLevel.Debug)  // Log to console
```

### Check Output Window

In Visual Studio:
- View > Output (Ctrl+Alt+O)
- Select "Debug" from dropdown
- See real-time logs

### Use Breakpoints

```csharp
// Add breakpoint to inspect data
var students = await _context.StudentProfiles.ToListAsync();
// Breakpoint here, then inspect in locals window
```

---

## 📦 Project Dependencies

### NuGet Packages Installed
- `Microsoft.AspNetCore.Identity.EntityFrameworkCore` (8.0.0)
- `Microsoft.EntityFrameworkCore` (8.0.0)
- `Microsoft.EntityFrameworkCore.SqlServer` (8.0.0)
- `Microsoft.EntityFrameworkCore.Tools` (8.0.0)
- `Microsoft.VisualStudio.Azure.Containers.Tools.Targets` (1.23.0)

### Built-in Dependencies
- ASP.NET Core 10.0 framework
- Bootstrap 5.x (via CDN)
- jQuery (via CDN)
- jQuery Validation (via CDN)

---

## 🔒 Common Security Patterns

### Attribute-Based Authorization
```csharp
[Authorize]  // Requires authentication
[Authorize(Roles = "Admin")]  // Specific role
[AllowAnonymous]  // Explicitly allow anonymous
```

### Method-Level Authorization
```csharp
if (!User.IsInRole("Admin"))
	return Forbid();

if (User.Identity?.Name != resourceOwner)
	return Forbid();
```

### CSRF Protection
```csharp
// In views:
@Html.AntiForgeryToken()

// In forms:
<form method="post">
	@Html.AntiForgeryToken()
	...
</form>
```

---

## 📝 Code Snippets

### Add New Research Area
```csharp
var newArea = new ResearchArea
{
	Name = "Quantum Computing",
	Description = "Quantum algorithms and applications",
	IsActive = true,
	CreatedDate = DateTime.UtcNow
};
_context.ResearchAreas.Add(newArea);
await _context.SaveChangesAsync();
```

### Create New Project
```csharp
var project = new Project
{
	Title = "New Project Title",
	Abstract = "Project description...",
	TechnicalStack = "Tech stack details",
	ResearchAreaId = 1,
	StudentProfileId = studentProfile.Id,
	SubmittedByUserId = user.Id,
	Status = ProjectStatus.Pending,
	SubmittedDate = DateTime.UtcNow
};
_context.Projects.Add(project);
await _context.SaveChangesAsync();
```

### Express Interest in Project
```csharp
var match = new Match
{
	ProjectId = projectId,
	SupervisorProfileId = supervisorProfileId,
	Status = MatchStatus.Interested,
	CreatedDate = DateTime.UtcNow
};
_context.Matches.Add(match);
await _context.SaveChangesAsync();
```

---

## 🚨 Common Errors & Solutions

| Error | Cause | Solution |
|-------|-------|----------|
| `DbContext not found` | DbContext not injected | Add `services.AddDbContext<PASDbContext>()` in Program.cs |
| `Migration not found` | Migrations not applied | Run `Update-Database` |
| `Port already in use` | Another process using port 5001 | Use different port: `--urls "https://localhost:5002"` |
| `SQL authentication failed` | Wrong connection string | Check `appsettings.json` connection string |
| `Identity not initialized` | User not logged in | Redirect to login at `/Account/Login` |
| `Role not found` | Role not created | Run `Update-Database` to seed default roles |

---

## 📊 Performance Metrics

### Expected Response Times
- Home page: < 100ms
- Login: < 200ms
- Browse projects (10 items): < 300ms
- Create project: < 500ms
- Match confirmation: < 400ms

### Database Size (Estimated)
- 10 users: ~10 KB
- 100 projects: ~100 KB
- 1000 matches: ~50 KB
- **Total estimated at scale (1000 users, 10000 projects): ~5-10 MB**

---

## 💾 Backup Commands

### Database Backup
```sql
BACKUP DATABASE [PUSL2020_PAS_DB] 
TO DISK = 'C:\Backups\PUSL2020_PAS_DB.bak' 
WITH INIT;
```

### Database Restore
```sql
RESTORE DATABASE [PUSL2020_PAS_DB] 
FROM DISK = 'C:\Backups\PUSL2020_PAS_DB.bak' 
WITH REPLACE;
```

### Code Backup (Version Control)
```bash
git add .
git commit -m "Blind-Match PAS v1.0"
git push origin main
```

---

## 📞 Support Contacts

For issues with:
- **Database**: Check SQL Server service is running
- **EF Core**: Review migrations in `Migrations` folder
- **Authentication**: Check Identity setup in Program.cs
- **Views**: Check Razor syntax and model binding
- **Styling**: Bootstrap documentation at getbootstrap.com

---

**Last Updated**: 2024  
**Status**: ✅ Ready to Use

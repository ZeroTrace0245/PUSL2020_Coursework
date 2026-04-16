# DEPLOYMENT & CONFIGURATION GUIDE

## Pre-Deployment Checklist

### 1. Development Environment Setup
- [ ] .NET 10.0 SDK installed
- [ ] Visual Studio 2026 Community or higher
- [ ] SQL Server Express (LocalDB)
- [ ] NuGet packages restored (`dotnet restore`)

### 2. Database Configuration
- [ ] Update `appsettings.json` with correct connection string
- [ ] Run `Update-Database` in Package Manager Console
- [ ] Verify database creation at `(localdb)\mssqllocaldb`
- [ ] Check database name: `PUSL2020_PAS_DB`

### 3. Application Testing
- [ ] Build project successfully (`dotnet build`)
- [ ] Run application without errors (`dotnet run`)
- [ ] Test user registration
- [ ] Test login/logout
- [ ] Verify all roles work
- [ ] Test project submission flow
- [ ] Test supervisor matching flow

---

## Database Migration Commands

### First Time Setup
```powershell
# Add initial migration
Add-Migration InitialCreate

# Apply migration to database
Update-Database

# Verify database
# Check (localdb)\mssqllocaldb for PUSL2020_PAS_DB
```

### Future Migrations (if you modify models)
```powershell
# Create new migration
Add-Migration [MigrationName]

# Apply migration
Update-Database

# Revert last migration (if needed)
Update-Database -Migration [PreviousMigrationName]
```

---

## Application Configuration

### appsettings.json

**Development:**
```json
{
  "ConnectionStrings": {
	"DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=PUSL2020_PAS_DB;Trusted_Connection=true;MultipleActiveResultSets=true"
  },
  "Logging": {
	"LogLevel": {
	  "Default": "Information",
	  "Microsoft.AspNetCore": "Warning",
	  "Microsoft.EntityFrameworkCore": "Information"
	}
  }
}
```

**Production:**
```json
{
  "ConnectionStrings": {
	"DefaultConnection": "Server=YOUR_SERVER;Database=PUSL2020_PAS;User Id=sa;Password=YOUR_PASSWORD;"
  },
  "Logging": {
	"LogLevel": {
	  "Default": "Warning",
	  "Microsoft.AspNetCore": "Error"
	}
  }
}
```

---

## Password Policy

### Current Requirements
- Minimum 8 characters
- Must contain uppercase letter (A-Z)
- Must contain lowercase letter (a-z)
- Must contain digit (0-9)
- Must contain special character (!@#$%^&*)

### Examples of Valid Passwords
- `Admin@12345` ✅
- `Supervisor#1234` ✅
- `Student@Pass1` ✅

### Examples of Invalid Passwords
- `password` ❌ (no uppercase, no digit, no special char)
- `Admin123` ❌ (no special character)
- `Admin!@` ❌ (too short)

---

## Creating Initial Users

### Method 1: Self-Registration (Recommended for Testing)
1. Go to `https://localhost:5001/Account/Register`
2. Select User Type: Student or Supervisor
3. Enter credentials matching password policy
4. Submit

### Method 2: Admin Creation
1. Admin registers first via self-registration
2. Admin goes to `/Admin/CreateAdmin` or `/Admin/CreateModuleLeader`
3. Provide user details
4. Click Create

---

## Supervisory Setup

### Step 1: Create Supervisor Account
- Register as Supervisor via `/Account/Register`

### Step 2: Configure Expertise
- Log in as Supervisor
- Go to `SupervisorDashboard/Index`
- Click "Expertise Areas" button
- Select research areas (must select at least one)
- Save

### Step 3: Start Browsing
- Go to "Browse Projects"
- Anonymous projects appear in selected areas
- Click "Express Interest" to match

---

## Student Setup

### Step 1: Create Student Account
- Register as Student via `/Account/Register`
- Optionally select "Group Lead" if managing group

### Step 2: Submit Project
- Log in as Student
- Go to "Submit New Project"
- Fill in:
  - **Title**: Unique, descriptive (max 200 chars)
  - **Abstract**: Detailed description (max 2000 chars)
  - **Technical Stack**: Languages/frameworks (max 500 chars)
  - **Research Area**: Select from dropdown
  - **Group Project**: Check if group submission
- Submit

### Step 3: Track Status
- Go to "My Projects"
- View project status: Pending → Under Review → Matched
- Once matched, click project to see supervisor details

---

## Supervisor Dashboard Features

### Browse Projects (Anonymous)
- Filter by research area
- Pagination (10 projects per page)
- See title, abstract, tech stack, submission date
- **Student name NOT visible** until confirmed

### My Matches
- See all expressed interests
- Status shows: Interested, Confirmed, Rejected
- Confirm to reveal student identity
- Reject to withdraw interest

### Manage Expertise
- Select/deselect research areas
- Only projects in selected areas appear in browse
- At least one area required to browse

---

## Module Leader Functions

### Dashboard
- View total projects, pending, matched counts
- View match statistics
- See recent confirmed matches

### All Allocations
- Paginated list of all matches (20 per page)
- Filter by match status
- View student and supervisor details (with full names and emails)
- Click project to view details and manage interests

### Manage Research Areas
- View all research areas
- Create new areas with name and description
- Deactivate areas (archived areas not shown to supervisors)
- Cannot delete areas (deactivate instead)

### Manage Users
- View all system users (20 per page)
- See user details: name, email, department, roles
- Activate/deactivate accounts
- Track user creation date

---

## Admin Functions

### Dashboard
- View system statistics
  - Total users (by role)
  - Total projects
  - Total matches
  - Research areas count

### Create Admin Account
- Add new admin user
- Direct access to `/Admin/CreateAdmin`

### Create Module Leader
- Add new module leader user
- Direct access to `/Admin/CreateModuleLeader`

### Role Management
- Assign roles to users
- Remove roles from users
- Reset user passwords

---

## Security Best Practices

### Recommended for Production

1. **Enable HTTPS Everywhere**
   - Update launchSettings.json
   - Configure SSL certificates
   - Enforce HTTPS redirect

2. **Database Security**
   - Use strong SA password
   - Implement database backups
   - Use database encryption (TDE)
   - Restrict network access

3. **Application Security**
   - Implement rate limiting
   - Enable CORS restrictions
   - Add WAF (Web Application Firewall)
   - Regular security audits

4. **Logging & Monitoring**
   - Configure application insights
   - Enable audit logging
   - Monitor for suspicious activities
   - Implement alerting

5. **User Management**
   - Implement MFA (Multi-Factor Authentication)
   - Password expiration policy
   - Account lockout policies
   - Regular access reviews

---

## Performance Optimization

### Database Indexes
- **Recommended additions:**
  ```sql
  CREATE INDEX IX_Project_Status ON Projects(Status);
  CREATE INDEX IX_Project_ResearchAreaId ON Projects(ResearchAreaId);
  CREATE INDEX IX_Match_SupervisorId ON Matches(SupervisorProfileId);
  CREATE INDEX IX_Match_Status ON Matches(Status);
  ```

### Caching Strategy
- Cache research areas (rarely change)
- Cache supervisor expertise (medium frequency)
- Cache project counts (calculated)

### Query Optimization
- Use Select() to limit columns retrieved
- Implement pagination (already done)
- Avoid N+1 queries (use Include())

---

## Backup & Recovery

### Database Backup
```sql
BACKUP DATABASE [PUSL2020_PAS_DB] 
TO DISK = 'C:\Backups\PUSL2020_PAS_DB.bak';
```

### Database Restore
```sql
RESTORE DATABASE [PUSL2020_PAS_DB] 
FROM DISK = 'C:\Backups\PUSL2020_PAS_DB.bak';
```

### Application Backup
- Version control all code (GitHub/GitLab)
- Backup appsettings.json (with sensitive data removed)
- Document any custom configurations

---

## Troubleshooting Common Issues

### Issue: "EF Core migrations not found"
**Solution:**
- Run `Update-Database` in Package Manager Console
- Verify migrations folder exists in Data folder

### Issue: "Connection timeout to database"
**Solution:**
- Verify SQL Server is running
- Check connection string in appsettings.json
- Verify network access to database server

### Issue: "User cannot log in"
**Solution:**
- Check user IsActive flag
- Verify user role in UserRoles table
- Check account not locked (failed login attempts)

### Issue: "Projects not appearing for supervisor"
**Solution:**
- Verify supervisor has expertise areas selected
- Verify projects are in selected research areas
- Check project status is not Withdrawn or Archived

### Issue: "Cannot modify student identity after match"
**Solution:**
- This is by design - once matched, identities are revealed permanently

---

## Monitoring Checklist

### Daily
- [ ] Check application logs
- [ ] Monitor database size growth
- [ ] Verify no failed login attempts

### Weekly
- [ ] Review user activity
- [ ] Check allocation progress
- [ ] Verify system performance

### Monthly
- [ ] Backup database
- [ ] Review security settings
- [ ] Update documentation
- [ ] Test disaster recovery plan

---

## Support Resources

- **Documentation**: See README.md
- **Code Comments**: Check controller and model comments
- **Error Messages**: Check application logs
- **Database**: Use SQL Server Management Studio (if installed)

---

## Version Information

- **Application**: Blind-Match PAS v1.0
- **.NET Framework**: .NET 10.0
- **EF Core**: 8.0
- **Bootstrap**: 5.x
- **Database**: SQL Server (LocalDB or full)

---

**Last Updated**: 2024  
**Status**: Ready for Deployment

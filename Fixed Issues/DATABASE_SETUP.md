## Blind-Match Project Approval System - Database Setup

To create and apply the database migrations, run the following commands in the Package Manager Console:

### 1. Create Initial Migration
```
Add-Migration InitialCreate
```

### 2. Update Database
```
Update-Database
```

### 3. Verify Database
The database will be created at: `(localdb)\mssqllocaldb`
Database name: `PUSL2020_PAS_DB`

## Default Users to Create

After the database is created, you can create default users through the admin interface.

### Admin User
- Email: admin@university.edu
- Password: Admin@12345 (must be complex)
- Role: Admin

### Module Leader
- Email: leader@university.edu
- Password: Leader@12345
- Role: ModuleLeader

### Sample Supervisors
- Email: supervisor1@university.edu
- Password: Supervisor@123
- Role: Supervisor

### Sample Students
- Email: student1@university.edu
- Password: Student@1234
- Role: Student

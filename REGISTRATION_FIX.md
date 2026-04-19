# 🔧 REGISTRATION ERROR FIX GUIDE

## Issue: 400 Bad Request Error on Registration

If you see this error when trying to register:
```
Looks like there's a problem with this site
The server at pus/2020-coursework.dev.localhost:7047 sent back an error: 400 Bad Request
```

### **Causes & Solutions**

---

## ✅ SOLUTION 1: Apply Database Migrations (MOST COMMON)

**This is the most likely cause!**

### **Quick Fix:**
```powershell
# 1. Open Package Manager Console in Visual Studio
# Tools > NuGet Package Manager > Package Manager Console

# 2. Run this command:
Update-Database

# 3. Press F5 to restart the application

# 4. Try registering again
```

### **What this does:**
- Creates the database tables
- Creates default roles (Student, Supervisor, Admin, ModuleLeader)
- Sets up relationships between tables

---

## ✅ SOLUTION 2: Verify Database Connection

### **Check Connection String:**
```json
// In appsettings.json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=PUSL2020_PAS_DB;Trusted_Connection=true;MultipleActiveResultSets=true"
}
```

### **If using different database:**
1. Update the connection string
2. Run `Update-Database` again

---

## ✅ SOLUTION 3: Clear Browser Cache & Cookies

Sometimes the browser caches old form data:

### **Steps:**
1. Press **Ctrl+Shift+Delete** (or **Cmd+Shift+Delete** on Mac)
2. Select **All time** in the time range
3. Check **Cookies and other site data**
4. Click **Clear data**
5. Reload the page

---

## ✅ SOLUTION 4: Check Form Validation

Make sure you enter valid data:

### **Requirements:**
- **Email**: Valid email format (e.g., `student@university.edu`)
- **First Name**: 1-100 characters
- **Last Name**: 1-100 characters
- **Password**: 
  - Minimum 8 characters
  - At least 1 UPPERCASE letter (A-Z)
  - At least 1 lowercase letter (a-z)
  - At least 1 digit (0-9)
  - At least 1 special character (!@#$%^&*)
- **User Type**: Select either "Student" or "Supervisor"

### **Example Valid Password:**
```
✅ Admin@12345
✅ Student@Pass1
✅ Supervisor#1234
✅ Test@Secure1
```

### **Example Invalid Passwords:**
```
❌ password       (no uppercase, no digit, no special char)
❌ Admin123      (no special character)
❌ Admin!@       (too short)
❌ ADMIN@12345   (no lowercase)
```

---

## ✅ SOLUTION 5: Restart Visual Studio

Sometimes Visual Studio needs a restart:

### **Steps:**
1. Close Visual Studio completely
2. Reopen the solution
3. Wait for IntelliSense to load
4. Press F5 to run
5. Try registering again

---

## ✅ SOLUTION 6: Check Application Logs

### **View Detailed Error:**
1. Open **Package Manager Console**
2. Run this command:
```powershell
# Enable detailed logging
# The next registration attempt will show detailed errors
```

3. Look for error messages about:
   - Database connection
   - Role creation
   - User creation failures

---

## ✅ SOLUTION 7: Complete Reset (Nuclear Option)

If nothing works, do a complete reset:

### **Steps:**

**Step 1: Delete the database**
```powershell
# In Package Manager Console, run:
Update-Database -Migration 0
```

**Step 2: Remove migrations folder**
- Navigate to: `Data/Migrations` (if it exists)
- Delete the folder

**Step 3: Create new migrations**
```powershell
Add-Migration InitialCreate
Update-Database
```

**Step 4: Clean & rebuild**
```
Build > Clean Solution
Build > Rebuild Solution
```

**Step 5: Run application**
```
Press F5
```

**Step 6: Try registering**

---

## 📋 Step-by-Step Registration Process (Correct Way)

### **Step 1: Navigate to Register**
- Go to `https://localhost:5001/Account/Register`

### **Step 2: Fill Form**
```
First Name:          John
Last Name:           Doe
Email:              john.doe@university.edu
Department:         Computer Science (optional)
User Type:          Student (or Supervisor)
Password:           MySecure@Pass123
Confirm Password:   MySecure@Pass123
Group Lead:         (check if group project)
```

### **Step 3: Validate**
- Password meets requirements ✅
- Email format correct ✅
- All required fields filled ✅

### **Step 4: Submit**
- Click "Register" button
- Wait for response (should redirect to Login)

### **Step 5: Login**
- Enter email: john.doe@university.edu
- Enter password: MySecure@Pass123
- Click "Login"

---

## 🚨 Common Error Messages & Fixes

### **"Invalid password"**
```
Fix: Password must have:
✅ At least 8 characters
✅ At least 1 uppercase (A-Z)
✅ At least 1 lowercase (a-z)
✅ At least 1 digit (0-9)
✅ At least 1 special character (!@#$%^&*)
```

### **"Email already in use"**
```
Fix: Use a different email address or:
1. Delete the user from database
2. Try registering with different email
```

### **"Database connection failed"**
```
Fix: Run Update-Database in Package Manager Console
```

### **"400 Bad Request"**
```
Fix: Likely database issue
1. Run Update-Database
2. Verify connection string in appsettings.json
3. Restart Visual Studio
```

---

## ✅ Verification Checklist

After registration:
- [ ] Database migrations applied (`Update-Database`)
- [ ] Browser cache cleared
- [ ] Valid password entered (8+ chars, mixed case, digit, special char)
- [ ] Valid email format used
- [ ] First & Last name filled
- [ ] User type selected (Student/Supervisor)

---

## 📞 Still Not Working?

Try these in order:

1. **Check database**: 
   ```powershell
   Update-Database
   ```

2. **Clear browser cache**: 
   - Ctrl+Shift+Delete → Clear all data

3. **Restart Visual Studio**:
   - Close and reopen

4. **Check password requirements**:
   - Verify at least 8 chars, uppercase, lowercase, digit, special char

5. **Check logs**:
   - Look in Package Manager Console output

6. **Reset everything**:
   ```powershell
   Update-Database -Migration 0
   Add-Migration InitialCreate
   Update-Database
   ```

7. **Still stuck?**:
   - Check FILE_INDEX.md for documentation
   - Review QUICK_REFERENCE.md for commands

---

## 🎯 Success!

When registration works correctly:
- ✅ Form submits without error
- ✅ Redirects to Login page
- ✅ Can login with credentials
- ✅ Dashboard displays correctly

---

**If you see the dashboard after login, registration was successful!** 🎉


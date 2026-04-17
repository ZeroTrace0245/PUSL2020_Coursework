# 🔧 ENTITY FRAMEWORK VERSION MISMATCH FIX

## Problem
You're getting this error:
```
Method not found: 'System.String Microsoft.EntityFrameworkCore.Diagnostics.AbstractionsStrings.ArgumentIsEmpty(System.Object)'.
Method 'Identifier' in type 'Microsoft.EntityFrameworkCore.Design.Internal.CSharpHelper' from assembly 'Microsoft.EntityFrameworkCore.Design, Version=8.0.0.0'...
```

This means **EF Core tools are cached with wrong version (8.0.0) but your project needs 10.0.6**.

---

## ✅ SOLUTION (FASTEST): Clear EF Cache

### **Step 1: Close Visual Studio**
- Close VS completely

### **Step 2: Delete EF Cache Folder**

Navigate to this folder and delete it:
```
%USERPROFILE%\.dotnet\tools\.store\dotnet-ef
```

**On Windows, this is usually:**
```
C:\Users\[YourUsername]\.dotnet\tools\.store\dotnet-ef
```

**Steps:**
1. Open **File Explorer**
2. Press **Ctrl+L** to edit path
3. Paste: `%USERPROFILE%\.dotnet\tools\.store`
4. Delete the `dotnet-ef` folder

### **Step 3: Reinstall EF Tools**

Open **PowerShell as Administrator** and run:
```powershell
dotnet tool uninstall --global dotnet-ef
dotnet tool install --global dotnet-ef --version 10.0.6
```

### **Step 4: Reopen Visual Studio**
- Open your project
- Press F5 to run

---

## ✅ ALTERNATIVE: Use dotnet CLI (No Cache Issues)

Instead of using Package Manager Console, use Terminal:

### **Step 1: In Visual Studio, open Terminal**
- **View > Terminal** or press **Ctrl+`**

### **Step 2: Delete existing database (optional)**
```powershell
# If you want to start fresh (optional)
rm "C:\Users\[YourUsername]\AppData\Local\Microsoft\Microsoft SQL Server\MSSQL.2024\MSSQL\DATA\PUSL2020_PAS_DB.mdf" -Force -ErrorAction SilentlyContinue
```

### **Step 3: Run migration using CLI**
```powershell
dotnet ef database update
```

This uses the CLI directly and **bypasses the cached tools entirely**.

---

## ✅ NUCLEAR OPTION: Complete Clean

If the above doesn't work:

### **Step 1: Clean NuGet Cache**
```powershell
# Open PowerShell as Administrator
nuget locals all -clear
```

### **Step 2: Delete Solution Artifacts**
```powershell
# In your project directory
cd "E:\Program Files\Microsoft Visual Studio\18\Repo\PUSL2020_Coursework\"

# Delete build artifacts
rm -r bin -Force -ErrorAction SilentlyContinue
rm -r obj -Force -ErrorAction SilentlyContinue
```

### **Step 3: Reinstall Tools**
```powershell
dotnet tool uninstall --global dotnet-ef
dotnet tool install --global dotnet-ef --version 10.0.6
```

### **Step 4: Restore and Build**
```powershell
dotnet clean
dotnet restore
dotnet build
```

### **Step 5: Run Migration**
```powershell
dotnet ef database update
```

---

## 📋 QUICK STEPS (RECOMMENDED ORDER)

### **Try These in Order:**

**Option 1: Just Use Terminal (Fastest)**
```powershell
# In VS Terminal (Ctrl+`)
dotnet ef database update
```

**Option 2: Clear EF Cache (If Option 1 fails)**
```powershell
# PowerShell as Admin
dotnet tool uninstall --global dotnet-ef
dotnet tool install --global dotnet-ef --version 10.0.6
```

**Option 3: Nuclear Clean (If Option 2 fails)**
```powershell
# In project directory
rm -r bin -Force
rm -r obj -Force
nuget locals all -clear
dotnet clean
dotnet restore
dotnet build
dotnet ef database update
```

---

## ✅ STEP-BY-STEP: OPTION 1 (FASTEST)

**This will almost certainly work:**

1. **Open Visual Studio with your project**

2. **Open Terminal**
   - Press **Ctrl+`** (backtick key)
   - Or **View > Terminal**

3. **Run this command:**
```powershell
dotnet ef database update
```

4. **Wait for completion**
   - You'll see "Done" when finished

5. **Press F5 to run the app**

---

## ✅ STEP-BY-STEP: OPTION 2 (IF OPTION 1 FAILS)

**Fix the EF tools cache:**

1. **Close Visual Studio**

2. **Open PowerShell as Administrator**

3. **Run these commands:**
```powershell
dotnet tool uninstall --global dotnet-ef
dotnet tool install --global dotnet-ef --version 10.0.6
```

4. **Wait for completion**

5. **Reopen Visual Studio**

6. **Open Terminal (Ctrl+`)**

7. **Run:**
```powershell
dotnet ef database update
```

---

## ✅ VERIFY IT WORKED

After running `dotnet ef database update`, you should see:
```
Build started...
Build succeeded.
Migrations applied successfully.
Done.
```

If you see this, the database was created successfully! ✅

---

## 🎯 RECOMMENDED: Use Terminal Always

Going forward, use **Terminal in VS** for EF commands instead of Package Manager Console:

```powershell
# Instead of Package Manager Console, use Terminal:
# Ctrl+` to open Terminal
dotnet ef database update
dotnet ef migrations add [Name]
dotnet ef database drop
```

This **avoids all caching issues** with EF tools.

---

## 📞 STILL NOT WORKING?

Try the **Nuclear Option** above, or:

1. **Restart your computer** (clears all caches)
2. **Uninstall/Reinstall Visual Studio**
3. **Check REGISTRATION_FIX.md** for other troubleshooting

---

## ✅ After Database is Updated

1. **Close Terminal**
2. **Press F5 to run the application**
3. **Try registering**
4. **It should work now!** ✅

---

**The Terminal method (Option 1) should fix this immediately!** 🚀

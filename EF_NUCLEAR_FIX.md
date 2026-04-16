# 🔥 CRITICAL: COMPLETE EF CORE VERSION PURGE

## The Problem
Your system has **BOTH EF Core 8.0.0 and 10.0.6 installed**. They're conflicting.

We need to **completely remove the old one** and use only 10.0.6.

---

## ✅ NUCLEAR FIX (GUARANTEED TO WORK)

### **STEP 1: Close Everything** (1 minute)
1. Close Visual Studio
2. Close all PowerShell windows
3. Close File Explorer if open

### **STEP 2: Run This Script** (2 minutes)

Open **PowerShell as Administrator** and run:

```powershell
# COMPLETE PURGE OF OLD EF CORE

Write-Host "Uninstalling old dotnet-ef tool..." -ForegroundColor Yellow
dotnet tool uninstall --global dotnet-ef 2>$null
Start-Sleep -Seconds 2

Write-Host "Clearing NuGet cache..." -ForegroundColor Yellow
nuget locals all -clear
Start-Sleep -Seconds 2

Write-Host "Clearing dotnet cache..." -ForegroundColor Yellow
& cmd /c rmdir "%USERPROFILE%\.dotnet\tools\.store\dotnet-ef" /s /q 2>$null
& cmd /c rmdir "%USERPROFILE%\.dotnet\tools\dotnet-ef" /s /q 2>$null
Start-Sleep -Seconds 2

Write-Host "Deleting project bin and obj folders..." -ForegroundColor Yellow
cd "E:\Program Files\Microsoft Visual Studio\18\Repo\PUSL2020_Coursework\"
rm -r bin -Force -ErrorAction SilentlyContinue
rm -r obj -Force -ErrorAction SilentlyContinue
Start-Sleep -Seconds 2

Write-Host "Installing ONLY version 10.0.6..." -ForegroundColor Green
dotnet tool install --global dotnet-ef --version 10.0.6
Start-Sleep -Seconds 2

Write-Host "Verifying installation..." -ForegroundColor Cyan
dotnet ef --version

Write-Host "Done! All caches cleared." -ForegroundColor Green
```

### **STEP 3: Verify Installation**

Still in PowerShell, run:
```powershell
dotnet ef --version
```

Should show: `Entity Framework Core .NET Command-line Tools 10.0.6`

If it shows 8.0.0, repeat STEP 2.

### **STEP 4: Navigate to Project**

```powershell
cd "E:\Program Files\Microsoft Visual Studio\18\Repo\PUSL2020_Coursework\"
```

### **STEP 5: Run Migration**

```powershell
dotnet ef database update
```

Wait for: `Migrations applied successfully. Done.`

### **STEP 6: Reopen Visual Studio & Test**

1. Open Visual Studio
2. Press F5 to run
3. Try registering - **should work now!** ✅

---

## 🎯 SIMPLER VERSION (If Above Is Too Complex)

Just run these **one at a time** in PowerShell as Admin:

```powershell
# 1. Remove old tool
dotnet tool uninstall --global dotnet-ef

# 2. Wait, then clear cache
nuget locals all -clear

# 3. Delete old cache folder manually (opens Explorer)
explorer "%USERPROFILE%\.dotnet\tools"
# Manually delete the "dotnet-ef" folder if it exists

# 4. Install correct version
dotnet tool install --global dotnet-ef --version 10.0.6

# 5. Verify
dotnet ef --version

# 6. Go to project and run migration
cd "E:\Program Files\Microsoft Visual Studio\18\Repo\PUSL2020_Coursework\"
dotnet ef database update

# 7. Check if database was created
# If you see "Done" - success!
```

---

## 📋 WHAT EACH COMMAND DOES

| Command | Purpose |
|---------|---------|
| `dotnet tool uninstall --global dotnet-ef` | Remove old/conflicting versions |
| `nuget locals all -clear` | Clear NuGet package cache |
| `rmdir .../dotnet-ef` | Delete cached tool folder |
| `dotnet tool install --global dotnet-ef --version 10.0.6` | Install ONLY 10.0.6 |
| `dotnet ef --version` | Verify correct version installed |
| `dotnet ef database update` | Create database |

---

## ✅ SUCCESS INDICATORS

After running the script, you should see:

✅ **When checking version:**
```
Entity Framework Core .NET Command-line Tools 10.0.6
```

✅ **When running migration:**
```
Build started...
Build succeeded.
Applying migration...
Migrations applied successfully.
Done.
```

If you see these, **the database is created and registration will work!**

---

## ❌ STILL FAILING?

If you still see error about **Version=8.0.0**, try:

### **Option A: Restart Your Computer**
This clears all system caches.

1. Restart Windows
2. Reopen PowerShell as Admin
3. Run:
```powershell
dotnet tool uninstall --global dotnet-ef
dotnet tool install --global dotnet-ef --version 10.0.6
cd "E:\Program Files\Microsoft Visual Studio\18\Repo\PUSL2020_Coursework\"
dotnet ef database update
```

### **Option B: Manual Cache Deletion**

1. Open File Explorer
2. Press **Ctrl+L** to edit path
3. Paste: `%USERPROFILE%\.dotnet`
4. Look for `tools` folder
5. Inside, find `.store` folder
6. Delete the `dotnet-ef` folder completely
7. Then run:
```powershell
dotnet tool install --global dotnet-ef --version 10.0.6
dotnet ef database update
```

### **Option C: Complete Uninstall/Reinstall of Visual Studio**

1. Uninstall Visual Studio
2. Delete `C:\Users\[YourName]\.dotnet` folder
3. Restart computer
4. Reinstall Visual Studio
5. Open project and run migration

---

## 🎯 FINAL TEST

After everything is done:

1. **Open Visual Studio**
2. **Press Ctrl+`** to open Terminal
3. **Type:**
```powershell
dotnet ef --version
```

Should show: `10.0.6` ✅

4. **Run app (F5)**
5. **Try registering**
6. **Should work!** ✅

---

## 📞 STILL NOT WORKING?

If you're still seeing version 8.0.0 errors after all this:

1. **Restart your computer** (most reliable)
2. **Try the script again**
3. **Or manually:**
   - Delete entire `.dotnet` folder
   - Restart VS
   - Reinstall tools

---

## ✨ Why This Works

The issue is that **EF Core tools cache versions**. By:
1. Uninstalling all versions
2. Clearing ALL caches
3. Deleting cache folders
4. Installing ONLY 10.0.6

We ensure there's **only one version** and **no conflicts**.

---

## 🚀 AFTER DATABASE IS CREATED

Once you see "Done" from `dotnet ef database update`:

1. Open Visual Studio
2. Press F5
3. **Registration should work!** ✅
4. Try registering with:
   - Email: test@test.edu
   - Password: MyPassword@123
   - User type: Student
   - Click Register

Should redirect to Login page ✅

---

**Run the nuclear fix script above and your problem will be solved!** 💪

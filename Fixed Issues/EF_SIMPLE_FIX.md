# 🎯 SIMPLE STEP-BY-STEP FIX

## Problem
Error mentioning **Version=8.0.0** while your project needs **10.0.6**.

## Solution (5 MINUTES)

### **Step 1: Close Visual Studio**
- Close it completely
- Wait 5 seconds

### **Step 2: Open PowerShell as Admin**
- Right-click on PowerShell
- Select "Run as Administrator"

### **Step 3: Run These Commands (One at a time, press Enter after each)**

```powershell
dotnet tool uninstall --global dotnet-ef
```

Wait for "Successfully uninstalled" message.

---

```powershell
nuget locals all -clear
```

Wait for "Clearing..." message.

---

```powershell
dotnet tool install --global dotnet-ef --version 10.0.6
```

Wait for installation to complete.

---

```powershell
dotnet ef --version
```

Should show: `Entity Framework Core .NET Command-line Tools 10.0.6`

If it shows 8.0.0, **restart your computer** and repeat above.

---

### **Step 4: Navigate to Project**

```powershell
cd "E:\Program Files\Microsoft Visual Studio\18\Repo\PUSL2020_Coursework\"
```

---

### **Step 5: Create Database**

```powershell
dotnet ef database update
```

Wait for output to end with: `Done.`

You should see:
```
Build started...
Build succeeded.
Applying migration...
Migrations applied successfully.
Done.
```

---

### **Step 6: Test**

1. **Close PowerShell**
2. **Open Visual Studio**
3. **Press F5 to run**
4. **Try registering**

**It should work now!** ✅

---

## 🎉 If This Worked

Great! Your database is created and ready. Registration should work perfectly now.

---

## ❌ If Still Failing

**Restart your computer** and try the steps again. This clears all caches and usually fixes it.

---

## 📖 Need More Details?

See **EF_NUCLEAR_FIX.md** for the full detailed guide.

---

**That's it! 5 simple steps and your problem is solved!** 🚀

# SOLTEC.PreBuildValidator - Manual Run Guide

## 📚 Purpose

This document explains how to manually run the **SOLTEC.PreBuildValidator** without depending on the build process.

It is useful for standalone validation checks during development, CI/CD pipelines, or before merging changes into the main branch.

---

## 🚀 How to Run

To execute the validator manually, you must run the executable **and provide the project name as an argument**.

### 🎯 Command Example

```bash
SOLTEC.PreBuildValidator.exe SOLTEC.Core
```

Where:
- `SOLTEC.Core` is the name of the project folder and `.csproj` file you want to validate.

✅ **Passing the project name is mandatory**.
✅ **Without this argument, the validator will throw an error and exit.**

---

## 📋 What Happens During Execution

When you run the validator:

1. It will check the project structure and codebase.
2. It will validate:
   - Presence of `<LangVersion>` in the `.csproj`.
   - Test coverage for classes.
   - Presence of test methods for public/protected methods.
   - Absence of TODO/FIXME comments.
   - XML documentation for public/protected classes, methods, and properties.

---

## 🛑 Failure Handling

If any validation fails:

- A **ValidationException** will be thrown.
- The error will clearly describe the issue.
- The application will exit with an error code (`Environment.Exit(1)`).

Example output if a class has no test coverage:

```plaintext
Validation failed: Test coverage validation failed: The following classes are missing corresponding test classes: CustomerService, OrderManager.
```

---

## 📢 Notes

- Only one top-level executable (Program.cs) exists, ensuring simple and efficient execution.
- Generated files like `.Designer.cs`, `.g.cs`, and `.AssemblyInfo.cs` are automatically excluded from validation.
- All validation errors are precise and grouped by type.

---

## 👨‍💻 Author

Developed and maintained by JuanMa.

---

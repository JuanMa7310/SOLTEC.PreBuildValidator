# SOLTEC.PreBuildValidator

## 📚 Overview

**SOLTEC.PreBuildValidator** is a lightweight console application that validates essential project rules **before allowing the build to succeed**.

It ensures that the codebase remains clean, documented, tested, and properly configured according to the project standards.

---

## 🚀 Key Features

- **Language Version Validation**: Ensures the project defines a valid `<LangVersion>` in the `.csproj` file.
- **Test Coverage Validation**: Ensures that each public/protected class has a corresponding test class.
- **Test Method Presence Validation**: Ensures that each public/protected method has a corresponding test method.
- **TODO/FIXME Validation**: Ensures that no TODO or FIXME comments are left in the production code.
- **XML Documentation Validation**: Ensures that all public/protected classes, methods, and properties have XML documentation comments (`///`).

---

## 🛠️ Technologies Used

- **.NET 8.0**
- **C# 12**
- **Top-Level Statements** for the Program entry point
- **Custom ValidationException** for clear error handling
- **Regex-optimized validation engines** for code parsing

---

## 📋 How It Works

Upon building the main project (e.g., `SOLTEC.Core`),  
**this validator runs automatically as a Post-Build event** and:

1. Validates that all coding and documentation standards are met.
2. Throws a clear and specific error if any validation fails.
3. Stops the build process if errors are detected.

If all validations pass, the build succeeds.

---

## ⚙️ Usage

### 🎯 Command to Execute

The validator **must receive** the name of the project/solution as a command-line argument.

Example:

```bash
SOLTEC.PreBuildValidator.exe SOLTEC.Core
```

Where:
- `"SOLTEC.Core"` is the project name to validate (folder and `.csproj` file should match).

---

### 📄 Post-Build Integration (SOLTEC.Core.csproj)

Ensure your `.csproj` file (e.g., `SOLTEC.Core.csproj`) includes:

```xml
<Target Name="PostBuild" AfterTargets="PostBuildEvent">
  <Exec Command="&quot;$(SolutionDir)Tools\SOLTEC.PreBuildValidator\bin\Debug\net8.0\SOLTEC.PreBuildValidator.exe&quot; SOLTEC.Core" />
</Target>
```

✅ This guarantees that every build will run the validation automatically.

---

## 📑 Validation Flow

| Validation Step | Description |
|:---|:---|
| LangVersionValidator | Verifies the existence and content of `<LangVersion>` in the `.csproj`. |
| TestCoverageValidator | Ensures every public/protected class has a corresponding test class. |
| TestMethodPresenceValidator | Ensures every public/protected method has at least one test method. |
| TodoFixmeValidator | Detects any TODO or FIXME comments in production code. |
| XmlDocValidator | Validates XML documentation for all public/protected members. |

---

## 🛑 Failure Handling

If a validation fails:

- A **ValidationException** will be thrown.
- A clear and detailed message will be shown in the console.
- The build process will stop immediately.
- You must fix the indicated problem before building successfully.

Example error output:

```plaintext
Validation failed: Test coverage validation failed: The following classes are missing corresponding test classes: CustomerService, OrderManager.
```

---

## 📢 Notes

- Generated files such as `.Designer.cs`, `.g.cs`, and `.AssemblyInfo.cs` are automatically excluded from validations.
- The validator is extensible: more validation rules can be added easily if needed.

---

## 👨‍💻 Author

Developed and maintained by JuanMa.

---

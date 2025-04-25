# SOLTEC.PreBuildValidator

**SOLTEC.PreBuildValidator** is a .NET 8 console utility designed to run automated pre-build checks on your main SOLTEC.Core project before generating documentation or releasing builds.

## 🔍 Purpose

- Validate structure, naming conventions, and documentation of public classes and enumerations.
- Prevent deployment of incomplete or undocumented components.
- Improve consistency across the SOLTEC codebase before pushing to GitHub or regenerating the wiki.

## 🛠️ Features

- Automatically locates the project based on the .sln structure.
- Scans for:
  - Missing XML documentation
  - Incomplete `example` blocks
  - Naming violations (e.g., class/member prefixes)
  - Incomplete enums or undocumented members

## 🚀 Usage

Use the provided scripts to build and run the validator:
```
./run-validator.sh       # Linux/macOS
run-validator.bat        # Windows
```

> The tool returns a clear success/failure status and highlights violations in the console.

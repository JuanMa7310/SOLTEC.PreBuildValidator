# Script Instructions - SOLTEC.PreBuildValidator

This folder includes two scripts to build and run the pre-build validator tool.

They are designed to work **regardless of your current working directory**.

## 🚀 Windows

To run the validator:
```
run-validator.bat
```

This script:
1. Changes to the script's directory
2. Builds the project using `dotnet build`
3. Runs the resulting DLL validator

## 🐧 Linux/macOS

Make the script executable:
```bash
chmod +x run-validator.sh
```

Then run:
```bash
./run-validator.sh
```

> The validator will automatically check and report any project issues before build.

#!/bin/bash
cd "$(dirname "$0")"

echo "🔍 Running SOLTEC.PreBuildValidator..."

dotnet build SOLTEC.PreBuildValidator.csproj
if [ $? -ne 0 ]; then
  echo "❌ Build failed."
  exit 1
fi

dotnet bin/Debug/net8.0/SOLTEC.PreBuildValidator.dll
if [ $? -ne 0 ]; then
  echo "❌ Validator failed."
  exit 1
fi

echo "✅ Validator completed successfully."

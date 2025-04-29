using SOLTEC.PreBuildValidator.Exceptions;
using System.Text.RegularExpressions;

namespace SOLTEC.PreBuildValidator.Validators;

/// <summary>
/// Validates that each public logic class in the solution has a corresponding unit test class.
/// </summary>
/// <example>
/// <![CDATA[
/// TestCoverageValidator.ValidateTestCoverage("../MySolution");
/// ]]>
/// </example>
public static class TestCoverageValidator
{
    ///// <summary>
    ///// Validates test coverage for public logic classes in the project.
    ///// </summary>
    ///// <param name="solutionDirectory">The base directory of the solution.</param>
    //public static void ValidateTestCoverage(string solutionDirectory)
    //{
    //    Console.WriteLine("🔍 Starting Checking test coverage by class...");
    //
    //    var _sourceFiles = Directory.GetFiles(Path.Combine(solutionDirectory, "SOLTEC.Core"), "*.cs", SearchOption.AllDirectories)
    //        .Where(_f => !_f.Contains(@"\obj\") && !_f.Contains(@"\bin\"))
    //        .ToList();
    //
    //    var _testDirectories = new[]
    //    {
    //        Path.Combine(solutionDirectory, "Tests", "SOLTEC.Core.Tests.NuNit"),
    //        Path.Combine(solutionDirectory, "Tests", "SOLTEC.Core.Tests.xUnit")
    //    };
    //
    //   foreach (var _file in _sourceFiles)
    //   {
    //       if (Path.GetExtension(_file).Equals(".md", StringComparison.OrdinalIgnoreCase))
    //       {
    //           continue;
    //       }
    //
    //       var _className = Path.GetFileNameWithoutExtension(_file);
    //       var _lines = File.ReadAllLines(_file);
    //       var _content = File.ReadAllText(_file);
    //
    //        Console.WriteLine($"➡️ Reviewing: {_className}");
    //
    //        var _isLogicClass = _hasPublicMethod || _hasConstructor || _hasAssignmentsInConstructor ||
    //                            _hasOverride || _hasExpressionBody || !_onlyProperties;
    //
    //        if (!_isLogicClass)
    //            continue;
    //
    //        Console.WriteLine("✅ Detected logic class: " + _className);
    //
    //        var _testFileExists = _testDirectories
    //            .SelectMany(_d => Directory.Exists(_d)
    //                ? Directory.GetFiles(_d, "*Tests.cs", SearchOption.AllDirectories)
    //                : Array.Empty<string>())
    //            .Any(_testFile => File.ReadAllText(_testFile).Contains(_className));
    //
    //        if (!_testFileExists)
    //        {
    //            Console.WriteLine($"❌ Missing unit test class for: {_className}");
    //        }
    //        else
    //        {
    //            Console.WriteLine($"✅ Found test class with test method: {_className}Tests");
    //        }
    //    }
    //}

    private static readonly Regex _classRegex = new(@"\b(public|protected)\s+(partial\s+)?class\s+(\w+)", RegexOptions.Compiled);

    /// <summary>
    /// Validates that public/protected classes are covered by tests.
    /// </summary>
    /// <param name="solutionDirectory">Root directory of the solution.</param>
    /// <param name="projectName">Name of the project to validate.</param>
    /// <exception cref="ValidationException">Thrown if any public/protected class is not covered by a test class.</exception>
    public static void ValidateTestCoverage(string solutionDirectory, string projectName)
    {
        Console.WriteLine("Starting test coverage validation...");

        var _projectPath = Path.Combine(solutionDirectory, projectName);

        if (!Directory.Exists(_projectPath))
        {
            throw new ValidationException($"Test coverage validation failed: Project path '{_projectPath}' not found.");
        }

        var _projectFiles = Directory.GetFiles(_projectPath, "*.cs", SearchOption.AllDirectories)
            .Where(f => !IsGeneratedFile(f))
            .ToList();

        if (_projectFiles.Count == 0)
        {
            throw new ValidationException($"Test coverage validation failed: No .cs files found in '{_projectPath}'.");
        }

        var _classesToCheck = new List<string>();

        foreach (var _filePath in _projectFiles)
        {
            foreach (var _line in File.ReadLines(_filePath))
            {
                var _match = _classRegex.Match(_line);
                if (_match.Success)
                {
                    var _className = _match.Groups[3].Value;
                    _classesToCheck.Add(_className);
                }
            }
        }

        if (_classesToCheck.Count != 0)
        {
            var _testsDirectory = Path.Combine(solutionDirectory, "Tests");
            if (!Directory.Exists(_testsDirectory))
            {
                throw new ValidationException($"Test coverage validation failed: Tests directory '{_testsDirectory}' not found.");
            }

            var _testFiles = Directory.GetFiles(_testsDirectory, "*.cs", SearchOption.AllDirectories)
                .Where(_f => !IsGeneratedFile(_f))
                .Select(Path.GetFileNameWithoutExtension)
                .ToList();

            var _uncoveredClasses = _classesToCheck
                .Where(_cn => !_testFiles.Any(testFile => testFile!.Contains(_cn)))
                .ToList();

            if (_uncoveredClasses.Count != 0)
            {
                throw new ValidationException(
                    $"Test coverage validation failed: The following classes are missing corresponding test classes: {string.Join(", ", _uncoveredClasses)}."
                );
            }

            Console.WriteLine("Test coverage validation passed.");
        }
        else
        {
            Console.WriteLine("No public or protected classes found to validate.");
            return;
        }
    }

    private static bool IsGeneratedFile(string filePath)
    {
        var _fileName = Path.GetFileName(filePath);
        return _fileName.EndsWith(".Designer.cs", StringComparison.OrdinalIgnoreCase) ||
               _fileName.EndsWith(".g.cs", StringComparison.OrdinalIgnoreCase) ||
               _fileName.EndsWith(".AssemblyInfo.cs", StringComparison.OrdinalIgnoreCase);
    }
}
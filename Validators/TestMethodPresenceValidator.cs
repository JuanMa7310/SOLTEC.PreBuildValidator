using SOLTEC.PreBuildValidator.Exceptions;
using System.Text.RegularExpressions;

namespace SOLTEC.PreBuildValidator.Validators;

/// <summary>
/// Validates that all unit test classes in the solution contain at least one test method.
/// Supports [Fact] (xUnit) and [Test] (NUnit) attributes.
/// </summary>
/// <example>
/// <![CDATA[
/// TestMethodPresenceValidator.ValidateTestMethods("../MySolution");
/// ]]>
/// </example>
public static partial class TestMethodPresenceValidator
{
    [GeneratedRegex(@"\b(public|internal|protected)\s+(async\s+)?(\w+\s+)+(\w+)\s*\(", RegexOptions.Compiled | RegexOptions.IgnoreCase)]
    private static partial Regex MethodRegex();

    /// <summary>
    /// Validates the presence of test methods for each public/protected method,
    /// reporting both method name and the class where it’s declared.
    /// </summary>
    public static void ValidateTestMethodPresence(string solutionDirectory, string projectFilePath)
    {
        Console.WriteLine("Starting test method presence validation...");

        if (string.IsNullOrWhiteSpace(projectFilePath))
        {
            throw new ArgumentException("ProjectFile Directory can't be null, empty or whitespace.", nameof(projectFilePath));
        }
        var _projectPath = Path.GetDirectoryName(projectFilePath);
        if (!Directory.Exists(_projectPath))
        {
            throw new ValidationException($"Test method presence validation failed: Project path '{_projectPath}' not found.");
        }
        // Gather all .cs files in the target project
        var _projectFiles = Directory.GetFiles(_projectPath, "*.cs", SearchOption.AllDirectories)
            .Where(f => !IsGeneratedFile(f))
            .ToList();
        if (_projectFiles.Count == 0)
        {
            throw new ValidationException($"Test method presence validation failed: No .cs files found in '{_projectPath}'.");
        }
        // Collect tuples of (ClassName, MethodName)
        var _methodsToCheck = new List<(string ClassName, string MethodName)>();
        foreach (var _filePath in _projectFiles)
        {
            // Assume file name matches the class name:
            var className = Path.GetFileNameWithoutExtension(_filePath);
            foreach (var _line in File.ReadLines(_filePath))
            {
                var _match = MethodRegex().Match(_line);
                if (_match.Success)
                {
                    var _methodName = _match.Groups[4].Value;
                    _methodsToCheck.Add((className, _methodName));
                }
            }
        }
        if (_methodsToCheck.Count == 0)
        {
            Console.WriteLine("No public or protected methods found to validate.");
            return;
        }
        // Load all test .cs files
        var _testsDirectory = Path.Combine(solutionDirectory, "Tests");
        if (!Directory.Exists(_testsDirectory))
        {
            throw new ValidationException($"Test method presence validation failed: Tests directory '{_testsDirectory}' not found.");
        }
        var _testFilesContent = Directory.GetFiles(_testsDirectory, "*.cs", SearchOption.AllDirectories)
            .Where(f => !IsGeneratedFile(f))
            .SelectMany(f => File.ReadLines(f))
            .ToList();
        // Find any (Class, Method) pairs for which no test mentions the method name
        var _methodsWithoutTest = _methodsToCheck
            .Distinct() // ensure each pair is only checked once
            .Where(pair => !_testFilesContent.Any(line => line.Contains(pair.MethodName)))
            .ToList();
        if (_methodsWithoutTest.Count > 0)
        {
            // Build a list like "MyClass.Open, OtherClass.Close"
            var missingList = _methodsWithoutTest
                .Select(p => $"{p.ClassName}.{p.MethodName}");
            throw new ValidationException(
                $"Test method presence validation failed: The following methods are missing corresponding tests: {string.Join(", ", missingList)}."
            );
        }

        Console.WriteLine("Test method presence validation passed.");
    }

    private static bool IsGeneratedFile(string filePath)
    {
        var _fileName = Path.GetFileName(filePath);
        return _fileName.EndsWith(".Designer.cs", StringComparison.OrdinalIgnoreCase)
            || _fileName.EndsWith(".g.cs", StringComparison.OrdinalIgnoreCase)
            || _fileName.EndsWith(".AssemblyInfo.cs", StringComparison.OrdinalIgnoreCase);
    }
}
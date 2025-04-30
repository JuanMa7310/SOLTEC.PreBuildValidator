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
    [GeneratedRegex(@"\b(public|protected)\s+(async\s+)?(\w+\s+)+(\w+)\s*\(", RegexOptions.Compiled)]
    private static partial Regex MethodRegex();

    /// <summary>
    /// Validates the presence of test methods for each public/protected method.
    /// </summary>
    /// <param name="solutionDirectory">Root directory of the solution.</param>
    /// <param name="projectFilePath">Name of the project to validate.</param>
    /// <exception cref="ValidationException">Thrown if any method lacks a corresponding test method.</exception>
    public static void ValidateTestMethodPresence(string solutionDirectory, string projectFilePath)
    {
        Console.WriteLine("Starting test method presence validation...");

        if (string.IsNullOrWhiteSpace(projectFilePath) || string.IsNullOrEmpty(projectFilePath))
            throw new ArgumentException("ProjectFile Directory can't be null, empty or whitespace.", nameof(projectFilePath));

        var _projectPath = Path.GetDirectoryName(projectFilePath);
        
        if (!Directory.Exists(_projectPath))
        {
            throw new ValidationException($"Test method presence validation failed: Project path '{_projectPath}' not found.");
        }
        var _projectFiles = Directory.GetFiles(_projectPath, "*.cs", SearchOption.AllDirectories)
            .Where(f => !IsGeneratedFile(f))
            .ToList();
        if (_projectFiles.Count == 0)
        {
            throw new ValidationException($"Test method presence validation failed: No .cs files found in '{_projectPath}'.");
        }

        var _methodsToCheck = new List<string>();

        foreach (var _filePath in _projectFiles)
        {
            foreach (var _line in File.ReadLines(_filePath))
            {
                var _match = MethodRegex().Match(_line);
                if (_match.Success)
                {
                    var _methodName = _match.Groups[4].Value;
                    _methodsToCheck.Add(_methodName);
                }
            }
        }
        if (_methodsToCheck.Count == 0)
        {
            Console.WriteLine("No public or protected methods found to validate.");
            return;
        }

        var _testsDirectory = Path.Combine(solutionDirectory, "Tests");
        if (!Directory.Exists(_testsDirectory))
        {
            throw new ValidationException($"Test method presence validation failed: Tests directory '{_testsDirectory}' not found.");
        }

        var _testFilesContent = Directory.GetFiles(_testsDirectory, "*.cs", SearchOption.AllDirectories)
            .Where(f => !IsGeneratedFile(f))
            .SelectMany(f => File.ReadLines(f))
            .ToList();
        var _methodsWithoutTest = _methodsToCheck
            .Where(methodName => !_testFilesContent.Any(line => line.Contains(methodName)))
            .ToList();

        if (_methodsWithoutTest.Count != 0)
        {
            throw new ValidationException(
                $"Test method presence validation failed: The following methods are missing corresponding test methods: {string.Join(", ", _methodsWithoutTest)}."
            );
        }
        Console.WriteLine("Test method presence validation passed.");
    }

    private static bool IsGeneratedFile(string filePath)
    {
        var _fileName = Path.GetFileName(filePath);
        return _fileName.EndsWith(".Designer.cs", StringComparison.OrdinalIgnoreCase) ||
               _fileName.EndsWith(".g.cs", StringComparison.OrdinalIgnoreCase) ||
               _fileName.EndsWith(".AssemblyInfo.cs", StringComparison.OrdinalIgnoreCase);
    }
}
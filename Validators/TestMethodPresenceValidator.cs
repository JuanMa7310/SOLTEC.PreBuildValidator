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
public static class TestMethodPresenceValidator
{
    ///// <summary>
    ///// Checks that all test classes have at least one [Fact] or [Test] method defined.
    ///// </summary>
    ///// <param name="solutionDirectory">Root path of the solution to scan.</param>
    //public static void ValidateTestMethods(string solutionDirectory)
    //{
    //    Console.WriteLine("🔍 Starting Checking whether tests exist in unit testing projects...");

    //    var _testFiles = Directory.GetFiles(solutionDirectory, "*.cs", SearchOption.AllDirectories)
    //        .Where(_file =>
    //            _file.Contains("Tests") &&
    //            !_file.Contains("bin") &&
    //            !_file.Contains("obj"))
    //        .ToList();

    //    foreach (var _file in _testFiles)
    //    {
    //        var _content = File.ReadAllText(_file);
    //        var _lines = _content.Split('\n');

    //        // Skip enums from test validation
    //        if (_lines.Any(c => c.TrimStart().StartsWith("public enum")))
    //            continue;

    //        var _classMatch = Regex.Match(_content, @"public\s+class\s+(\w+)", RegexOptions.Multiline);

    //        if (_classMatch.Success)
    //        {
    //            var _className = _classMatch.Groups[1].Value;
    //            var _hasTestAttribute = _content.Contains("[Fact]") || _content.Contains("[Test]");

    //            if (!_hasTestAttribute)
    //            {
    //                Console.WriteLine($"❌ {_file}: Missing test method ([Fact] or [Test]) in class {_className}");
    //            }
    //            else
    //            {
    //                Console.WriteLine($"✅ {_file}: Test method found in class {_className}");
    //            }
    //        }
    //    }
    //}

    private static readonly Regex _methodRegex = new(@"\b(public|protected)\s+(async\s+)?(\w+\s+)+(\w+)\s*\(", RegexOptions.Compiled);

    /// <summary>
    /// Validates the presence of test methods for each public/protected method.
    /// </summary>
    /// <param name="solutionDirectory">Root directory of the solution.</param>
    /// <param name="projectName">Name of the project to validate.</param>
    /// <exception cref="ValidationException">Thrown if any method lacks a corresponding test method.</exception>
    public static void ValidateTestMethodPresence(string solutionDirectory, string projectName)
    {
        Console.WriteLine("Starting test method presence validation...");

        var _projectPath = Path.Combine(solutionDirectory, projectName);

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
                var _match = _methodRegex.Match(_line);
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
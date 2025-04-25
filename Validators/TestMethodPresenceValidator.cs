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
    /// <summary>
    /// Checks that all test classes have at least one [Fact] or [Test] method defined.
    /// </summary>
    /// <param name="solutionDirectory">Root path of the solution to scan.</param>
    public static void ValidateTestMethods(string solutionDirectory)
    {
        Console.WriteLine("🔍 Starting Checking whether tests exist in unit testing projects...");

        var _testFiles = Directory.GetFiles(solutionDirectory, "*.cs", SearchOption.AllDirectories)
            .Where(_file =>
                _file.Contains("Tests") &&
                !_file.Contains("bin") &&
                !_file.Contains("obj"))
            .ToList();

        foreach (var _file in _testFiles)
        {
            var _content = File.ReadAllText(_file);
            var _lines = _content.Split('\n');

            // Skip enums from test validation
            if (_lines.Any(c => c.TrimStart().StartsWith("public enum")))
                continue;

            var _classMatch = Regex.Match(_content, @"public\s+class\s+(\w+)", RegexOptions.Multiline);

            if (_classMatch.Success)
            {
                var _className = _classMatch.Groups[1].Value;
                var _hasTestAttribute = _content.Contains("[Fact]") || _content.Contains("[Test]");

                if (!_hasTestAttribute)
                {
                    Console.WriteLine($"❌ {_file}: Missing test method ([Fact] or [Test]) in class {_className}");
                }
                else
                {
                    Console.WriteLine($"✅ {_file}: Test method found in class {_className}");
                }
            }
        }
    }
}
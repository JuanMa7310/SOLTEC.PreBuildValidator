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
    /// <summary>
    /// Validates test coverage for public logic classes in the project.
    /// </summary>
    /// <param name="solutionDirectory">The base directory of the solution.</param>
    public static void ValidateTestCoverage(string solutionDirectory)
    {
        Console.WriteLine("🔍 Starting Checking test coverage by class...");

        var _sourceFiles = Directory.GetFiles(Path.Combine(solutionDirectory, "SOLTEC.Core"), "*.cs", SearchOption.AllDirectories)
            .Where(_f => !_f.Contains(@"\obj\") && !_f.Contains(@"\bin\"))
            .ToList();

        var _testDirectories = new[]
        {
            Path.Combine(solutionDirectory, "Tests", "SOLTEC.Core.Tests.NuNit"),
            Path.Combine(solutionDirectory, "Tests", "SOLTEC.Core.Tests.xUnit")
        };

        foreach (var _file in _sourceFiles)
        {
            var _className = Path.GetFileNameWithoutExtension(_file);
            var _lines = File.ReadAllLines(_file);
            var _content = File.ReadAllText(_file);

            if (_className == "Program" || _className.Contains("Assembly") || _className.Contains("GlobalUsings"))
                continue;

            var _hasPublicMethod = _lines.Any(_l => _l.Trim().StartsWith("public ") && _l.Contains("("));
            var _hasConstructor = _lines.Any(_l => _l.Contains($"public {_className}("));
            var _hasAssignmentsInConstructor = _lines.Any(_l => _l.Contains("this.") || _l.Contains("g"));
            var _hasOverride = _lines.Any(_l => _l.Trim().StartsWith("public override"));
            var _hasExpressionBody = _lines.Any(_l => _l.Contains("=>"));
            var _onlyProperties = _lines.All(_l => !_l.Contains("(") || _l.Contains(" get;") || _l.Contains(" set;"));

            if (!_hasPublicMethod && !_hasConstructor && !_hasAssignmentsInConstructor &&
                !_hasOverride && !_hasExpressionBody && !_onlyProperties)
                continue;

            Console.WriteLine($"➡️ Reviewing: {_className}");

            var _isLogicClass = _hasPublicMethod || _hasConstructor || _hasAssignmentsInConstructor ||
                                _hasOverride || _hasExpressionBody || !_onlyProperties;

            if (!_isLogicClass)
                continue;

            Console.WriteLine("✅ Detected logic class: " + _className);

            var _testFileExists = _testDirectories
                .SelectMany(_d => Directory.Exists(_d)
                    ? Directory.GetFiles(_d, "*Tests.cs", SearchOption.AllDirectories)
                    : Array.Empty<string>())
                .Any(_testFile => File.ReadAllText(_testFile).Contains(_className));

            if (!_testFileExists)
            {
                Console.WriteLine($"❌ Missing unit test class for: {_className}");
            }
            else
            {
                Console.WriteLine($"✅ Found test class with test method: {_className}Tests");
            }
        }
    }
}
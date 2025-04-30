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
public static partial class TestCoverageValidator
{
    [GeneratedRegex(@"\b(public|protected)\s+(partial\s+)?class\s+(\w+)", RegexOptions.Compiled)]
    private static partial Regex ClassRegex();
    [GeneratedRegex(@"public\s+[^=]+\([^)]*\)", RegexOptions.Compiled)]
    private static partial Regex MethodRegex();
    [GeneratedRegex(@"(public|protected)?\s*enum\s+\w+", RegexOptions.Compiled)]
    private static partial Regex EnumRegex();
    [GeneratedRegex(@"<(.+?)>")]
    private static partial Regex GenericTypeRegex();

    /// <summary>
    /// Validates that logic classes are covered by corresponding unit tests.
    /// </summary>
    /// <param name="solutionDirectory">Root directory of the solution.</param>
    /// <param name="projectFilePath">Directory of the project to validate.</param>
    /// <exception cref="ValidationException">Thrown if any logic class is not covered by a test class.</exception>
    public static void ValidateTestCoverage(string solutionDirectory, string projectFilePath)
    {
        Console.WriteLine("Starting test coverage validation...");

        if (string.IsNullOrWhiteSpace(projectFilePath) || string.IsNullOrEmpty(projectFilePath))
            throw new ArgumentException("ProjectFile Directory can't be null, empty or whitespace.", nameof(projectFilePath));

        var _projectDirectoryPath = Path.GetDirectoryName(projectFilePath);
        var _projectFiles = Directory.GetFiles(_projectDirectoryPath!, "*.cs", SearchOption.AllDirectories)
            .Where(f => !IsGeneratedFile(f))
            .ToList();
        if (_projectFiles.Count == 0)
        {
            throw new ValidationException($"Test coverage validation failed: No .cs files found in '{projectFilePath}'.");
        }
        var _classesToCheck = new List<string>();
        foreach (var _filePath in _projectFiles)
        {
            var _lines = File.ReadAllLines(_filePath);
            for (int i = 0; i < _lines.Length; i++)
            {
                var _line = _lines[i];
                if (EnumRegex().IsMatch(_line)) 
                    continue;
                var _match = ClassRegex().Match(_line);
                if (_match.Success)
                {
                    var _className = _match.Groups[3].Value;
                    if (_className.Contains('<'))
                    {
                        _className = GenericTypeRegex().Replace(_className, m =>
                        {
                            var types = m.Groups[1].Value
                                .Replace(" ", "")       // eliminar espacios
                                .Replace(",", "");      // eliminar comas
                            return types;
                        });
                    }
                    if (_line.Contains(": Exception") || _line.Contains(": ResultException"))
                        continue;
                    // Revisar las siguientes líneas hasta que se cierre la clase para encontrar métodos públicos
                    int _braceCount = 0;
                    bool _hasMethod = false;
                    for (int j = ++i; j < _lines.Length; j++)
                    {
                        if (_lines[j].Contains('{')) 
                            _braceCount++;
                        if (_lines[j].Contains('}')) 
                            _braceCount--;
                        if (_braceCount <= 0) 
                            break;
                        if (MethodRegex().IsMatch(_lines[j]))
                        {
                            _hasMethod = true;
                            i = ++j;
                            break;
                        }
                    }
                    if (_hasMethod)
                    {
                        _classesToCheck.Add(_className);
                    }
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
                    $"Test coverage validation failed: The following logic classes are missing corresponding test classes: {string.Join(", ", _uncoveredClasses)}."
                );
            }
            Console.WriteLine("✅ Test coverage validation passed.");
        }
        else
        {
            Console.WriteLine("No logic classes found to validate.");
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
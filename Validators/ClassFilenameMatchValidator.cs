using SOLTEC.PreBuildValidator.Exceptions;
using System.Text.RegularExpressions;

namespace SOLTEC.PreBuildValidator.Validators;

/// <summary>
/// Validates that each class name matches the filename.
/// </summary>
public static partial class ClassFilenameMatchValidator
{
    [GeneratedRegex(@"\b(public|internal|protected)?\s*(partial\s+)?class\s+(\w+)(<([^>]+)>)?", RegexOptions.Compiled | RegexOptions.IgnoreCase)]
    private static partial Regex ClassRegex();
    [GeneratedRegex(@"//.*")]
    private static partial Regex SingleLine();
    [GeneratedRegex(@"/\*.*?\*/", RegexOptions.Singleline)]
    private static partial Regex MultiLine();
    [GeneratedRegex(@"///.*")]
    private static partial Regex XMLComments();

    /// <summary>
    /// Validates that each class in the source project matches the name of its file.
    /// </summary>
    /// <param name="solutionDirectory">Root directory of the solution.</param>
    /// <param name="projectFilePath">Name of the project and Path to validate.</param>
    /// <exception cref="ValidationException">Thrown when a mismatch is found between filename and class name.</exception>
    public static void ValidateClassNameMatchesFilename(string solutionDirectory, string projectFilePath)
    {
        Console.WriteLine("🔍 Starting validation of class name matching filename...");

        if (string.IsNullOrWhiteSpace(projectFilePath) || string.IsNullOrEmpty(projectFilePath))
            throw new ArgumentException("ProjectFile Directory can't be null, empty or whitespace.", nameof(projectFilePath));

        var _projectDirectoryPath = Path.GetDirectoryName(projectFilePath);
        if (!Directory.Exists(_projectDirectoryPath))
            throw new DirectoryNotFoundException($"Project directory not found: {_projectDirectoryPath}");

        List<string> _projectFiles = [.. 
            Directory
            .GetFiles(_projectDirectoryPath, "*.cs", SearchOption.AllDirectories)
            .Where(_f =>
                !IsGeneratedFile(_f) &&
                !_f.Contains(@"\obj\") &&
                !_f.Contains(@"\bin\") &&
                !_f.Contains(@"\TestResults\") &&
                !_f.Contains(@"\.vs\") &&
                !_f.Contains(@"Temporary") &&
                !_f.Contains(@"FileManagement.cs"))
            ];

        foreach (var _file in _projectFiles)
        {
            var _filename = Path.GetFileNameWithoutExtension(_file);
            var _content = File.ReadAllText(_file);
            var _cleanedContent = RemoveComments(_content);
            var _match = ClassRegex().Match(_cleanedContent);
            if (_match.Success)
            {
                var _className = _match.Groups[3].Value;
                var _generics = _match.Groups[5].Value;
                if (!string.IsNullOrWhiteSpace(_generics))
                {
                    var _genericSuffix = string.Concat(_generics.Split(',').Select(p => p.Trim()));
                    _className += _genericSuffix;
                }
                if (_filename != _className)
                {
                    throw new ValidationException($"Class name '{_className}' does not match filename '{_filename}' in {_file}.");
                }
            }
        }

        Console.WriteLine("✅ Class name and filename match validation passed.");
    }
    /// <summary>
    /// Removes comments from source code content.
    /// </summary>
    private static string RemoveComments(string source)
    {
        source = SingleLine().Replace(source, "");  // Single-line
        source = MultiLine().Replace(source, "");   // Multi-line
        source = XMLComments().Replace(source, ""); // XML comments
        return source;
    }

    private static bool IsGeneratedFile(string filePath)
    {
        var _fileName = Path.GetFileName(filePath);
        return _fileName.EndsWith(".Designer.cs", StringComparison.OrdinalIgnoreCase) ||
               _fileName.EndsWith(".g.cs", StringComparison.OrdinalIgnoreCase) ||
               _fileName.EndsWith(".AssemblyInfo.cs", StringComparison.OrdinalIgnoreCase);
    }

}

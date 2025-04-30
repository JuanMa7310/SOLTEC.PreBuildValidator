using SOLTEC.PreBuildValidator.Exceptions;
using System.Text.RegularExpressions;

namespace SOLTEC.PreBuildValidator.Validators;

/// <summary>
/// Scans all C# files in a given directory to detect unresolved TODO or FIXME comments.
/// </summary>
/// <example>
/// Example usage:
/// <![CDATA[
/// TodoFixmeValidator.ValidateTodoFixme("../MySolution");
/// ]]>
/// </example>
public static partial class TodoFixmeValidator
{
    [GeneratedRegex(@"//\s*(TODO|FIXME)\b", RegexOptions.Compiled | RegexOptions.IgnoreCase)]
    private static partial Regex TodoFixmeRegex();

    /// <summary>
    /// Validates the absence of TODO and FIXME comments in source code files.
    /// </summary>
    /// <param name="projectFilePath">Name of the main project.</param>
    /// <exception cref="ValidationException">Thrown if any TODO or FIXME comment is found.</exception>
    public static void ValidateTodoFixme(string projectFilePath)
    {
        Console.WriteLine("Starting TODO/FIXME comments validation...");

        if (string.IsNullOrWhiteSpace(projectFilePath) || string.IsNullOrEmpty(projectFilePath))
            throw new ArgumentException("ProjectFile Directory can't be null, empty or whitespace.", nameof(projectFilePath));

        var _projectPath = Path.GetDirectoryName(projectFilePath);

        if (!Directory.Exists(_projectPath))
        {
            throw new ValidationException($"TODO/FIXME validation failed: Project path '{_projectPath}' not found.");
        }

        var _projectFiles = Directory.GetFiles(_projectPath, "*.cs", SearchOption.AllDirectories)
            .Where(f => !IsGeneratedFile(f))
            .ToList();

        if (_projectFiles.Count == 0)
        {
            Console.WriteLine("No source files found to validate TODO/FIXME.");
            return;
        }

        var _filesWithTodoFixme = new List<string>();
        foreach (var _filePath in _projectFiles)
        {
            var _lines = File.ReadLines(_filePath);

            if (_lines.Any(line => TodoFixmeRegex().IsMatch(line)))
            {
                _filesWithTodoFixme.Add(Path.GetFileName(_filePath));
            }
        }

        if (_filesWithTodoFixme.Count != 0)
        {
            throw new ValidationException(
                $"TODO/FIXME validation failed: The following files contain TODO or FIXME comments: {string.Join(", ", _filesWithTodoFixme)}."
            );
        }

        Console.WriteLine("TODO/FIXME validation passed.");
    }

    private static bool IsGeneratedFile(string filePath)
    {
        var _fileName = Path.GetFileName(filePath);
        return _fileName.EndsWith(".Designer.cs", StringComparison.OrdinalIgnoreCase) ||
               _fileName.EndsWith(".g.cs", StringComparison.OrdinalIgnoreCase) ||
               _fileName.EndsWith(".AssemblyInfo.cs", StringComparison.OrdinalIgnoreCase);
    }
}
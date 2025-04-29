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
public static class TodoFixmeValidator
{
    ///// <summary>
    ///// Validates the presence of TODO or FIXME comments in source files, marking them as pending issues.
    ///// </summary>
    ///// <param name="solutionDirectory">Root path to the solution where files should be checked.</param>
    //public static void ValidateTodoFixme(string solutionDirectory)
    //{
    //    Console.WriteLine("🔍 Starting Checking TODO / FIXME...");

    //    var _csFiles = Directory.GetFiles(solutionDirectory, "*.cs", SearchOption.AllDirectories);

    //    foreach (var _file in _csFiles)
    //    {
    //        Console.WriteLine($"📝 Checking TODO/FIXME in file: {_file}...");
    //        var _lines = File.ReadAllLines(_file);

    //        for (var _i = 0; _i < _lines.Length; _i++)
    //        {
    //            if (_lines[_i].Contains("TODO") || _lines[_i].Contains("FIXME"))
    //            {
    //                Console.WriteLine($"❌ {_file}: Unresolved TODO/FIXME found at line {_i + 1}: {_lines[_i].Trim()}");
    //            }
    //        }
    //    }
    //}
    private static readonly Regex _todoFixmeRegex = new(@"//\s*(TODO|FIXME)\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    /// <summary>
    /// Validates the absence of TODO and FIXME comments in source code files.
    /// </summary>
    /// <param name="solutionDirectory">Root directory of the solution.</param>
    /// <param name="projectName">Name of the main project.</param>
    /// <exception cref="ValidationException">Thrown if any TODO or FIXME comment is found.</exception>
    public static void ValidateTodoFixme(string solutionDirectory, string projectName)
    {
        Console.WriteLine("Starting TODO/FIXME comments validation...");

        var _projectPath = Path.Combine(solutionDirectory, projectName);

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

            if (_lines.Any(line => _todoFixmeRegex.IsMatch(line)))
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
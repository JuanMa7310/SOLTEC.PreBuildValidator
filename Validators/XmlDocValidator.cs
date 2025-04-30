using SOLTEC.PreBuildValidator.Exceptions;
using System.Text.RegularExpressions;

namespace SOLTEC.PreBuildValidator.Validators;

/// <summary>
/// Validates that all public classes and public methods contain proper XML documentation.
/// </summary>
/// <example>
/// Example usage:
/// <![CDATA[
/// XmlDocValidator.ValidateXmlDocumentation("../MySolution");
/// ]]>
/// </example>
public static class XmlDocValidator
{
    private static readonly Regex _publicProtectedMemberRegex = new(@"\b(public|protected)\s+(class|interface|struct|enum|delegate|void|\w+)\s+\w+", RegexOptions.Compiled);
    private static readonly Regex _xmlDocCommentRegex = new(@"^\s*///", RegexOptions.Compiled);

    /// <summary>
    /// Validates that XML documentation exists for public/protected classes, methods, and properties.
    /// </summary>
    /// <param name="solutionDirectory">Root directory of the solution.</param>
    /// <param name="projectFilePath">Name of the main project.</param>
    /// <exception cref="ValidationException">Thrown if missing XML documentation is found.</exception>
    public static void ValidateXmlDocumentation(string solutionDirectory, string projectFilePath)
    {
        Console.WriteLine("Starting XML documentation validation...");

        var _projectPath = Path.GetDirectoryName(projectFilePath);

        if (!Directory.Exists(_projectPath))
        {
            throw new ValidationException($"XML documentation validation failed: Project path '{_projectPath}' not found.");
        }

        var _projectFiles = Directory.GetFiles(_projectPath, "*.cs", SearchOption.AllDirectories)
            .Where(f => !IsGeneratedFile(f))
            .ToList();

        if (_projectFiles.Count == 0)
        {
            Console.WriteLine("No source files found to validate XML documentation.");
            return;
        }

        var _membersMissingDoc = new List<string>();

        foreach (var _filePath in _projectFiles)
        {
            var _lines = File.ReadAllLines(_filePath);

            for (int _length = 0; _length < _lines.Length; _length++)
            {
                var _line = _lines[_length];

                if (_publicProtectedMemberRegex.IsMatch(_line))
                {
                    bool _hasXmlDoc = false;

                    // Look backwards to see if there's a "///" comment immediately above
                    for (int _j = _length - 1; _j >= 0; _j--)
                    {
                        var _previousLine = _lines[_j].Trim();

                        if (string.IsNullOrWhiteSpace(_previousLine))
                            continue;

                        if (_xmlDocCommentRegex.IsMatch(_previousLine))
                        {
                            _hasXmlDoc = true;
                            break;
                        }

                        // If we find a non-comment line before a "///", assume missing doc
                        break;
                    }

                    if (!_hasXmlDoc)
                    {
                        _membersMissingDoc.Add($"{Path.GetFileName(_filePath)}: Line {_length + 1} - {_line.Trim()}");
                    }
                }
            }
        }

        if (_membersMissingDoc.Count != 0)
        {
            throw new ValidationException(
                $"XML documentation validation failed: Missing documentation for the following members:\n{string.Join("\n", _membersMissingDoc)}"
            );
        }

        Console.WriteLine("XML documentation validation passed.");
    }

    private static bool IsGeneratedFile(string filePath)
    {
        var _fileName = Path.GetFileName(filePath);
        return _fileName.EndsWith(".Designer.cs", StringComparison.OrdinalIgnoreCase) ||
               _fileName.EndsWith(".g.cs", StringComparison.OrdinalIgnoreCase) ||
               _fileName.EndsWith(".AssemblyInfo.cs", StringComparison.OrdinalIgnoreCase);
    }
}
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
    /// <summary>
    /// Performs XML documentation validation across all .cs files in the given solution directory.
    /// </summary>
    /// <param name="solutionDirectory">Path to the root directory of the solution.</param>
    public static void ValidateXmlDocumentation(string solutionDirectory)
    {
        Console.WriteLine("🔍 Starting Checking XML documentation...");

        var _csFiles = Directory.GetFiles(solutionDirectory, "*.cs", SearchOption.AllDirectories);

        foreach (var _file in _csFiles)
        {
            if (Path.GetExtension(_file).Equals(".md", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var _lines = File.ReadAllLines(_file);
            for (var _i = 0; _i < _lines.Length; _i++)
            {
                var _line = _lines[_i].Trim();

                // Check for public class or struct or interface
                if (Regex.IsMatch(_line, @"^(public\s+)?(class|struct|interface)\s+\w"))
                {
                    if (_i == 0 || !_lines[_i - 1].TrimStart().StartsWith("///"))
                    {
                        Console.WriteLine($"❌ {_file}: Public class/interface/struct missing XML documentation at line {_i + 1}");
                    }
                }

                // Check for public method
                if (Regex.IsMatch(_line, @"^public\s+(static\s+)?(\w+[\<\>\[\]]*\s+)+\w+\s*\("))
                {
                    if (_i == 0 || !_lines[_i - 1].TrimStart().StartsWith("///"))
                    {
                        Console.WriteLine($"❌ {_file}: Public method missing XML documentation at line {_i + 1}");
                    }
                }

                // Optional: check for duplicate or malformed XML comments
                if (_line.StartsWith("///") && !_line.Contains("<summary>") && !_line.Contains("</summary>") && !_line.Contains("<") && !_line.Contains(">"))
                {
                    Console.WriteLine($"⚠️ {_file}: Possible malformed XML documentation at line {_i + 1}");
                }
            }
        }
    }
}
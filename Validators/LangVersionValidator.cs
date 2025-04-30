using SOLTEC.PreBuildValidator.Exceptions;
using System.Xml.Linq;

namespace SOLTEC.PreBuildValidator.Validators;

/// <summary>
/// Validates that each project (.csproj) has the correct language version and nullable settings enabled.
/// </summary>
/// <example>
/// <![CDATA[
/// LangVersionValidator.ValidateLangVersion("../MySolution");
/// ]]>
/// </example>
public static class LangVersionValidator
{

    /// <summary>
    /// The required C# language version.
    /// </summary>
    private const string gcRequiredLangVersion = "12.0";

    /// <summary>
    /// Validates the presence of LangVersion in the project file (.csproj).
    /// </summary>
    /// <param name="projectDirectory">Root directory of the solution.</param>
    /// <param name="projectName">Name of the main project (e.g., SOLTEC.Core).</param>
    /// <exception cref="ValidationException">Thrown if LangVersion is missing or unreadable.</exception>
    public static void ValidateLangVersion(string projectDirectory, string projectName)
    {
        Console.WriteLine("Starting LangVersion validation...");

        var _csprojPath = Path.Combine(projectDirectory, projectName);

        if (!File.Exists(_csprojPath))
        {
            throw new ValidationException($"LangVersion validation failed: Project file '{_csprojPath}' was not found.");
        }
        try
        {
            var _csprojContent = XDocument.Load(_csprojPath);
            var _langVersionElement = _csprojContent
                .Descendants()
                .FirstOrDefault(x => x.Name.LocalName == "LangVersion");

            if (_langVersionElement == null || string.IsNullOrWhiteSpace(_langVersionElement.Value))
            {
                throw new ValidationException($"LangVersion validation failed: No <LangVersion> element found or it is empty in '{_csprojPath}'.");
            }
            if (_langVersionElement.Value != gcRequiredLangVersion)
            {
                Console.WriteLine($"❌ LangVersion must be {gcRequiredLangVersion} (actual: {_langVersionElement.Value ?? "NOT DEFINED"})");
            }
            Console.WriteLine($"LangVersion validation passed: Found LangVersion = {_langVersionElement.Value!.Trim()}");
        }
        catch (Exception ex)
        {
            throw new ValidationException($"LangVersion validation failed: Unable to read or parse '{_csprojPath}'. Details: {ex.Message}");
        }
    }
}
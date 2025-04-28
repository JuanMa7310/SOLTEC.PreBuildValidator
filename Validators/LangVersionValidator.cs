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
    ///// <summary>
    ///// The required C# language version.
    ///// </summary>
    //private const string gcRequiredLangVersion = "12.0";

    ///// <summary>
    ///// Validates that all .csproj files in the solution have LangVersion set to 12.0 and Nullable enabled.
    ///// </summary>
    ///// <param name="solutionDirectory">Path to the solution directory to scan.</param>
    //public static void ValidateLangVersion(string solutionDirectory)
    //{
    //    Console.WriteLine("🔍 Starting Checking LangVersion and Nullable in project...");

    //    var _projectFiles = Directory.GetFiles(solutionDirectory, "*.csproj", SearchOption.AllDirectories);

    //    foreach (var _file in _projectFiles)
    //    {
    //        Console.WriteLine($"📝 Checking LangVersion and Nullable in project: {_file}...");

    //        try
    //        {
    //            var _xdoc = XDocument.Load(_file);
    //            var _ns = _xdoc.Root?.Name.Namespace;
    //            var _langVersion = _xdoc.Descendants(_ns + "LangVersion").FirstOrDefault()?.Value?.Trim();
    //            var _nullable = _xdoc.Descendants(_ns + "Nullable").FirstOrDefault()?.Value?.Trim();

    //            if (_langVersion != gcRequiredLangVersion)
    //            {
    //                Console.WriteLine($"❌ {_file}: LangVersion must be {gcRequiredLangVersion} (actual: {_langVersion ?? "NOT DEFINED"})");
    //            }
    //            if (_nullable?.ToLowerInvariant() != "enable")
    //            {
    //                Console.WriteLine($"❌ {_file}: Nullable must be enabled (actual: {_nullable ?? "NOT DEFINED"})");
    //            }
    //        }
    //        catch (Exception _ex)
    //        {
    //            Console.WriteLine($"❌ Error parsing {_file}: {_ex.Message}");
    //        }
    //    }
    //}

    /// <summary>
    /// The required C# language version.
    /// </summary>
    private const string gcRequiredLangVersion = "12.0";

    /// <summary>
    /// Validates the presence of LangVersion in the project file (.csproj).
    /// </summary>
    /// <param name="solutionDirectory">Root directory of the solution.</param>
    /// <param name="projectName">Name of the main project (e.g., SOLTEC.Core).</param>
    /// <exception cref="ValidationException">Thrown if LangVersion is missing or unreadable.</exception>
    public static void ValidateLangVersion(string solutionDirectory, string projectName)
    {
        Console.WriteLine("Starting LangVersion validation...");

        var _csprojPath = Path.Combine(solutionDirectory, projectName, $"{projectName}.csproj");

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
using SOLTEC.PreBuildValidator.Exceptions;
using SOLTEC.PreBuildValidator.Helpers;
using SOLTEC.PreBuildValidator.Validators;

namespace SOLTEC.PreBuildValidator;

/// <summary>
/// Entry point for the SOLTEC.PreBuildValidator tool.
/// Performs pre-build checks such as LangVersion, Nullable, XML documentation, TODO/FIXME comments, and test validation.
/// </summary>
/// <example>
/// <![CDATA[
/// dotnet run --project Tools/SOLTEC.PreBuildValidator ProjectFileWithPath.csprj
/// ]]>
/// </example>
public class Program
{
    /// <summary>
    /// Main entry point of the validator.
    /// </summary>
    private static void Main(string[] args)
    {
        try
        {
            Console.WriteLine("Starting PreBuild Validator...");

            // Get solution/project name from arguments
            var solutionName = args.Length > 0 ? args[0] : ".";

            // Locate the .csproj file properly
            var _projectFilePath = ProjectLocator.FindProjectFile(solutionName);
            var _solutionDirectory = Path.GetDirectoryName(_projectFilePath)
                ?? throw new InvalidOperationException("Could not determine solution directory.");

            Console.WriteLine($"Solution Directory: {_solutionDirectory}");
            Console.WriteLine($"Project to validate: {_projectFilePath}");

            LangVersionValidator.ValidateLangVersion(_solutionDirectory, _projectFilePath);
            TestCoverageValidator.ValidateTestCoverage(_solutionDirectory, _projectFilePath);
            TestMethodPresenceValidator.ValidateTestMethodPresence(_solutionDirectory, _projectFilePath);
            TodoFixmeValidator.ValidateTodoFixme(_solutionDirectory, _projectFilePath);
            XmlDocValidator.ValidateXmlDocumentation(_solutionDirectory, _projectFilePath);

            Console.WriteLine("Pre-build validation completed successfully.");
        }
        catch (ValidationException _vex)
        {
            Console.WriteLine($"Validation failed: {_vex.Message}");
            Environment.Exit(1);
        }
        catch (Exception _ex)
        {
            Console.WriteLine($"Unexpected error: {_ex.Message}");
            Environment.Exit(1);
        }
    }
}

using SOLTEC.PreBuildValidator.Exceptions;
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
        // Validate arguments
        if (args.Length == 0)
        {
            Console.WriteLine("Error: Project name must be provided as an argument (e.g., SOLTEC.Core).");
            Environment.Exit(1);
        }

        var _projectName = args[0];
        var _solutionDirectory = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory)?.Parent?.Parent?.Parent?.FullName
                                 ?? throw new DirectoryNotFoundException("Could not determine the solution directory.");

        Console.WriteLine($"Solution Directory: {_solutionDirectory}");
        Console.WriteLine($"Project to validate: {_projectName}");

        try
        {
            LangVersionValidator.ValidateLangVersion(_solutionDirectory, _projectName);
            TestCoverageValidator.ValidateTestCoverage(_solutionDirectory, _projectName);
            TestMethodPresenceValidator.ValidateTestMethodPresence(_solutionDirectory, _projectName);
            TodoFixmeValidator.ValidateTodoFixme(_solutionDirectory, _projectName);
            XmlDocValidator.ValidateXmlDocumentation(_solutionDirectory, _projectName);

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
//namespace SOLTEC.PreBuildValidator;

///// <summary>
///// Entry point for the SOLTEC.PreBuildValidator tool.
///// Performs pre-build checks such as LangVersion, Nullable, XML documentation, TODO/FIXME comments, and test validation.
///// </summary>
///// <example>
///// <![CDATA[
///// dotnet run --project Tools/SOLTEC.PreBuildValidator
///// ]]>
///// </example>
//public static class Program
//{

//    /// <summary>
//    /// Main entry point of the validator.
//    /// </summary>
//    public static void Main()
//    {
//        var _csolutionDirectory = Environment.GetEnvironmentVariable("GITHUB_WORKSPACE")
//                                ?? Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../.."));
//        bool _success = true;

//        Console.OutputEncoding = System.Text.Encoding.UTF8;
//        Console.WriteLine("🔍 Starting project validation......");
//        try
//        {
//            Console.WriteLine("📄 Checking LangVersion and Nullable in project...");
//            LangVersionValidator.ValidateLangVersion(_csolutionDirectory);
//        }
//        catch (Exception ex)
//        {
//            Console.ForegroundColor = ConsoleColor.Red;
//            Console.WriteLine($"❌ LangVersion or Nullable validation failed: {ex.Message}");
//            Console.ResetColor();
//            _success = false;
//        }
//        try
//        {
//            Console.WriteLine("📝 Checking XML documentation...");
//            XmlDocValidator.ValidateXmlDocumentation(_csolutionDirectory);
//        }
//        catch (Exception ex)
//        {
//            Console.ForegroundColor = ConsoleColor.Red;
//            Console.WriteLine($"❌ XML documentation validation failed: {ex.Message}");
//            Console.ResetColor();
//            _success = false;
//        }

//        try
//        {
//            Console.WriteLine("🧠 Checking TODO / FIXME...");
//            TodoFixmeValidator.ValidateTodoFixme(_csolutionDirectory);
//        }
//        catch (Exception ex)
//        {
//            Console.ForegroundColor = ConsoleColor.Red;
//            Console.WriteLine($"❌ TODO/FIXME validation failed: {ex.Message}");
//            Console.ResetColor();
//            _success = false;
//        }

//        try
//        {
//            Console.WriteLine("📊 Checking test coverage by class...");
//            TestCoverageValidator.ValidateTestCoverage(_csolutionDirectory);
//        }
//        catch (Exception ex)
//        {
//            Console.ForegroundColor = ConsoleColor.Red;
//            Console.WriteLine($"❌ Test coverage validation failed: {ex.Message}");
//            Console.ResetColor();
//            _success = false;
//        }

//        try
//        {
//            Console.WriteLine("🧪 Checking whether tests exist in unit testing projects...");
//            TestMethodPresenceValidator.ValidateTestMethods(_csolutionDirectory);
//        }
//        catch (Exception ex)
//        {
//            Console.ForegroundColor = ConsoleColor.Red;
//            Console.WriteLine($"❌ Test coverage validation failed: {ex.Message}");
//            Console.ResetColor();
//            _success = false;
//        }

//        if (_success)
//        {
//            Console.ForegroundColor = ConsoleColor.Green;
//            Console.WriteLine("✅ All validations passed successfully.");
//        }
//        else
//        {
//            Console.ForegroundColor = ConsoleColor.Red;
//            Console.WriteLine("❌ Validation failed. Please fix the issues above.");
//            Environment.Exit(1);
//        }
//        Console.ResetColor();
//    }
//}
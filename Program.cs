using SOLTEC.PreBuildValidator.Validators;

namespace SOLTEC.PreBuildValidator;

/// <summary>
/// Entry point for the SOLTEC.PreBuildValidator tool.
/// Performs pre-build checks such as LangVersion, Nullable, XML documentation, TODO/FIXME comments, and test validation.
/// </summary>
/// <example>
/// <![CDATA[
/// dotnet run --project Tools/SOLTEC.PreBuildValidator
/// ]]>
/// </example>
public static class Program
{

    /// <summary>
    /// Main entry point of the validator.
    /// </summary>
    public static void Main()
    {
        var _csolutionDirectory = Environment.GetEnvironmentVariable("GITHUB_WORKSPACE")
                                ?? Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../.."));
        bool _success = true;

        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.WriteLine("🔍 Starting project validation......");
        try
        {
            Console.WriteLine("📄 Checking LangVersion and Nullable in project...");
            LangVersionValidator.ValidateLangVersion(_csolutionDirectory);
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"❌ LangVersion or Nullable validation failed: {ex.Message}");
            Console.ResetColor();
            _success = false;
        }
        try
        {
            Console.WriteLine("📝 Checking XML documentation...");
            XmlDocValidator.ValidateXmlDocumentation(_csolutionDirectory);
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"❌ XML documentation validation failed: {ex.Message}");
            Console.ResetColor();
            _success = false;
        }

        try
        {
            Console.WriteLine("🧠 Checking TODO / FIXME...");
            TodoFixmeValidator.ValidateTodoFixme(_csolutionDirectory);
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"❌ TODO/FIXME validation failed: {ex.Message}");
            Console.ResetColor();
            _success = false;
        }

        try
        {
            Console.WriteLine("📊 Checking test coverage by class...");
            TestCoverageValidator.ValidateTestCoverage(_csolutionDirectory);
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"❌ Test coverage validation failed: {ex.Message}");
            Console.ResetColor();
            _success = false;
        }

        try
        {
            Console.WriteLine("🧪 Checking whether tests exist in unit testing projects...");
            TestMethodPresenceValidator.ValidateTestMethods(_csolutionDirectory);
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"❌ Test coverage validation failed: {ex.Message}");
            Console.ResetColor();
            _success = false;
        }

        if (_success)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("✅ All validations passed successfully.");
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("❌ Validation failed. Please fix the issues above.");
            Environment.Exit(1);
        }
        Console.ResetColor();
    }
}
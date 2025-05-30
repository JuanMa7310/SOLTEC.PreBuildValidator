﻿using SOLTEC.PreBuildValidator.Exceptions;
using SOLTEC.PreBuildValidator.Helpers;
using SOLTEC.PreBuildValidator.Validators;
using System.Text;

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
        Console.OutputEncoding = Encoding.UTF8;
        try
        {
            var _projectName = args.Length > 0 ? args[0] : throw new ArgumentException("Missing required argument: project name");

            Console.WriteLine($"Starting PreBuild Validator {_projectName}...");

            // Get solution/project name from arguments
            var _solutionDirectory = ProjectLocator.FindSolutionDirectory();
            var _projectFilePath = ProjectLocator.FindProjectFile(_solutionDirectory, _projectName);

            Console.WriteLine($"Solution Directory: {_solutionDirectory}");
            Console.WriteLine($"Project Directory: {_projectFilePath}");

            LangVersionValidator.ValidateLangVersion(_solutionDirectory, _projectFilePath);
            ClassFilenameMatchValidator.ValidateClassNameMatchesFilename(_solutionDirectory, _projectFilePath);
            TestCoverageValidator.ValidateTestCoverage(_solutionDirectory, _projectFilePath);
            TestMethodPresenceValidator.ValidateTestMethodPresence(_solutionDirectory, _projectFilePath);
            TodoFixmeValidator.ValidateTodoFixme(_projectFilePath);
            XmlDocValidator.ValidateXmlDocumentation(_projectFilePath);

            Console.WriteLine($"Pre-build validation {_projectName} completed successfully.");
        }
        catch (ValidationException _vex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"✖ Pre-build validation failed: {_vex.Message}");
            Console.ResetColor();
            Environment.Exit(1);
        }
        catch (Exception _ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Unexpected error: {_ex.Message}");
            Console.ResetColor();
            Environment.Exit(1);
        }
    }
}

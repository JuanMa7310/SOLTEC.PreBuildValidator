using System.ComponentModel.DataAnnotations;

namespace SOLTEC.PreBuildValidator.Helpers;

/// <summary>
/// Helper class to locate the project file (.csproj) based on the solution name provided.
/// </summary>
public static class ProjectLocator
{
    /// <summary>
    /// Finds the path to the .csproj file based on the solution/project name or the current directory.
    /// </summary>
    /// <param name="solutionName">Name of the solution/project, or "." to detect automatically.</param>
    /// <returns>Full path to the project file.</returns>
    /// <exception cref="ValidationException">Thrown if the project file cannot be determined.</exception>
    public static string FindProjectFile(string solutionName)
    {
        var solutionDirectory = Directory.GetCurrentDirectory();

        if (string.IsNullOrWhiteSpace(solutionName) || solutionName == ".")
        {
            // Auto-detect project file in current directory
            var csprojFiles = Directory.GetFiles(solutionDirectory, "*.csproj", SearchOption.TopDirectoryOnly);

            if (csprojFiles.Length == 0)
            {
                throw new ValidationException("No project (.csproj) file found in the current directory.");
            }
            if (csprojFiles.Length > 1)
            {
                throw new ValidationException("Multiple project (.csproj) files found. Please specify the project name explicitly.");
            }

            return csprojFiles[0]; // Return the only .csproj found
        }
        else
        {
            var projectDirectory = Path.Combine(solutionDirectory, solutionName);
            var projectFilePath = Path.Combine(projectDirectory, $"{solutionName}.csproj");

            if (!File.Exists(projectFilePath))
            {
                throw new ValidationException($"Project file '{projectFilePath}' not found.");
            }

            return projectFilePath;
        }
    }
}

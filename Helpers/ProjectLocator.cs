namespace SOLTEC.PreBuildValidator.Helpers;

/// <summary>
/// Helper class to locate the project file (.csproj) based on the solution name provided.
/// </summary>
public static class ProjectLocator
{
    /// <summary>
    /// Finds the .csproj path of the target project assuming the following structure:
    /// Attempts to find the full path to the .csproj file matching the given project or solution name.
    /// ROOT/
    /// ├── SOLUTION/
    /// │   └── PROJECT/
    /// │       └── PROJECT.csproj
    /// </summary>
    /// <param name="solutionDirectory">The root directory where the repository was cloned.</param>
    /// <param name="projectName">The name of the project.</param>
    /// <returns>The full path to the project file.</returns>
    /// <exception cref="Exception">Thrown if the project file cannot be found.</exception>
    public static string FindProjectFile(string solutionDirectory, string projectName)
    {
        Console.WriteLine($"> Searching for project '{projectName}.csproj' under: {solutionDirectory}");

        var _projectFilePath = Directory
            .GetFiles(solutionDirectory, $"{projectName}.csproj", SearchOption.AllDirectories)
            .FirstOrDefault();

        if (string.IsNullOrWhiteSpace(_projectFilePath))
        {
            throw new Exception($"Project file '{projectName}.csproj' was not found in: {solutionDirectory}");
        }

        return _projectFilePath;
    }

    /// <summary>
    /// Attempts to locate the directory containing the .sln file by walking up from the execution path.
    /// </summary>
    /// <returns>The full path to the solution directory.</returns>
    /// <exception cref="Exception">Thrown if no .sln file is found.</exception>
    public static string FindSolutionDirectory()
    {
        var _current = Directory.GetCurrentDirectory();

        while (!string.IsNullOrEmpty(_current))
        {
            var _slnFile = Directory.GetFiles(_current, "*.sln", SearchOption.TopDirectoryOnly).FirstOrDefault();
            if (!string.IsNullOrEmpty(_slnFile))
            {
                return _current;
            }
            _current = Directory.GetParent(_current)?.FullName;
        }

        throw new Exception("Solution (.sln) file not found from current execution path.");
    }
}

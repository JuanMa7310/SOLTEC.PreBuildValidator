namespace SOLTEC.PreBuildValidator.Validators;

/// <summary>
/// Scans all C# files in a given directory to detect unresolved TODO or FIXME comments.
/// </summary>
/// <example>
/// Example usage:
/// <![CDATA[
/// TodoFixmeValidator.ValidateTodoFixme("../MySolution");
/// ]]>
/// </example>
public static class TodoFixmeValidator
{
    /// <summary>
    /// Validates the presence of TODO or FIXME comments in source files, marking them as pending issues.
    /// </summary>
    /// <param name="solutionDirectory">Root path to the solution where files should be checked.</param>
    public static void ValidateTodoFixme(string solutionDirectory)
    {
        Console.WriteLine("🔍 Starting Checking TODO / FIXME...");

        var _csFiles = Directory.GetFiles(solutionDirectory, "*.cs", SearchOption.AllDirectories);

        foreach (var _file in _csFiles)
        {
            Console.WriteLine($"📝 Checking TODO/FIXME in file: {_file}...");
            var _lines = File.ReadAllLines(_file);

            for (var _i = 0; _i < _lines.Length; _i++)
            {
                if (_lines[_i].Contains("TODO") || _lines[_i].Contains("FIXME"))
                {
                    Console.WriteLine($"❌ {_file}: Unresolved TODO/FIXME found at line {_i + 1}: {_lines[_i].Trim()}");
                }
            }
        }
    }
}
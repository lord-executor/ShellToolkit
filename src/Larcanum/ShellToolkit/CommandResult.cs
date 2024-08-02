namespace Larcanum.ShellToolkit;

public class CommandResult
{
    public int ExitCode { get; init; }
    public string? Output { get; init; }
    public string? Error { get; init; }
}

using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Larcanum.ShellToolkit;

public partial class Command : ICommand
{
    /// <summary>
    /// Matches one of two basic patterns
    /// - Any number of non-greedy consecutive non-space characters except for a single double quote character unless
    ///   that double quote character is immediately preceded by a back-slash, followed by any number of spaces or the
    ///   end of the input sequence (Group 1)
    /// - A double quote character followed by any number of non-greedy characters up until the next double quote
    ///   character which is not immediately preceded by a back-slash, followed by any number of spaces or the end of
    ///   the input sequence (Group 2)
    /// </summary>
    [GeneratedRegex(@"((?:[^""]|\\"")+?)(?:\s+|$)|""(.*?)(?<!\\)""(?:\s+|$)")]
    private static partial Regex ArgumentExpression { get; }

    [GeneratedRegex(@"\\([""])")]
    private static partial Regex EscapeSequenceExpression { get; }

    public static Command Create(string cmd, IEnumerable<string> cmdArgs)
    {
        return new Command(cmd, cmdArgs);
    }

    public static Command Create(string cmd, string cmdArgs)
    {
        return new Command(cmd, cmdArgs.Split(" ", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries));
    }

    public static Command Create(string cmd)
    {
        var args = ArgumentExpression
            .Matches(cmd).Select(m => m.Groups[1].Success ? m.Groups[1].Value : m.Groups[2].Value)
            .Select(val => EscapeSequenceExpression.Replace(val, "$1"))
            .ToArray();
        var commandName = args[0];
        return new Command(commandName, args[1..]);
    }

    private readonly string _cmd;
    private readonly IEnumerable<string> _cmdArgs;
    private readonly string _workingDir = Environment.CurrentDirectory;

    private Command(string cmd, IEnumerable<string> cmdArgs)
    {
        _cmd = cmd;
        _cmdArgs = cmdArgs;
    }

    public ProcessStartInfo ToProcessStartInfo()
    {
        var info = new ProcessStartInfo
        {
            FileName = _cmd,
            WorkingDirectory = _workingDir,
        };

        foreach (var arg in _cmdArgs)
        {
            info.ArgumentList.Add(arg);
        }

        return info;
    }

    public override string ToString()
    {
        return $"{_cmd} {string.Join(" ", _cmdArgs)}";
    }
}

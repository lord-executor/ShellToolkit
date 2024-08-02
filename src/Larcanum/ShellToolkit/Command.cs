using System.Diagnostics;

namespace Larcanum.ShellToolkit;

public class Command : ICommand
{
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
        var args = cmd.Split(" ", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).ToList();
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

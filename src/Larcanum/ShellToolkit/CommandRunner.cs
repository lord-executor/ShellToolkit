using System.Diagnostics;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Larcanum.ShellToolkit;

public class CommandRunner : ICommandRunner
{
    public static CommandRunner Create()
    {
        return new CommandRunner(new Settings(), new NullLogger<CommandRunner>());
    }

    public static CommandRunner Create(Settings settings)
    {
        return new CommandRunner(settings, new NullLogger<CommandRunner>());
    }

    public static CommandRunner Create(ILogger<CommandRunner> logger)
    {
        return new CommandRunner(new Settings(), logger);
    }

    private readonly Settings _settings;
    private readonly ILogger _logger;

    public CommandRunner(Settings settings, ILogger<CommandRunner> logger)
        : this(settings, (ILogger)logger)
    {
    }

    public CommandRunner(Settings settings, ILogger logger)
    {
        _settings = settings;
        _logger = logger;
    }

    public Task<int> ExecAsync(ICommand cmd, CancellationToken ct = default)
    {
        _logger.LogInformation("[exec]: {cmd}", cmd);

        var info = cmd.ToProcessStartInfo();
        info.UseShellExecute = false;
        // This is important for "forwarding" access to the console to the child process and even though
        // false is the default value, we want to make this very explicit here. This allows tools with fancy console
        // UI like menus and progress bars to work when created as child processes.
        info.CreateNoWindow = false;

        using var process = Process.Start(info);
        process?.WaitForExit();

        return Task.FromResult(process?.ExitCode ?? _settings.NoProcessSpawnedExitCode);
    }

    public async Task<int> ExecAsync(IPipeline pipeline, CancellationToken ct = default)
    {
        _logger.LogInformation("[exec]: {pipeline}", pipeline);
        return (await pipeline.Run(OutputMode.Default, ct)).ExitCode;
    }

    public async Task<CommandResult> CaptureAsync(ICommand cmd, CancellationToken ct = default)
    {
        _logger.LogDebug("[exec-bg]: {cmd}", cmd);

        var output = new StringWriter();
        var error = new StringWriter();
        using var p = await RunRedirected(cmd, msg => output.WriteLine(msg), msg => error.WriteLine(msg), ct);

        return new CommandResult
        {
            ExitCode = p.ExitCode,
            Output = output.ToString(),
            Error = error.ToString(),
        };
    }

    public async Task<CommandResult> CaptureAsync(IPipeline pipeline, CancellationToken ct = default)
    {
        _logger.LogDebug("[exec-bg]: {pipeline}", pipeline);
        return await pipeline.Run(OutputMode.Capture, ct);
    }

    public void ExecDetached(ICommand cmd)
    {
        _logger.LogDebug("[exec-dt]: {cmd}", cmd);

        var info = cmd.ToProcessStartInfo();
        info.UseShellExecute = false;

        Process.Start(info);
    }

    private async Task<Process> RunRedirected(ICommand cmd, Action<string> output, Action<string> error, CancellationToken ct)
    {
        var info = cmd.ToProcessStartInfo();
        info.UseShellExecute = false;
        info.CreateNoWindow = true;
        info.RedirectStandardInput = true;
        info.RedirectStandardOutput = true;
        info.RedirectStandardError = true;

        var p = new Process() { StartInfo = info };
        p.OutputDataReceived += (_, eventArgs) =>
        {
            if (eventArgs.Data != null)
            {
                output(eventArgs.Data!);
            }
        };
        p.ErrorDataReceived += (_, eventArgs) =>
        {
            if (eventArgs.Data != null)
            {
                error(eventArgs.Data!);
            }
        };

        p.Start();

        p.BeginErrorReadLine();
        p.BeginOutputReadLine();

        await p.WaitForExitAsync(ct);

        return p;
    }
}

namespace Larcanum.ShellToolkit;

public interface IPipeline
{
    Task<CommandResult> Run(OutputMode mode = OutputMode.Default, CancellationToken ct = default)
    {
        return Run(PipelineOutput.Empty, mode, ct);
    }

    Task<CommandResult> Run(PipelineOutput initial, OutputMode mode, CancellationToken ct = default);
}

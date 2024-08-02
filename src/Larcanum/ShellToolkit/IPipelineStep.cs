namespace Larcanum.ShellToolkit;

public interface IPipelineStep
{
    Task<PipelineOutput> Connect(PipelineOutput previous, OutputMode mode, CancellationToken ct = default);
}

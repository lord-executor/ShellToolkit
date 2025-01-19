namespace Larcanum.ShellToolkit;

public interface IBoundCommand
{
    Task<CommandResult> CaptureAsync(CancellationToken ct = default);
    Task<int> ExecAsync(CancellationToken ct = default);
    void ExecDetached();

    IBoundCommand ThrowOnError();
}

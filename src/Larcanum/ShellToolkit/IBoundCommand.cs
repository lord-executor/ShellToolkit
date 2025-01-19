namespace Larcanum.ShellToolkit;

public interface IBoundCommand
{
    Task<CommandResult> CaptureAsync(CancellationToken ct = default);
    Task<int> ExecAsync(CancellationToken ct = default);
    void ExecDetached();

    IBoundCommand ThrowOnError();
}

public static class BoundCommandExtensions
{
    public static Task<string> CaptureAsStringAsync(this IBoundCommand command, CancellationToken ct = default) =>
        command.ThrowOnError().CaptureAsync(ct).ContinueWith(t => t.Result.AsString(), ct);

    public static Task<string?> CaptureAsNullableStringAsync(this IBoundCommand command, CancellationToken ct = default) =>
        command.ThrowOnError().CaptureAsync(ct).ContinueWith(t => t.Result.AsNullableString(), ct);
}

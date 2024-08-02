namespace Larcanum.ShellToolkit;

public static class PipelineExtensions
{
    public static IPipeline Pipe(this ICommand source, ICommand next)
    {
        return new Pipeline(source).Pipe(next);
    }

    public static IPipeline Pipe(this ICommand source, FileInfo file)
    {
        return new Pipeline(source).Pipe(file);
    }
}

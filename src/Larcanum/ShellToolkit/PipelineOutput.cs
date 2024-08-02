using System.Diagnostics;

namespace Larcanum.ShellToolkit;

public class PipelineOutput
{
    public static readonly PipelineOutput Empty = new PipelineOutput();

    public Process? Process { get; }
    public StreamReader? Out { get; }

    public PipelineOutput(Process process)
    {
        Process = process;
        if (process.StartInfo.RedirectStandardOutput)
        {
            Out = process.StandardOutput;
        }
    }

    public PipelineOutput(StreamReader input)
    {
        Out = input;
    }

    private PipelineOutput() { }
}

using System.Diagnostics;

namespace Larcanum.ShellToolkit;

public interface ICommand
{
    ProcessStartInfo ToProcessStartInfo();
}

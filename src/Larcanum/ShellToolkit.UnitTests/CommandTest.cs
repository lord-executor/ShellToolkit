using FluentAssertions;

using Larcanum.ShellToolkit;

using Xunit;

namespace ShellToolkit.UnitTests;

public class CommandTest
{
    [Fact]
    public void Create_WithEnumerableArgs_PopulatesArgumentList()
    {
        var cmd = Command.Create("test", ["arg1", "arg2", "arg 3"]);

        var psi = cmd.ToProcessStartInfo();
        psi.FileName.Should().Be("test");
        psi.ArgumentList.Should().ContainInOrder(["arg1", "arg2", "arg 3"]);
    }

    [Fact]
    public void Create_WithStringArgs_PopulatesArgumentList()
    {
        var cmd = Command.Create("test", "arg1 arg2 arg 3");

        var psi = cmd.ToProcessStartInfo();
        psi.FileName.Should().Be("test");
        psi.ArgumentList.Should().ContainInOrder(["arg1", "arg2", "arg", "3"]);
    }

    [Fact]
    public void Create_FromSingleString_PopulatesArgumentList()
    {
        var cmd = Command.Create("test arg1 arg2 arg 3");

        var psi = cmd.ToProcessStartInfo();
        psi.FileName.Should().Be("test");
        psi.ArgumentList.Should().ContainInOrder(["arg1", "arg2", "arg", "3"]);
    }
}

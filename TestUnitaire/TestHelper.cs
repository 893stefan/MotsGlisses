using System;
using System.IO;

namespace TestUnitaire;

internal static class TestHelper
{
    public static string RacineSolution()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir != null && !File.Exists(Path.Combine(dir.FullName, "MotsGlisses.csproj")))
            dir = dir.Parent;

        if (dir == null)
            throw new DirectoryNotFoundException("Racine MotsGlisses introuvable");

        return dir.FullName;
    }
}

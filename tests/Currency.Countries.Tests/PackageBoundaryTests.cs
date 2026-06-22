namespace Currency.Countries.Tests;

public class PackageBoundaryTests
{
    [Fact]
    public void CoreCurrencyPackage_DoesNotDependOnCountries()
    {
        var projectFile = Path.Combine(FindRepositoryRoot(), "src", "Currency", "Currency.csproj");
        var projectText = File.ReadAllText(projectFile);

        Assert.DoesNotContain("ISOCodex.Countries", projectText, StringComparison.Ordinal);
    }

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory != null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "ISOCodex.Currency.sln")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException("Could not find the repository root.");
    }
}

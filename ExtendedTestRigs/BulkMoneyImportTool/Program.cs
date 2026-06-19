using System.Globalization;
using ISOCodex.Currency;

var inputPath = args.Length > 0 ? args[0] : Path.Combine("SampleData", "money-import.csv");

if (!File.Exists(inputPath))
{
    var outputRelativePath = Path.Combine(AppContext.BaseDirectory, inputPath);

    if (File.Exists(outputRelativePath))
    {
        inputPath = outputRelativePath;
    }
    else
    {
        Console.Error.WriteLine($"Input file not found: {inputPath}");
        return 1;
    }
}

var registry = DefaultCurrencyRegistry.Instance;
var roundingService = new CurrencyRoundingService();
var results = new List<ImportResult>();

foreach (var row in ReadRows(inputPath).Skip(1))
{
    results.Add(ProcessRow(row, registry, roundingService));
}

var outputDirectory = Path.Combine(AppContext.BaseDirectory, "Output");
Directory.CreateDirectory(outputDirectory);

var resultsPath = Path.Combine(outputDirectory, "import-results.csv");
var summaryPath = Path.Combine(outputDirectory, "import-summary.txt");

File.WriteAllLines(resultsPath, ToCsv(results));
File.WriteAllLines(summaryPath, ToSummary(results));

Console.WriteLine($"Processed {results.Count} rows.");
Console.WriteLine($"Accepted: {results.Count(result => result.Status is ImportStatus.Accepted or ImportStatus.Rounded)}");
Console.WriteLine($"Rejected: {results.Count(result => result.Status == ImportStatus.Rejected)}");
Console.WriteLine($"Results: {resultsPath}");
Console.WriteLine($"Summary: {summaryPath}");

return 0;

static ImportResult ProcessRow(string[] row, ICurrencyRegistry registry, ICurrencyRoundingService roundingService)
{
    var reference = GetColumn(row, 0);
    var amountText = GetColumn(row, 1);
    var currencyText = GetColumn(row, 2);
    var roundingText = GetColumn(row, 3);

    try
    {
        if (!decimal.TryParse(amountText, NumberStyles.Number, CultureInfo.InvariantCulture, out var amount))
        {
            return ImportResult.Rejected(reference, amountText, currencyText, "Amount is not a valid decimal.");
        }

        var currencyCode = CurrencyCode.Parse(currencyText);
        var currency = registry.Get(currencyCode);
        var policy = CreatePolicy(roundingText);
        var roundedAmount = policy is null
            ? amount
            : roundingService.RoundAmount(amount, currency, policy);
        var money = Money.Of(roundedAmount, currencyCode);
        var status = roundedAmount == amount ? ImportStatus.Accepted : ImportStatus.Rounded;
        var note = status == ImportStatus.Rounded
            ? $"Rounded from {amount.ToString(CultureInfo.InvariantCulture)} using {roundingText}."
            : "Accepted without rounding.";

        return new ImportResult(reference, status, money.Amount, money.Currency.Code, money.ToMinorUnits(), note);
    }
    catch (Exception ex) when (ex is ArgumentException or ArgumentOutOfRangeException or InvalidOperationException)
    {
        return ImportResult.Rejected(reference, amountText, currencyText, ex.Message);
    }
}

static CurrencyRoundingPolicy? CreatePolicy(string roundingText)
{
    return roundingText.Trim().ToUpperInvariant() switch
    {
        "" or "NONE" => null,
        "STANDARD" => CurrencyRoundingPolicy.Standard(MidpointRounding.ToEven),
        "AWAYFROMZERO" => CurrencyRoundingPolicy.AwayFromZero(),
        "CASH" => CurrencyRoundingPolicy.Cash(MidpointRounding.AwayFromZero),
        _ => throw new ArgumentException($"Unknown rounding mode '{roundingText}'.")
    };
}

static IEnumerable<string[]> ReadRows(string path)
{
    foreach (var line in File.ReadLines(path))
    {
        if (!string.IsNullOrWhiteSpace(line))
        {
            yield return line.Split(',').Select(column => column.Trim()).ToArray();
        }
    }
}

static string GetColumn(string[] row, int index)
{
    return index < row.Length ? row[index] : string.Empty;
}

static IEnumerable<string> ToCsv(IEnumerable<ImportResult> results)
{
    yield return "Reference,Status,Amount,Currency,MinorUnits,Note";

    foreach (var result in results)
    {
        yield return string.Join(
            ',',
            Escape(result.Reference),
            result.Status,
            result.Amount?.ToString(CultureInfo.InvariantCulture) ?? string.Empty,
            Escape(result.Currency ?? string.Empty),
            result.MinorUnits?.ToString(CultureInfo.InvariantCulture) ?? string.Empty,
            Escape(result.Note));
    }
}

static IEnumerable<string> ToSummary(IReadOnlyCollection<ImportResult> results)
{
    yield return "Bulk money import summary";
    yield return "=========================";
    yield return $"Rows: {results.Count}";
    yield return $"Accepted: {results.Count(result => result.Status == ImportStatus.Accepted)}";
    yield return $"Rounded: {results.Count(result => result.Status == ImportStatus.Rounded)}";
    yield return $"Rejected: {results.Count(result => result.Status == ImportStatus.Rejected)}";
    yield return string.Empty;

    foreach (var result in results)
    {
        yield return $"{result.Reference}: {result.Status} - {result.Note}";
    }
}

static string Escape(string value)
{
    return value.Contains(',') || value.Contains('"')
        ? $"\"{value.Replace("\"", "\"\"")}\""
        : value;
}

internal enum ImportStatus
{
    Accepted,
    Rounded,
    Rejected
}

internal sealed record ImportResult(
    string Reference,
    ImportStatus Status,
    decimal? Amount,
    string? Currency,
    long? MinorUnits,
    string Note)
{
    public static ImportResult Rejected(string reference, string amount, string currency, string note)
    {
        return new ImportResult(reference, ImportStatus.Rejected, null, currency, null, $"{amount}: {note}");
    }
}

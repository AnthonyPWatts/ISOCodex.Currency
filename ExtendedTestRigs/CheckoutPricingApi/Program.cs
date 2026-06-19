using ISOCodex.Currency;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
var registry = DefaultCurrencyRegistry.Instance;

app.MapGet("/", () => Results.Ok(new
{
    service = "ISOCodex.Currency checkout pricing API",
    endpoints = new[] { "GET /currencies/{code}", "POST /quotes" }
}));

app.MapGet("/currencies/{code}", (string code) =>
{
    if (!CurrencyCode.TryParse(code, out var currencyCode))
    {
        return Results.BadRequest(new ErrorResponse($"'{code}' is not a registered currency code."));
    }

    var currency = registry.Get(currencyCode);

    return Results.Ok(new CurrencyResponse(
        currency.Code.Code,
        currency.NumericCode,
        currency.EnglishName,
        currency.MinorUnit.IsApplicable ? currency.MinorUnit.DecimalPlaces : null,
        currency.CashRoundingIncrement));
});

app.MapPost("/quotes", (QuoteRequest request) =>
{
    try
    {
        if (!CurrencyCode.TryParse(request.Currency, out var currencyCode))
        {
            return Results.BadRequest(new ErrorResponse($"'{request.Currency}' is not a registered currency code."));
        }

        if (request.Items.Count == 0)
        {
            return Results.BadRequest(new ErrorResponse("At least one line item is required."));
        }

        var roundingPolicy = CreatePolicy(request.Rounding);
        var subtotal = Money.Zero(currencyCode);
        var lines = new List<QuoteLineResponse>();

        foreach (var item in request.Items)
        {
            if (item.Quantity <= 0)
            {
                return Results.BadRequest(new ErrorResponse($"Item '{item.Sku}' must have a positive quantity."));
            }

            var unitPrice = Money.Of(item.UnitPrice, currencyCode);
            var lineTotal = unitPrice.Multiply(item.Quantity, roundingPolicy);
            subtotal += lineTotal;
            lines.Add(new QuoteLineResponse(item.Sku, item.Quantity, ToDto(unitPrice), ToDto(lineTotal)));
        }

        var tax = subtotal.Multiply(request.TaxRate, roundingPolicy);
        var total = subtotal + tax;

        return Results.Ok(new QuoteResponse(request.Currency.ToUpperInvariant(), lines, ToDto(subtotal), request.TaxRate, ToDto(tax), ToDto(total)));
    }
    catch (Exception ex) when (ex is ArgumentException or ArgumentOutOfRangeException or InvalidOperationException or DivideByZeroException)
    {
        return Results.BadRequest(new ErrorResponse(ex.Message));
    }
});

app.Run();

static CurrencyRoundingPolicy CreatePolicy(string? rounding)
{
    return (rounding ?? "Standard").Trim().ToUpperInvariant() switch
    {
        "" or "STANDARD" => CurrencyRoundingPolicy.Standard(MidpointRounding.ToEven),
        "AWAYFROMZERO" => CurrencyRoundingPolicy.AwayFromZero(),
        "CASH" => CurrencyRoundingPolicy.Cash(MidpointRounding.AwayFromZero),
        _ => throw new ArgumentException($"Unknown rounding mode '{rounding}'.")
    };
}

static MoneyDto ToDto(Money money)
{
    return new MoneyDto(money.Amount, money.Currency.Code);
}

internal sealed record QuoteRequest(string Currency, decimal TaxRate, string? Rounding, List<QuoteLineRequest> Items);

internal sealed record QuoteLineRequest(string Sku, decimal UnitPrice, int Quantity);

internal sealed record QuoteResponse(
    string Currency,
    IReadOnlyList<QuoteLineResponse> Lines,
    MoneyDto Subtotal,
    decimal TaxRate,
    MoneyDto Tax,
    MoneyDto Total);

internal sealed record QuoteLineResponse(string Sku, int Quantity, MoneyDto UnitPrice, MoneyDto LineTotal);

internal sealed record MoneyDto(decimal Amount, string Currency);

internal sealed record CurrencyResponse(
    string Code,
    string NumericCode,
    string EnglishName,
    int? DecimalPlaces,
    decimal? CashRoundingIncrement);

internal sealed record ErrorResponse(string Error);

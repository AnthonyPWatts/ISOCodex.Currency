using Dapper;

namespace ISOCodex.Currency.Dapper;

/// <summary>
/// Registers ISOCodex.Currency type handlers with Dapper.
/// </summary>
public static class DapperCurrencyTypeHandlers
{
    /// <summary>
    /// Registers all currency type handlers supplied by this package.
    /// </summary>
    public static void Register()
    {
        SqlMapper.AddTypeHandler(CurrencyCodeTypeHandler.Instance);
    }
}

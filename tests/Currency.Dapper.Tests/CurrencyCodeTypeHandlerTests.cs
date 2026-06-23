using System.Data;
using System.Diagnostics.CodeAnalysis;
using ISOCodex.Currency;
using ISOCodex.Currency.Dapper;

namespace Currency.Dapper.Tests;

public class CurrencyCodeTypeHandlerTests
{
    [Fact]
    public void SetValue_StoresAlpha3CodeAsThreeCharacterString()
    {
        var parameter = new FakeDataParameter();

        CurrencyCodeTypeHandler.Instance.SetValue(parameter, CurrencyCode.GBP);

        Assert.Equal(DbType.String, parameter.DbType);
        Assert.Equal(3, parameter.Size);
        Assert.Equal("GBP", parameter.Value);
    }

    [Fact]
    public void SetValue_RejectsDefaultCurrencyCode()
    {
        var parameter = new FakeDataParameter();

        Assert.Throws<InvalidOperationException>(() =>
            CurrencyCodeTypeHandler.Instance.SetValue(parameter, default));
    }

    [Fact]
    public void Parse_AcceptsCaseInsensitiveString()
    {
        var result = CurrencyCodeTypeHandler.Instance.Parse("gbp");

        Assert.Equal(CurrencyCode.GBP, result);
    }

    [Fact]
    public void Parse_ReturnsExistingCurrencyCode()
    {
        var result = CurrencyCodeTypeHandler.Instance.Parse(CurrencyCode.EUR);

        Assert.Equal(CurrencyCode.EUR, result);
    }

    [Fact]
    public void Parse_RejectsNullDatabaseValue()
    {
        Assert.Throws<DataException>(() => CurrencyCodeTypeHandler.Instance.Parse(DBNull.Value));
    }

    [Fact]
    public void Parse_RejectsUnknownCurrencyCode()
    {
        var exception = Assert.Throws<DataException>(() => CurrencyCodeTypeHandler.Instance.Parse("ZZZ"));

        Assert.Contains("ZZZ", exception.Message);
    }

    [Fact]
    public void Register_DoesNotThrow()
    {
        DapperCurrencyTypeHandlers.Register();
    }

    private sealed class FakeDataParameter : IDbDataParameter
    {
        private string _parameterName = string.Empty;
        private string _sourceColumn = string.Empty;

        public DbType DbType { get; set; }

        public ParameterDirection Direction { get; set; } = ParameterDirection.Input;

        public bool IsNullable => false;

        [AllowNull]
        public string ParameterName
        {
            get => _parameterName;
            set => _parameterName = value ?? string.Empty;
        }

        [AllowNull]
        public string SourceColumn
        {
            get => _sourceColumn;
            set => _sourceColumn = value ?? string.Empty;
        }

        public DataRowVersion SourceVersion { get; set; } = DataRowVersion.Current;

        public object? Value { get; set; }

        public byte Precision { get; set; }

        public byte Scale { get; set; }

        public int Size { get; set; }
    }
}

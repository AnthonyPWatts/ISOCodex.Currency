using ISOCodex.Currency;
using ISOCodex.Currency.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Currency.EntityFrameworkCore.Tests;

public class EntityFrameworkCurrencyTests
{
    [Fact]
    public void CurrencyCodeValueConverter_RoundTripsRegisteredCodes()
    {
        var converter = CurrencyCodeValueConverter.Instance;

        Assert.Equal("GBP", converter.ConvertToProvider(CurrencyCode.GBP));
        Assert.Equal(CurrencyCode.GBP, converter.ConvertFromProvider("gbp"));
        Assert.Throws<ArgumentException>(() => converter.ConvertFromProvider("ZZZ"));
    }

    [Fact]
    public void ModelBuilder_MapsMoneyToAmountAndCurrencyColumns()
    {
        using var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();

        using var context = CreateContext(connection);
        context.Database.EnsureCreated();

        using var command = connection.CreateCommand();
        command.CommandText = "PRAGMA table_info('Orders');";

        using var reader = command.ExecuteReader();
        var columns = new Dictionary<string, string>(StringComparer.Ordinal);

        while (reader.Read())
        {
            columns.Add(reader.GetString(1), reader.GetString(2));
        }

        Assert.Contains("TotalAmount", columns.Keys);
        Assert.Contains("TotalCurrency", columns.Keys);
        Assert.Contains("CurrencyCode", columns.Keys);
    }

    [Fact]
    public void SqliteContext_RoundTripsMoneyAndCurrencyCode()
    {
        using var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();

        using (var context = CreateContext(connection))
        {
            context.Database.EnsureCreated();
            context.Orders.Add(new Order
            {
                Total = Money.Of(12.34m, CurrencyCode.GBP),
                Currency = CurrencyCode.GBP
            });
            context.SaveChanges();
        }

        using (var context = CreateContext(connection))
        {
            var order = Assert.Single(context.Orders);

            Assert.Equal(Money.Of(12.34m, CurrencyCode.GBP), order.Total);
            Assert.Equal(CurrencyCode.GBP, order.Currency);
        }
    }

    private static OrdersContext CreateContext(SqliteConnection connection)
    {
        var options = new DbContextOptionsBuilder<OrdersContext>()
            .UseSqlite(connection)
            .Options;

        return new OrdersContext(options);
    }

    private sealed class OrdersContext : DbContext
    {
        public OrdersContext(DbContextOptions<OrdersContext> options)
            : base(options)
        {
        }

        public DbSet<Order> Orders => Set<Order>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(order => order.Id);
                entity.Property(order => order.Currency)
                    .HasCurrencyCodeConversion()
                    .HasColumnName("CurrencyCode");
                entity.ComplexMoney(
                    order => order.Total,
                    amountColumn: "TotalAmount",
                    currencyColumn: "TotalCurrency");
            });
        }
    }

    private sealed class Order
    {
        public int Id { get; set; }

        public Money Total { get; set; }

        public CurrencyCode Currency { get; set; }
    }
}

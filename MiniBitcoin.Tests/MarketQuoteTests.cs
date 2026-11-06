using MiniBitcoin.Console;
using Xunit;

namespace MiniBitcoin.Tests;

/// <summary>
/// Testes unitários da cotação de mercado do Bitcoin (MarketQuote).
/// </summary>
public class MarketQuoteTests
{
    [Fact]
    public void Construtor_ComPrecoValido_DeveInicializarCotacao()
    {
        var mercado = new MarketQuote(350_000m);

        Assert.Equal(350_000m, mercado.PricePerBtc);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Construtor_ComPrecoInvalido_DeveLancarArgumentException(int preco)
        => Assert.Throws<ArgumentException>(() => new MarketQuote(preco));

    [Fact]
    public void UpdatePrice_ComPrecoValido_DeveAtualizarCotacao()
    {
        var mercado = new MarketQuote(100_000m);

        mercado.UpdatePrice(120_000m);

        Assert.Equal(120_000m, mercado.PricePerBtc);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-350)]
    public void UpdatePrice_ComPrecoInvalido_DeveLancarArgumentException(int novoPreco)
    {
        var mercado = new MarketQuote(100_000m);

        Assert.Throws<ArgumentException>(() => mercado.UpdatePrice(novoPreco));
    }
}

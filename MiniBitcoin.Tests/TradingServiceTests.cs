using MiniBitcoin.Console;
using Xunit;

namespace MiniBitcoin.Tests;

/// <summary>
/// Testes unitários da lógica de compra e venda de Bitcoin:
/// cenários de sucesso e de transações inválidas (TDD — Etapa 2).
/// </summary>
public class TradingServiceTests
{
    private readonly TradingService _servico = new();

    [Fact]
    public void Buy_ComSaldoSuficiente_DeveConverterBrlEmBtcEAtualizarCarteira()
    {
        var carteira = new Wallet(1_000m, 0m);
        var mercado = new MarketQuote(100_000m);

        Transaction transacao = _servico.Buy(carteira, mercado, 250m);

        Assert.Equal(750m, carteira.BrlBalance);
        Assert.Equal(0.0025m, carteira.BtcBalance);
        Assert.Equal(TransactionType.Buy, transacao.Type);
        Assert.Equal(250m, transacao.AmountBrl);
        Assert.Equal(0.0025m, transacao.AmountBtc);
        Assert.Equal(100_000m, transacao.PricePerBtc);
    }

    [Fact]
    public void Buy_ComSucessivasCompras_DeveAcumularSaldoEmBtc()
    {
        var carteira = new Wallet(1_000m, 0m);
        var mercado = new MarketQuote(100_000m);

        _servico.Buy(carteira, mercado, 250m);
        _servico.Buy(carteira, mercado, 250m);

        Assert.Equal(500m, carteira.BrlBalance);
        Assert.Equal(0.005m, carteira.BtcBalance);
    }

    [Fact]
    public void Buy_ComSaldoBrlInsuficiente_DeveLancarErroEPreservarSaldos()
    {
        var carteira = new Wallet(100m, 0m);
        var mercado = new MarketQuote(100_000m);

        Assert.Throws<InvalidOperationException>(() => _servico.Buy(carteira, mercado, 150m));

        Assert.Equal(100m, carteira.BrlBalance);
        Assert.Equal(0m, carteira.BtcBalance);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void Buy_ComValorInvalido_DeveLancarArgumentException(int valor)
        => Assert.Throws<ArgumentException>(
            () => _servico.Buy(new Wallet(1_000m, 0m), new MarketQuote(100_000m), valor));

    [Fact]
    public void Buy_ComValorAbaixoDaFracaoMinima_DeveLancarInvalidOperationException()
    {
        var carteira = new Wallet(1_000m, 0m);
        var mercado = new MarketQuote(10_000_000m);

        // R$ 0,01 / R$ 10.000.000,00 = 0,000000001 BTC -> arredonda para 0 satoshis
        Assert.Throws<InvalidOperationException>(() => _servico.Buy(carteira, mercado, 0.01m));
    }

    [Fact]
    public void Sell_ComSaldoBtcSuficiente_DeveConverterBtcEmBrlEAtualizarCarteira()
    {
        var carteira = new Wallet(0m, 0.5m);
        var mercado = new MarketQuote(200_000m);

        Transaction transacao = _servico.Sell(carteira, mercado, 0.25m);

        Assert.Equal(0.25m, carteira.BtcBalance);
        Assert.Equal(50_000m, carteira.BrlBalance);
        Assert.Equal(TransactionType.Sell, transacao.Type);
        Assert.Equal(50_000m, transacao.AmountBrl);
        Assert.Equal(0.25m, transacao.AmountBtc);
        Assert.Equal(200_000m, transacao.PricePerBtc);
    }

    [Fact]
    public void Sell_ComSaldoBtcInsuficiente_DeveLancarErroEPreservarSaldos()
    {
        var carteira = new Wallet(0m, 0.5m);
        var mercado = new MarketQuote(200_000m);

        Assert.Throws<InvalidOperationException>(() => _servico.Sell(carteira, mercado, 0.6m));

        Assert.Equal(0m, carteira.BrlBalance);
        Assert.Equal(0.5m, carteira.BtcBalance);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Sell_ComQuantidadeInvalida_DeveLancarArgumentException(int quantidade)
        => Assert.Throws<ArgumentException>(
            () => _servico.Sell(new Wallet(0m, 1m), new MarketQuote(100_000m), quantidade));

    [Fact]
    public void Sell_ComCotacaoAtualizada_DeveUsarPrecoCorrente()
    {
        var carteira = new Wallet(0m, 1m);
        var mercado = new MarketQuote(100_000m);
        mercado.UpdatePrice(250_000m);

        _servico.Sell(carteira, mercado, 0.1m);

        Assert.Equal(25_000m, carteira.BrlBalance);
    }
}

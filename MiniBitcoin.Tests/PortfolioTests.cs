using MiniBitcoin.Console.Domain;
using MiniBitcoin.Console.Services;
using Xunit;

namespace MiniBitcoin.Tests;

/// <summary>
/// Testes unitários do Portfolio (fachada que integra carteira, mercado e
/// extrato), cobrindo o fluxo completo do extrato e a gestão do patrimônio.
/// </summary>
public class PortfolioTests
{
    private static Portfolio CriarPortfolioPadrao()
        => new(new Wallet(10_000m, 0m), new MarketQuote(100_000m));

    [Fact]
    public void Buy_AtravesDoPortfolio_DeveAtualizarCarteiraERegistrarNoExtrato()
    {
        Portfolio portfolio = CriarPortfolioPadrao();

        Transaction transacao = portfolio.Buy(2_000m);

        Assert.Equal(8_000m, portfolio.Wallet.BrlBalance);
        Assert.Equal(0.02m, portfolio.Wallet.BtcBalance);

        Transaction unica = Assert.Single(portfolio.Statement.Transactions);
        Assert.Same(transacao, unica);
    }

    [Fact]
    public void Sell_AtravesDoPortfolio_DeveAtualizarCarteiraERegistrarNoExtrato()
    {
        var portfolio = new Portfolio(new Wallet(1_000m, 0.5m), new MarketQuote(200_000m));

        Transaction transacao = portfolio.Sell(0.25m);

        Assert.Equal(0.25m, portfolio.Wallet.BtcBalance);
        Assert.Equal(51_000m, portfolio.Wallet.BrlBalance);

        Transaction unica = Assert.Single(portfolio.Statement.Transactions);
        Assert.Same(transacao, unica);
    }

    [Fact]
    public void FluxoCompleto_ComprasEVendas_ExtratoDeveConsolidarPatrimonio()
    {
        Portfolio portfolio = CriarPortfolioPadrao();

        portfolio.Buy(2_000m);                    // +0,02 BTC
        portfolio.Buy(1_000m);                    // +0,01 BTC
        portfolio.Sell(0.01m);                    // -0,01 BTC => +R$ 1.000,00
        portfolio.Market.UpdatePrice(250_000m);   // cotação sobe

        Assert.Equal(3, portfolio.Statement.Count);
        Assert.Equal(3_000m, portfolio.Statement.TotalBoughtBrl);
        Assert.Equal(1_000m, portfolio.Statement.TotalSoldBrl);

        // BRL: 10.000 - 3.000 + 1.000 = 8.000 | BTC: 0,02
        Assert.Equal(8_000m, portfolio.Wallet.BrlBalance);
        Assert.Equal(0.02m, portfolio.Wallet.BtcBalance);

        // Patrimônio: 8.000 + 0,02 x 250.000 = R$ 13.000,00
        Assert.Equal(13_000m, portfolio.PatrimonyBrl);
    }

    [Fact]
    public void Patrimonio_SemTransacoes_DeveSomarSaldosIniciaisValorizados()
    {
        var portfolio = new Portfolio(new Wallet(1_000m, 0.1m), new MarketQuote(300_000m));

        Assert.Equal(31_000m, portfolio.PatrimonyBrl);
    }
}

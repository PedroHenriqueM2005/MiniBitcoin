using MiniBitcoin.Console.Domain;
using MiniBitcoin.Console.Services;
using Xunit;

namespace MiniBitcoin.Tests;

/// <summary>
/// Testes unitários do Extrato/Histórico de transações e do cálculo do
/// patrimônio total consolidado (TDD — Etapa 3).
/// </summary>
public class StatementTests
{
    [Fact]
    public void Add_DeveRegistrarTransacoesNaOrdemDeExecucao()
    {
        var servico = new TradingService();
        var carteira = new Wallet(1_000m, 0m);
        var mercado = new MarketQuote(100_000m);

        Transaction compra1 = servico.Buy(carteira, mercado, 100m);
        Transaction venda = servico.Sell(carteira, mercado, 0.0005m);
        Transaction compra2 = servico.Buy(carteira, mercado, 100m);

        var extrato = new Statement();
        extrato.Add(compra1);
        extrato.Add(venda);
        extrato.Add(compra2);

        Assert.Equal(3, extrato.Count);
        Assert.Equal(new[] { compra1, venda, compra2 }, extrato.Transactions);
    }

    [Fact]
    public void Add_ComTransacaoNula_DeveLancarArgumentNullException()
        => Assert.Throws<ArgumentNullException>(() => new Statement().Add(null!));

    [Fact]
    public void ExtratoVazio_DeveEstarVazioEPatrimonioIgualAosSaldos()
    {
        var extrato = new Statement();
        var carteira = new Wallet(1_000m, 0.5m);
        var mercado = new MarketQuote(100_000m);

        Assert.Empty(extrato.Transactions);
        Assert.Equal(0, extrato.Count);

        // 1.000 BRL + 0,5 BTC x R$ 100.000 = R$ 51.000,00
        Assert.Equal(51_000m, extrato.CalculatePatrimonyBrl(carteira, mercado));
    }

    [Fact]
    public void Totais_ComprasEVendas_DevemSomarValoresCorretos()
    {
        var extrato = new Statement();
        extrato.Add(new Transaction(TransactionType.Buy, 500m, 0.005m, 100_000m));
        extrato.Add(new Transaction(TransactionType.Buy, 300m, 0.003m, 100_000m));
        extrato.Add(new Transaction(TransactionType.Sell, 200m, 0.002m, 100_000m));

        Assert.Equal(800m, extrato.TotalBoughtBrl);
        Assert.Equal(200m, extrato.TotalSoldBrl);
        Assert.Equal(3, extrato.Count);
    }

    [Fact]
    public void CalculatePatrimonyBrl_ComCotacaoAtualizada_DeveValorizarBtcPeloPrecoCorrente()
    {
        var extrato = new Statement();
        var carteira = new Wallet(500m, 1m);
        var mercado = new MarketQuote(100_000m);

        mercado.UpdatePrice(250_000m);

        Assert.Equal(250_500m, extrato.CalculatePatrimonyBrl(carteira, mercado));
    }
}

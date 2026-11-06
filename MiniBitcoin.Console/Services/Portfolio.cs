using MiniBitcoin.Console.Domain;

namespace MiniBitcoin.Console.Services;

/// <summary>
/// Fachada de gestão do portfólio: integra a carteira, a cotação de mercado e
/// o extrato, registrando automaticamente cada ordem de compra e venda
/// executada no histórico de transações.
/// </summary>
public class Portfolio
{
    private readonly TradingService _servicoDeTransacoes = new();

    public Portfolio(Wallet carteira, MarketQuote mercado)
    {
        Wallet = carteira ?? throw new ArgumentNullException(nameof(carteira));
        Market = mercado ?? throw new ArgumentNullException(nameof(mercado));
    }

    /// <summary>Carteira com os saldos em BRL e BTC.</summary>
    public Wallet Wallet { get; }

    /// <summary>Mercado com a cotação atual do Bitcoin.</summary>
    public MarketQuote Market { get; }

    /// <summary>Extrato com o histórico de transações realizadas.</summary>
    public Statement Statement { get; } = new();

    /// <summary>Executa uma ordem de compra de Bitcoin e a registra no extrato.</summary>
    public Transaction Buy(decimal valorBrl)
    {
        Transaction transacao = _servicoDeTransacoes.Buy(Wallet, Market, valorBrl);
        Statement.Add(transacao);
        return transacao;
    }

    /// <summary>Executa uma ordem de venda de Bitcoin e a registra no extrato.</summary>
    public Transaction Sell(decimal quantidadeBtc)
    {
        Transaction transacao = _servicoDeTransacoes.Sell(Wallet, Market, quantidadeBtc);
        Statement.Add(transacao);
        return transacao;
    }

    /// <summary>Patrimônio total consolidado em BRL pela cotação atual.</summary>
    public decimal PatrimonyBrl => Statement.CalculatePatrimonyBrl(Wallet, Market);
}

using MiniBitcoin.Console.Domain;

namespace MiniBitcoin.Console.Services;

/// <summary>
/// Extrato/Histórico de transações da carteira: registra cada operação de
/// compra e venda realizada e calcula o patrimônio total consolidado em BRL
/// com base na cotação atual do Bitcoin.
/// </summary>
public class Statement
{
    private readonly List<Transaction> _transacoes = new();

    /// <summary>Transações registradas, na ordem em que foram executadas.</summary>
    public IReadOnlyList<Transaction> Transactions => _transacoes.AsReadOnly();

    /// <summary>Quantidade de operações registradas no extrato.</summary>
    public int Count => _transacoes.Count;

    /// <summary>Total comprado (em BRL) desde a abertura da carteira.</summary>
    public decimal TotalBoughtBrl
        => _transacoes.Where(t => t.Type == TransactionType.Buy).Sum(t => t.AmountBrl);

    /// <summary>Total vendido (em BRL) desde a abertura da carteira.</summary>
    public decimal TotalSoldBrl
        => _transacoes.Where(t => t.Type == TransactionType.Sell).Sum(t => t.AmountBrl);

    /// <summary>Registra uma transação de compra ou venda no extrato.</summary>
    public void Add(Transaction transacao)
        => _transacoes.Add(transacao ?? throw new ArgumentNullException(nameof(transacao)));

    /// <summary>
    /// Calcula o patrimônio total consolidado em BRL: saldo em reais somado ao
    /// valor de mercado do saldo em Bitcoin pela cotação atual.
    /// </summary>
    public decimal CalculatePatrimonyBrl(Wallet carteira, MarketQuote mercado)
    {
        ArgumentNullException.ThrowIfNull(carteira);
        ArgumentNullException.ThrowIfNull(mercado);

        decimal valorBtcEmBrl = Math.Round(
            carteira.BtcBalance * mercado.PricePerBtc, 2, MidpointRounding.AwayFromZero);

        return carteira.BrlBalance + valorBtcEmBrl;
    }
}

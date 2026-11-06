namespace MiniBitcoin.Console.Domain;

/// <summary>Tipo de operação registrada no extrato.</summary>
public enum TransactionType
{
    /// <summary>Compra de Bitcoin com saldo em BRL.</summary>
    Buy = 1,

    /// <summary>Venda de Bitcoin convertida em BRL.</summary>
    Sell = 2
}

/// <summary>
/// Registro imutável de uma operação de compra ou venda de Bitcoin
/// executada pelo sistema.
/// </summary>
public sealed class Transaction
{
    public Transaction(
        TransactionType type,
        decimal amountBrl,
        decimal amountBtc,
        decimal pricePerBtc,
        DateTime? timestampUtc = null)
    {
        Type = type;
        AmountBrl = amountBrl;
        AmountBtc = amountBtc;
        PricePerBtc = pricePerBtc;
        TimestampUtc = timestampUtc ?? DateTime.UtcNow;
    }

    /// <summary>Identificador único da transação.</summary>
    public Guid Id { get; } = Guid.NewGuid();

    /// <summary>Tipo da operação (compra ou venda).</summary>
    public TransactionType Type { get; }

    /// <summary>Valor da operação em BRL.</summary>
    public decimal AmountBrl { get; }

    /// <summary>Quantidade de Bitcoin negociada.</summary>
    public decimal AmountBtc { get; }

    /// <summary>Cotação do BTC aplicada na operação.</summary>
    public decimal PricePerBtc { get; }

    /// <summary>Data e hora (UTC) em que a transação foi executada.</summary>
    public DateTime TimestampUtc { get; }
}

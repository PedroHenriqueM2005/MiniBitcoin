using MiniBitcoin.Console.Domain;

namespace MiniBitcoin.Console.Services;

/// <summary>
/// Serviço que executa ordens de compra e venda de Bitcoin com base na
/// cotação atual do mercado, aplicando as validações de saldo da carteira.
/// </summary>
public class TradingService
{
    /// <summary>Precisão de Bitcoin: 1 BTC = 100.000.000 satoshis (8 casas decimais).</summary>
    private const int CasasDecimaisBtc = 8;

    /// <summary>Precisão monetária do BRL (2 casas decimais).</summary>
    private const int CasasDecimaisBrl = 2;

    /// <summary>
    /// Executa uma ordem de compra: debita o valor em BRL da carteira e credita a
    /// quantidade equivalente de BTC, convertida pela cotação atual.
    /// </summary>
    public Transaction Buy(Wallet carteira, MarketQuote mercado, decimal valorBrl)
    {
        if (valorBrl <= 0)
            throw new ArgumentException("O valor da compra deve ser maior que zero.", nameof(valorBrl));

        decimal quantidadeBtc = Math.Round(
            valorBrl / mercado.PricePerBtc, CasasDecimaisBtc, MidpointRounding.AwayFromZero);

        if (quantidadeBtc <= 0)
            throw new InvalidOperationException(
                "O valor informado é insuficiente para adquirir a fração mínima de Bitcoin (1 satoshi).");

        carteira.DebitBrl(valorBrl);
        carteira.CreditBtc(quantidadeBtc);

        return new Transaction(TransactionType.Buy, valorBrl, quantidadeBtc, mercado.PricePerBtc);
    }

    /// <summary>
    /// Executa uma ordem de venda: debita a quantidade de BTC da carteira e credita o
    /// valor equivalente em BRL, convertido pela cotação atual.
    /// </summary>
    public Transaction Sell(Wallet carteira, MarketQuote mercado, decimal quantidadeBtc)
    {
        if (quantidadeBtc <= 0)
            throw new ArgumentException("A quantidade de Bitcoin a vender deve ser maior que zero.", nameof(quantidadeBtc));

        decimal valorBrl = Math.Round(
            quantidadeBtc * mercado.PricePerBtc, CasasDecimaisBrl, MidpointRounding.AwayFromZero);

        carteira.DebitBtc(quantidadeBtc);
        carteira.CreditBrl(valorBrl);

        return new Transaction(TransactionType.Sell, valorBrl, quantidadeBtc, mercado.PricePerBtc);
    }
}

namespace MiniBitcoin.Console;

/// <summary>
/// Entidade Carteira: mantém o saldo em moeda fiduciária (BRL) e o saldo em
/// Bitcoin (BTC, com precisão de satoshis — 8 casas decimais), garantindo que
/// nenhum saldo possa ficar negativo.
/// </summary>
public class Wallet
{
    /// <summary>Saldo disponível em reais (BRL).</summary>
    public decimal BrlBalance { get; private set; }

    /// <summary>Saldo disponível em Bitcoin (BTC).</summary>
    public decimal BtcBalance { get; private set; }

    /// <summary>
    /// Cria a carteira com saldos iniciais em BRL e BTC.
    /// Valores negativos não são permitidos.
    /// </summary>
    public Wallet(decimal saldoInicialBrl, decimal saldoInicialBtc)
    {
        if (saldoInicialBrl < 0)
            throw new ArgumentException("O saldo inicial em BRL não pode ser negativo.", nameof(saldoInicialBrl));

        if (saldoInicialBtc < 0)
            throw new ArgumentException("O saldo inicial em Bitcoin não pode ser negativo.", nameof(saldoInicialBtc));

        BrlBalance = saldoInicialBrl;
        BtcBalance = saldoInicialBtc;
    }

    /// <summary>Debita um valor em BRL, exigindo saldo suficiente.</summary>
    public void DebitBrl(decimal valor)
    {
        ValidarValorPositivo(valor, "BRL");

        if (valor > BrlBalance)
            throw new InvalidOperationException("Saldo em BRL insuficiente para a operação.");

        BrlBalance -= valor;
    }

    /// <summary>Credita um valor em BRL.</summary>
    public void CreditBrl(decimal valor)
    {
        ValidarValorPositivo(valor, "BRL");
        BrlBalance += valor;
    }

    /// <summary>Debita uma quantidade de BTC, exigindo saldo suficiente.</summary>
    public void DebitBtc(decimal quantidade)
    {
        ValidarValorPositivo(quantidade, "BTC");

        if (quantidade > BtcBalance)
            throw new InvalidOperationException("Saldo em Bitcoin insuficiente para a operação.");

        BtcBalance -= quantidade;
    }

    /// <summary>Credita uma quantidade de BTC.</summary>
    public void CreditBtc(decimal quantidade)
    {
        ValidarValorPositivo(quantidade, "BTC");
        BtcBalance += quantidade;
    }

    private static void ValidarValorPositivo(decimal valor, string moeda)
    {
        if (valor <= 0)
            throw new ArgumentException($"O valor da operação em {moeda} deve ser maior que zero.", nameof(valor));
    }
}

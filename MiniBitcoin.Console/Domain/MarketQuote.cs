namespace MiniBitcoin.Console.Domain;

/// <summary>
/// Cotação de mercado do Bitcoin (simulada e atualizável no sistema).
/// O preço é sempre expresso em BRL por 1 BTC e deve ser positivo.
/// </summary>
public class MarketQuote
{
    /// <summary>Preço atual do Bitcoin em BRL.</summary>
    public decimal PricePerBtc { get; private set; }

    public MarketQuote(decimal precoInicialPorBtc)
    {
        ValidarPreco(precoInicialPorBtc);
        PricePerBtc = precoInicialPorBtc;
    }

    /// <summary>Atualiza a cotação atual do Bitcoin.</summary>
    public void UpdatePrice(decimal novoPrecoPorBtc)
    {
        ValidarPreco(novoPrecoPorBtc);
        PricePerBtc = novoPrecoPorBtc;
    }

    private static void ValidarPreco(decimal preco)
    {
        if (preco <= 0)
            throw new ArgumentException("A cotação do Bitcoin deve ser maior que zero.", nameof(preco));
    }
}

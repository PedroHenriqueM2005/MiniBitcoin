using MiniBitcoin.Console.Domain;
using MiniBitcoin.Console.Services;
using Xunit;

namespace MiniBitcoin.Tests;

/// <summary>
/// Testes unitários da entidade Carteira (Wallet):
/// saldos iniciais válidos/inválidos e regras de débito/crédito.
/// </summary>
public class WalletTests
{
    [Fact]
    public void Construtor_ComSaldosIniciaisValidos_DeveCriarCarteiraComSaldosInformados()
    {
        var carteira = new Wallet(1_000m, 0.5m);

        Assert.Equal(1_000m, carteira.BrlBalance);
        Assert.Equal(0.5m, carteira.BtcBalance);
    }

    [Fact]
    public void Construtor_ComSaldosIniciaisZerados_DevePermitirCriacao()
    {
        var carteira = new Wallet(0m, 0m);

        Assert.Equal(0m, carteira.BrlBalance);
        Assert.Equal(0m, carteira.BtcBalance);
    }

    [Fact]
    public void Construtor_ComSaldoInicialBrlNegativo_DeveLancarArgumentException()
        => Assert.Throws<ArgumentException>(() => new Wallet(-1m, 0m));

    [Fact]
    public void Construtor_ComSaldoInicialBtcNegativo_DeveLancarArgumentException()
        => Assert.Throws<ArgumentException>(() => new Wallet(0m, -0.0001m));

    [Fact]
    public void DebitBrl_ComSaldoSuficiente_DeveReduzirSaldo()
    {
        var carteira = new Wallet(1_000m, 0m);

        carteira.DebitBrl(400m);

        Assert.Equal(600m, carteira.BrlBalance);
    }

    [Fact]
    public void DebitBrl_ComValorExatamenteIgualAoSaldo_DeveZerarSaldo()
    {
        var carteira = new Wallet(1_000m, 0m);

        carteira.DebitBrl(1_000m);

        Assert.Equal(0m, carteira.BrlBalance);
    }

    [Fact]
    public void DebitBrl_ComSaldoInsuficiente_DeveLancarInvalidOperationException()
        => Assert.Throws<InvalidOperationException>(() => new Wallet(100m, 0m).DebitBrl(100.01m));

    [Theory]
    [InlineData(-100)]
    [InlineData(0)]
    public void DebitBrl_ComValorInvalido_DeveLancarArgumentException(int valor)
        => Assert.Throws<ArgumentException>(() => new Wallet(100m, 0m).DebitBrl(valor));

    [Fact]
    public void CreditBrl_ComValorValido_DeveAumentarSaldo()
    {
        var carteira = new Wallet(100m, 0m);

        carteira.CreditBrl(250m);

        Assert.Equal(350m, carteira.BrlBalance);
    }

    [Theory]
    [InlineData(-50)]
    [InlineData(0)]
    public void CreditBrl_ComValorInvalido_DeveLancarArgumentException(int valor)
        => Assert.Throws<ArgumentException>(() => new Wallet(100m, 0m).CreditBrl(valor));

    [Fact]
    public void DebitBtc_ComSaldoSuficiente_DeveReduzirSaldo()
    {
        var carteira = new Wallet(0m, 1m);

        carteira.DebitBtc(0.25m);

        Assert.Equal(0.75m, carteira.BtcBalance);
    }

    [Fact]
    public void DebitBtc_ComSaldoInsuficiente_DeveLancarInvalidOperationException()
        => Assert.Throws<InvalidOperationException>(() => new Wallet(0m, 0.5m).DebitBtc(0.50000001m));

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public void CreditBtc_ComValorInvalido_DeveLancarArgumentException(int valor)
        => Assert.Throws<ArgumentException>(() => new Wallet(0m, 0m).CreditBtc(valor));
}

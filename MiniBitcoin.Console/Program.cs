using System.Globalization;
using MiniBitcoin.Console.Domain;
using MiniBitcoin.Console.Services;

// ============================================================================
//  MiniBitcoin — Carteira digital de Bitcoin (aplicação de console)
//  Projeto da disciplina Gestão e Qualidade de Software
// ============================================================================

Console.OutputEncoding = System.Text.Encoding.UTF8;

Console.WriteLine("================================================================");
Console.WriteLine("      MiniBitcoin — Carteira Digital de Bitcoin (simulador)");
Console.WriteLine("================================================================");
Console.WriteLine();

CultureInfo cultura = ObterCultura();
Portfolio portfolio = ConfigurarCarteiraInicial();
ExecutarMenuPrincipal(portfolio);

Console.WriteLine();
Console.WriteLine("Obrigado por utilizar o MiniBitcoin. Até logo!");

// ----------------------------------------------------------------------------
// Configuração inicial da carteira
// ----------------------------------------------------------------------------
Portfolio ConfigurarCarteiraInicial()
{
    Console.WriteLine("Configuração inicial (pressione ENTER para aceitar o valor padrão):");

    decimal saldoBrl = LerDecimal("  Saldo inicial em BRL", padrao: 10_000m);
    decimal saldoBtc = LerDecimal("  Saldo inicial em BTC", padrao: 0m);
    decimal cotacao = LerDecimal("  Cotação do Bitcoin (R$ por BTC)", padrao: 350_000m, minimo: 0.01m);

    Console.WriteLine();
    Console.WriteLine("Carteira configurada com sucesso!");
    Console.WriteLine();

    return new Portfolio(new Wallet(saldoBrl, saldoBtc), new MarketQuote(cotacao));
}

// ----------------------------------------------------------------------------
// Menu principal
// ----------------------------------------------------------------------------
void ExecutarMenuPrincipal(Portfolio portfolio)
{
    while (true)
    {
        ExibirMenu();

        string? opcao = Console.ReadLine();
        if (opcao is null)
        {
            return; // fim da entrada padrão (EOF)
        }

        switch (opcao.Trim())
        {
            case "1":
                ConsultarCotacao(portfolio);
                break;
            case "2":
                AtualizarCotacao(portfolio);
                break;
            case "3":
                ExibirSaldos(portfolio);
                break;
            case "4":
                ComprarBitcoin(portfolio);
                break;
            case "5":
                VenderBitcoin(portfolio);
                break;
            case "6":
                ExibirExtrato(portfolio);
                break;
            case "0":
                return;
            default:
                Console.WriteLine("Opção inválida. Tente novamente.");
                break;
        }

        Console.WriteLine();
    }
}

void ExibirMenu()
{
    Console.WriteLine("------------------------- MENU -------------------------");
    Console.WriteLine("  1 - Consultar cotação do Bitcoin");
    Console.WriteLine("  2 - Atualizar cotação do Bitcoin");
    Console.WriteLine("  3 - Ver saldos da carteira");
    Console.WriteLine("  4 - Comprar Bitcoin");
    Console.WriteLine("  5 - Vender Bitcoin");
    Console.WriteLine("  6 - Exibir extrato e patrimônio");
    Console.WriteLine("  0 - Sair");
    Console.WriteLine("--------------------------------------------------------");
    Console.Write("Escolha uma opção: ");
}

// ----------------------------------------------------------------------------
// Ações do menu
// ----------------------------------------------------------------------------
void ConsultarCotacao(Portfolio portfolio)
{
    Console.WriteLine();
    Console.WriteLine($"Cotação atual do Bitcoin: {FormatarBrl(portfolio.Market.PricePerBtc)} por 1 BTC");
}

void AtualizarCotacao(Portfolio portfolio)
{
    Console.WriteLine();
    decimal novaCotacao = LerDecimal("  Nova cotação do Bitcoin (R$ por BTC)", padrao: null, minimo: 0.01m);
    portfolio.Market.UpdatePrice(novaCotacao);
    Console.WriteLine($"  Cotação atualizada para {FormatarBrl(novaCotacao)} por 1 BTC.");
}

void ExibirSaldos(Portfolio portfolio)
{
    Console.WriteLine();
    Console.WriteLine("------------------------- SALDOS -----------------------");
    Console.WriteLine($"  Saldo em BRL : {FormatarBrl(portfolio.Wallet.BrlBalance),18}");
    Console.WriteLine($"  Saldo em BTC : {FormatarBtc(portfolio.Wallet.BtcBalance),18}");
    Console.WriteLine($"  Patrimônio   : {FormatarBrl(portfolio.PatrimonyBrl),18}");
    Console.WriteLine("--------------------------------------------------------");
}

void ComprarBitcoin(Portfolio portfolio)
{
    Console.WriteLine();
    decimal valor = LerDecimal("  Valor da compra em BRL", padrao: null, minimo: 0.01m);

    try
    {
        Transaction transacao = portfolio.Buy(valor);
        Console.WriteLine($"  Compra realizada: {FormatarBtc(transacao.AmountBtc)} adquiridos por {FormatarBrl(transacao.AmountBrl)}.");
        Console.WriteLine($"  Cotação aplicada: {FormatarBrl(transacao.PricePerBtc)} por 1 BTC.");
    }
    catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
    {
        ExibirErro(ex);
    }
}

void VenderBitcoin(Portfolio portfolio)
{
    Console.WriteLine();
    decimal quantidade = LerDecimal("  Quantidade de BTC para vender", padrao: null, minimo: 0.00000001m);

    try
    {
        Transaction transacao = portfolio.Sell(quantidade);
        Console.WriteLine($"  Venda realizada: {FormatarBtc(transacao.AmountBtc)} vendidos por {FormatarBrl(transacao.AmountBrl)}.");
        Console.WriteLine($"  Cotação aplicada: {FormatarBrl(transacao.PricePerBtc)} por 1 BTC.");
    }
    catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
    {
        ExibirErro(ex);
    }
}

void ExibirExtrato(Portfolio portfolio)
{
    Console.WriteLine();
    Console.WriteLine("==================== EXTRATO DE TRANSAÇÕES ====================");

    if (portfolio.Statement.Count == 0)
    {
        Console.WriteLine("  Nenhuma transação realizada até o momento.");
    }
    else
    {
        Console.WriteLine($"  {"Data/Hora",-17}{"Operação",-10}{"Valor (R$)",15}{"Qtd. (BTC)",18}{"Cotação (R$)",15}");
        Console.WriteLine($"  {new string('-', 77)}");

        foreach (Transaction transacao in portfolio.Statement.Transactions)
        {
            string operacao = transacao.Type == TransactionType.Buy ? "COMPRA" : "VENDA";
            Console.WriteLine(
                $"  {transacao.TimestampUtc.ToString("dd/MM/yyyy HH:mm", cultura),-17}" +
                $"{operacao,-10}" +
                $"{transacao.AmountBrl.ToString("N2", cultura),15}" +
                $"{transacao.AmountBtc.ToString("N8", cultura),18}" +
                $"{transacao.PricePerBtc.ToString("N2", cultura),15}");
        }

        Console.WriteLine($"  {new string('-', 77)}");
        Console.WriteLine($"  Total comprado : {FormatarBrl(portfolio.Statement.TotalBoughtBrl)}");
        Console.WriteLine($"  Total vendido  : {FormatarBrl(portfolio.Statement.TotalSoldBrl)}");
        Console.WriteLine($"  Operações      : {portfolio.Statement.Count}");
    }

    decimal valorBtc = Math.Round(
        portfolio.Wallet.BtcBalance * portfolio.Market.PricePerBtc, 2, MidpointRounding.AwayFromZero);

    Console.WriteLine($"  Saldo atual    : {FormatarBrl(portfolio.Wallet.BrlBalance)} + {FormatarBtc(portfolio.Wallet.BtcBalance)} (valorizado: {FormatarBrl(valorBtc)})");
    Console.WriteLine($"  PATRIMÔNIO CONSOLIDADO: {FormatarBrl(portfolio.PatrimonyBrl)}");
    Console.WriteLine("================================================================");
}

// ----------------------------------------------------------------------------
// Auxiliares de interface
// ----------------------------------------------------------------------------
void ExibirErro(Exception ex)
    => Console.WriteLine($"  [ERRO] {ex.Message}");

decimal LerDecimal(string mensagem, decimal? padrao = null, decimal minimo = 0m)
{
    while (true)
    {
        string sufixo = padrao.HasValue
            ? $" (padrão: {padrao.Value.ToString("0.########", cultura)})"
            : string.Empty;

        Console.Write($"{mensagem}{sufixo}: ");

        string? linha = Console.ReadLine();
        if (linha is null)
        {
            return padrao ?? minimo; // fim da entrada padrão (EOF)
        }

        string texto = linha.Trim().Replace(',', '.');
        if (texto.Length == 0)
        {
            if (padrao.HasValue)
            {
                return padrao.Value;
            }

            Console.WriteLine("  Informe um valor.");
            continue;
        }

        if (!decimal.TryParse(texto, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal valor))
        {
            Console.WriteLine("  Valor inválido. Utilize apenas números (ex.: 1500,50).");
            continue;
        }

        if (valor < minimo)
        {
            Console.WriteLine($"  Informe um valor maior ou igual a {minimo.ToString("0.########", cultura)}.");
            continue;
        }

        return valor;
    }
}

CultureInfo ObterCultura()
{
    try
    {
        return new CultureInfo("pt-BR");
    }
    catch (CultureNotFoundException)
    {
        return CultureInfo.InvariantCulture;
    }
}

string FormatarBrl(decimal valor)
    => "R$ " + valor.ToString("N2", cultura);

string FormatarBtc(decimal valor)
    => valor.ToString("N8", cultura).TrimEnd('0').TrimEnd(',') + " BTC";

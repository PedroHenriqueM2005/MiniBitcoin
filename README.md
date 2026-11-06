# MiniBitcoin

**MiniBitcoin** é uma aplicação de console escrita em **C# / .NET 8** que simula a gestão de
uma carteira de Bitcoin: controle de saldos em reais (BRL) e em Bitcoin (BTC), cotação de
mercado simulada e atualizável, execução de ordens de compra e venda de frações de BTC e
extrato consolidado das operações com o patrimônio total.

> Projeto acadêmico da disciplina **Gestão e Qualidade de Software** — desenvolvido com
> **TDD (Test-Driven Development)** e histórico de versionamento aderente ao padrão
> **Conventional Commits**.

## Funcionalidades

| # | Requisito | Descrição |
|---|-----------|-----------|
| 1 | Gestão de carteira (Wallet) | Saldos em BRL e BTC (frações, com precisão de satoshi), com validações que impedem saldos iniciais negativos e débitos inválidos |
| 2 | Cotação e simulação de mercado | Preço do Bitcoin mantido no sistema, consultável e atualizável a qualquer momento |
| 3 | Compra e venda de Bitcoin | Ordens validadas (valor positivo, fundos suficientes, fração mínima de 1 satoshi) e conversão pela cotação atual |
| 4 | Extrato de transações | Histórico completo de compras e vendas (data/hora, valores, cotação aplicada) e patrimônio total consolidado em BRL |

## Arquitetura

A solução contém dois projetos:

```
MiniBitcoin/
├── MiniBitcoin.sln
├── MiniBitcoin.Console/            # aplicação de console
│   ├── Domain/                     # entidades de negócio
│   │   ├── Wallet.cs               # entidade Carteira (saldos BRL/BTC e validações)
│   │   ├── MarketQuote.cs          # cotação do Bitcoin
│   │   └── Transaction.cs          # registro imutável de compra/venda + tipo da transação
│   ├── Services/                   # lógica de aplicação
│   │   ├── TradingService.cs       # ordens de compra/venda e conversão pela cotação
│   │   ├── Statement.cs            # extrato, totais e patrimônio consolidado
│   │   └── Portfolio.cs            # fachada que integra carteira, mercado e extrato
│   └── Program.cs                  # interface interativa de console (menu)
└── MiniBitcoin.Tests/              # testes unitários (xUnit)
    ├── WalletTests.cs
    ├── MarketQuoteTests.cs
    ├── TradingServiceTests.cs
    ├── StatementTests.cs
    └── PortfolioTests.cs
```

**Separação de responsabilidades:**

- **Domain** — entidades de negócio puras (carteira, cotação e transação), sem dependência
  de interface ou de serviços;
- **Services** — lógica de aplicação: execução das ordens de compra/venda, registro no
  extrato e consolidação do patrimônio;
- **Program.cs (UI)** — apenas leitura de entrada do usuário, apresentação e tratamento de
  erros.

### Regras de negócio

- Saldos iniciais e valores de operação nunca podem ser negativos;
- Um débito só ocorre com fundos suficientes — se a operação for inválida, os saldos
  permanecem inalterados (validação acontece antes da movimentação);
- A cotação do Bitcoin é sempre positiva;
- Compras são convertidas com precisão de **8 casas decimais** (1 BTC = 100.000.000
  satoshis); valores que arredondem para menos de 1 satoshi são rejeitados;
- Vendas são convertidas para BRL com 2 casas decimais;
- **Patrimônio consolidado** = saldo em BRL + (saldo em BTC × cotação atual).

## Pré-requisitos

- [.NET SDK 8.0](https://dotnet.microsoft.com/download/dotnet/8.0) ou superior.

## Como executar

```bash
git clone https://github.com/PedroHenriqueM2005/MiniBitcoin.git
cd MiniBitcoin
dotnet run --project MiniBitcoin.Console/MiniBitcoin.Console.csproj
```

No início, o sistema solicita o saldo inicial em BRL, o saldo inicial em BTC e a cotação
inicial do Bitcoin (pressione ENTER para aceitar os padrões: R$ 10.000,00 / 0 BTC /
R$ 350.000,00). Em seguida, o menu interativo oferece:

```
 1 - Consultar cotação do Bitcoin
 2 - Atualizar cotação do Bitcoin
 3 - Ver saldos da carteira
 4 - Comprar Bitcoin
 5 - Vender Bitcoin
 6 - Exibir extrato e patrimônio
 0 - Sair
```

## Como executar os testes

```bash
dotnet test
```

A suíte (42 testes) cobre:

- **Carteira:** saldos iniciais válidos/zerados/negativos, débitos com saldo suficiente,
  exatamente igual ao saldo e insuficiente, créditos/débitos com valores inválidos;
- **Cotação:** inicialização e atualização com preços válidos e inválidos;
- **Compra e venda:** conversão correta pela cotação, acumulação de operações, saldo
  insuficiente (com preservação dos saldos), valores inválidos, compra abaixo da fração
  mínima de 1 satoshi e venda com cotação atualizada;
- **Extrato e patrimônio:** registro ordenado das transações, totais comprado/vendido,
  patrimônio consolidado com cotação atualizada e o fluxo completo de integração
  (compra → venda → extrato → patrimônio).

## Metodologia — TDD + Conventional Commits

Cada etapa foi desenvolvida no ciclo **vermelho → verde** do TDD: primeiro os testes
unitários (commit `test`) — que falham — e, em seguida, a implementação (commit `feat`)
que os faz passar. Por isso, os commits de teste precedem os de implementação no histórico.

| Tipo | Uso no projeto |
|------|----------------|
| `chore` | Estrutura inicial da solução e dos projetos |
| `test` | Suítes de testes unitários de cada etapa (TDD) |
| `feat` | Funcionalidades: carteira, transações, extrato e menu de console |
| `refactor` | Reorganização de namespaces (Domain/Services) e legibilidade |
| `docs` | Documentação (este README) |

Histórico de commits (`git log --oneline`):

```
docs: add comprehensive instructions in README.md
refactor(core): improve code readability and organize namespaces
feat(console): implement interactive menu for bitcoin wallet application
feat(extrato): implement transaction history and portfolio summary
test(extrato): add unit tests for transaction ledger and portfolio valuation
feat(transacao): implement bitcoin purchase and sale logic with quote validation
test(transacao): add unit tests for bitcoin buy and sell operations
feat(carteira): implement wallet entity and initial balance rules
test(carteira): add unit tests for wallet balance validation
chore: initial project structure with console and xunit apps
```

## Autores

- [Pedro Henrique Martins](https://github.com/PedroHenriqueM2005)

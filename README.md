# TDE - Gerador de Relatórios

Projeto desenvolvido para a disciplina de Padrões de Projeto de Software do curso de Análise e Desenvolvimento de Sistemas.

## Objetivo

Desenvolver uma aplicação em C# e posteriormente realizar sua refatoração aplicando os princípios SOLID e o padrão criacional Factory Method.

## Cenário

A aplicação será um sistema de geração e exportação de relatórios em diferentes formatos:

- PDF
- CSV
- JSON

O projeto será desenvolvido inicialmente em uma versão funcional, porém propositalmente mal estruturada, para posteriormente demonstrar o processo de refatoração.

## Tecnologias

- C#
- .NET
- Git
- GitHub

## Como Executar

O diretório principal contém a solução e múltiplos projetos. Para executar os comandos corretamente, especifique o arquivo do projeto ou solução:

**Para compilar o projeto:**
```bash
dotnet build TdeGeradorRelatorios.sln
```

**Para executar a aplicação:**
```bash
dotnet run --project tde-gerador-relatorios.csproj
```

**Para rodar os testes:**
```bash
dotnet test TdeGeradorRelatorios.sln
```

## Etapas

- [x] Implementação inicial
- [x] Identificação das violações SOLID
- [x] Refatoração
- [x] Aplicação dos princípios SOLID
- [x] Implementação do Factory Method
- [x] Testes
- [x] Documentação

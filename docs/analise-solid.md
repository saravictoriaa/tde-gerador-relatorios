# Análise Arquitetural da Versão Inicial

## 1. Objetivo

Este documento apresenta a análise arquitetural da primeira versão do sistema gerador de relatórios. O objetivo é mapear a estrutura do código antes da refatoração, identificando problemas de acoplamento e distribuição de responsabilidades, de modo a fundamentar tecnicamente a aplicação dos princípios SOLID e do padrão criacional *Factory Method*.

---

## 2. Situação Atual

A implementação inicial segue uma abordagem procedural, concentrando o fluxo de execução na classe `Program` (caracterizando o antipadrão *God Class*). Atualmente, as seguintes responsabilidades estão agrupadas no mesmo arquivo:

- Captura de dados via interface de linha de comando.
- Validação de regras de negócio (campos obrigatórios) e geração automática de datas.
- Seleção do formato de saída por meio de uma estrutura condicional (`switch`).
- Formatação de dados e integração com bibliotecas específicas (QuestPDF e `System.Text.Json`).
- Operações de I/O para gravação dos arquivos em disco.

---

## 3. Análise dos Princípios SOLID

Abaixo, detalhamos como o código atual se relaciona com os cinco princípios SOLID, identificando as violações existentes e os casos não aplicáveis.

### 3.1 SRP — Princípio da Responsabilidade Única (Single Responsibility Principle)
**Status: Violação identificada.**

A classe `Program` concentra múltiplas razões para ser modificada. Qualquer ajuste na interface de usuário, na formatação do JSON ou na biblioteca de geração de PDF exige alterações na mesma classe. Essa sobreposição de responsabilidades entre Apresentação, Domínio e Infraestrutura reduz a coesão do código e torna a manutenção mais complexa.

### 3.2 OCP — Princípio do Aberto/Fechado (Open/Closed Principle)
**Status: Violação identificada.**

O roteamento do formato de relatório depende diretamente de um bloco `switch (opcao)`. Caso o escopo do projeto cresça e exija um novo formato, como XML, será obrigatório modificar a classe principal adicionando um novo `case`. A arquitetura atual falha em permitir a extensão do comportamento sem a alteração do código já existente.

### 3.3 LSP — Princípio da Substituição de Liskov (Liskov Substitution Principle)
**Status: Não aplicável na versão atual.**

O princípio de Liskov garante que classes derivadas possam substituir suas classes base sem comprometer a integridade do sistema. Como a implementação atual não possui interfaces, classes abstratas ou qualquer hierarquia de herança, o princípio não se aplica neste momento.

### 3.4 ISP — Princípio da Segregação de Interfaces (Interface Segregation Principle)
**Status: Não aplicável na versão atual.**

O ISP orienta a criação de interfaces específicas e coesas para não forçar classes a implementarem métodos não utilizados. Devido à ausência total de abstrações e interfaces nesta etapa inicial, não há violação a ser registrada.

### 3.5 DIP — Princípio da Inversão de Dependência (Dependency Inversion Principle)
**Status: Violação identificada.**

O módulo principal da aplicação (alto nível) depende diretamente de implementações concretas e detalhes de infraestrutura (baixo nível), e não de abstrações. 

Exemplos desse forte acoplamento incluem:
- Instanciação direta da biblioteca QuestPDF (`Document.Create`).
- Dependência estática das configurações do `JsonSerializer` (como o `JavaScriptEncoder`).
- Chamadas diretas de métodos do sistema operacional para acesso ao disco (`File.WriteAllText`).

Dessa forma, o núcleo da aplicação conhece exatamente os mecanismos de formatação e persistência, inviabilizando a substituição ou os testes automatizados dessas operações sem impactar o serviço principal.

---

## 4. Situação Após a Refatoração

A refatoração introduziu uma estrutura em camadas bem definidas, aplicando o padrão criacional *Factory Method* e separando as responsabilidades em classes coesas. A seguir, analisamos como cada princípio SOLID foi tratado na versão refatorada.

### Arquitetura resultante

```
Program
  └── seleciona RelatorioCreator conforme entrada do usuário
        └── RelatorioCreator (classe abstrata — Creator)
              ├── CriarGerador() : IGeradorRelatorio   ← Factory Method
              └── Gerar(Relatorio, string)              ← Template Method

ConcreteCreators            ConcreteProducts
  PdfCreator   →  cria →   GeradorPdf   : IGeradorRelatorio
  CsvCreator   →  cria →   GeradorCsv   : IGeradorRelatorio
  JsonCreator  →  cria →   GeradorJson  : IGeradorRelatorio

Models
  Relatorio (record) — dados do domínio

Interfaces
  IGeradorRelatorio — contrato único: Gerar(Relatorio, string)
```

---

### 4.1 SRP — Princípio da Responsabilidade Única

**Status: Tratado.**

Cada classe passou a ter uma única razão para mudar:

- `GeradorPdf` — responsável exclusivamente pela geração de documentos PDF via QuestPDF.
- `GeradorCsv` — responsável pela formatação e gravação de arquivos CSV com UTF-8 BOM e separador `;`.
- `GeradorJson` — responsável pela serialização JSON com preservação de acentuação.
- `RelatorioCreator` — responsável pelo fluxo de criação e delegação da geração.
- `Program` — responsável pela interação com o usuário, coleta de dados e seleção do Creator.

A lógica de geração de PDF, CSV e JSON saiu do `Program` e foi encapsulada em classes dedicadas. Uma alteração na biblioteca QuestPDF, por exemplo, afeta apenas `GeradorPdf`, sem tocar no restante do sistema.

---

### 4.2 OCP — Princípio do Aberto/Fechado

**Status: Tratado.**

Adicionar um novo formato de relatório (por exemplo, XML) não exige modificar nenhuma das classes existentes. O processo se resume a:

1. Criar `GeradorXml : IGeradorRelatorio` — nova implementação do produto.
2. Criar `XmlCreator : RelatorioCreator` — novo Creator que retorna `GeradorXml`.
3. Adicionar um `case "4"` no `switch` do `Program` para mapear a entrada do usuário ao novo Creator.

O `switch` no `Program` não contém lógica de geração — ele apenas traduz a escolha do usuário para o Creator correspondente, que é responsabilidade natural da camada de apresentação. As classes de geração existentes permanecem sem alteração.

---

### 4.3 LSP — Princípio da Substituição de Liskov

**Status: Aplicável e respeitado.**

Na versão inicial, o princípio não era aplicável por ausência de hierarquia. Após a refatoração, existe uma hierarquia concreta: `PdfCreator`, `CsvCreator` e `JsonCreator` estendem `RelatorioCreator`.

O contrato de `RelatorioCreator` define dois comportamentos:
- `CriarGerador()` deve retornar uma implementação válida de `IGeradorRelatorio`.
- `Gerar(relatorio, caminho)` deve produzir um arquivo no caminho informado.

Todos os ConcreteCreators satisfazem esse contrato sem alterar pré-condições, pós-condições ou invariantes. O teste `QualquerCreator_UsadoComoRelatorioCreator_DeveGerarArquivo` demonstra isso diretamente: o cliente opera exclusivamente sobre `RelatorioCreator` e o comportamento se mantém correto independentemente de qual subclasse está em uso.

---

### 4.4 ISP — Princípio da Segregação de Interfaces

**Status: Boa aderência.**

A interface `IGeradorRelatorio` define um único método:

```csharp
void Gerar(Relatorio relatorio, string caminhoDestino);
```

Essa interface é coesa e não obriga nenhuma implementação a depender de comportamentos que não utiliza. `GeradorPdf`, `GeradorCsv` e `GeradorJson` implementam apenas o que precisam. Não há métodos desnecessários nem interfaces excessivamente genéricas.

Cabe observar que a boa aderência ao ISP neste projeto é favorecida pela simplicidade do domínio — um único ponto de variação (o formato de saída) com uma única operação significativa (gerar). Em sistemas maiores, esse equilíbrio exigiria atenção contínua.

---

### 4.5 DIP — Princípio da Inversão de Dependência

**Status: Parcialmente tratado.**

O `Program` passou a depender de `RelatorioCreator` (abstração), e não das implementações concretas dos geradores. O fluxo principal não conhece `GeradorPdf`, `GeradorCsv` ou `GeradorJson` diretamente.

```csharp
// Program.cs — depende da abstração, não dos concretos
RelatorioCreator creator = new PdfCreator();
creator.Gerar(relatorio, caminho);
```

No entanto, o `Program` ainda instancia diretamente os ConcreteCreators (`new PdfCreator()`, `new CsvCreator()`, `new JsonCreator()`). Para uma inversão completa, seria necessário introduzir injeção de dependência ou um mecanismo de resolução externo. Dado o escopo acadêmico e o tamanho do projeto, essa limitação é aceitável e documentada — a dependência concreta ficou restrita ao ponto de entrada da aplicação, que é o local mais adequado para decisões de composição.

---

## 5. Comparação Antes × Depois

| Princípio | Versão Inicial | Versão Refatorada |
|-----------|---------------|-------------------|
| **SRP** | `Program` concentrava geração de PDF, CSV e JSON, validação, I/O e interação | Cada gerador tem responsabilidade única; `Program` coordena apenas o fluxo de entrada |
| **OCP** | Novo formato exigia modificar o `switch` com lógica de geração no `Program` | Novo formato = novo `Creator` + novo `Gerador`; código existente não é alterado |
| **LSP** | Não aplicável — sem hierarquia de herança | Respeitado — ConcreteCreators substituem `RelatorioCreator` sem quebrar o contrato do cliente |
| **ISP** | Não aplicável — sem interfaces | Boa aderência — `IGeradorRelatorio` tem um único método coeso, sem obrigações desnecessárias |
| **DIP** | `Program` dependia diretamente de QuestPDF, `JsonSerializer` e `File` | `Program` depende de `RelatorioCreator` (abstração); instanciação dos concretos restrita ao ponto de entrada |
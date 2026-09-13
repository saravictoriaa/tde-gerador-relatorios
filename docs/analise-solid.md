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
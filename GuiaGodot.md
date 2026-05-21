# Guia Técnico de Desenvolvimento

Este documento é a **"Fonte da Verdade"** para o grupo de programação. Ele detalha a stack, o fluxo de trabalho e as diretrizes para o desenvolvimento do nosso jogo Pixel Art 2D Top-Down focado exclusivamente em **Desktop (Windows/Linux)**.

---

## 1. Stack Tecnológica (Atualizada)

- **Engine:** Godot 4.x (Versão .NET/C#).
- **Linguagem:** C# (Utilizando **.NET 10**).
- **Renderizador:** **Forward+** (Clustered Forward) - _Melhor suporte para luzes, sombras e efeitos modernos no PC_.
- **Backend & Local Data:** - **Firebase:** Auth e Firestore (via SDK C# nativo ou REST).
- **Local:** SQLite ou JSON (para configurações e cache offline).

- **Versionamento:** Git (GitHub) via Terminal/CLI.

---

## 2. Configuração do Ambiente de Desenvolvimento

Cada programador deve garantir a padronização das ferramentas para evitar erros de compilação cruzada.

### Instalações Necessárias

1. **Godot .NET Edition:** Versão estável mais recente (4.x Mono).
2. **SDK .NET 10:** Obrigatório para suporte às features mais recentes da linguagem.
3. **IDE Recomendada:** VS Code com **C# Dev Kit** ou Visual Studio 2022+.

---

## 3. Arquitetura de Código: Foco em Desktop

Sem as limitações de outros dispositivos, podemos explorar melhor o hardware e o sistema:

### O que muda no C#

- **Multithreading Real:** Podemos usar `Task.Run()` e threads em segundo plano para processamento pesado (geração procedural, IA complexa) sem as restrições de COOP/COEP da Web.
- **Sistema de Arquivos:** Uso total de `System.IO`. Podemos criar logs locais, salvar replays e gerenciar arquivos de configuração em `user://` (AppData/Local).
- **Performance:** Menos preocupação com o tamanho do binário final. Podemos usar bibliotecas NuGet pesadas se necessário.

---

## 4. Fluxo de Trabalho e Estrutura (Actors & System)

Para um grupo de 4 programadores, a **composição** é a regra de ouro:

### Organização de Cenas

- **Padronização por Domínio:** Cada entidade é uma pasta contendo `.tscn`, `.cs` e recursos específicos (ex: `res://src/actors/player/`).
- **Componentização:** Use o padrão de "Nós de Componente".
- _Exemplo:_ Um nó `HealthComponent.cs` que pode ser arrastado tanto para o Player quanto para o Inimigo.

### Git Rules (PC Edition)

1. **LFS (Large File Storage):** Ativar Git LFS para os assets de arte e áudio.
2. **Branches:** Trabalhar com `feature/nome-da-tarefa`. Nunca fazer push direto na `main`.
3. **Merge Conflicts:** Evitem editar o mesmo `.tscn` simultaneamente. Priorizem editar scripts `.cs` separados.

---

## 5. Especificações Técnicas (Programação & Arte)

Diretrizes para garantir o "feeling" de um jogo de PC de alta qualidade:

- **Input System:** Usar o `Input Map` da Godot suportando Teclado + Mouse e Gamepad (XInput).
- **Pixel Art Fidelity:**
- **Texture Filter:** `Nearest` global.
- **Window Mode:** Suporte a _Borderless Fullscreen_ e _Window Resizing_.
- **Y-Sorting:** Obrigatório no `Node2D` pai dos atores para profundidade visual.

- **Resolução:** O jogo deve ser projetado para $1920 \times 1080$ (Full HD) nativo ou escalado perfeitamente a partir de uma base menor.

## 7. Controle de Qualidade (QA)

- **Logs de Erro:** Implementar um sistema que gera um arquivo `.log` em caso de crash (facilita o debug entre os membros do grupo).
- **Stress Test:** Como o foco é PC, testar o jogo em diferentes resoluções e taxas de atualização (60Hz, 144Hz). Use `delta` em todos os cálculos de movimento para garantir independência de framerate.
- **Code Review:** Antes de cada merge, pelo menos um outro programador deve revisar o C# para garantir que os padrões de arquitetura estão sendo seguidos.

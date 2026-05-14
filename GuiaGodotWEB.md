# Guia Técnico de Desenvolvimento: Projeto Web Game (IFAM)

Este documento serve como a "Fonte da Verdade" para o grupo de programação (4 membros). Ele detalha a stack tecnológica, o fluxo de trabalho e as restrições específicas para o desenvolvimento do nosso jogo Pixel Art 2D Top-Down focado exclusivamente em **Web**.

---

## 1. Stack Tecnológica (Aprovada)

- **Engine:** Godot 4.x (Versão .NET/C#).
- **Linguagem:** C# (Utilizando .NET 10).
- **Renderizador:** Compatibility (OpenGL 3 / WebGL 2) - **Obrigatório para Web**.
- **Backend & Hospedagem:** Firebase (Hosting, Firestore e Auth).
- **Versionamento:** Git (GitHub) via Terminal/CLI.

---

## 2. Configuração do Ambiente de Desenvolvimento

Cada um dos 4 programadores deve garantir que o comando `dotnet --version` retorne `10.x` no terminal.

### Instalações Necessárias

1.  **Godot .NET Edition:** Certifique-se de usar a versão Mono.
2.  **SDK .NET 10:** Configurado no PATH do sistema.
3.  **WASM Tools:** Execute `dotnet workload install wasm-tools` para habilitar a compilação web.
4.  **VS Code + C# Dev Kit:** Editor externo recomendado para melhor suporte ao C#.

---

## 3. Arquitetura de Código: Web vs. PC

Diferente do desenvolvimento para Desktop, nossa arquitetura deve respeitar as limitações do navegador:

### O que muda no C#

- **Assincronismo:** Todas as chamadas ao Firebase devem ser `async/await`. Nunca trave a Main Thread, ou o navegador marcará o jogo como "Sem Resposta".
- **Sistema de Arquivos:** Não use `System.IO` puro. Utilize a API da Godot (`FileAccess`) ou, preferencialmente, salve o estado do jogo no **Firebase**.
- **Gerenciamento de Memória:** O navegador limita a RAM da aba. Evite carregar todos os assets de uma vez; use carregamento dinâmico de cenas.

---

## 4. Fluxo de Trabalho e Git

Com 4 programadores e 4 artistas, a organização é vital para evitar conflitos:

1. **Estrutura de Cenas:** Cada objeto (Player, Inimigo, Bau) DEVE ser uma cena `.tscn` independente.
2. **Git Rules:**
   - Nunca edite a cena principal do nível ao mesmo tempo que outro colega.
   - Sempre dê `Pull` antes de começar e `Push` ao terminar uma tarefa pequena.
   - Mantenha a pasta `.godot/` e `bin/` no `.gitignore`.
3. **Namespaces em C#:** Organizem o código em namespaces (ex: `Game.Player`, `Game.UI`, `Game.Network`) para evitar colisões de nomes de classes.

---

## 5. Especificações de Pixel Art (Diretrizes para os Artistas)

Para manter a fidelidade visual no navegador:

- **Texture Filter:** Deve ser configurado como **Nearest** (Geral e por Sprite).
- **Resolução Base:** Recomendado $320 \times 180$ ou $640 \times 360$ (Escalável para 16:9).
- **Y-Sorting:** Ativar `Z Index -> As Relative` e `Y Sort Enabled` no Node pai para garantir que o Player passe por trás/frente de objetos corretamente.

---

## 6. Integração Firebase

- **Método:** Utilizaremos a **REST API** do Firebase via classe `HTTPRequest` da Godot. Isso é mais leve para WebAssembly do que importar o SDK completo do Firebase C#.
- **Hospedagem:** O deploy final será via `firebase deploy`. O arquivo `firebase.json` deve conter os headers de COOP/COEP para permitir que o multithreading da Godot 4 funcione.

---

## 7. Ressalvas e Dicas

- **Áudio na Web:** O jogo começará mudo. Implemente uma tela de "Clique para Iniciar" para satisfazer a política de segurança dos navegadores.
- **Teste Constante:** Façam um export para Web ao final de cada sprint. Bugs que funcionam no PC mas quebram no navegador são comuns.
- **Singletons:** Usem um `GameManager.cs` (Autoload) para gerenciar o estado global e a comunicação com o Firebase.

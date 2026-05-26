using Godot;
using System;

/// <summary>
/// Gerenciador de cenas centralizado para o jogo.
/// </summary> <remarks>
/// Este nó deve ser adicionado à cena principal (por exemplo, <c>Main.tscn</c>) para fornecer acesso global ao carregamento e gerenciamento de cenas.
/// Ele segue o padrão singleton para garantir que haja apenas uma instância ativa durante a execução do jogo.
/// </remarks>
/// <example>
/// Para usar o gerenciador, adicione um nó <c>SceneManager</c> à cena principal e chame seus métodos para carregar cenas de jogo ou UI:
/// <code>
/// SceneManager.Instance.LoadScene("res://src/scenes/bedroom.tscn");
/// SceneManager.Instance.LoadUIScene("res://src/scenes/ui_menu.tscn");
/// </code>
/// </example>r
public sealed partial class SceneManager : Node
{
    #region Singleton Pattern
    public static SceneManager Instance { get; private set; }

    private SceneManager() { }
    public override void _EnterTree()
    {
        if (Instance != null)
        {
            GD.PrintErr("Multiple instances of SceneManager detected. This should not happen.");
            QueueFree();
            return;
        }

        Instance = this;
    }

    public override void _ExitTree()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
    #endregion

    #region Getters

    // Nodes futuramente guardados.
    private Node masterNode = null;
    private Node2D sceneNode2D = null;
    private Control uiNode = null;

    /// <summary>
    /// Retorna o nó raiz <c>Main</c> da cena principal.
    /// </summary>
    /// <returns>O nó <c>Main</c>, ou <c>null</c> se não estiver presente.</returns>
    public Node GetMasterNode()
    {
        // processa só na primeira chamada, 
        // depois guarda o resultado para evitar buscas repetidas
        if (masterNode == null)
        {
            masterNode = GetTree().Root.GetNode<Node>("Main");
            if (masterNode == null)
            {
                GD.PrintErr("Main node not found. Ensure there is a node named 'Main' in the scene.");
            } 
            return masterNode;
        } 
        return masterNode;
    }

    /// <summary>
    /// Retorna o nó <c>Scene</c> usado para instanciar cenas de jogo.
    /// </summary>
    /// <returns>O nó <c>Scene</c> como <c>Node2D</c>, ou <c>null</c> se não existir.</returns>
    public Node2D GetSceneNode2D()
    {
        if (sceneNode2D == null)
        {
            var masterNode = GetMasterNode();
            if (masterNode != null)
            {
                sceneNode2D = masterNode.GetNode<Node2D>("Scene");
                if (sceneNode2D == null)
                {
                    GD.PrintErr("Scene node not found. Ensure there is a Node2D named 'Scene' under the Master node.");
                }
            }
        }
        return sceneNode2D;
    }

    /// <summary>
    /// Retorna o nó <c>UI</c> usado para carregar interfaces de usuário.
    /// </summary>
    /// <returns>O nó <c>UI</c> como <c>Control</c>, ou <c>null</c> se não existir.</returns>
    public Control GetUINode()
    {
        if (uiNode == null)
        {
            var masterNode = GetMasterNode();
            if (masterNode != null)
            {
                uiNode = masterNode.GetNode<Control>("UI");
                if (uiNode == null)
                {
                    GD.PrintErr("UI node not found. Ensure there is a Control node named 'UI' under the Master node.");
                }
            }
        }
        return uiNode;
    }
    #endregion

    #region Scene Management
    /// <summary>
    /// Carrega e instancia uma cena de jogo dentro do nó <c>Scene</c>.
    /// </summary>
    /// <param name="scenePath">Caminho do recurso de cena (por exemplo, <c>res://src/scenes/bedroom.tscn</c>).</param>
    /// <remarks>
    /// Antes de adicionar a nova cena, todos os filhos de <c>Scene</c> são liberados.
    /// </remarks>
    /// <example>
    /// <code>
    /// SceneManager.Instance.LoadScene("res://src/scenes/bedroom.tscn");
    /// </code>
    /// </example>
    public void LoadScene(string scenePath)
    {
        var newScene = GD.Load<PackedScene>(scenePath);
        if (newScene == null)
        {
            GD.PrintErr($"Failed to load scene at path: {scenePath}");
            return;
        }

        var sceneNode = GetSceneNode2D();
        if (sceneNode == null)
        {
            GD.PrintErr("Scene node not found. Cannot load scene.");
            return;
        }

        // Clear existing children
        ClearScene();

        // Instance and add the new scene
        var newSceneInstance = newScene.Instantiate<Node2D>();
        sceneNode.AddChild(newSceneInstance);
    }

    /// <summary>
    /// Carrega e instancia uma cena de interface dentro do nó <c>UI</c>.
    /// </summary>
    /// <param name="scenePath">Caminho do recurso de UI (por exemplo, <c>res://src/scenes/ui_menu.tscn</c>).</param>
    /// <example>
    /// <code>
    /// SceneManager.Instance.LoadUIScene("res://src/scenes/ui_menu.tscn");
    /// </code>
    /// </example>
    public void LoadUIScene(string scenePath)
    {
        var newScene = GD.Load<PackedScene>(scenePath);
        if (newScene == null)
        {
            GD.PrintErr($"Failed to load UI scene at path: {scenePath}");
            return;
        }

        var uiNode = GetUINode();
        if (uiNode == null)
        {
            GD.PrintErr("UI node not found. Cannot load UI scene.");
            return;
        }

        // Clear existing children
        ClearUI();

        // Instance and add the new UI scene
        var newSceneInstance = newScene.Instantiate<Control>();
        uiNode.AddChild(newSceneInstance);
    }
    #endregion

    #region Utility Methods
    /// <summary>
    /// Remove todos os filhos do nó <c>Scene</c> sem destruir o próprio nó raiz.
    /// </summary>
    public void ClearScene()
    {
        var sceneNode = GetSceneNode2D();
        if (sceneNode == null)
        {
            GD.PrintErr("Scene node not found. Cannot clear scene.");
            return;
        }

        foreach (Node child in sceneNode.GetChildren())
        {
            child.QueueFree();
        }
    }

    /// <summary>
    /// Remove todos os filhos do nó <c>UI</c>, mantendo a raiz de interface.
    /// </summary>
    public void ClearUI()
    {
        var uiNode = GetUINode();
        if (uiNode == null)
        {
            GD.PrintErr("UI node not found. Cannot clear UI.");
            return;
        }

        foreach (Node child in uiNode.GetChildren())
        {
            child.QueueFree();
        }
    }
    #endregion

    #region Debugging
    /// <summary>
    /// Exibe no console o nome do nó mestre atual usado pelo gerenciador.
    /// </summary>
    /// <example>
    /// <code>
    /// SceneManager.Instance.PrintCurrentScene();
    /// </code>
    /// </example>
    public void PrintCurrentScene()
    {
        var masterNode = GetMasterNode();
        if (masterNode == null)
        {
            GD.PrintErr("Master node not found. Cannot print current scene.");
            return;
        }

        GD.Print($"Current Scene: {masterNode.Name}");
    }
    #endregion
}

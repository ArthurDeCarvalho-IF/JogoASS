using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

public partial class DialogueManager : Node
{
    public static DialogueManager Instance { get; private set; }

    [Export(PropertyHint.File, "*.json")] public string DialogueDataPath = "res://assets/dialogue/falas.json";
    [Export(PropertyHint.File, "*.tscn")] public string DialogueScenePath = "res://src/scenes/dialoguebx.tscn";
    [Export] public NodePath DialogueTextLabelPath = "CanvasLayer/Panel/MarginContainer/RichTextLabel";
    [Export] public string InteractActionName = "game_interact";

    public bool isInDialogue { get; private set; }

    // Runtime caches for fast lookup and overlap tracking.
    private readonly Dictionary<string, DialogueEntry> dialogueById = new();
    private readonly List<DialogueInteractable> availableInteractables = new();

    private DialogueEntry activeDialogueEntry;
    private DialogueInteractable activeInteractable;
    private RichTextLabel activeDialogueLabel;
    private Control activeDialogueControl;

    public override void _EnterTree()
    {
        if (Instance != null && Instance != this)
        {
            GD.PrintErr("Multiple instances of DialogueManager detected. ???");
            QueueFree();
            return;
        }

        Instance = this;
    }

    public override void _ExitTree()
    {
        if (Instance == this)
        {
            Instance = null; // Clean up singleton reference on scene change or removal.
        }
    }

    public override void _Ready()
    {
        LoadDialogueEntries();
    }

    public override void _Process(double delta)
    {
        // Single interaction entrypoint:
        // - If dialogue is active, advance it.
        // - If dialogue is inactive, try to start from the current interactable.
        if (!Input.IsActionJustPressed(InteractActionName))
        {
            return;
        }

        if (isInDialogue)
        {
            AdvanceDialogue();
            return;
        }

        if (activeInteractable == null)
        {
            return;
        }

        StartDialogue(activeInteractable.DialogueStartId);
    }

    public void RegisterInteractable(DialogueInteractable interactable)
    {
        if (interactable == null)
        {
            return;
        }

        if (!availableInteractables.Contains(interactable))
        {
            availableInteractables.Add(interactable);
        }

        // The last entered interactable becomes the current interaction target.
        activeInteractable = interactable;
    }

    public void UnregisterInteractable(DialogueInteractable interactable)
    {
        if (interactable == null)
        {
            return;
        }

        availableInteractables.Remove(interactable);

        if (activeInteractable != interactable)
        {
            return;
        }

        // Fallback: if there are still overlaps, use the most recently entered one.
        if (availableInteractables.Count == 0)
        {
            activeInteractable = null;
            return;
        }

        activeInteractable = availableInteractables[availableInteractables.Count - 1];
    }

    public bool StartDialogue(string dialogueId)
    {
        if (string.IsNullOrWhiteSpace(dialogueId))
        {
            GD.PrintErr("DialogueManager: dialogueId is empty.");
            return false;
        }

        if (!dialogueById.TryGetValue(dialogueId, out DialogueEntry dialogueEntry))
        {
            GD.PrintErr($"DialogueManager: dialogue id '{dialogueId}' was not found.");
            return false;
        }

        if (!PrepareDialogueUI())
        {
            return false;
        }

        activeDialogueEntry = dialogueEntry;
        isInDialogue = true;

        // Lock player state while dialogue is active.
        Player.Instance?.BeginTalkingState();
        UpdateDialogueText();
        return true;
    }

    public void EndDialogue()
    {
        if (!isInDialogue)
        {
            return;
        }

        isInDialogue = false;
        activeDialogueEntry = null;
        activeDialogueLabel = null;

        // Prefer explicit cleanup of the dialogue control we spawned.
        if (IsInstanceValid(activeDialogueControl))
        {
            activeDialogueControl.QueueFree();
            activeDialogueControl = null;
        }
        else if (SceneManager.Instance != null)
        {
            // Compatibility fallback for older scene flow.
            SceneManager.Instance.ClearUI();
        }

        // Unlock player state after dialogue closes.
        Player.Instance?.EndTalkingState();
    }

    private void AdvanceDialogue()
    {
        if (activeDialogueEntry == null)
        {
            EndDialogue();
            return;
        }

        string nextDialogueId = GetNextDialogueId(activeDialogueEntry);
        if (string.IsNullOrWhiteSpace(nextDialogueId))
        {
            EndDialogue();
            return;
        }

        if (!dialogueById.TryGetValue(nextDialogueId, out DialogueEntry nextEntry))
        {
            GD.PrintErr($"DialogueManager: next dialogue id '{nextDialogueId}' was not found.");
            EndDialogue();
            return;
        }

        activeDialogueEntry = nextEntry;
        UpdateDialogueText();
    }

    private void UpdateDialogueText()
    {
        if (activeDialogueEntry == null || activeDialogueLabel == null)
        {
            return;
        }

        string speaker = string.IsNullOrWhiteSpace(activeDialogueEntry.SpeakerName)
            ? "Actor"
            : activeDialogueEntry.SpeakerName;

        string speech = activeDialogueEntry.Speech ?? string.Empty;

        // The dialogue box label supports BBCode formatting.
        activeDialogueLabel.Text = $"[b]{speaker}:[/b]\n{speech}";
    }

    private bool PrepareDialogueUI()
    {
        PackedScene dialoguePackedScene = GD.Load<PackedScene>(DialogueScenePath);
        if (dialoguePackedScene == null)
        {
            GD.PrintErr($"DialogueManager: dialogue scene '{DialogueScenePath}' could not be loaded.");
            return false;
        }

        // Remove stale instance from a previous dialogue before loading a new one.
        if (IsInstanceValid(activeDialogueControl))
        {
            activeDialogueControl.QueueFree();
            activeDialogueControl = null;
        }

        Control dialogueScene = dialoguePackedScene.Instantiate<Control>();
        if (dialogueScene == null)
        {
            GD.PrintErr("DialogueManager: dialogue scene root is invalid.");
            return false;
        }

        Control uiNode = SceneManager.Instance?.GetUINode();
        if (uiNode != null)
        {
            // Preferred path: use the project's central UI root when available.
            uiNode.AddChild(dialogueScene);
        }
        else
        {
            // Fallback path: when a gameplay scene is run directly as main scene
            // (for example bedroom.tscn), attach UI to current scene root.
            Node fallbackParent = GetTree().CurrentScene ?? GetTree().Root;
            fallbackParent.AddChild(dialogueScene);
        }

        activeDialogueControl = dialogueScene;
        activeDialogueControl.ZIndex = 100;

        activeDialogueLabel = dialogueScene.GetNodeOrNull<RichTextLabel>(DialogueTextLabelPath);
        if (activeDialogueLabel == null)
        {
            GD.PrintErr($"DialogueManager: dialogue label '{DialogueTextLabelPath}' was not found.");
            if (IsInstanceValid(activeDialogueControl))
            {
                activeDialogueControl.QueueFree(); 
                activeDialogueControl = null;
            }
            return false;
        }

        return true;
    }

    private void LoadDialogueEntries() 
    {
        dialogueById.Clear();

        string fullPath = ProjectSettings.GlobalizePath(DialogueDataPath);
        if (!File.Exists(fullPath))
        {
            GD.PrintErr($"DialogueManager: dialogue file '{DialogueDataPath}' was not found.");
            return;
        }

        string rawJson = File.ReadAllText(fullPath);

        JsonSerializerOptions serializerOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        List<DialogueEntry> entries = JsonSerializer.Deserialize<List<DialogueEntry>>(rawJson, serializerOptions);
        if (entries == null)
        {
            GD.PrintErr($"DialogueManager: dialogue file '{DialogueDataPath}' is empty or invalid.");
            return;
        }

        foreach (DialogueEntry entry in entries)
        {
            if (entry == null || string.IsNullOrWhiteSpace(entry.Id))
            {
                continue;
            }

            dialogueById[entry.Id] = entry;
        }
    }

    private static string GetNextDialogueId(DialogueEntry entry)
    {
        // For now we follow the first branch so behavior stays deterministic.
        if (entry.Choices == null || entry.Choices.Count == 0)
        {
            return string.Empty;
        }

        // We intentionally keep the logic simple for now:
        // pick the first valid branch and ignore malformed/empty choices.
        foreach (DialogueChoice choice in entry.Choices)
        {
            if (!string.IsNullOrWhiteSpace(choice?.NextId))
            {
                return choice.NextId;
            }
        }

        return string.Empty;
    }

    // Data Transfer Objects used only for deserialization.
    private sealed class DialogueEntry
    {
        [JsonPropertyName("Id")]
        public string Id { get; set; }

        [JsonPropertyName("nome_npc")]
        public string SpeakerName { get; set; }

        [JsonPropertyName("fala")]
        public string Speech { get; set; }

        [JsonPropertyName("escolhas")]
        public List<DialogueChoice> Choices { get; set; } = new();
    }

    private sealed class DialogueChoice
    {
        [JsonPropertyName("label")]
        public string Label { get; set; }

        [JsonPropertyName("nextId")]
        public string NextId { get; set; }
    }
}

using Godot;

public partial class DialogueInteractable : Area2D //Responsável por registrar o player no DialogueManager quando ele entrar na área, e desregistrar quando sair.
{
    [Export] public string DialogueStartId = string.Empty;

    public override void _Ready()
    {
        // Keep setup warnings explicit so scene issues are easy to find.
        if (string.IsNullOrWhiteSpace(DialogueStartId))
        {
            GD.PrintErr($"{Name}: DialogueStartId is empty. This interactable will be ignored.");
        }
        BodyEntered += OnBodyEntered;
        BodyExited += OnBodyExited;
    }

    public override void _ExitTree()
    {
        // Safety cleanup for scene changes/removals.
        DialogueManager.Instance?.UnregisterInteractable(this);
    }

    private void OnBodyEntered(Node2D body)
    {
        // verificação não muito necessária pq n tem outro ser no jg), mas evita q se registre +1 vez enquanto parado na área
        if (body is not Player || string.IsNullOrWhiteSpace(DialogueStartId)) 
        {
            return;
        }

        DialogueManager.Instance?.RegisterInteractable(this);
    }

    private void OnBodyExited(Node2D body)
    {
        if (body is not Player)
        {
            return;
        }

        DialogueManager.Instance?.UnregisterInteractable(this);
    }
}

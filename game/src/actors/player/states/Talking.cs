using Godot;
using System;

public partial class Talking : PlayerState
{
    public override void Enter()
    {
        base.Enter();

        if (PlayerRef == null)
        {
            return;
        }

        // Reuse idle animation while dialogue keeps the player locked.
        PlayerRef.Sprite.Play("idle");
    }

    public override void Update(double delta)
    {
        if (PlayerRef == null)
        {
            return;
        }
        // While talking, we intentionally avoid all state transitions.
        // DialogueManager decides when to release this state via EndTalkingState().
    }

    public override void PhysicsUpdate(double delta)
    {
        if (PlayerRef == null)
        {
            return;
        }

        // Dialogue should fully freeze player movement.
        PlayerRef.velocity = Vector2.Zero;
        PlayerRef.Velocity = Vector2.Zero;
        PlayerRef.MoveAndSlide();
    }

    public override void Exit()
    {
    }
}
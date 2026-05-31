using Godot;
using System;
using System.Numerics;

public partial class Idle : PlayerState
{
	public override void Enter()
	{
        base.Enter();
        PlayerRef.Sprite.Play("idle");
	}
    public override void Update(double delta)
    {
        if (PlayerRef == null) return;
        // Dialogue lock has priority over movement transitions.
        if (PlayerRef.IsTalking)
        {
            Machine.TransitionTo("talking");
            return;
        }
        if (PlayerRef.DebugMode)
        {
            Machine.TransitionTo("debug");
            return;
        }
        if (PlayerRef.direction != Godot.Vector2.Zero) Machine.TransitionTo("run");
        
    }
    public override void PhysicsUpdate(double delta)
    {
        PlayerRef.Velocity = PlayerRef.velocity;
        PlayerRef.MoveAndSlide();
    }
    public override void Exit()
    {
        
    }
}

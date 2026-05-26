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
        if (PlayerRef.DebugMode) Machine.TransitionTo("debug");
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

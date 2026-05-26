using Godot;
using System;
using System.Numerics;

public partial class Run : PlayerState
{
	public override void Enter()
	{
        base.Enter();
        PlayerRef.Sprite.Play("run");
	}
    public override void Update(double delta)
    {
        if (PlayerRef == null) return;
        if (PlayerRef.DebugMode) Machine.TransitionTo("debug");
        if (PlayerRef.direction == Godot.Vector2.Zero) Machine.TransitionTo("idle"); 
        
    }
    public override void PhysicsUpdate(double delta)
    {
        PlayerRef.velocity = PlayerRef.Velocity; 
        PlayerRef.velocity.X = SmoothMove(
            PlayerRef.velocity.X,
            PlayerRef.MoveSpd * PlayerRef.direction.X,
            (float)delta * ((PlayerRef.direction.X == 0f) ? PlayerRef.fricttion : PlayerRef.acceleration)
        );
        PlayerRef.velocity.Y = SmoothMove(
            PlayerRef.velocity.Y,
            PlayerRef.MoveSpd * PlayerRef.direction.Y,
            (float)delta * ((PlayerRef.direction.Y == 0f) ? PlayerRef.fricttion : PlayerRef.acceleration)
        );

        
        PlayerRef.Velocity = PlayerRef.velocity;
        PlayerRef.MoveAndSlide();
    }
    public override void Exit()
    {
        
    }
    private float SmoothMove(float from, float to, float delta)
    {
        return Mathf.MoveToward(
            from, // base velocity 
            to, //moveSpeed * Direction
            delta // Smoothing * delta
        );
    }
}

using Godot;


public partial class Idle : PlayerState
{
	public override void Enter()
	{
        base.Enter();
        PlayerRef._animStateMachine.Travel("Idle");
	}
    public override void Update(double delta)
    {
        if (PlayerRef == null) return;
        if (PlayerRef.DebugMode) Machine.TransitionTo("debug");

        if (PlayerRef.direction != Godot.Vector2.Zero) Machine.TransitionTo("walk");   
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

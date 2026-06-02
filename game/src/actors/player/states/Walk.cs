using Godot;


public partial class Walk : PlayerState
{
	public override void Enter()
	{
        base.Enter();
        PlayerRef._animStateMachine.Travel("Walk");
	}
    public override void Update(double delta)
    {
        if (PlayerRef == null) return;
        if (PlayerRef.DebugMode) Machine.TransitionTo("debug");

        if (PlayerRef.direction == Vector2.Zero) Machine.TransitionTo("idle");
        else {
            PlayerRef._animTree.Set("parameters/Walk/blend_position", PlayerRef.direction);
            PlayerRef._animTree.Set("parameters/Idle/blend_position", PlayerRef.direction);   
        }
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

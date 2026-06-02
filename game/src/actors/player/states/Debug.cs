using Godot;
using System;

public partial class Debug : PlayerState
{
    bool collisionSwitch;
	public override void Enter()
	{
        base.Enter();   
	}
    public override void Update(double delta)
    {
        if (PlayerRef.DebugMode) Machine.TransitionTo("idle");
        if (PlayerRef.back) {
            onCollision(collisionSwitch);      
            collisionSwitch = !collisionSwitch;
            GD.Print(collisionSwitch);
        }
    }   
    public override void PhysicsUpdate(double delta)
    {
        PlayerRef.velocity = PlayerRef.Velocity;

        PlayerRef.velocity = PlayerRef.direction * PlayerRef.MoveSpd * 3;
        
        PlayerRef.Velocity = PlayerRef.velocity;
        PlayerRef.MoveAndSlide();
    }
    public override void Exit()
    {
        onCollision(true);
    }
    private void onCollision(bool button)
    {
        PlayerRef._collision.SetDeferred(CollisionShape2D.PropertyName.Disabled, !button);
    }
    private void RestartScene()
	{
		GetTree().ReloadCurrentScene();
	}
}
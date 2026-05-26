using Godot;
using System;

public partial class StateMachine : Node
{
	private Godot.Collections.Dictionary<string,State> statesList = new();
	[Export] public State BeginState;
	 public State ActualState;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready() 
    {
		foreach (State state in GetChildren())
		{
			statesList[state.Name.ToString().ToLower()] = state;
			state.Machine = this;
		}
		ActualState = BeginState;
		
		ActualState?.Enter();
	}
	public override void _Process(double delta)
    {
        ActualState?.Update(delta);
        // GD.Print("estado: "+ActualState.Name+" velv: "+Player.Instance.velocity+" direction: "+Player.Instance.direction.X);
    }
    public override void _PhysicsProcess(double delta){ActualState?.PhysicsUpdate(delta);}
	public void TransitionTo(String newState)
	{
		string key = newState.ToLower();
		if (newState == null ) return;

		ActualState?.Exit();
		ActualState = statesList[key];
		statesList[key]?.Enter();
	}
}

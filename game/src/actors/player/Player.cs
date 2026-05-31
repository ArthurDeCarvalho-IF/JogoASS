using Godot;
using System;
using System.Collections.Generic;

public partial class Player : CharacterBody2D
{	
	public static Player Instance {get; private set;}
	[Export] public float MoveSpd = 40f;
	[Export] public AnimatedSprite2D Sprite;
	[Export] public CollisionShape2D Collision;
	[Export] public Camera2D Camera;
	[Export] public float acceleration = 3000f;
	[Export] public float fricttion = 5000f;
	public Vector2 velocity;
	public Vector2 direction;
	public bool DebugMode;
	public bool back;
	public bool IsTalking { get; private set; }

	private StateMachine stateMachine;

	public override void _EnterTree() 
	{
		Instance = this;			
	}
    public override void _Ready()
    {
		stateMachine = GetNodeOrNull<StateMachine>("StateMachine");
	}

	public override void _PhysicsProcess(double delta)
	{
		direction = Input.GetVector("game_left", "game_right", "game_up", "game_down");
		DebugMode = Input.IsActionJustPressed("debug_mode");
		back = Input.IsActionJustPressed("game_back");
	}

	// Called by DialogueManager when a conversation starts.
	// It freezes movement-related values and moves the FSM to Talking.
	public void BeginTalkingState()
	{
		if (IsTalking) return;

		IsTalking = true;
		velocity = Vector2.Zero;
		Velocity = Vector2.Zero;
		direction = Vector2.Zero;

		stateMachine?.TransitionTo("talking");
	}

	// Called by DialogueManager when the conversation ends.
	// It releases the lock and restores an appropriate base state.
	public void EndTalkingState()
	{
		if (!IsTalking) return;

		IsTalking = false;

		if (stateMachine == null) return;

		string nextState = direction == Vector2.Zero ? "idle" : "run";
		stateMachine.TransitionTo(nextState);
	}
}

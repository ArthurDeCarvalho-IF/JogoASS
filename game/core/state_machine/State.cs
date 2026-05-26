using Godot;
using System;

public abstract partial class State : Node
{	
	public StateMachine Machine;
	public virtual void Enter(){}
	public virtual void Update(double delta){}
	public virtual void PhysicsUpdate(double delta){}
	public virtual void Exit(){}
}

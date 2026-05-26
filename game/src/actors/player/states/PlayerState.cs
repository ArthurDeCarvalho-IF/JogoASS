using Godot;
using System;

public abstract partial class PlayerState : State
{
    protected Player PlayerRef; 
    public override void Enter() {PlayerRef = Player.Instance;}
}
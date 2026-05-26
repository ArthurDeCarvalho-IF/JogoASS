using Godot;
using System;

public partial class Door : Area2D
{
    [Export] public int myID;
    [Export] public int targetID;
    [Export(PropertyHint.File, "*.tscn")] public string targetScene;
    public override void _Ready()
    {
        DoorsManager.Instance.RegisterDoor(myID,targetScene,targetID);
        this.BodyEntered += OnBodyEntered;
        this.BodyExited += OnBodyExited;
    }
    public void OnBodyEntered(Node2D Body)
    {
        if (!(Body is Player)) return;
        DoorsManager.Instance.startTeleport(myID);
    }
    public void OnBodyExited(Node2D Body)
    {
        if (!(Body is Player)) return;
        DoorsManager.Instance.canTeleport = true;


    }
}

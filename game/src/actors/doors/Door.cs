using Godot;
using System;

public partial class Door : Area2D
{
    [Export] public int myID;
    [Export] public int targetID;
    [Export(PropertyHint.File, "*.tscn")] public string targetScene;

    public override void _EnterTree()
    {
        DoorsManager.Instance.RegisterDoor(myID,targetID,targetScene,GlobalPosition);
    }
    public override void _Ready()
    {
        this.BodyEntered += OnBodyEntered;
        this.BodyExited += OnBodyExited;
        if (myID == DoorsManager.Instance.moveToDoor)
        {
            DoorsManager.Instance.canTeleport = false;
            CallDeferred(MethodName.ChangePlayerGlobalPosition);
        }
    }
    public void OnBodyEntered(Node2D Body)
    {
        if (Body is not Player && !DoorsManager.Instance.canTeleport) return;
        DoorsManager.Instance.startTeleport(myID);
    }
    public void OnBodyExited(Node2D Body)
    {
        if (Body is not Player) return;
        DoorsManager.Instance.canTeleport = true;
    }
    public void ChangePlayerGlobalPosition()
    {
        Player.Instance.GlobalPosition = DoorsManager.Instance.doorsInfos[myID].OwnerGlobalPosition;
    }
}

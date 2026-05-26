using Godot;
using System;
using System.Collections.Generic;

public partial class DoorsManager : Node
{
    public Dictionary<int,DoorData> doorsInfos = new();
    public static DoorsManager Instance {get; private set;}

    public int moveToDoor = 0;
    public bool canTeleport = false;
    public override void _EnterTree()
    {
        Instance = this;
    }

    public void RegisterDoor(int id, string ownerScene, int targetID)
    {
        doorsInfos[id] = new DoorData(ownerScene,targetID);
        GD.Print(doorsInfos);
    }

    public void startTeleport(int id)
    {
        canTeleport = false;
        GetTree().ChangeSceneToFile(doorsInfos[id].TargetScene);
        GD.Print("de: "+id +" | para: "+doorsInfos[id].TargetID +" | por: "+doorsInfos[id].TargetScene);
    }
}

public readonly struct DoorData
{
    public readonly string TargetScene;
    public readonly int TargetID;
    public DoorData(string scenePath, int targetID)
    {
        TargetScene = scenePath;
        TargetID = targetID;
    }

}



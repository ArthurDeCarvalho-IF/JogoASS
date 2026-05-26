using Godot;
using System;
using System.Collections.Generic;

public partial class DoorsManager : Node
{
    public Dictionary<int,DoorData> doorsInfos = new();
    public static DoorsManager Instance {get; private set;}

    public int moveToDoor = 0;
    public bool canTeleport = true;
    public override void _EnterTree()
    {
        Instance = this;
    }

    public void RegisterDoor(int ownerID, int targetID, string ownerScene, Vector2 ownerPosition)
    {
        doorsInfos[ownerID] = new DoorData(targetID,ownerScene, ownerPosition);
        GD.Print(doorsInfos);
    }

    public void startTeleport(int ID)
    {
        if (!canTeleport) return;
        canTeleport = false;
        
        DoorData doorInfos = doorsInfos[ID];

        moveToDoor = doorInfos.TargetID;

        // fade out 
        GetTree().CallDeferred(SceneTree.MethodName.ChangeSceneToFile,doorInfos.OwnerScenePath);
        // fade in
        GD.Print("de: "+ID +" | para: "+doorInfos.TargetID +" | por: "+doorInfos.OwnerScenePath +" | global pos: "+doorInfos.OwnerGlobalPosition);
    }
}

public readonly struct DoorData
{
    public readonly string OwnerScenePath;
    public readonly int TargetID;
    public readonly Vector2 OwnerGlobalPosition;
    public DoorData(int targetID,string ownerScenePath, Vector2 ownerPosition)
    {
        OwnerScenePath = ownerScenePath;
        TargetID = targetID;
        OwnerGlobalPosition = ownerPosition;
    }

}



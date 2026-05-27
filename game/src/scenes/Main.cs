using Godot;
using System;

class TestPlayerData : UserData
{
    public int Score { get; set; } = 0;
}


public sealed partial class Main : Node
{
    TestPlayerData playerData;
    public override void _Ready()
    {

        playerData = UserDataManager.Instance.RegisterUserData(
            new Identifier("debug", "test_player_data"), new TestPlayerData()
        );
        GD.Print(playerData);

        SceneManager.Instance.LoadScene("res://src/scenes/bedroom.tscn");
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("ui_accept"))
        {
            playerData.Score += 1;
            GD.Print($"Score: {playerData.Score}");
        }
    }

    public override void _ExitTree()
    {
    }
}

using Godot;
using System;


public sealed partial class Main : Node
{
    public override void _Ready()
    {
        SceneManager.Instance.LoadScene("res://src/scenes/bedroom.tscn");
    }
}

using Godot;
using System;

public class Start : Button
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        this.OnButtonDown(() => Scene.Load("res://Scenes/Game/Game.tscn"));
    }

    static void Restart(Command command)
    {
        Scene.Load("res://Scenes/Title/TitleScreen.tscn");
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}

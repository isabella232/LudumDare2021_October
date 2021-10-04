using Godot;
using System;

public class Witch : Control
{
    public Witch()
    {
        sprites = new Texture[]
        {
            GD.Load<Texture>("res://Assets/Witch/Witch_neutral.png"),
            GD.Load<Texture>("res://Assets/Witch/Witch_happy.png"),
            GD.Load<Texture>("res://Assets/Witch/Witch_annoyed.png"),
        };
    }

    public enum Mood
    {
        neutral,
        happy,
        angry,
    }

    Texture[] sprites;    

    public void SetText(Mood mood, string value)
    {
        this.FindChild<Label>().Text = value;
        this.FindChild<TextureRect>().Texture = sprites[(int)mood];
    }
}

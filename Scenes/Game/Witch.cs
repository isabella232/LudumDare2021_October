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
        this.FindChild<Label>().PercentVisible = 0;
        this.FindChild<TextureRect>().Texture = sprites[(int)mood];
    }
    
    AudioStreamPlayer audio;
    float percentage;
    public override void _Process(float delta)
    {
        var label = this.FindChild<Label>();
        label.PercentVisible += delta*2f;

        if (label.PercentVisible < 1 && Visible)
        {
            if (audio.IsNull())
            {
                audio = new AudioStreamPlayer();
                this.AddChild(audio);
                audio.Stream = GD.Load<AudioStream>("res://Assets/Witch/subTri_1a.wav");
            }
            audio.Play(0);
        }   
        else audio.Stop();
    }
}

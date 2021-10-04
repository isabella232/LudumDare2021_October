using Godot;
using System;

public class WalkingSFX : AudioStreamPlayer
{
    public static void Play()
    {
        if (!player.IsValid())
        {
            player = new WalkingSFX();
            Coroutine.DelayFrames(0, () => Scene.Current.AddChild(player));
        }
        player.count = 0;
    }

    static WalkingSFX player;

    AudioStream[] stream;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        stream = new AudioStream[]
        {
            GD.Load<AudioStream>("res://Assets/FootSteps/Foley Armour 01.wav"),
            GD.Load<AudioStream>("res://Assets/FootSteps/Foley Armour 02.wav"),
            GD.Load<AudioStream>("res://Assets/FootSteps/Foley Armour 03.wav"),
        };
    }

    int count;
    float timer= 1;

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        timer += delta;
        if (count < 3 && timer > .3f)
        {
            timer = 0;
            count ++;
            Stream = stream.GetRandom();
            Play(0);
        }
    }
}

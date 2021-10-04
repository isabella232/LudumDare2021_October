using Godot;
using System;

public class TitleBGM : AudioStreamPlayer
{
    public override void _Process(float delta)
    {
        if (!Playing)
            Play();
    }
}

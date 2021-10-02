using Godot;
using System;

public class TakeToWitch : Control
{
    public void SetText(string value)
    {
        this.FindChild<Label>().Text = value;
    }
}

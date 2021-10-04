using Godot;
using System;

public enum Game_Areas
{
    Home,
    Cave,
    Forrest,
    Graveyard,
    Lake
}

public class AreaSelection : TextureRect
{

    public override void _Ready()
    {
        var button = GetChild(0) as Button;
        button = button.Duplicate() as Button;
        var area_count = Enum<Game_Areas>.Count;

        var game = this.FindParent<Game>();

        foreach(var area in Enum<Game_Areas>.Values)
        {
            var index = (int)area;
            Button new_button;
            if (index == 0)
            {
                new_button = GetChild(0) as Button;
                new_button.Disabled = true;
            }
            else
            {
                new_button = button.Duplicate() as Button;
                this.AddChild(new_button);
            }
            new_button.AnchorTop = (1/(float)area_count)*(float)index;
            new_button.AnchorBottom = (1/(float)area_count)*(float)(index+1);
            new_button.Text = area.ToString();

            new_button.OnButtonDown(() => 
            {
                game.GoTo(area);
                WalkingSFX.Play();
            });

            new_button.OnProcess(delta => {
                if (game.current_area == area)
                    new_button.Disabled = true;
                else new_button.Disabled = false;
            });
        }

        button.QueueFree();
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}

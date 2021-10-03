using Godot;
using System;

public enum items
{
    gem,
    skull,
    bunny,
    squirrel,
    eyeball,
    shovel,
    fish,
    net
}


public class Game : Node
{
    public items wanted_item, picked_item;

    int count;

    public override void _Ready()
    {

        var areas = this.FindChild<Areas>();
        GoTo(Game_Areas.Home);
        ShowAreaSelection();

        this.FindChild<Witch>().SetText(clues.GetRandom());
        foreach (var button in this.FindChild<TakeToWitch>().FindChildren<Button>())
        {
            if (button.Name == "Yes")
            {
                button.OnButtonDown(() => {
                    HidePopup();
                    GoTo(Game_Areas.Home);
                    this.FindChild<Witch>().SetText(quips.GetRandom());
                    Coroutine.DelaySeconds(3f, () => 
                    {
                        this.FindChild<Witch>().SetText(clues.GetRandom());
                        ShowAreaSelection();
                        count ++;
                        if (count > 5)
                        {
                            if (Rand.Float01 > .5f)
                                Scene.Load("res://Scenes/Win/Win.tscn");
                            else Scene.Load("res://Scenes/Lose/Lose.tscn");
                        }
                    });
                });
            }
            if (button.Name == "No")
            {
                button.OnButtonDown(() => {
                    this.FindChild<TakeToWitch>().Visible = false;
                    ShowAreaSelection();
                });
            }
        }

        foreach(var button in areas.FindChildren<Godot.TextureButton>())
        {
            if (Enum<items>.TryGetValueFromString(button.Name, out var item))
            {
                button.OnButtonDown( () => {
                    ShowPopUp(item);
                    Debug.Log("Clicked", item);
                });
            }
            else Debug.Log(button.Name, "does not have a corresponding item");
        }
    }

    public override void _ExitTree()
    {
        Coroutine.ClearAllCoroutines();
    }

    public Game_Areas current_area;
    public void GoTo(Game_Areas area)
    {
        foreach(Control node in this.FindChild<Areas>().GetChildren())
        {
            node.Visible = false;
        }
        this.FindChild<Areas>().GetChild<Control>((int)area).Visible = true;
        current_area = area;
    }

    string[] clues = new string[]{
        "I want an item that makes me sing",
        "I want something that gives you wings",
        "I want a hareball",
        "I want to puke",
        "No one can see these pretties"
    };

    string[] quips = new string[]{
        "No no no you nincom poop",
        "Ahh yes that's it exactly",
        "3.. 2... 3.. 1.. eh? what was it again?",
        "bubububuuu paaah!",
        "fortune smiles on us both"
    };

    public void ShowPopUp(items item)
    {
        var popup = this.FindChild<TakeToWitch>();
        popup.Visible = true;
        popup.SetText($"Take {item} to the witch?");
        HideAreaSelection();
    }

    public void HidePopup()
    {
        this.FindChild<TakeToWitch>().Visible = false;
    }

    public void HideAreaSelection()
    {
        this.FindChild<AreaSelection>().Visible = false;
    }

    public void ShowAreaSelection()
    {
        this.FindChild<AreaSelection>().Visible = true;
    }
}
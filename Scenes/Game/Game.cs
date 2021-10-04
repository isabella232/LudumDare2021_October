using Godot;
using System;

public enum items
{
    Mushroom,
    Glow_worm,
    Gem,

    Rabbit,
    Pinecone,
    Log,

    Seaweed,
    Duck,
    Net,

    Skull,
    Shovel,
    Crow,
}


public class Game : Node
{
    public items wanted_item, picked_item;

    int wins;
    int count;

    public override void _Ready()
    {

        if (this.TryFindChild<AudioStreamPlayer>(out var player))
        {
            player.OnProcess(delta =>
            {
                if (!player.Playing)
                    player.Play();
            });
        }
        else Debug.Log("Could not find bgm player");


        var areas = this.FindChild<Areas>();
        GoTo(Game_Areas.Home);
        ShowAreaSelection();
        HidePopup();

        this.FindChild<Witch>().SetText(Witch.Mood.neutral, NextClue());
        foreach (var button in this.FindChild<TakeToWitch>().FindChildren<Button>())
        {
            if (button.Name == "Yes")
            {
                button.OnButtonDown(() =>
                {
                    WalkingSFX.Play();
                    HidePopup();
                    GoTo(Game_Areas.Home);
                    var witch = this.FindChild<Witch>();
                    if (wanted_item == picked_item)
                    {
                        witch.SetText(Witch.Mood.happy, success_quips.GetRandom());
                        witch.FindChild<AnimationPlayer>().Play("Success");
                        wins++;
                    }
                    else
                    {
                        witch.SetText(Witch.Mood.angry, failure_quips.GetRandom());
                        witch.FindChild<AnimationPlayer>().Play("Failure");
                    }

                    Coroutine.DelaySeconds(3f, () =>
                    {
                        witch.SetText(Witch.Mood.neutral, NextClue());
                        ShowAreaSelection();
                        count++;
                        if (count > 5)
                        {
                            if (wins >= 3)
                                Scene.Load("res://Scenes/Win/Win.tscn");
                            else Scene.Load("res://Scenes/Lose/Lose.tscn");
                        }
                    });
                });
            }
            if (button.Name == "No")
            {
                button.OnButtonDown(() =>
                {
                    this.FindChild<TakeToWitch>().Visible = false;
                    ShowAreaSelection();
                });
            }
        }

        foreach (var button in areas.FindChildren<Godot.TextureButton>())
        {
            if (Enum<items>.TryGetValueFromString(button.Name, out var item))
            {
                button.OnButtonDown(() =>
                {
                    ShowPopUp(item);
                    picked_item = item;
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
        foreach (Control node in this.FindChild<Areas>().GetChildren())
        {
            node.Visible = false;
        }
        this.FindChild<Areas>().GetChild<Control>((int)area).Visible = true;
        current_area = area;
    }

    string[] failure_quips = new string[]{
        "No no no you nincom poop",
        "bubububuuu paaah!",
        "................",
        "GAAAAAAAHHH WRONG WRONG WRONG"
    };

    string[] success_quips = new string[]
    {
        "Ahh yes that's it exactly",
        "3.. 2... 3.. 1.. eh? what was it again?",
        "Fortune smiles on us both",
        "Looks like you won't end up in my soup"
    };

    string NextClue()
    {
        wanted_item = Enum<items>.Values.GetRandom();

        Debug.Log(wanted_item);
        switch (wanted_item)
        {
            case items.Mushroom:
                return "Retrieve an item that’s truly magical";
            case items.Glow_worm:
                return "I need to see easier";
            case items.Gem:
                return "I like a healthy glow as much as the next old hag";

            case items.Rabbit:
                return "The potion needs more kick";
            case items.Pinecone:
                return "Some animals eat these ..I don’t know why";
            case items.Log:
                return "You need one for yuletide";

            case items.Seaweed:
                return "This item reminds me of my fingers";
            case items.Duck:
                return "If I catch one I’ll eat it ";
            case items.Net:
                return "It’s like a web";

            case items.Skull:
                return "Retrieve my favourite decayed item";
            case items.Shovel:
                return "The potion needs more depth";
            case items.Crow:
                return "Go fetch my friend";
            
            default: 
                return "I don't know anymore...";
        }
    }


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
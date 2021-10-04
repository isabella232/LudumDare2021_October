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

        if (this.TryFindChild<AudioStreamPlayer>(out var bgm))
        {
            bgm.OnProcess(delta =>
            {
                if (!bgm.Playing)
                    bgm.Play();
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
                return new string[]{
                    "Retrieve an item that’s truly magical",
                    "This item lives in a dark dark place.. I don't mean me!"
                    }.GetRandom();
            
            case items.Glow_worm:
                return new string[]{
                    "I need this item to attact children",
                    "I need to see easier"
                    }.GetRandom();
            
            case items.Gem:
                return new string[]{
                    "I like a healthy glow as much as the next old hag",
                    "A must-have altar item"
                    }.GetRandom();

            case items.Rabbit:
                return new string[]{
                    "The potion needs more kick",
                    "This item is common in Ostara rituals"
                    }.GetRandom(); 
            
            case items.Pinecone:
                return new string[]{
                    "Some animals eat these ..I don’t know why",
                    "I hang these around the house (it helps with the smell)"
                }.GetRandom(); 

            case items.Log:
                return  new string[]{
                    "You need one for yuletide",
                    "This item is used to make houses, when they should be made out of gingerbread"
                }.GetRandom();

            case items.Seaweed:
                return new string[]{
                    "This item reminds me of my fingers",
                     "I use this item for salads; I’m on a health kick."
                }.GetRandom(); 

            case items.Duck:
                return new string[]{
                    "If I catch one I’ll eat it ",
                    "The young ones are cute ..I guess"
                }.GetRandom();
                
            case items.Net:
                return new string[]{
                    "It’s like a web",
                    "I like to throw this item at children"
                }.GetRandom(); 

            case items.Skull:
                return new string[]{
                    "Retrieve my favourite decayed item",
                    "To get this item or not get this item, that is the question"
                }.GetRandom(); 

            case items.Shovel:
                return new string[]{
                    "The potion needs more depth",
                    "So it turns out, you can’t ride this item instead"
                }.GetRandom();

            case items.Crow:
                return new string[]{
                    "Go fetch my friend",
                    "This item reminds me of Samhaim"
                }.GetRandom();

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
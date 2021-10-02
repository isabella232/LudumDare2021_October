using Events;
using GameSystems;

public static class Scene
{
    public static Godot.SceneTree Tree {get; private set;}
    public static Godot.Node Current => Tree.CurrentScene;
    public static void Load(string path) => Tree.ChangeScene(path);
    
    [Event(int.MinValue)] static void SceneBootstrap(GameSystems.Events.SystemBootstrap args)
    {
        Tree = args.bootstrap.GetTree();
    }
}
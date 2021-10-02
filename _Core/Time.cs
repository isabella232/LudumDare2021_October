using Events;
using GameSystems;
using Godot;

public class Time
{
    static Time() => timer.Start();
    static System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
    
    public static System.TimeSpan timespan_since_startup => timer.Elapsed;
    public static float seconds_since_startup => (float)timer.Elapsed.TotalSeconds;
    public static float frame_time { get; private set; }
    public static float fixed_delta { get; private set; }
    public static float scale
    {
        get => Engine.TimeScale;
        set => Engine.TimeScale = value.min(0);
    }
    
    public static int frame_count { get; private set; }
    public static int fixed_count { get; private set; }

    public static float frames_per_second => Engine.GetFramesPerSecond();
    public static bool isPhysicsStep => Engine.IsInPhysicsFrame();
    public static bool paused
    {
        get => Scene.Tree.Paused; set => Scene.Tree.Paused = value;
    }

    [Event(int.MinValue)] static void OnUpdate(GameSystems.Events.SystemUpdate args)
    {
        frame_count++;
        frame_time = args.delta_time;
    }

    [Event(int.MinValue)] static void OnPhysicsUpdate(GameSystems.Events.SystemPhysicsUpdate args)
    {
        fixed_count++;
        fixed_delta = args.delta_time;
    }
}
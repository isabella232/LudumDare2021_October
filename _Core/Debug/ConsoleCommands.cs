using System.Collections;
using System.Collections.Generic;
using Events;

public static class ConsoleCommand
{
    static bool showfps;
    [Command("Toggles FPS Counter")]
    static void FPS(Command args)
    {
        showfps = !showfps;
        Coroutine.Start(() => showfps, () => Debug.Label("FPS: ", Time.frames_per_second));
    }

    static bool show_frame_time;
    [Command("Toggles Frame Time Display")]
    static void FrameTime(Command args)
    {
        show_frame_time = !show_frame_time;
        Coroutine.Start(() => show_frame_time, () => Debug.Label($"Frame Time: {Time.frame_time: 0.0000}", "ms"));
    }

    static bool show_frame_count;
    [Command("Toggles Frame Count Display")]
    static void FrameCount(Command args)
    {
        show_frame_count = !show_frame_count;
        Coroutine.Start(() => show_frame_count, () => Debug.Label($"Frame Count: {Time.frame_count}"));
    }

    [Command("Exits the application")]
    static void Quit(Command value)
    {
        Scene.Tree.Quit();
    }

    [Command("Exits the application")]
    static void Close(Command value)
    {
        Scene.Tree.Quit();
    }

    [Command("Exits the application")]
    static void Exit(Command value)
    {
        Scene.Tree.Quit();
    }

    [Command("Maximizes Window")]
    static void Maximize(Command value)
    {
        Godot.OS.WindowMaximized = !Godot.OS.WindowMaximized;
    }

    [Command("Pauses the Game")]
    static void Pause(Command value)
    {
        Scene.Tree.Paused = true;
    }

    [Command("Unpauses the Game")]
    static void UnPause(Command value)
    {
        Scene.Tree.Paused = false;
    }

    [Command("Minimizes Game Window")]
    static void Minimize(Command value)
    {
        Godot.OS.WindowMinimized = !Godot.OS.WindowMinimized;
    }

    [Command("Toggles borderless window")]
    static void Borderless(Command args)
    {
        Godot.OS.WindowBorderless = !Godot.OS.WindowBorderless;
    }

    [Command("Changes which screen the game is on")]
    static void Screen(Command value)
    {
        if (value.TryParse(0, out int val))
        {
            if (val < Godot.OS.GetScreenCount() && val >= 0)
                Godot.OS.CurrentScreen = val;
        }
    }

    [Command("Toggle Fullscreen")]
    static void FullScreen(Command value)
    {
        Godot.OS.WindowFullscreen = !Godot.OS.WindowFullscreen;
    }

    [Command("Logs input to console")]
    static void Log(Command value)
    {
        if (value.original.Length < 5)
            return;
        string val = value.original.Remove(0, 4);
        GameSystems.Console.Log(val);
    }

    [Command("Clears console window")]
    static void Clear(Command args)
    {
        GameSystems.Console.Clear();
    }


    [Command("Changes global delta time by input multiplier")]
    static void Timescale(Command args)
    {
        if (args.TryParse(0, out float val))
        {
            Time.scale = val.min(0);
        }
    }

    static bool show_time;
    [Command("Toggles the current time")]
    static void ShowTime(Command args)
    {
        show_time = !show_time;

        Coroutine.Start(() =>
        {
            if (show_time)
            {
                var time = Godot.OS.GetTime();
                Debug.Label(time["hour"], time["minute"]);
                return true;
            }
            return false;
        });
    }

    static bool show_memory;
    [Command("Toggles total managed memory usage")]
    static void ShowMemory(Command args)
    {
        show_memory = !show_memory;

        Coroutine.Start(() =>
        {
            if (show_memory)
            {
                var current = (double)System.GC.GetTotalMemory(false);
                string label;
                if (current < 1024)
                    label = $"{current: 0} bytes";
                else if ((current /= 1024) < 1024)
                    label = $"{current: 0.00} kb";
                else if ((current /= 1024) < 1024)
                    label = $"{current: 0.00} mb";
                else
                {
                    current /= 1024;
                    label = $"{current: 0.00} gb";
                }
                Debug.Label($"Total Memory: {label}");
                return true;
            }
            return false;
        });
    }

    static bool show_draw_calls;
    [Command]
    static void ShowDrawCalls(Command args)
    {
        show_draw_calls = !show_draw_calls;

        Coroutine.Start(() => show_draw_calls, () =>
        {
            Debug.Label("2D Draw Calls:", Scene.Current.GetViewport().GetRenderInfo(Godot.Viewport.RenderInfo.Info2dDrawCallsInFrame));
            Debug.Label("3D DrawCalls:", Scene.Current.GetViewport().GetRenderInfo(Godot.Viewport.RenderInfo.DrawCallsInFrame));
        });
    }

    [Command("Sets Mouse Modes\n confined = mouse confined within window borders \n hidden = mouse pointer hidden\n free = visible and not confined\n captured = confined and hidden")]
    static void MouseMode(Command args)
    {
        switch (args.ToString())
        {
            case "confined":
                Godot.Input.SetMouseMode(Godot.Input.MouseMode.Confined);
                break;
            case "hidden":
            case "invisible":
                Godot.Input.SetMouseMode(Godot.Input.MouseMode.Hidden);
                break;
            case "show":
            case "free":
            case "visible":
                Godot.Input.SetMouseMode(Godot.Input.MouseMode.Visible);
                break;
            case "captured":
            case "capture":
                Godot.Input.SetMouseMode(Godot.Input.MouseMode.Captured);
                break;
        }
    }
}




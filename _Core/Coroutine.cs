using Events;
using System;
using System.Collections;
using System.Collections.Generic;

public static class Coroutine
{
    /// <summary>
    /// calls the update until update returns false
    /// </summary>
    public static void Start(System.Func<bool> update)
    {
        coroutines[coroutine_count] = update;
        coroutine_count++;
        if (coroutine_count == coroutines.Length)
            System.Array.Resize(ref coroutines, coroutines.Length * 2);
    }

    /// <summary>
    /// Performs the action until the condition is false
    /// </summary>
    public static void Start(System.Func<bool> condition, System.Action action)
    {
        Start(() => { action(); return condition(); });
    }

    /// <summary>
    /// Starts coroutine to update each frame
    /// </summary>
    public static void Start(this IEnumerator coroutine)
    {
        Start(() => coroutine.MoveNext());
    }

    /// <summary>
    /// Delays an action to be performed until after frames have passed
    /// </summary>
    public static void DelayFrames(int frames, System.Action action)
    {
        int target_frame = Time.frame_count + frames;

        Start( () => {
            if (Time.frame_count > target_frame)
            {
                action();
                return false;
            }
            return true;
        });
    }

    public static void DelaySeconds(float seconds, System.Action action)
    {
        float target_seconds = Time.seconds_since_startup + seconds;

        Start( () => {
            if (Time.seconds_since_startup > target_seconds)
            {
                action();
                return false;
            }
            return true;
        });
    }

    public static void ClearAllCoroutines()
    {
        coroutines.Clear();
        coroutine_count = 0;
    }

    static System.Func<bool>[] coroutines = new System.Func<bool>[16];

    static int coroutine_count;

    [Event]
    static void Update(Events.FrameUpdate args)
    {
        for (int i = coroutine_count - 1; i >= 0; --i)
        {
            if (!coroutines[i]())
            {
                coroutine_count--;
                coroutines[i] = coroutines[coroutine_count];
                coroutines[coroutine_count] = default;
            }
        }
    }
}
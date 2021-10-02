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
        Start(() => {action(); return condition();});
    }

    /// <summary>
    /// Starts coroutine to update each frame
    /// </summary>
    public static void Start(this IEnumerator coroutine)
    {
        Start( () => coroutine.MoveNext());
    }

    /// <summary>
    /// Delays an action to be performed until after frames have passed
    /// </summary>
    public static void Delay(in System.Action action, int frames = 0)
    {
        if (frame_delayed.Length == frame_delay_count)
            Array.Resize(ref frame_delayed, frame_delayed.Length * 2);
        frame_delayed[frame_delay_count] = (action, Time.frame_count + frames);
        frame_delay_count ++;
    }

    public static void ClearAllCoroutines()
    {
        coroutines.Clear();
        coroutine_count = 0;
    }

    public static void ClearAllDelayedActions()
    {
        frame_delayed.Clear();
        frame_delay_count = 0;
    }

    static (System.Action Action, int frame)[] frame_delayed = new (System.Action, int)[16];
    static System.Func<bool>[] coroutines = new System.Func<bool>[16];

    static int coroutine_count, frame_delay_count = 0;

    [Event]
    static void Update(Events.FrameUpdate args)
    {
        for(int i = coroutine_count - 1; i>= 0; --i)
        {
            if (!coroutines[i]())
            {
                coroutine_count --;
                coroutines[coroutine_count] = coroutines[i];
                coroutines[coroutine_count] = default;
            }
        }

        var frame = Time.frame_count;
        for(int i = frame_delay_count - 1; i  >= 0; --i)
        {            
            if (frame_delayed[i].frame >= frame)
            {
                frame_delay_count --;
                frame_delayed[i].Action();
                frame_delayed[i] = frame_delayed[frame_delay_count];
                frame_delayed[frame_delay_count] = default;
            }
        }
    }
}
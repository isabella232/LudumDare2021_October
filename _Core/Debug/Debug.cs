using Godot;
using System;
using System.Collections.Generic;
using System.Reflection;
using GameSystems;
using Events;

public struct Debug
{
#if DEBUG
    public static bool Enable = true;
#else
    public static bool Enable = false;
#endif
    static List<string> Labels = new List<string>();
    static List<(Vector2 origin, Vector2 end, Color color, float thickness)> Line2D = new List<(Vector2 origin, Vector2 end, Color color, float thickness)>();
    static List<(Vector3 origin, Vector3 end, Color color)> Line3D = new List<(Vector3 origin, Vector3 end, Color color)>();

    /// <summary>
    /// Logs message to output
    /// </summary>
    public static void Log(params object[] items)
    {
        string log = "";
        foreach (var item in items)
        {
            log += item == null ? "NULL" : item.ToString();
            log += " ";
        }
        GD.Print(log);
        GameSystems.Console.Log(log);
    }

    [System.Diagnostics.Conditional("DEBUG")]
    public static void Assert(bool condition, string message = "Assertion Failed") => Assert(condition, message);

    [System.Diagnostics.Conditional("DEBUG")]
    public static void AssertStack(bool condition, int logged_stack_frame, params object[] message)
    {
        logged_stack_frame = logged_stack_frame.min(0);

        if (!condition)
        {
            var m = "";
            foreach (var item in message)
            {
                if (item == null)
                    m += "NULL";
                else m += item.ToString();
                m += " ";
            }

            var frames = (new System.Diagnostics.StackTrace(true)).GetFrames();
            if (frames.Length > (1 + logged_stack_frame))
            {
                var frame = frames[(1 + logged_stack_frame)];
                m = m + $"-> File:{frame.GetFileName()?.Replace(System.IO.Directory.GetCurrentDirectory(), "")}  Line:{frame.GetFileLineNumber()}";
                Debug.Log(m);
            }
            throw new Exception(m);
        }
    }

    [System.Diagnostics.Conditional("DEBUG")]
    public static void Assert(bool condition, params object[] message) => AssertStack(condition, 0, message);

    /// <summary>
    /// Asserts that all Nodes are Valid
    /// </summary>
    /// <param name="nodes"></param>
    [System.Diagnostics.Conditional("DEBUG")]
    public static void Assert(params Godot.Node[] valid_nodes)
    {
        int count = 0;
        foreach (Godot.Node node in valid_nodes)
        {
            count++;
            if (node.IsNull())
            {
                var frames = (new System.Diagnostics.StackTrace(true)).GetFrames();
                if (frames.Length > 1)
                {
                    var frame = frames[1];
                    var message = $"Node {count} in Assertion was Null -> File:{frame.GetFileName()?.Replace(System.IO.Directory.GetCurrentDirectory(), "")}  Line:{frame.GetFileLineNumber()}";
                    Debug.Log(message);
                    throw new Exception(message);
                }
                throw new Exception("Null Node Exception");
            }
        }
    }

    [System.Diagnostics.Conditional("DEBUG")]
    public static void LogFileLine()
    {
        var frames = (new System.Diagnostics.StackTrace(true)).GetFrames();
        if (frames.Length > 1)
        {
            var frame = frames[1];
            Debug.Log($"File:{frame.GetFileName()?.Replace(System.IO.Directory.GetCurrentDirectory(), "")}", $"Line:{frame.GetFileLineNumber()}");
        }
    }

    public static Exception Exception(params object[] obj)
    {
        var message = "";
        foreach (var item in obj)
        {
            message += item == null ? "NULL" : item.ToString();
            message += " ";
        }

        var frames = (new System.Diagnostics.StackTrace(true)).GetFrames();
        if (frames.Length > 1)
        {
            var frame = frames[1];
            message = $"{message} -> File:{frame.GetFileName()?.Replace(System.IO.Directory.GetCurrentDirectory(), "")}  Line:{frame.GetFileLineNumber()}";
            Debug.Log(message);
            throw new Exception(message);
        }
        return new Exception(message);
    }

    static System.Text.StringBuilder builder = new System.Text.StringBuilder();

    /// <summary>
    /// Draws a message to the screen
    /// Messages are cleared every frame
    /// </summary>
    public static void Label(params object[] args)
    {
        if (!Enable || Labels.Count > 5000) return;
        builder.Clear();
        foreach (var item in args)
        {
            if (item == null)
            {
                builder.Append("NULL");
            }
            else if (item is float float_val)
            {
                string val = $"{float_val: 0.00}";
                builder.Append(val);
            }
            else if (item is Vector3 v3)
            {
                string val = $"v3( {v3.x: 0.00}x, {v3.y: 0.00}y, {v3.z: 0.00}z )";
                builder.Append(val);
            }
            else if (item is Vector2 v2)
            {
                string val = $"v2( {v2.x: 0.00}x, {v2.y: 0.00}y )";
                builder.Append(val);
            }
            else builder.Append(item);

            builder.Append(" ");
        }
        builder.Append("\n");
        Labels.Add(builder.ToString());
    }

    public static void DrawLine3D(Vector3 global_origin, Vector3 global_end, Color color)
    {
        if (!Enable) return;
        Line3D.Add((global_origin, global_end, color));
    }

    public static void DrawLine2D(Vector2 global_origin, Vector2 global_end, Color color, float thickness = 1)
    {
        if (!Enable) return;
        Line2D.Add((global_origin, global_end, color, thickness));
    }

    public static void DrawBox2D(Vector2 global_origin, Vector2 scale, Color color, float thickness = 1f, bool cross = false)
    {
        var minx = global_origin.x - scale.x;
        var miny = global_origin.y - scale.y;
        var maxx = global_origin.x + scale.x;
        var maxy = global_origin.y + scale.y;

        Debug.DrawLine2D(new Vector2(minx, miny), new Vector2(minx, maxy), color, thickness);
        Debug.DrawLine2D(new Vector2(minx, maxy), new Vector2(maxx, maxy), color, thickness);
        Debug.DrawLine2D(new Vector2(maxx, maxy), new Vector2(maxx, miny), color, thickness);
        Debug.DrawLine2D(new Vector2(maxx, miny), new Vector2(minx, miny), color, thickness);

        if (cross)
        {
            Debug.DrawLine2D(new Vector2(minx, miny), new Vector2(maxx, maxy), color, thickness);
            Debug.DrawLine2D(new Vector2(minx, maxy), new Vector2(maxx, miny), color, thickness);
        }
    }

    public static void DrawCircle2D(Vector2 global_origin, float radius, Color color)
    {
        float offset = 360f / 16f;
        for (float angle = 0; angle < 360; angle += offset)
        {
            var deg = Mathf.Deg2Rad(angle);
            var deg2 = Mathf.Deg2Rad(angle + offset);
            var pos = global_origin + new Vector2(Mathf.Cos(deg), Mathf.Sin(deg)) * radius;
            var pos2 = global_origin + new Vector2(Mathf.Cos(deg2), Mathf.Sin(deg2)) * radius;
            Debug.DrawLine2D(pos, pos2, color);
        }
    }

    public static void DrawCircle3D(Vector3 global_origin, float radius, Color color)
    {
        var cam = Scene.Current.GetViewport().GetCamera();
        if (Node.IsInstanceValid(cam))
            DrawCircle3D(global_origin, cam.GlobalTransform.basis.z, radius, color);
    }

    readonly static float arc_angle = Mathf.Deg2Rad(360f / 16f);
    public static void DrawCircle3D(Vector3 global_origin, Vector3 normal, float radius, Color color)
    {
        if (normal == Vector3.Zero)
            return;
        if (normal == Vector3.Up)
            normal.x += 0.00001f;

        normal = normal.Normalized();
        Transform t = Transform.Identity;
        t = t.LookingAt(normal, Vector3.Up);
        t.origin = global_origin;

        var angle = 0f;
        var start = t.Xform(new Vector3(radius, 0, 0));
        for (int i = 0; i < 16; ++i)
        {
            angle += arc_angle;
            var end = t.Xform(new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0));
            DrawLine3D(start, end, color);
            start = end;
        }
    }

    public enum TimeFrame
    {
        microseconds,
        milliseconds,
        seconds
    }

    public static void Time(string name, Action action, bool use_label = false)
        => Time(name, 1, action, use_label: use_label);

    public static void Time(string name, int iterations, Action action, bool use_label = false, TimeFrame time = TimeFrame.milliseconds)
    {
        TimeSpan start = global::Time.timespan_since_startup;
        for (int i = 0; i < iterations; ++i)
            action();
        TimeSpan end = global::Time.timespan_since_startup - start;
        string time_value;
        switch (time)
        {
            case TimeFrame.microseconds:
                time_value = $"{end.TotalMilliseconds / 1000: 0.00}us";
                break;
            case TimeFrame.milliseconds:
                time_value = $"{end.TotalMilliseconds: 0.00}ms";
                break;
            default:
                time_value = $"{end.TotalSeconds: 0.00}s";
                break;
        }
        if (use_label)
            Debug.Label(name, time_value, (iterations > 1 ? iterations.ToString() + "x" : ""));
        else
            Debug.Log(name, time_value, (iterations > 1 ? iterations.ToString() + "x" : ""));
    }


    [Event]
    static void OnUpdate(GameSystems.Events.SystemUpdate args)
    {
        DrawDebugLabels.Update();
        DrawDebugLine2D.UpdateDrawLine2D();
        DrawDebugLine3D.UpdateDrawLine3D();
    }

    class DrawDebugLine3D : ImmediateGeometry
    {
        static DrawDebugLine3D instance;

        public static void UpdateDrawLine3D()
        {
            if (Line3D.Count > 0)
            {
                if (!Node.IsInstanceValid(instance))
                {
                    Scene.Current.AddChild(instance = new DrawDebugLine3D());
                    instance.Owner = Scene.Current;
                    var mat = new SpatialMaterial();
                    instance.MaterialOverride = mat;
                    mat.VertexColorUseAsAlbedo = true;
                    mat.FlagsUnshaded = true;
                    mat.FlagsDoNotReceiveShadows = true;

                }
                instance.Clear();
                instance.Begin(Mesh.PrimitiveType.Lines);
                foreach (var line in Line3D)
                {
                    instance.SetColor(line.color);
                    instance.AddVertex(line.origin);
                    instance.AddVertex(line.end);
                }
                instance.End();
                Line3D.Clear();
            }
            else if (Node.IsInstanceValid(instance))
                instance.QueueFree();
        }
    }

    class DrawDebugLine2D : Line2D
    {
        public override void _Draw()
        {
            foreach (var line in Line2D)
            {
                DrawLine(line.origin, line.end, line.color, line.thickness);
            }
        }

        static DrawDebugLine2D instance;
        public static void UpdateDrawLine2D()
        {
            if (Line2D.Count > 0)
            {
                if (!Node.IsInstanceValid(instance))
                {
                    Scene.Current.AddChild(instance = new DrawDebugLine2D());
                    instance.Owner = Scene.Current;
                }
                instance.ZIndex = 4096;
                instance.Update();
                Line2D.Clear();
            }
            else if (Node.IsInstanceValid(instance))
                instance.QueueFree();
        }
    }

    static class DrawDebugLabels
    {
        static Godot.Control control, label_container;
        static Label[] drawn_labels = new Label[8];
        static VScrollBar scrollBar;
        static bool draw_on_right = true;

        [Command("Swaps which side the debug label is drawn")]
        static void SwapDebugSides(Command command)
        {
            draw_on_right = !draw_on_right;
            if (control.IsValid())
                control.QueueFree();
        }

        public static void Update()
        {
            if (!Debug.Enable)
            {
                if (control.IsValid())
                    control.QueueFree();
                return;
            }

            var label_height = 22f;
            if (!control.IsValid())
            {
                control = GUI.Draw<Control>();
                if (draw_on_right)
                {
                    control.SetAnchors(.7f, .99f, .01f, .99f);
                    control.SetMargins(0, 0, 0, 0);
                }
                else
                {
                    control.SetAnchors(.01f, .3f, .01f, .99f);
                    control.SetMargins(0, 0, 0, 0);
                }
                control.Name = "DEBUG LABEL GUI";

                scrollBar = GUI.Draw<VScrollBar>(control);

                float scroll_width = .05f;
                if (draw_on_right)
                {
                    scrollBar.SetAnchors(1f - scroll_width, 1, 0, 1);
                    scrollBar.SetMargins(0, 0, 0, 0);
                }
                else
                {
                    scrollBar.SetAnchors(0, scroll_width, 0, 1);
                    scrollBar.SetMargins(0, 0, 0, 0);
                }

                scrollBar.MinValue = 0;
                scrollBar.MaxValue = Labels.Count.min(1);
                scrollBar.Value = 0;

                label_container = GUI.Draw<Control>(control);
                if (draw_on_right)
                {
                    label_container.SetAnchors(0f, 1f - scroll_width, 0, 1);
                }
                else
                {
                    label_container.SetAnchors(scroll_width, 1, 0, 1);
                }
            }

            if (Labels.Count == 0)
                control.Visible = false;
            else control.Visible = true;

            var count = (int)(control.RectSize.y / label_height);
            if (drawn_labels.Length < count)
            {
                System.Array.Resize(ref drawn_labels, count);
            }

            scrollBar.MaxValue = (Labels.Count - 1).min(1);
            var current_index = (int)scrollBar.Value;

            for (int i = 0; i < drawn_labels.Length; ++i)
            {
                if (drawn_labels[i].IsNull())
                {
                    var label = drawn_labels[i] = GUI.Draw<Label>(label_container);

                    var panel = GUI.Draw<Panel>(label);
                    panel.SetAnchors(0, 1, 0, 1);
                    panel.SetMargins(0, 0, 0, 0);
                    panel.ShowBehindParent = true;

                    if (draw_on_right)
                    {
                        label.Align = Godot.Label.AlignEnum.Right;
                        label.GrowHorizontal = Control.GrowDirection.Begin;
                        label.SetAnchors(1, 1, 0, 0);
                    }
                    else
                    {
                        label.SetAnchors(0, 1, 0, 0);
                    }

                    label.RectClipContent = true;

                }
                if (current_index + i >= Labels.Count)
                    drawn_labels[i].Visible = false;
                else
                {
                    var label = drawn_labels[i];
                    label.Visible = true;
                    label.Text = $"  {Labels[current_index + i]}  ";
                    label.SetMargins(0, 0, 0, 0);
                    label.RectSize = new Vector2(0, label_height);
                    label.RectPosition = new Vector2(label.RectPosition.x, label_height * i);
                }
            }
            Labels.Clear();
        }
    }

    [Command("Toggles Debug Gizmos")]
    static void ToggleDebug(Command value)
    {
        Debug.Enable = !Debug.Enable;
    }

    /// <summary>
    /// Marks a method to be called when Debug.RunTests() is called.
    /// Method must be static, not generic or abstract, have no arguements and be within a non generic class.
    /// Simply throw an exception if the test fails.
    /// </summary>
    [System.AttributeUsage(AttributeTargets.Method)]
    public class Test : System.Attribute { }

    /// <summary>
    /// Runs all static methods marked with the Debug.Test attribute.
    /// </summary>
    public static void RunTests() => RunTests(null);

    [Command("Runs all static methods with a Debug.Test attribute")]
    static void RunTests(Command args)
    {
        Debug.Log("Starting Tests....");
        int count = 0;
        int pass = 0;
        foreach (var type in System.Reflection.Assembly.GetExecutingAssembly().GetTypes())
        {
            foreach (var method in type.GetMethods(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic))
            {
                if (method.GetCustomAttribute<Debug.Test>() != null)
                {
                    string calling_method = $"{type.Name}.{method.Name}";
                    if (type.IsGenericType)
                    {
                        Debug.Log(calling_method, "test functions can only be within non generic classes");
                        continue;
                    }
                    if (method.GetParameters().Length > 0)
                    {
                        Debug.Log(calling_method, "needs no arguments to run as a test");
                        continue;
                    }
                    if (method.IsAbstract || method.IsGenericMethod)
                    {
                        Debug.Log(calling_method, "cannot be abstract or generic to run as a test");
                        continue;
                    }
                    count++;

                    try
                    {
                        method.Invoke(null, null);
                        Debug.Log("PASSED:", calling_method);
                        pass++;
                    }
                    catch (System.Exception e)
                    {
                        Debug.Log("->", e.InnerException?.Message);
                        Debug.Log("FAILED:", calling_method);
                    }
                }
            }
        }
        Debug.Log("All Tests Complete: ", pass, "/", count);

        if (pass == count)
            Debug.Log("All Tests have passed");
        else
            Debug.Log(count - pass, "tests failed");
    }
}
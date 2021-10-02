using System.Linq;
using System.Collections.Generic;
using System.Text;
using System;

public static partial class Extensions
{
    // FLOAT

    /// <summary>
    /// remaps within range -1, 1
    /// </summary>
    public static float remap(this float value, float min, float max)
    {
        if (min > max)
        {
            var temp = min;
            min = max;
            max = temp;
        }
        var diff = max - min;
        if (value < min) value = min;
        if (value > max) value = max;

        value = value - min;
        value = value / diff;
        return value * 2f - 1;
    }

    public static float abs(this float target) 
        => target < 0 ? target * -1 : target;

    public static float sign(this float target)
        => target < 0 ? -1f : 1f;

    public static float min(this float target, float min)
        => target < min ? min : target;

    public static float max(this float target, float max)
        => target > max ? max : target;

    public static float clamp(this float target, float min, float max)
    {
        if (min > max)
        {
            var temp = max;
            max = min;
            min = temp;
        }
        return target < min ? min : target > max ? max : target;
    }

    // DOUBLE

    public static double remap(this double value, double min, double max)
    {
        if (min > max)
        {
            var temp = min;
            min = max;
            max = temp;
        }
        var diff = max - min;
        if (value < min) value = min;
        if (value > max) value = max;

        value = value - min;
        value = value / diff;
        return value * 2f - 1;
    }

    public static double abs(this double target) 
        => target < 0 ? target * -1 : target;

    public static double sign(this double target)
        => target < 0 ? -1f : 1f;

    public static double min(this double target, double min)
        => target < min ? min : target;

    public static double max(this double target, double max)
        => target > max ? max : target;

    public static double clamp(this double target, double min, double max)
    {
        if (min > max)
        {
            var temp = max;
            max = min;
            min = temp;
        }
        return target < min ? min : target > max ? max : target;
    }

    // INT

    public static int abs(this int target) 
        => target < 0 ? target * -1 : target;

    public static int sign(this int target)
        => target < 0 ? -1 : 1;

    public static int min(this int target, int min)
        => target < min ? min : target;

    public static int max(this int target, int max)
        => target > max ? max : target;

    public static int clamp(this int target, int min, int max)
    {
        if (min > max)
        {
            var temp = max;
            max = min;
            min = temp;
        }
        return target < min ? min : target > max ? max : target;
    }

    // LONG

    public static long abs (this long target)
        => target < 0 ? -target : target; 

    public static long sign(this long target) => target < 0 ? -1: 1;
    
    public static long min(this long target, long min)
        => target < min ? min : target;

    public static long max (this long target, long max)
        => target > max ? max : target;

    public static long clamp(this long target, long min, long max)
    {
        if (min > max)
        {
            var temp = max;
            min = max;
            max = temp;
        }
        return target < min ? min : target > max ? max : target;
    }



    static StringBuilder builder = new StringBuilder();
    public static string AddSpaceBeforeCaps(this string current)
    {
        builder.Clear();
        if (current.Length > 1)
        {
            builder.Append(current[0]);
            for (int i = 1; i < current.Length; ++i)
            {
                if (char.IsUpper(current[i]) && char.IsLower(current[i - 1]))
                    builder.Append(" ");
                builder.Append(current[i]);
            }
        }
        return builder.ToString();
    }

    public static bool TryConvertToFileSafeString(this string current, out string value)
    {
        if (string.IsNullOrEmpty(current))
        {
            value = default;
            return false;
        }

        builder.Clear();
        bool valid_first = false;
        for (int i = 0; i < current.Length; ++i)
        {
            var letter = current[i];
            if (!valid_first)
            {
                if (char.IsLetter(letter))
                {
                    builder.Append(letter);
                    valid_first = true;
                }
            }
            else
            {
                if (char.IsLetterOrDigit(letter) || letter == '_' || letter == ' ')
                    builder.Append(letter);
            }
        }
        value = builder.ToString();
        return value.Length > 0;
    }

    public static int toHash(this string value)
    {
        int p = 1572869;
        int power = 1;
        int hashval = 0;

        unchecked
        {
            for (int i = 0; i < value.Length; ++i)
            {
                power *= p;
                hashval = (hashval + value[i] * power);
            }
        }
        return hashval;
    }

    public delegate void ThreadAction<C1>(int index, ref C1 c1);

    public static void ForeachParallel<T>(this T[] array, int start_index, int end_index, ThreadAction<T> action)
    {
        System.Threading.Tasks.Parallel.For(start_index, end_index, index => action(index, ref array[index]));
    }

    public static void ForeachParallel<T>(this IReadOnlyList<T> list, Action<T> action)
    {
        System.Threading.Tasks.Parallel.For(0, list.Count, (int index) => action(list[index]));
    }

    public static void ForeachParallel<T>(this IList<T> list, Action<T> action)
    {
        System.Threading.Tasks.Parallel.For(0, list.Count, (int index) => action(list[index]));
    }

    public static void Clear<T>(this T[] array)
    {
        if (array != null)
            System.Array.Clear(array, 0, array.Length);
    }
}

public static class Enum<T> where T : System.Enum
{
    public static readonly string[] Names = System.Enum.GetNames(typeof(T));
    public static readonly int Count = Names.Length;
    public static readonly T[] Values = System.Enum.GetValues(typeof(T)).Cast<T>().ToArray();

    public static bool TryGetValueFromInt(int integer, out T Value)
    {
        foreach(var value in Values)
        {
            if ((int)(object)value == integer)
            {
                Value = value;
                return true;
            }
        }
        Value = default;
        return false;
    }
}

public static class GodotExtensions
{
    public static bool IsNull(this Godot.Object node) => !Godot.Object.IsInstanceValid(node);
    public static bool IsValid(this Godot.Object node) => Godot.Object.IsInstanceValid(node);

    public static T FindParent<T>(this Godot.Node node) where T : class
    {
        if (!Godot.Object.IsInstanceValid(node))
            return null;

        if (node is T value)
            return value;

        return node.GetParent().FindParent<T>();
    }

    public static bool TryFindParent<T>(this Godot.Node node, out T obj) where T : class
    {
        obj = node.FindParent<T>();
        return Godot.Node.IsInstanceValid(obj as Godot.Node);
    }

    public static T FindChild<T>(this Godot.Node node) where T : class
    {
        if (!Godot.Node.IsInstanceValid(node))
            return null;
        return Find(node);

        T Find(Godot.Node test)
        {
            if (test is T target)
                return target;

            foreach (Godot.Node child in test.GetChildren())
            {
                var val = Find(child);
                if (val != null)
                    return val;
            }
            return null;
        }
    }

    public static T FindOrAddChild<T>(this Godot.Node node) where T: Godot.Node, new()
    {
        if (!TryFindChild<T>(node, out T obj))
        {
            obj = new T();
            node.AddChild(obj);
        }
        return obj;
    }

    public static bool TryFindChild<T>(this Godot.Node node, out T obj) where T : class
    {
        obj = node.FindChild<T>();
        return Godot.Node.IsInstanceValid(obj as Godot.Node);
    }

    /// <summary>
    /// Returns all types within child heirachy inclusive of queried node.
    /// </summary>
    public static List<T> FindChildren<T>(this Godot.Node node) where T : class
    {
        if (!Godot.Node.IsInstanceValid(node))
            return new List<T>();

        List<T> children = new List<T>();
        if (node is T type)
            children.Add(type);

        FindInChildren(node);
        return children;

        void FindInChildren(Godot.Node target)
        {
            foreach (Godot.Node child in target.GetChildren())
            {
                if (child is T value)
                    children.Add(value);
                FindInChildren(child);
            }
        }
    }

    public static float tilt(this Godot.Vector2 vector2)
    {
        var val = Godot.Mathf.Sqrt(vector2.x * vector2.x + vector2.y * vector2.y);
        return val > 1 ? 1 : val;
    }

    public static Godot.Color lerp(this Godot.Color current, Godot.Color target, float weight)
        => weight < 0 ? current : weight > 1 ? target : weight * (target - current) + current;

    public static float lerp(this float current, float target, float weight)
        => weight < 0 ? current : weight > 1 ? target : weight * (target - current) + current;

    public static Godot.Vector2 lerp(this Godot.Vector2 current, Godot.Vector2 target, float weight)
    {
        return weight < 0 ? current : weight > 1 ? target : weight * (target - current) + current;
    }

    public static Godot.Vector3 lerp(this Godot.Vector3 current, Godot.Vector3 target, float weight)
    {
        return weight < 0 ? current : weight > 1 ? target : weight * (target - current) + current;
    }

    public static Godot.Transform lerp(this Godot.Transform current, Godot.Transform target, float weight)
        => current.InterpolateWith(target, weight);

    public static void LookAt(this Godot.Spatial node, Godot.Vector3 direction, float weight = 1f)
        => LookAt(node, direction, weight, Godot.Vector3.Up);

    public static void LookAt(this Godot.Spatial node, Godot.Vector3 direction, float weight, Godot.Vector3 up)
    {
        if (direction.Length() > 0f)
        {
            var t = node.Transform;
            var target = t.LookingAt(-direction, up);
            node.Transform = t.InterpolateWith(target, weight);
        }
    }

    public static void LookAtGlobal(this Godot.Spatial node, Godot.Vector3 global_position, float weight = 1f)
        => LookAtGlobal(node, global_position, weight, Godot.Vector3.Up);

    public static void LookAtGlobal(this Godot.Spatial node, Godot.Vector3 global_position, float weight, Godot.Vector3 up)
    {
        var target = global_position - node.GlobalTransform.origin;
        node.LookAt(target, weight, up);
    }

    public static Godot.Vector3 setX(this Godot.Vector3 current, float value)
    {
        current.x = value;
        return current;
    }

    public static Godot.Vector3 setY(this Godot.Vector3 current, float value)
    {
        current.y = value;
        return current;
    }

    public static Godot.Vector3 setZ(this Godot.Vector3 current, float value)
    {
        current.z = value;
        return current;
    }

    public static Godot.Vector2 setX(this Godot.Vector2 current, float value)
    {
        current.x = value;
        return current;
    }

    public static Godot.Vector2 setY(this Godot.Vector2 current, float value)
    {
        current.y = value;
        return current;
    }

    public static T SetMargins<T>(this T control, float left, float right, float top, float bottom) where T: Godot.Control
    {
        control.MarginLeft = left;
        control.MarginRight = right;
        control.MarginTop = top;
        control.MarginBottom = bottom;
        return control;
    }

    public static T SetRect<T>(this T control, float x, float y, float width, float height) where T : Godot.Control
    {
        control.RectPosition = new Godot.Vector2(x, y);
        control.RectSize = new Godot.Vector2(width, height);
        return control;
    }

    public static T SetAnchors<T>(this T control, float left, float right, float top, float bottom) where T: Godot.Control
    {
        control.AnchorLeft = left;
        control.AnchorRight = right;
        control.AnchorTop = top;
        control.AnchorBottom = bottom;
        return control;
    }

    public static T SetPosition<T>(this T control, float x, float y) where T: Godot.Control
    {
        control.RectPosition = new Godot.Vector2(x, y);
        return control;
    }

    public static T SetSize<T>(this T control, float width, float height) where T: Godot.Control
    {
        control.RectSize = new Godot.Vector2(width, height);
        return control;
    }

    public static T SetWidth<T>(this T control, float value) where T: Godot.Control
    {
        var size= control.RectSize;
        size.x = value;
        control.RectSize = size;
        return control;
    }

    public static T SetHeight<T>(this T control, float value) where T: Godot.Control
    {
        var size = control.RectSize;
        size.y = value;
        control.RectSize = size;
        return control;
    }

    public static T SetPositionX<T>(this T control, float value) where T: Godot.Control
    {
        var position = control.RectPosition;
        position.x = value;
        control.RectPosition = position;
        return control;
    }

    public static T SetPositionY<T>(this T control, float value) where T: Godot.Control
    {
        var position = control.RectPosition;
        position.y = value;
        control.RectPosition = position;
        return control;
    }
}

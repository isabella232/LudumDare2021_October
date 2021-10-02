using Godot;
using System.Collections.Generic;

/// <summary>
/// Makes GUI Objects
/// </summary>
static class GUI
{
    static CanvasLayer _layer;
    static Control _screen;
    static Control screen
    {
        get
        {
            if (_screen.IsNull())
            {
                _screen = new Control();
                _screen.AnchorBottom = 1;
                _screen.AnchorRight = 1;
                layer.AddChild(_screen);
            }
            return _screen;
        }
    }

    static CanvasLayer layer
    {
        get
        {
            if (_layer.IsNull())
            {
                _layer = new CanvasLayer();
                Layout.position = new Vector2(16, 16);
                Coroutine.Delay(() => Scene.Current.AddChild(_layer), 1);
            }
            return _layer;
        }
    }

    public static float Screen_Width => screen.RectSize.x;
    public static float Screen_Height => screen.RectSize.y;

    public static Label Label(Rect2 rect, string text, Node parent = default)
    {
        var label = Draw<Label>(rect, parent);
        label.ClipText = true;
        label.Text = text;
        label.Valign = Godot.Label.VAlign.Center;
        return label;
    }

    public static Button Button(Rect2 rect, string text, Node parent = default)
    {
        var button = Draw<Button>(rect, parent);
        button.Text = text;
        return button;
    }

    public static Godot.TextureButton Button(Rect2 rect, Texture normal, Texture pressed, Texture focus, Node parent = default, Texture hover = null, Godot.BitMap mask = null)
    {
        var button = Draw<Godot.TextureButton>(rect, parent);
        button.TextureNormal = normal;
        button.TexturePressed = pressed;
        button.TextureFocused = focus;
        button.TextureHover = hover;
        button.TextureClickMask = mask;
        return button;
    }

    public static TextEdit TextEdit(Rect2 rect, string text, Node parent = default)
    {
        var text_edit = Draw<TextEdit>(rect, parent);
        text_edit.Text = text;
        layer.AddChild(text_edit);
        return text_edit;
    }

    public static CheckBox Toggle(Rect2 rect, string label, bool value, Node parent = default)
    {
        var check = Draw<CheckBox>(rect, parent);
        check.ClipText = true;
        check.Text = label;
        check.Pressed = value;
        return check;
    }

    public static ColorRect Box(Rect2 rect, Color color, Node parent = default)
    {
        var crect = Draw<ColorRect>(rect, parent);
        crect.Color = color;
        return crect;
    }

    public static HScrollBar HScrollBar(Rect2 rect, float value, float min, float max, Node parent = default)
    {
        var hscroll = Draw<HScrollBar>(rect, parent);
        if (min > max)
        {
            var temp = min;
            min = max;
            max = min;
        }
        value = value.clamp(min, max);
        hscroll.MinValue = min;
        hscroll.MaxValue = max;
        hscroll.Value = value;
        return hscroll;
    }

    public static VScrollBar VScrollBar(Rect2 rect, float value, float min, float max, Node parent = default)
    {
        var vscroll = Draw<VScrollBar>(rect, parent);

        if (min > max)
        {
            var temp = min;
            min = max;
            max = min;
        }
        value = value.clamp(min, max);
        vscroll.MinValue = min;
        vscroll.MaxValue = max;
        vscroll.Value = value;
        return vscroll;
    }

    enum Testing
    {
        item_a,
        item_b,
        your_mum,
        how_could_you
    }

    public static Button MenuButton<Selection>(Rect2 rect, string label, System.Action<Selection> onSelection, Node parent = default) where Selection : System.Enum
    {
        var button = GUI.Button(rect, label, parent);
        var popup = GUI.Draw<Godot.PopupMenu>(default, button);
        popup.OnSelect(onSelection);
        popup.ShowOnTop = true;
        button.OnButtonDown(() =>
        {
            if (popup.Visible)
                popup.Hide();
            else popup.Show();
        });
        popup.OnVisibilityChanged(() =>
        {
            popup.RectPosition = rect.Position + new Vector2(button.RectSize.x, 0);
        });
        return button;
    }

    public static T Draw<T>(Rect2 rect, Node parent = default) where T : Godot.Control, new()
        => Draw<T>(rect.Position, rect.Size, parent);

    public static T Draw<T>(Node parent = default) where T : Godot.Control, new()
        => Draw<T>(new Vector2(), new Vector2(), parent);

    public static T Draw<T>(float x, float y, float width, float height, Node parent = default) where T : Godot.Control, new()
        => Draw<T>(new Vector2(x, y), new Vector2(width, height), parent);

    public static T Draw<T>(Vector2 position, Vector2 size, Node parent = default) where T : Godot.Control, new()
    {
        var control = new T();
        if (parent.IsNull())
            layer.AddChild(control);
        else
            parent.AddChild(control);
        control.RectPosition = position;
        control.RectSize = size;
        return control;
    }

    public static class Layout
    {
        public static Vector2 position = new Vector2(16, 16);
        public static Vector2 size = new Vector2(250, 22);
        public static float vertical_separation = 2;
        static float next_offset => size.y + vertical_separation;

        public static Label Label(string text, Node parent = default)
        {
            var node = GUI.Label(new Rect2(position, size), text, parent);
            position.y += next_offset;
            return node;
        }

        public static TextEdit Text(string label, string text, Node parent = default)
        {
            var label_control = Draw<Godot.Label>(position, size, parent);
            position.y += next_offset;
            label_control.Text = label;
            var text_edit = Draw<Godot.TextEdit>(label_control).SetAnchors(.5f, 1f, 0f, 1f);
            text_edit.Text = text;
            return text_edit;
        }

        public static CheckBox Toggle(string label, bool value, Node parent = default)
        {
            var control = GUI.Draw<Control>(position, size, parent);
            position.y += next_offset;

            var label_control = GUI.Draw<Label>(control);
            label_control.Text = label;
            label_control.SetAnchors(0, .5f, 0, 1f);

            var check = GUI.Draw<Godot.CheckBox>(control);
            check.SetAnchors(.5f, 1f, 0f, 1f);
            check.Pressed = value;
            return check;
        }

        public static Button Button(string label, Node parent = default)
        {
            var button = GUI.Button(new Rect2(position, size), label, parent);
            position.y += next_offset;
            return button;
        }

        public static Button MenuButton<Selection>(string label, System.Action<Selection> OnSelection, Node parent = null) where Selection : System.Enum
        {
            var button = GUI.MenuButton(new Rect2(position, size), label, OnSelection, parent);
            position.y += next_offset;
            return button;
        }

        public static TextureButton Button(Texture normal, Texture pressed, Texture focus, Node parent = default, Texture hover = null, Godot.BitMap mask = null)
        {
            var button = GUI.Button(new Rect2(position, size), normal, pressed, focus, parent, hover, mask);
            position.y += next_offset;
            return button;
        }

        public static HScrollBar FloatRange(string label, float value, float min, float max, Node parent = default)
        {
            var control = GUI.Draw<Control>(position, size, parent);
            position.y += next_offset;

            var label_control = GUI.Draw<Label>(control);
            label_control.Text = label;
            label_control.SetAnchors(0, .5f, 0, 1f);
            var scroll = GUI.Draw<HScrollBar>(control);
            scroll.SetAnchors(.5f, .75f, .25f, .75f);
            if (min > max)
            {
                var temp = min;
                max = min;
                min = temp;
            }
            scroll.Value = value.clamp(min, max);
            var text = GUI.Draw<TextEdit>(control);
            text.SetAnchors(.75f, 1f, 0f, 1f);
            text.Text = scroll.Value.ToString();

            scroll.OnValueChanged(val =>
            {
                text.Text = val.ToString();
            });

            text.OnTextChanged(() =>
            {
                if (float.TryParse(text.Text, out var val))
                {
                    scroll.Value = val;
                }
                else text.Text = scroll.Value.ToString();
            });
            return scroll;
        }
    }

    public static void DestroyAll()
    {
        if (_layer.IsValid())
            _layer.QueueFree();
    }
}
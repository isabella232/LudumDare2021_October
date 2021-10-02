using System.Collections.Generic;

public static class Signals
{
    static Emitter Connect<Emitter, Args>(string notification, Emitter emitter, in System.Action<Args> action) where Emitter : Godot.Node
    {
        foreach (Godot.Node child in emitter.GetChildren())
            if (child.Name == notification)
            {
                ((Relay<Args>)child).action += action;
                return emitter;
            }

        var relay = new Relay<Args>();
        relay.action += action;
        relay.Name = notification;
        emitter.AddChild(relay);
        emitter.Connect(notification, relay, "Invoke");
        return emitter;
    }

    static Emitter Connect<Emitter>(string notification, Emitter emitter, in System.Action action) where Emitter : Godot.Node
    {
        foreach (Godot.Node child in emitter.GetChildren())
            if (child.Name == notification)
            {
                ((Relay)child).action += action;
                return emitter;
            }

        var relay = new Relay();
        relay.Name = notification;
        relay.action += action;
        emitter.AddChild(relay);
        emitter.Connect(notification, relay, "Invoke");
        return emitter;
    }

    class Relay : Godot.Node
    {
        public System.Action action;

        public void Invoke()
        {
            action?.Invoke();
        }
    }

    class Relay<Args> : Godot.Node
    {
        public System.Action<Args> action;

        public void Invoke(Args args)
        {
            action?.Invoke(args);
        }
    }

    public static Emitter OnBodyEnter2D<Emitter>(this Emitter emitter, System.Action<Godot.Node> action) where Emitter : Godot.RigidBody2D
        => Connect("body_entered", emitter, action);

    public static Emitter OnBodyExit2D<Emitter>(this Emitter emitter, System.Action<Godot.Node> action) where Emitter : Godot.RigidBody2D
        => Connect("body_exited", emitter, action);

    public static Emitter OnEnterArea2D<Emitter>(this Emitter emitter, System.Action<Godot.Node> action) where Emitter : Godot.Area2D
        => Connect("area_entered", emitter, action);

    public static Emitter OnExitArea2D<Emitter>(this Emitter emitter, System.Action<Godot.Node> action) where Emitter : Godot.Area2D
        => Connect("area_exited", emitter, action);

    public static Emitter OnAreaEnterBody2D<Emitter>(this Emitter emitter, System.Action<Godot.Node> action) where Emitter : Godot.Area2D
        => Connect("body_entered", emitter, action);

    public static Emitter OnAreaExitBody2D<Emitter>(this Emitter emitter, System.Action<Godot.Node> action) where Emitter : Godot.Area2D
        => Connect("body_exited", emitter, action);

    public static Emitter OnEnterBody<Emitter>(this Emitter emitter, System.Action<Godot.Node> action) where Emitter : Godot.RigidBody
        => Connect("body_entered", emitter, action);

    public static Emitter OnExitBody<Emitter>(this Emitter emitter, System.Action<Godot.Node> action) where Emitter : Godot.RigidBody
        => Connect("body_exited", emitter, action);

    public static Emitter OnAreaEnterArea<Emitter>(this Emitter emitter, System.Action<Godot.Node> action) where Emitter : Godot.Area
        => Connect("area_entered", emitter, action);

    public static Emitter OnAreaExitArea<Emitter>(this Emitter emitter, System.Action<Godot.Node> action) where Emitter : Godot.Area
        => Connect("area_exited", emitter, action);

    public static Emitter OnAreaEnterBody<Emitter>(this Emitter emitter, System.Action< Godot.Node> action) where Emitter : Godot.Area
        => Connect("body_entered", emitter, action);

    public static Emitter OnAreaExitBody<Emitter>(this Emitter emitter, System.Action< Godot.Node> action) where Emitter : Godot.Area
        => Connect("body_exited", emitter, action);

    public static Emitter OnMouseEnterCollider<Emitter>(this Emitter emitter, System.Action< Godot.Node> action) where Emitter : Godot.CollisionObject
        => Connect("mouse_entered", emitter, action);

    public static Emitter OnMouseExitCollider<Emitter>(this Emitter emitter, System.Action< Godot.Node> action) where Emitter : Godot.CollisionObject
        => Connect("mouse_exited", emitter, action);

    public static Emitter OnTextChanged<Emitter>(this Emitter emitter, System.Action action) where Emitter : Godot.TextEdit
        => Connect("text_changed", emitter, action);

    public static Emitter OnRequestComplete<Emitter>(this Emitter emitter, System.Action action) where Emitter : Godot.TextEdit
        => Connect("request_completion", emitter, action);

    public static Emitter OnValueChanged<Emitter>(this Emitter emitter, System.Action<float> action) where Emitter : Godot.Range
        => Connect("value_changed", emitter, action);

    public static Emitter OnMouseEnter<Emitter>(this Emitter emitter, System.Action action) where Emitter : Godot.Control
        => Connect("mouse_entered", emitter, action);

    public static Emitter OnMouseExit<Emitter>(this Emitter emitter, System.Action action) where Emitter : Godot.Control
        => Connect("mouse_exited", emitter, action);

    public static Emitter OnButtonDown<Emitter>(this Emitter emitter, System.Action action) where Emitter : Godot.BaseButton
        => Connect("button_down", emitter, action);

    public static Emitter OnButtonUp<Emitter>(this Emitter emitter, System.Action action) where Emitter : Godot.BaseButton
        => Connect("button_up", emitter, action);

    public static Emitter OnToggle<Emitter>(this Emitter emitter, System.Action<bool> action) where Emitter : Godot.BaseButton
        => Connect("toggled", emitter, action);

    public static Emitter OnReady<Emitter>(this Emitter emitter, System.Action action) where Emitter : Godot.Node
        => Connect("ready", emitter, action);

    public static Emitter OnFocusedEntered<Emitter>(this Emitter emitter, System.Action action) where Emitter : Godot.Control
        => Connect("focus_entered", emitter, action);

    public static Emitter OnFocusedExit<Emitter>(this Emitter emitter, System.Action action) where Emitter : Godot.Control
        => Connect("focus_exited", emitter, action);

    public static Emitter OnEnterTree<Emitter>(this Emitter emitter, System.Action action) where Emitter : Godot.Node
        => Connect("tree_entered", emitter, action);

    public static Emitter OnExitTree<Emitter>(this Emitter emitter, System.Action action) where Emitter : Godot.Node
        => Connect("tree_exited", emitter, action);

    public static Emitter OnProcess<Emitter>(this Emitter emitter, System.Action<float> action) where Emitter : Godot.Node
    {
        foreach(Godot.Node child in emitter.GetChildren())
        {
            if (child is UpdateNode updater)
            {
                updater.action += action;
                return emitter;
            }
        }
        emitter.AddChild(new UpdateNode{action = action});
        return emitter;        
    }

    public static Emitter OnVisibilityChanged<Emitter>(this Emitter emitter, System.Action action) where Emitter : Godot.CanvasItem
        => Connect("visibility_changed", emitter, action);

    class UpdateNode : Godot.Node
    {
        public System.Action<float> action;

        public override void _Process(float delta)
        {
            action?.Invoke(delta);
        }
    }

    public static Emitter OnPhysicsProcess<Emitter>(this Emitter emitter, System.Action<float> action) where Emitter : Godot.Node
    {
        foreach(Godot.Node child in emitter.GetChildren())
        {
            if (child is PhysicsUpdateNode updater)
            {
                updater.action += action;
                return emitter;
            }
        }
        emitter.AddChild(new PhysicsUpdateNode{action = action});
        return emitter;  
    }

    class PhysicsUpdateNode : Godot.Node
    {
        public System.Action<float> action;

        public override void _PhysicsProcess(float delta)
        {
            action?.Invoke(delta);
        }
    }

    public static Emitter OnSelect<Emitter, Selection>(this Emitter emitter, System.Action<Selection> action) where Emitter : Godot.PopupMenu where Selection : System.Enum
    {
        foreach(var child in emitter.GetChildren())
            if (child is MenuSelectRelay<Selection> relay)
            {
                relay.action += action;
                return emitter;
            }
        
        {
            var relay = new MenuSelectRelay<Selection>();
            relay.action += action;
            emitter.AddChild(relay);        
        }
        return emitter;
    }

    class MenuSelectRelay<Selection> : Godot.Node where Selection : System.Enum
    {
        public override void _Ready()
        {
            var popup = this.GetParent() as Godot.PopupMenu;
            foreach (var item in Enum<Selection>.Values)
                popup.AddItem(item.ToString().Replace("_", " "), (int)(object)item);
            popup.Connect("id_pressed", this, "Connect");
        }

        public System.Action<Selection> action;
        void Connect(int id)
        {
            foreach(Selection val in Enum<Selection>.Values)
            {
                if ((int)(object)val == id)
                {
                    action?.Invoke(val);
                    return;
                }
            }
        }
    }


    public static void AddSendMessageCallback<T>(this Godot.Node node, in System.Action<T> callback)
    {
        if (node.IsValid())
        {
            if (!node_event_handlers.TryGetValue(node, out var event_handle))
            {
                node_event_handlers[node] = event_handle = new EventHandle();
                node.AddChild(event_handle);
            }
            event_handle.Register(callback);
        }
    }

    public static void RemoveSendMessageCallback<T>(this Godot.Node node, in System.Action<T> callback)
    {
        if (node_event_handlers.TryGetValue(node, out var handler))
            handler.UnRegister(callback);
    }

    public static void SendMessage<T>(this Godot.Node node, in T args, bool propogate_up = false)
    {
        if (node_event_handlers.TryGetValue(node, out var handler))
            handler.Invoke(args);

        if (propogate_up)
            if (node.IsValid())
                SendMessage(node.GetParent(), args, propogate_up);
    }

    static Dictionary<Godot.Node, EventHandle> node_event_handlers = new Dictionary<Godot.Node, EventHandle>();
    class EventHandle : Godot.Node
    {
        Dictionary<int, object> events = new Dictionary<int, object>();

        static int newID;
        class Event<T>
        {
            public static readonly int ID = newID++;

            public System.Action<T> callback;

            public override string ToString()
            => $"{typeof(T).Name}";
        }

        public void Register<T>(System.Action<T> callback)
        {
            if (!events.TryGetValue(Event<T>.ID, out var val))
                events[Event<T>.ID] = val = new Event<T>();
            ((Event<T>)val).callback += callback;
        }

        public void UnRegister<T>(System.Action<T> callback)
        {
            if (events.TryGetValue(Event<T>.ID, out var val))
                ((Event<T>)val).callback -= callback;
        }

        public void Invoke<T>(T args)
        {
            if (events.TryGetValue(Event<T>.ID, out var val))
            {
                ((Event<T>)val).callback?.Invoke(args);
            }
        }

        public override void _ExitTree()
        {
            node_event_handlers.Remove(GetParent());
        }
    }
}

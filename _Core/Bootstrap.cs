using Godot;

namespace Events
{
    using GameSystems;
    using System.Collections.Generic;
    using GameSystems.Events;

    public class Bootstrap : Node
    {
        public override void _EnterTree()
        {
            this.ProcessPriority = int.MinValue;
            PauseMode = PauseModeEnum.Process;
            Event.Send(new GameSystems.Events.SystemBootstrap{bootstrap = this});
            Event.Send(this);
        }

        public override void _Process(float delta)
        {
            Event.Send(new SystemUpdate { delta_time = delta });
            delta = Time.paused ? 0 : delta;

            Event.Send(new Events.FramePreUpdate { delta_time = delta });
            Event.Send(new Events.FrameUpdate { delta_time = delta });
            Event.Send(new Events.FrameLateUpdate { delta_time = delta });
            Event.Send(new SystemLateUpdate { delta_time = delta });
        }

        public override void _PhysicsProcess(float delta)
        {
            Event.Send(new SystemPhysicsUpdate { delta_time = delta });

            delta = Time.paused ? 0 : delta;

            Event.Send(new Events.PhysicsPreUpdate { delta_time = delta });
            Event.Send(new Events.PhysicsUpdate { delta_time = delta });
            Event.Send(new Events.PhysicsLateUpdate { delta_time = delta });
            Event.Send(new Events.PhysicsLateUpdate{delta_time = delta});
        }

        public override void _Input(InputEvent input_event)
        {
            Event.Send(input_event);
        }

        public override void _Notification(int what)
        {
            if (what == MainLoop.NotificationWmQuitRequest)
            {
                Event.Send(new Events.ShutDown());
            }
        }
    }

    public struct FramePreUpdate { public float delta_time; }

    public struct FrameUpdate { public float delta_time; }

    public struct FrameLateUpdate { public float delta_time; }

    public struct PhysicsPreUpdate { public float delta_time; }

    public struct PhysicsUpdate { public float delta_time; }

    public struct PhysicsLateUpdate { public float delta_time; }

    public struct ShutDown {}
}

namespace GameSystems.Events
{
    public struct SystemBootstrap {public global::Events.Bootstrap bootstrap;}
    public struct SystemUpdate { public float delta_time; }
    public struct SystemLateUpdate { public float delta_time; }
    public struct SystemPhysicsUpdate { public float delta_time; }
    public struct SystemPhysicsLateUpdate { public float delta_time; }
}
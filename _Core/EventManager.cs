using System;
using System.Reflection;
using System.Collections.Generic;
using Events;

/// <summary>
/// Takes a single arguement
/// Static methods marked [Event] with <T> paramteter will recieve messages when Event.Send<T>(T args) is called
/// </summary>
[System.AttributeUsage(System.AttributeTargets.Method, AllowMultiple = false)]
class Event : Attribute
{
    public Event(int order) => this.order = order;
    public Event() { }
    public int order;
    static List<MethodInfo> infos = new List<MethodInfo>();

    static Event()
    {
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            if (assembly.FullName.StartsWith("System.")) continue;
            if (assembly.FullName.StartsWith("System,")) continue;
            if (assembly.FullName.StartsWith("netstandard")) continue;
            if (assembly.FullName.StartsWith("mscorlib")) continue;
            if (assembly.FullName.StartsWith("Godot")) continue;

            foreach (var type in assembly.GetTypes())
            {
                if (type.IsGenericType) continue;
                foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance))
                {
                    if (method.GetCustomAttribute<Event>(false) == null) continue;
                    if (!method.IsStatic) throw new Exception($"{method.DeclaringType.FullName}.{method.Name} must be marked static to use [Event] attribute");
                    var param = method.GetParameters();
                    if (param.Length != 1) throw new Exception($"{method.DeclaringType.FullName}.{method.Name} is marked as {nameof(Event)} but has too many arguements");
                    infos.Add(method);
                }
            }
        }
    }

    public static void Send<T>(T args)
    {
        if (!GatherDiagnostics) EventHandler<T>.handle?.Invoke(args);
        else EventHandler<T>.Invoke(args);
    }
    static List<EventDiagnostic> all_timings = new List<EventDiagnostic>();
    static class EventHandler<T>
    {
        static EventDiagnostic diagnostic;

        public static void Invoke(T args)
        {
            var list = handle?.GetInvocationList();
            int count = list == null ? 0 : list.Length;
            if (diagnostic == null)
            {
                diagnostic = new EventDiagnostic();
                all_timings.Add(diagnostic);
                diagnostic.event_name = typeof(T).Name.AddSpaceBeforeCaps().ToUpper();
                diagnostic.methods = new string[count];
                diagnostic.method_times = new TimeSpan[count];

                for (int i = 0; i < count; ++i)
                {
                    var method = list[i].Method;
                    var owner = method.DeclaringType.Name.AddSpaceBeforeCaps().ToLower();
                    var name = method.Name.AddSpaceBeforeCaps().ToLower();
                    diagnostic.methods[i] = $"{owner} -> {name}".AddSpaceBeforeCaps().ToLower();
                }
            }
            for (int i = 0; i < count; ++i)
            {
                var time = Time.timespan_since_startup;
                ((Action<T>)list[i]).Invoke(args);
                diagnostic.method_times[i] = Time.timespan_since_startup - time;
            }
        }

        static EventHandler()
        {
            List<(int order, MethodInfo method)> pending = new List<(int order, MethodInfo method)>();
            foreach (var method in infos)
            {
                if (method.GetParameters()[0].ParameterType != typeof(T)) continue;
                pending.Add((method.GetCustomAttribute<Event>(), method));
            }

            pending.Sort((x, y) =>
            {
                int val = x.order.CompareTo(y.order);
                if (val == 0) val = x.method.Name.CompareTo(y.method.Name);
                return val;
            });

            foreach (var item in pending)
            {
                handle += (Action<T>)Delegate.CreateDelegate(typeof(Action<>).MakeGenericType(typeof(T)), null, item.method);
            }
        }
        public static Action<T> handle;
    }
    static bool GatherDiagnostics;
    public static implicit operator int(Event callback) => callback == null ? 0 : callback.order;

    class EventDiagnostic
    {
        public string event_name;
        public TimeSpan event_time;
        public string[] methods;
        public TimeSpan[] method_times;
    }

    [Command]
    static void ShowEvents(Command args)
    {
        GatherDiagnostics = !GatherDiagnostics;
        Coroutine.Start(() =>
        {
            if (GatherDiagnostics)
            {
                Debug.Label("---------------------");
                Debug.Label("       EVENTS        ");
                Debug.Label("---------------------");

                foreach (var item in all_timings)
                {
                    if (item.methods.Length == 0) continue;
                    Debug.Label(item.event_name, $"{item.event_time.TotalMilliseconds: 0.000}ms");
                    Debug.Label("------------------");
                    item.event_time = default;
                    for (int i = 0; i < item.methods.Length; ++i)
                    {
                        Debug.Label(item.methods[i], $"{item.method_times[i].TotalMilliseconds: 0.000}ms");
                        item.event_time += item.method_times[i];
                    }
                    Debug.Label("");
                    Debug.Label("");
                }
            }
            return GatherDiagnostics;
        });
    }
}
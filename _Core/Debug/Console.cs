using Godot;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace GameSystems
{
    using GameSystems.Events;

    class Console
    {
        static Console_GUI node;

        [Event]
        static void OnUpdate(SystemUpdate args)
        {
            if (Inputs.key_back_quote.OnPress())
            {
                if (!Node.IsInstanceValid(node))
                {
                    node = new Console_GUI();
                    Scene.Current.AddChild(node);
                    node.PauseMode = Node.PauseModeEnum.Process;
                    update_log = true;
                }
                else
                {
                    node.QueueFree();
                    node = null;
                }
            }
        }

        public static void Clear()
        {
            log.Clear();
            update_log = true;
        }

        static bool update_log;
        static List<string> log = new List<string>();
        public static void Log(string message)
        {
            log.Insert(0, message);
            if (log.Count > 10000)
                log.RemoveAt(log.Count - 1);
            update_log = true;
        }

        class Console_GUI : CanvasLayer
        {
            static List<string> previousCommands = new List<string>();
            static Dictionary<string, CommandInfo> commands = new Dictionary<string, CommandInfo>();
            class CommandInfo
            {
                public string ImplementingClass;
                public string MethodName;
                public string help;
                public Action<Command> Action;

                public override string ToString()
                => $"{ImplementingClass}->{MethodName}";
            }

            static Console_GUI()
            {
                foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
                {
                    if (type.IsGenericType) continue;
                    foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
                    {
                        var p = method.GetParameters();
                        if (p.Length != 1 || p[0].ParameterType != typeof(Command))
                            continue;
                        
                        var info = new CommandInfo();
                        info.MethodName = method.Name;
                        info.ImplementingClass = method.DeclaringType.Name;
                        var command = method.GetCustomAttribute<Command>(false);
                        if (command != null)
                            info.help = command.original;
                            
                        string commandName = method.Name.ToLower().Replace("_", "");
                        if (commands.TryGetValue(method.Name.ToLower(), out var stored))
                            Debug.Log("Command:", info, "name colliison, overwriting", stored);
                        commands[commandName] = info;
                        info.Action = (Action<Command>)Delegate.CreateDelegate(typeof(Action<Command>), null, method);
                    }
                }
            }

            LineEdit console_input = new LineEdit();
            Label console_log = new Label();
            Panel panel = new Panel();
            VScrollBar scroll = new VScrollBar();

            int command_index = -1;
            public override void _Process(float delta)
            {
                ExecuteConsoleInput();
                CyclePreviousCommands();
                DrawConsoleLog();
            }

            void CyclePreviousCommands()
            {
                if (previousCommands.Count > 0)
                {
                    if (Inputs.key_down_arrow.OnPress())
                    {
                        command_index--;
                        if (command_index < 0)
                            command_index = previousCommands.Count - 1;
                        console_input.Text = previousCommands[command_index];
                    }

                    if (Inputs.key_up_arrow.OnPress())
                    {
                        command_index = (command_index + 1) % previousCommands.Count;
                        command_index = command_index.min(0);
                        console_input.Text = previousCommands[command_index];
                    }
                }
            }

            void ExecuteConsoleInput()
            {
                if ( (Inputs.key_enter.OnPress() || Inputs.key_pad_enter.OnPress()) && !string.IsNullOrEmpty(console_input.Text))
                {
                    command_index = -1;
                    previousCommands.Insert(0, console_input.Text);
                    if (previousCommands.Count > 50)
                        previousCommands.RemoveAt(previousCommands.Count - 1);
                    string[] items = console_input.Text.ToLower().Split(' ');
                    for (int i = items.Length-1; i >= 0; --i)
                    {
                        string command = concact(i, items);
                        if (commands.TryGetValue(command, out var execute))
                        {
                            var args = new Command();
                            args[-1] = console_input.Text;
                            for (var itr = i + 1; itr < items.Length; ++itr)
                            {
                                if (!string.IsNullOrEmpty(items[itr]))
                                    args[itr - (i + 1)] = items[itr];
                            }
                            console_input.Text = "";
                            execute.Action(args);
                            return;
                        }
                    }
                    console_input.Text = "";
                }

                string concact(int to_index, string[] items)
                {
                    string val = "";
                    for(int i = 0; i <= to_index; ++ i)
                        val += items[i];
                    return val;
                }
            }

            public override void _EnterTree()
            {
                AddChildNodes();
                SetupPanels(20f);
                CallDeferred(nameof(GrabFocus));
            }

            void AddChildNodes()
            {
                this.AddChild(console_input);
                this.AddChild(scroll);
                this.AddChild(panel);
                panel.AddChild(console_log);
            }

            void SetupPanels(float TextSize)
            {
                console_input.AnchorRight = .3f;
                console_input.MarginBottom = TextSize;

                scroll.MarginRight = 12;
                scroll.AnchorBottom = 1;
                scroll.MarginTop = TextSize + 4;

                panel.MarginTop = TextSize + 5;
                panel.MarginBottom = -1;
                panel.MarginLeft = scroll.MarginRight;
                panel.AnchorBottom = 1;
                panel.AnchorRight = .3f;
                panel.Modulate = new Color(1, 1, 1, .9f);

                console_log.AnchorRight = 1;
                console_log.AnchorBottom = 1;
                console_log.MarginLeft = 2;
                console_log.MarginRight = -2;
                console_log.Autowrap = true;
            }

            int last_scroll_index;
            static System.Text.StringBuilder builder = new System.Text.StringBuilder();
            void DrawConsoleLog()
            {
                int index = ((int)scroll.Value).clamp(0, log.Count - 1);
                if (last_scroll_index != index)
                    update_log = true;

                scroll.MaxValue = log.Count.min(1);

                if (update_log)
                {
                    int maxLines = (int)(panel.RectSize.y / (console_input.RectSize.y - 7));

                    for (int i = index; i < log.Count && i < index + maxLines; ++i)
                    {
                        builder.Append(log[i]);
                        builder.Append("\n");
                    }
                    console_log.Text = builder.ToString();
                    builder.Clear();
                }
                last_scroll_index = index;
                update_log = false;
            }

            void GrabFocus()
            {
                console_input.GrabFocus();
                console_input.Text = "";
            }

            [Command("type help followed by any of the following commands for info about that command")]
            static void Help(Command value)
            {
                var command = "";

                for (int i = 0; i < value.Count; ++i)
                {
                    command += value[i];
                    if (commands.TryGetValue(command, out var info))
                    {
                        Debug.Log();
                        Debug.Log(info.help == null ? "No additional info found.." : info.help);
                        Debug.Log(info);
                        return;
                    }
                }

                Console.Log("");
                foreach (var item in commands)
                {
                    Console.Log(item.Value.MethodName.AddSpaceBeforeCaps());
                }
                Console.Log("");
                Console.Log("type help followed by any of the following commands for info about that command");
                Console.Log("--- HELP ---");
            }

            [Command("Filters command console to show only entries containing supplied input")]
            static void Filter(Command args)
            {
                for (int i = log.Count - 1; i >= 0; --i)
                {
                    var text = log[i].ToLower();

                    for (int index = 0; index < args.Count; ++index)
                    {
                        if (text.IndexOf(args.ToString(index), System.StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            goto Continue;
                        }
                    }
                    log.RemoveAt(i);
                    update_log = true;

                Continue:
                    continue;
                }
            }
        }
    }
}

/// <summary>
/// Calls method when typed into the console window\n
/// Must look like [Command] void method_Name(Command args).
/// Only works on static methods. 
/// </summary>
[System.AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class Command : System.Attribute
{
    public string this[int index]
    {
        get
        {
            if (index < 0) return "";
            if (index >= args_count) return "";
            return args[index];
        }
        set
        {
            if (index < 0)
            {
                original = value;
                return;
            }
            if (index >= args.Length)
                Array.Resize(ref args, index + 1);
            args[index] = value;
            if (args_count <= index)
                args_count = index+1;
        }
    }

    public Command() { }

    public Command(string info)
    {
        original = info;
    }

    /// <summary>
    /// The original command input
    /// </summary>
    public string original;
    string[] args = new string[16];
    int args_count = 0;

    public int Count => args_count;

    public bool TryParse(int arg, out int value)
        => int.TryParse(this[arg], out value);

    public bool TryParse(int arg, out float value)
        => float.TryParse(this[arg], out value);

    bool TryParse(int arg, out bool value)
    {
        var val = this[arg];
        if (val == "true" || val == "t")
            value = true;
        else if (val == "false" || val == "f")
            value = false;
        else
        {
            value = default;
            return false;
        }
        return true;
    }

    bool TryParse(int arg, out string value)
    {
        value = this[arg];
        return value == "";
    }

    public bool ToBool(int arg = 0)
    {
        TryParse(arg, out bool val);
        return val;
    }

    public int ToInt(int arg = 0)
    {
        TryParse(arg, out int val);
        return val;
    }

    public string ToString(int arg)
        => this[arg];

    public float ToFloat(int arg = 0)
    {
        TryParse(arg, out float val);
        return val;
    }

    public override string ToString() => ToString(0);
}
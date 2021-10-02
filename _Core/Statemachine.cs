
public class StateMachine<States> where States : struct, System.Enum
{
    public StateMachine()
    {
        next_state = current_state;
    }

    public bool enable_transitions = true;

    States? _next_state;
    public States? next_state
    {
        get => _next_state;
        set
        {
            if (enable_transitions)
                _next_state = value;
        }
    }

    /// <summary>
    /// times update has been called
    /// </summary>
    public int total_updates { get; private set; }

    /// <value></value>
    public States previous_state { get; private set; }

    /// <summary>
    /// times update was called while in previous state
    /// </summary>
    /// <value></value>
    public int previous_state_updates {get; private set;}

    /// <summary>
    /// time spent in the previous state
    /// </summary>
    public float previous_state_time { get; private set; }

    public States current_state { get; private set; }
    
    /// <summary>
    /// times update was called in the current state so far
    /// </summary>
    /// <value></value>
    public int current_state_updates {get; private set;}

    /// <summary>
    /// time spent in the current state
    /// </summary>
    public float current_state_time { get; private set; }

    /// <summary>
    /// true if this update is the first update in current state
    /// </summary>
    public bool entered_state => current_state_time == 0;

    /// <summary>
    /// true if state is going change next update
    /// </summary>
    public bool exiting_state => next_state != null;

    /// <summary>
    /// time between updates
    /// </summary>
    public float delta_time { get; private set; }

    public void Update(float delta_time)
    {
        total_updates ++;
        this.delta_time = delta_time;

        if (next_state.HasValue)
        {
            previous_state = current_state;
            current_state = next_state.Value;
            next_state = null;
            previous_state_time = current_state_time;
            previous_state_updates = current_state_updates;
            current_state_time = 0;
            current_state_updates = 0;
        }
        else
        {
            current_state_time += delta_time;
            current_state_updates ++;
        }
    }

    public override string ToString() => $"{typeof(States).Name.Replace("_", " ")}: {current_state.ToString().Replace("_", " ")}";
}
using System;
using UnityEngine;

public class EventBus
{
    private EventBus() { }

    private static EventBus _instance;

    public static EventBus Instance
    {
        get
        {
            if (_instance == null)
                _instance = new EventBus();
            return _instance;
        }
    }
    public Action PlayerDied;
    public Action<int> PlayerDamaged;

/*    public event Action<bool, float> GroundedChanged;
    public event Action Jumped;*/

}

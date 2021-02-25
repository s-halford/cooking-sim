using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Buttons
{
    Right, Left, Up, Down, Action
}

[System.Serializable]
public class InputAxisState
{
    public string axisName;
    public Buttons button;

    public float rawValue
    {
        get
        {
            return Input.GetAxisRaw(axisName);
        }
    }

    public bool didKeyDown
    {
        get
        {
            return Input.GetKeyDown(axisName);
        }
    }
}

public class InputManager : MonoBehaviour
{
    public InputAxisState[] inputs;
    public InputState inputState;

    private void Update()
    {
        foreach(var input in inputs)
        {
            inputState.SetButtonValue(input.button, input.rawValue);
        }
    }
}

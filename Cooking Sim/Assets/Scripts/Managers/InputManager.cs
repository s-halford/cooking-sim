using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Our abstract buttons
// Buttons are abstracted to allow for easy multiplayer setup
public enum Buttons
{
    Right, Left, Up, Down, Action
}


// We want to map our buttons in the inspector, so serialize the InputAxis state
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

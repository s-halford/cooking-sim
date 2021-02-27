using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Our abstract buttons
// Buttons are abstracted to allow for easy multiplayer setup
public enum Buttons
{
    Right, Left, Up, Down, Action
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

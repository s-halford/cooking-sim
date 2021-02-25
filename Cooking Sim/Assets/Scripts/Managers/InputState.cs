using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonState
{
    public float rawValue;
}


public class InputState : MonoBehaviour
{
    private Dictionary<Buttons, ButtonState> buttonStates = new Dictionary<Buttons, ButtonState>();

    // Set the rawValue for our abstracted button
    public void SetButtonValue(Buttons key, float rawValue)
    {
        // Add our buttons if they haven't been created yet
        if (!buttonStates.ContainsKey(key))
            buttonStates.Add(key, new ButtonState());

        var state = buttonStates[key];

        state.rawValue = rawValue;
    }

    // Determine if button is pressed by checking if rawValue is not zero
    public bool GetButtonValue(Buttons key)
    {
        if (buttonStates.ContainsKey(key))
            return buttonStates[key].rawValue != 0;
        else
            return false;
    }
}

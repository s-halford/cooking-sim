using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

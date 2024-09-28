using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputConfig
{
    
}

public class ControlInput
{
    public Controls controlType;
    public KeyCode keyCode;
    public ControlInput(Controls controlType, KeyCode keyCode)
    {
        this.controlType = controlType;
        this.keyCode = keyCode;
    }
}

public enum Controls
{

}
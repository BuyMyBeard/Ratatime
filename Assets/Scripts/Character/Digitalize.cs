using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

#if UNITY_EDITOR
[InitializeOnLoad]
#endif
public class Digitalize : InputProcessor<float>
{
    #if UNITY_EDITOR
    static Digitalize()
    {
        Initialize();
    }
    #endif

    [RuntimeInitializeOnLoadMethod]
    static void Initialize()
    {
        InputSystem.RegisterProcessor<Digitalize>();
    }

    [Tooltip("min")]
    public float min = 0;
    public override float Process(float value, InputControl control)
    {
        if (value > min)
            return 1;
        else if (value < -min)
            return -1;
        return 0;

    }
}

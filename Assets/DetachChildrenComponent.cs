using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetachChildrenComponent : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        transform.DetachChildren();
    }
}

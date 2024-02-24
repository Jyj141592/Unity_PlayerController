using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerController{
public class PCNode : ScriptableObject
{
    public string actionName;
    [HideInInspector]
    public Vector2 position;
    [HideInInspector]
    public string guid;
    //[HideInInspector]
    public List<Transition> transition;
}
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerController{
public class PCNode : ScriptableObject
{
    public string actionName;
    public Vector2 position;
    public string guid;
    public List<Transition> transition;
}
}
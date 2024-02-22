using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace PlayerController.Editor{
public class PCEdgeView : Edge
{
    public Transition transition;
    public PCEdgeView(Transition transition){
        this.transition = transition;
    }
}
}
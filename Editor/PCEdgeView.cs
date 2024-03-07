using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using System;

namespace PlayerController.Editor{
public class PCEdgeView : Edge
{
    public Transition transition;
    public Action<PCEdgeView> onEdgeSelected;
    public int transitionIndex{
        get => _transitionIndex;
        set{
            if(value != _transitionIndex){
                _transitionIndex = value;
                updated = true;
            }  
        }
    }
    //public Action onUpdated;
    public bool updated = false;
    public Action onDeleted;
    private int _transitionIndex = -1;
    public PCEdgeView(Edge edge, Transition transition, Action<PCEdgeView> action, int index){
        this.transition = transition;
        output = edge.output;
        input = edge.input;
        onEdgeSelected = action;
        transitionIndex = index;
    }
    public PCEdgeView(Port input, Port output, Transition transition, Action<PCEdgeView> action, int index){
        this.transition = transition;
        this.output = output;
        this.input = input;
        onEdgeSelected = action;
        transitionIndex = index;
    }
    public override void OnSelected()
    {
        base.OnSelected();
        onEdgeSelected?.Invoke(this);
    }
    public override string ToString()
    {
        PCNodeView i = input.node as PCNodeView;
        PCNodeView o = output.node as PCNodeView;
        return o.node.actionName + " -> " + i.node.actionName;
    }
}
}
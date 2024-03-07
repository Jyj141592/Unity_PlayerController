using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;
using UnityEditor;
using System.Linq;
using UnityEditor.Rendering;
using UnityEngine.UI;

namespace PlayerController.Editor{
public class PCNodeView : Node
{
    public PCNode node;
    public TextElement nodeTitle;
    public Port inputPort = null;
    public Port outputPort = null;
    private Color defaultColor = new Color(80f / 255f, 80f / 255f, 80f / 255f);
    public Action<PCNodeView> onNodeSelected;
    //public Action onUpdated;
    public bool updated = false;
    public Action onDeleted;
    private SerializedObject obj;
    private Func<string, string, string> onNodeNameChanged;

#region Initialize
    public PCNodeView(PCNode node, Action<PCNodeView> action, Func<string, string, string> onNodeNameChanged){
        this.node = node;
        onNodeSelected = action;
        this.onNodeNameChanged = onNodeNameChanged;
        obj = new SerializedObject(node);
    }
    public void Draw(string titleText){
        mainContainer.Remove(titleContainer);
        topContainer.Remove(inputContainer);
        topContainer.Remove(outputContainer);
        mainContainer.style.backgroundColor = defaultColor;
        mainContainer.style.minWidth = 90;
        mainContainer.style.maxWidth = 300;

        // Create input port
        if(node is not PlayerControllerAsset && node is not AnyState){
            inputPort = CreatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Multi);
            inputPort.style.marginLeft = 8;
            mainContainer.Insert(0, inputPort);
        }
        else{
            VisualElement element = new VisualElement();
            element.style.height = 20;
            mainContainer.Insert(0, element);
        }

        nodeTitle = new TextElement();
        nodeTitle.BindProperty(obj.FindProperty("_actionName"));

        nodeTitle.style.height = 20;
        nodeTitle.style.alignSelf = Align.Center;
        nodeTitle.style.fontSize = 15;
        mainContainer.Add(nodeTitle);

        outputPort = CreatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Multi);
        outputPort.style.marginRight = 8;
        mainContainer.Add(outputPort);
    }
    private Port CreatePort(Orientation orientation, Direction direction, Port.Capacity capacity){
        Port port = InstantiatePort(orientation, direction, capacity, typeof(bool));
        port.portName = null;
        FlexDirection dir = direction == Direction.Input ? FlexDirection.Column : FlexDirection.ColumnReverse;
        port.style.alignSelf = Align.Center;
        return port;
    }
#endregion Initialize

#region Callbacks
    public override void OnSelected()
    {
        base.OnSelected();
        onNodeSelected?.Invoke(this);
    }
    // Modifing Asset
    public override void SetPosition(Rect newPos)
    {
        base.SetPosition(newPos);
        // Undo
        Undo.RecordObject(node, "Set Position");
        obj.FindProperty("_position").vector2Value = newPos.position;
        obj.ApplyModifiedProperties();
    }

    public void MoveTransitionIndex(int from, int to){
        Undo.RecordObject(node, "Change Transition Order");
        SerializedObject obj = new SerializedObject(node);
        SerializedProperty property = obj.FindProperty("_transitions");
        var list = outputPort.connections.ToList();
        PCEdgeView edge = list[from] as PCEdgeView;
        edge.transitionIndex = to;
        PCEdgeView edgeView;
        outputPort.DisconnectAll();
        if(to > from){
            for(int i = 0; i < from; i++){
                edgeView = list[i] as PCEdgeView;
                outputPort.Connect(edgeView);
            }
            for(int i = from; i < to; i++){
                edgeView = list[i + 1] as PCEdgeView;
                outputPort.Connect(edgeView);
                property.MoveArrayElement(i, i + 1);
                edgeView.transition = node.transitions[i];
            }
            outputPort.Connect(edge);
            edge.transition = node.transitions[to];
            for(int i = to + 1; i < list.Count(); i++){
                edgeView = list[i] as PCEdgeView;
                outputPort.Connect(edgeView);
            }
        }
        else{
            for(int i = 0; i < to; i++){
                edgeView = list[i] as PCEdgeView;
                outputPort.Connect(edgeView);
            }
            outputPort.Connect(edge);
            edge.transition = node.transitions[to];
            for(int i = to + 1; i <= from; i++){
                edgeView = list[i - 1] as PCEdgeView;
                outputPort.Connect(edgeView);
                edgeView.transition = node.transitions[i];
            }
            for(int i = from; i > to; i--){
                property.MoveArrayElement(i, i - 1);
            }
            for(int i = from + 1; i < list.Count(); i++){
                edgeView = list[i] as PCEdgeView;
                outputPort.Connect(edgeView);
            }
        }
        obj.ApplyModifiedProperties();
        if(!Application.isPlaying)
            AssetDatabase.SaveAssets();
        int j = 0;
        for(j = 0; j < outputPort.connections.Count(); j++){
            PCEdgeView view = outputPort.connections.ElementAt(j) as PCEdgeView;
            view.transition = node.transitions[j];
            view.transitionIndex = j;
        }
    }
    
    public string OnNodeNameChanged(string oldVal, string newVal){
        if(nodeTitle.text.Equals(newVal)) return newVal;
        Undo.RecordObject(node, "Rename Node");
        string newName = onNodeNameChanged.Invoke(oldVal, newVal);
        obj.FindProperty("_actionName").stringValue = newName;
        obj.FindProperty("_actionID").intValue = Animator.StringToHash(newName);
        obj.ApplyModifiedProperties();

        if(!Application.isPlaying){
            AssetDatabase.SaveAssets();
        }
        return newName;
    }
    public void OnDeleteEdge(int index){
        for(int i = 0; i < outputPort.connections.Count(); i++){
            PCEdgeView edge = outputPort.connections.ElementAt(i) as PCEdgeView;
            edge.transition = node.transitions[i];
            edge.transitionIndex = i;
        }
        updated = true;
    }

    public void OnStateUpdate(){
        if(node.state == PCNode.NodeState.Runnning){
            mainContainer.style.backgroundColor = Color.yellow;
        }
        else{
            mainContainer.style.backgroundColor = defaultColor;
        }
    }
#endregion Callbacks
}
}
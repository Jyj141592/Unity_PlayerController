using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using System;
using UnityEditor;
using System.Linq;
using UnityEditor.Rendering;

namespace PlayerController.Editor{
public class PCNodeView : Node
{
    public PCNode node;
    public TextElement nodeTitle;
    public Port inputPort = null;
    public Port outputPort = null;
    private Color defaultColor = new Color(80f / 255f, 80f / 255f, 80f / 255f);
    public Action<PCNodeView> onNodeSelected;
    public Action onUpdated;
    public Action onDeleted;
    private Func<string, string, string> onNodeNameChanged;

#region Initialize
    public PCNodeView(PCNode node, Action<PCNodeView> action, Func<string, string, string> onNodeNameChanged){
        this.node = node;
        onNodeSelected = action;
        this.onNodeNameChanged = onNodeNameChanged;
    }
    public void Draw(string titleText){
        mainContainer.Remove(titleContainer);
        topContainer.Remove(inputContainer);
        topContainer.Remove(outputContainer);
        mainContainer.style.backgroundColor = defaultColor;
        mainContainer.style.minWidth = 90;
        mainContainer.style.maxWidth = 300;

        // Create input port
        if(node is not PlayerControllerAsset){
            inputPort = CreatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Multi);
            inputPort.style.marginLeft = 8;
            mainContainer.Insert(0, inputPort);
        }
        else{
            VisualElement element = new VisualElement();
            element.style.height = 20;
            mainContainer.Insert(0, element);
        }

        nodeTitle = new TextElement(){text = titleText};
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
        node.position = newPos.position;
        if(!Application.isPlaying)
            AssetDatabase.SaveAssets();
    }

    public void MoveTransitionIndex(int from, int to){
        Undo.RecordObject(node, "Change Transition Order");
        SerializedObject obj = new SerializedObject(node);
        SerializedProperty property = obj.FindProperty("transition");
        var list = outputPort.connections.ToList();
        PCEdgeView edge = list[from] as PCEdgeView;
        edge.transitionIndex = to;
        PCEdgeView edgeView;
        Transition transition = node.transition[from];
        outputPort.DisconnectAll();
        if(to > from){
            for(int i = 0; i < from; i++){
                edgeView = list[i] as PCEdgeView;
                edgeView.transitionIndex = i;
                outputPort.Connect(edgeView);
            }
            for(int i = from; i < to; i++){
                edgeView = list[i + 1] as PCEdgeView;
                edgeView.transitionIndex = i;
                outputPort.Connect(edgeView);
                property.MoveArrayElement(i, i + 1);
                edgeView.transition = node.transition[i];
            }
            outputPort.Connect(edge);
            edge.transition = node.transition[to];
            for(int i = to + 1; i < list.Count(); i++){
                edgeView = list[i] as PCEdgeView;
                edgeView.transitionIndex = i;
                outputPort.Connect(edgeView);
            }
        }
        else{
            for(int i = 0; i < to; i++){
                edgeView = list[i] as PCEdgeView;
                edgeView.transitionIndex = i;
                outputPort.Connect(edgeView);
            }
            outputPort.Connect(edge);
            edge.transition = node.transition[to];
            for(int i = to + 1; i <= from; i++){
                edgeView = list[i - 1] as PCEdgeView;
                edgeView.transitionIndex = i;
                outputPort.Connect(edgeView);
                edgeView.transition = node.transition[i];
            }
            for(int i = from; i > to; i--){
                property.MoveArrayElement(i, i - 1);
            }
            for(int i = from + 1; i < list.Count(); i++){
                edgeView = list[i] as PCEdgeView;
                edgeView.transitionIndex = i;
                outputPort.Connect(edgeView);
            }
        }
        obj.ApplyModifiedProperties();
        if(!Application.isPlaying)
            AssetDatabase.SaveAssets();
    }
    
    public string OnNodeNameChanged(string oldVal, string newVal){
        if(nodeTitle.text.Equals(newVal)) return newVal;
        Undo.RecordObject(node, "Rename Node");
        string newName = onNodeNameChanged.Invoke(oldVal, newVal);
        nodeTitle.text = newName;
        node.actionName = newName;

        if(!Application.isPlaying){
            AssetDatabase.SaveAssets();
        }
        return newName;
    }
    public void OnDeleteEdge(int index){
        for(int i = index; i < outputPort.connections.Count(); i++){
            PCEdgeView edge = outputPort.connections.ElementAt(i) as PCEdgeView;
            edge.transitionIndex = i;
        }
        onUpdated?.Invoke();
    }
#endregion Callbacks
}
}
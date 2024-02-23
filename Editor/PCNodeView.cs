using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using System;
using UnityEditor;

namespace PlayerController.Editor{
public class PCNodeView : Node
{
    public PCNode node;
    public TextElement nodeTitle;
    public Port inputPort = null;
    public Port outputPort = null;
    private Color defaultColor = new Color(80f / 255f, 80f / 255f, 80f / 255f);
    public Action<PCNodeView> onNodeSelected;
    public bool updated = true;
    public bool deleted = false;
    public Func<string, string, string> onNodeNameChanged;

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

        AssetDatabase.SaveAssets();
    }

    //
    // public string OnNodeNameChanged(string oldVal, string newVal){
    //     string newName = onNodeNameChanged(oldVal, newVal);
    //     if(newName.Equals(newVal)){
            
    //         return newVal;
    //     }
    //     else{
    //         return newName;
    //     }
    // }
#endregion Callbacks
}
}
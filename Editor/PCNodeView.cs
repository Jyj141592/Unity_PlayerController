using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace PlayerController.Editor{
public class PCNodeView : Node
{
    private PCNode node;
    public Port inputPort = null;
    public Port outputPort = null;
    private Color defaultColor = new Color(80f / 255f, 80f / 255f, 80f / 255f);

#region Initialize
    public PCNodeView(PCNode node){
        this.node = node;
    }
    public void Draw(){
        mainContainer.Remove(titleContainer);
        topContainer.Remove(inputContainer);
        topContainer.Remove(outputContainer);
        mainContainer.style.backgroundColor = defaultColor;
        mainContainer.style.minWidth = 90;
        mainContainer.style.maxWidth = 300;

        string titleText;
        // Create input port
        if(node is not PlayerControllerAsset){
            inputPort = CreatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Multi);
            inputPort.style.marginLeft = 8;
            mainContainer.Insert(0, inputPort);
            titleText = PCEditorUtility.NamespaceToClassName(node.GetType().ToString());
        }
        else{
            VisualElement element = new VisualElement();
            element.style.height = 20;
            mainContainer.Insert(0, element);
            titleText = "Entry";
        }

        TextElement title = new TextElement(){text = titleText};
        title.style.height = 20;
        title.style.alignSelf = Align.Center;
        title.style.fontSize = 15;
        mainContainer.Add(title);

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
}
}
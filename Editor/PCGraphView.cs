using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace PlayerController.Editor{
public class PCGraphView : GraphView
{
    public new class UxmlFactory : UxmlFactory<PCGraphView, GraphView.UxmlTraits>{}
    public PlayerControllerAsset entryNode;

#region Initialize
    public PCGraphView(){
        CreateGridBackground();
        AddStyleSheet("Assets/PlayerController/Editor/PCWindow.uss");
        AddManipulators();
    }
    private void CreateGridBackground(){
        GridBackground gridBackground = new GridBackground();
        gridBackground.StretchToParentSize();
        Insert(0, gridBackground);
    }
    private void AddStyleSheet(string path){
        StyleSheet styleSheet = (StyleSheet) EditorGUIUtility.Load(path);
        styleSheets.Add(styleSheet);
    }
    private void AddManipulators(){
        this.AddManipulator(new ContentDragger());
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
        this.AddManipulator(new SelectionDragger());
        var rectangle = new RectangleSelector();
        rectangle.target = this;
        this.AddManipulator(rectangle);
    }
#endregion Initialize

#region Load
public void LoadGraph(PlayerControllerAsset node){
    ClearGraph();
    CreateNodeView(node);

}

public void ClearGraph(){
    DeleteElements(graphElements.ToList());
}

#endregion Load

#region Create Elements
private PCNodeView CreateNodeView(PCNode node){
    PCNodeView nodeView = new PCNodeView(node);
    nodeView.Draw();
    AddElement(nodeView);
    nodeView.SetPosition(new Rect(0,0,0,0));
    return nodeView;
}


public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter){
    List<Port> compatiblePorts = new List<Port>();
    ports.ForEach(port =>{
        if(startPort.node == port.node) return;
        if(startPort == port) return;
        if(startPort.direction == port.direction) return;
        compatiblePorts.Add(port);
    });
    return compatiblePorts;
}
#endregion Create Elements

#region Callbacks

#endregion Callbacks

#region Utility
public override void BuildContextualMenu(ContextualMenuPopulateEvent evt) {
        //base.BuildContextualMenu(evt);
        if(evt.target is PCNodeView){
            
        }
        Vector2 nodePosition = this.ChangeCoordinatesTo(contentViewContainer, evt.localMousePosition);
        evt.menu.AppendAction("Make Transition",callback => {},DropdownMenuAction.Status.Disabled );
    }
#endregion Utility
}
}
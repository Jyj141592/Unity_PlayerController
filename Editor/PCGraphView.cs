using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using System.Reflection;

namespace PlayerController.Editor{
public class PCGraphView : GraphView
{
    public new class UxmlFactory : UxmlFactory<PCGraphView, GraphView.UxmlTraits>{}
    public PlayerControllerAsset entryNode;
    public PCNodeView rootNode;
    public Dictionary<string, PCNodeView> nodeViews;

#region Initialize
    public PCGraphView(){
        CreateGridBackground();
        AddStyleSheet("Assets/PlayerController/Editor/PCWindow.uss");
        AddManipulators();
        nodeViews = new Dictionary<string, PCNodeView>();
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
    rootNode = CreateNodeView(node);

}

public void ClearGraph(){
    DeleteElements(graphElements.ToList());
}

#endregion Load

#region Create Elements
private PCNodeView CreateNodeView(PCNode node){
    PCNodeView nodeView = new PCNodeView(node);
    nodeView.Draw();
    nodeView.SetPosition(new Rect(node.position,Vector2.zero));
    AddElement(nodeView);
    nodeViews.Add(node.guid, nodeView);
    return nodeView;
}

private PCNodeView CreateNodeView(System.Type type, Vector2 position){
    var node = (PCNode) ScriptableObject.CreateInstance(type);
    PCNodeView nodeView = new PCNodeView(node);
    node.position = position;
    node.guid = GUID.Generate().ToString();
    nodeView.Draw();
    nodeView.SetPosition(new Rect(position, Vector2.zero));
    AddElement(nodeView);
    nodeViews.Add(node.guid, nodeView);

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
        // if(evt.target is PCNodeView node){
        //     evt.menu.AppendAction("Make Transition",callback => {});
            
        // }
        Vector2 nodePosition = this.ChangeCoordinatesTo(contentViewContainer, evt.localMousePosition);
        evt.menu.AppendAction("Add Subgraph node", callback => {
            CreateNodeView(typeof(SubGraphNode), nodePosition);
        });
        var types = TypeCache.GetTypesDerivedFrom<PCNode>();
        foreach(System.Type type in types){
            if(type.Equals(typeof(PlayerControllerAsset))) continue;
            else if(type.Equals(typeof(SubGraphNode))) continue;

            string path = "New node/";
            var attribute = type.GetCustomAttribute<CreateNodeMenuAttribute>();
            if(attribute == null){
                path = path + PCEditorUtility.NamespaceToClassName(type.Name);
            }
            else path = path + attribute.path;
            evt.menu.AppendAction(path, callback => {
                CreateNodeView(type, nodePosition);
            });
        }
    }


#endregion Utility
}
}
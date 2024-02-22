using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using System.Reflection;
using System;

namespace PlayerController.Editor{
public class PCGraphView : GraphView
{
    public new class UxmlFactory : UxmlFactory<PCGraphView, GraphView.UxmlTraits>{}
    public PlayerControllerAsset entryNode;
    public PCNodeView rootNode;
    public Dictionary<string, PCNodeView> nodeViews;
    public Action<PCNodeView> onNodeSelected;
    public Action<PCEdgeView> onEdgeSelected;

#region Initialize
    public PCGraphView(){
        CreateGridBackground();
        AddStyleSheet("Assets/PlayerController/Editor/PCWindow.uss");
        AddManipulators();
        nodeViews = new Dictionary<string, PCNodeView>();
        // set callbacks
        SetOnGraphViewChanged();
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
        entryNode = node;
        rootNode = LoadNodeView(node);
        foreach(var n in entryNode.nodes){
            LoadNodeView(n);
        }
    }

    public void ClearGraph(){
        DeleteElements(graphElements.ToList());
    }

    private PCNodeView LoadNodeView(PCNode node){
        PCNodeView nodeView = new PCNodeView(node, onNodeSelected);
        nodeView.Draw();
        nodeView.SetPosition(new Rect(node.position,Vector2.zero));
        AddElement(nodeView);
        nodeViews.Add(node.guid, nodeView);
        return nodeView;
    }

    // private PCEdgeView LoadEdgeView(Transition transition){
        
    //     return new PCEdgeView(transition);
    // }
#endregion Load

#region Create Elements
    private PCNodeView CreateNodeView(System.Type type, Vector2 position){
        var node = (PCNode) ScriptableObject.CreateInstance(type);
        node.position = position;
        node.guid = GUID.Generate().ToString();
        entryNode.nodes.Add(node);
        if(!Application.isPlaying){
            AssetDatabase.AddObjectToAsset(node, entryNode);
            // Undo Redo
            //
            AssetDatabase.SaveAssets();
        }
        PCNodeView nodeView = LoadNodeView(node);

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
// create edge & move elements
private void SetOnGraphViewChanged(){
    graphViewChanged = (changes) => {
        if(changes.edgesToCreate != null){
            for(int i = 0; i<changes.edgesToCreate.Count;i++){
                changes.edgesToCreate[i] = new PCEdgeView(changes.edgesToCreate[i], new Transition(), onEdgeSelected);
            }
        }

        return changes;
    };
}
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
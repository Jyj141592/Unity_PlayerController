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
    public HashSet<string> nodeNames;
    public Action<PCNodeView> onNodeSelected;
    public Action<PCEdgeView> onEdgeSelected;

#region Initialize
    public PCGraphView(){
        CreateGridBackground();
        AddStyleSheet("Assets/PlayerController/Editor/PCWindow.uss");
        AddManipulators();
        nodeViews = new Dictionary<string, PCNodeView>();
        nodeNames = new HashSet<string>();
        // set callbacks
        SetOnGraphViewChanged();
        SetElementsDeletion();
        Undo.undoRedoPerformed += () => {
            LoadGraph(entryNode);
            AssetDatabase.SaveAssets();
        };
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
    public void SetPositionToRoot(float wWidth){
        Vector2 pos = new Vector2(-entryNode.position.x + wWidth / 2, -entryNode.position.y);
        UpdateViewTransform(pos, Vector3.one);
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
        LoadEdgeViews(node);
        foreach(var n in entryNode.nodes){
            LoadEdgeViews(n);
        }
    }

    public void ClearGraph(){
        DeleteElements(graphElements.ToList());
        nodeViews.Clear();
        nodeNames.Clear();
        rootNode = null;
        entryNode = null;
    }

    private PCNodeView LoadNodeView(PCNode node){
        PCNodeView nodeView = new PCNodeView(node, onNodeSelected, OnNodeNameChanged);
        nodeView.Draw(node.actionName);
        nodeView.SetPosition(new Rect(node.position,Vector2.zero));
        AddElement(nodeView);
        nodeViews.Add(node.guid, nodeView);
        nodeNames.Add(node.actionName);
        return nodeView;
    }

    private void LoadEdgeViews(PCNode node){
        PCNodeView output = nodeViews[node.guid];
        if(node.transition != null){
            foreach(var t in node.transition){
                PCNodeView input = nodeViews[t.dest.guid];
                PCEdgeView edgeView = new PCEdgeView(input.inputPort, output.outputPort, t, onEdgeSelected);
                output.outputPort.Connect(edgeView);
                input.inputPort.Connect(edgeView);
                AddElement(edgeView);          
            }     
        }  
    }
#endregion Load

#region Create Elements
    // Modifing Asset
    private PCNodeView CreateNodeView(System.Type type, Vector2 position){
        Debug.Log("function start");
        var node = (PCNode) ScriptableObject.CreateInstance(type);
        node.position = position;
        node.guid = GUID.Generate().ToString();
        node.transition = new List<Transition>();
        node.actionName = GetUniqueName(PCEditorUtility.NamespaceToClassName(node.GetType().ToString()));
        
        Debug.Log("before recorde");
        //Undo.RecordObject(entryNode, "Create Node");
        Debug.Log("after record");

        entryNode.nodes.Add(node);
        if(!Application.isPlaying){
            AssetDatabase.AddObjectToAsset(node, entryNode);
            //Undo.RegisterCreatedObjectUndo(node, "Create Node");

            AssetDatabase.SaveAssets();

            Debug.Log("after save");
        }
        PCNodeView nodeView = LoadNodeView(node);

        return nodeView;
    }
    // Modifing Asset
    private Transition AddTransition(PCNodeView input, PCNodeView output){
        // undo
        //Undo.RecordObject(output.node, "Create Transition");
        Transition transition = new Transition(input.node);
        output.node.transition.Add(transition);

        AssetDatabase.SaveAssets();

        return transition;
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

#region Delete Elements
    // Modify Assets
    private void DeleteNode(PCNodeView nodeView, List<GraphElement> deleteElements){
        // undo
        //Undo.RecordObject(entryNode, "Delete Node");

        DisconnectAll(nodeView, deleteElements);
        entryNode.nodes.Remove(nodeView.node);
        nodeViews.Remove(nodeView.node.guid);
        nodeNames.Remove(nodeView.node.actionName);
        nodeView.deleted = true;
        if(!Application.isPlaying){
            AssetDatabase.RemoveObjectFromAsset(nodeView.node);
            //Undo.DestroyObjectImmediate(nodeView.node);

            AssetDatabase.SaveAssets();
        }
    }
    // Modify Assets
    private void DeleteEdge(PCEdgeView edge){
        PCNodeView nodeView = edge.output.node as PCNodeView;
        // undo
        //Undo.RecordObject(nodeView.node, "Delete Transition");
        nodeView.node.transition.Remove(edge.transition);
        nodeView.updated = true;
        AssetDatabase.SaveAssets();
    }
    private void DisconnectAll(PCNodeView nodeView, List<GraphElement> deleteElements){
        if(nodeView.inputPort != null && nodeView.inputPort.connected){
            foreach(Edge edge in nodeView.inputPort.connections){
                DeleteEdge(edge as PCEdgeView);
            }
            deleteElements.AddRange(nodeView.inputPort.connections);
        }
        if(nodeView.outputPort != null && nodeView.outputPort.connected){
            foreach(Edge edge in nodeView.outputPort.connections){
                DeleteEdge(edge as PCEdgeView);
            }
            deleteElements.AddRange(nodeView.outputPort.connections);
        }
    }
#endregion Delete Elements

#region Callbacks
// create edge & move elements
    private void SetOnGraphViewChanged(){
        graphViewChanged = (changes) => {
            if(changes.edgesToCreate != null){
                for(int i = 0; i<changes.edgesToCreate.Count;i++){
                    PCNodeView input = changes.edgesToCreate[i].input.node as PCNodeView;
                    PCNodeView output = changes.edgesToCreate[i].output.node as PCNodeView;
                    Transition transition = AddTransition(input, output);
                    output.updated = true;
                    changes.edgesToCreate[i] = new PCEdgeView(changes.edgesToCreate[i], transition, onEdgeSelected);
                }
            }
            if(changes.elementsToRemove != null){
                foreach(var element in changes.elementsToRemove){
                    if(element is PCEdgeView edge){
                        DeleteEdge(edge);
                    }
                }
            }
            return changes;
        };
    }

    private void SetElementsDeletion(){
        deleteSelection = (operationName, askUser) => {
            List<GraphElement> deleteElements = new List<GraphElement>();
            foreach(GraphElement element in selection){
                if(element is PCNodeView nodeView){
                    if(nodeView.node is PlayerControllerAsset) continue;
                    else{
                        DeleteNode(nodeView, deleteElements);
                    }
                }
                deleteElements.Add(element);
            }
            DeleteElements(deleteElements);
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
            Debug.Log("invoke function");

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
    
    public string GetUniqueName(string name){
        if(!nodeNames.Contains(name)) return name;
        string ret;
        int i = 1;
        while(true){
            ret = name + $"({i})";
            if(!nodeNames.Contains(ret)) break;
        }
        return ret;
    }

    public string OnNodeNameChanged(string oldVal, string newVal){
        if(nodeNames.Contains(newVal)){
            return GetUniqueName(newVal);
        }
        else{
            nodeNames.Remove(oldVal);
            nodeNames.Add(newVal);
            return newVal;
        }
    }

#endregion Utility
}
}
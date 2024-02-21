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
    public EntryNode entryNode;

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
public void LoadGraph(EntryNode node){
    ClearGraph();
    CreateNodeView(node);
    
}

public void ClearGraph(){
    
}

#endregion Load

#region Create Elements
private PCNodeView CreateNodeView(PCNode node){
    PCNodeView nodeView = new PCNodeView(node);
    AddElement(nodeView);
    nodeView.SetPosition(new Rect(0,0,0,0));
    return nodeView;
}

#endregion Create Elements
}
}
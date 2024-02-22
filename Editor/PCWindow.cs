using Packages.Rider.Editor.ProjectGeneration;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;


namespace PlayerController.Editor{
public class PCWindow : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;
    private PCGraphView graphView;
    private InspectorView nodeInspector;
    private InspectorView edgeInspector;
    private VisualElement overlay;
    public int instanceID;
    public string guid;

#region Initialize
    public static PCWindow Open(string title, int instanceID){
        PCWindow wnd = ScriptableObject.CreateInstance<PCWindow>();
        wnd.titleContent = new GUIContent(title);
        wnd.guid = GUID.Generate().ToString();
        wnd.instanceID = instanceID;
        string path = AssetDatabase.GetAssetPath(instanceID);
        EditorPrefs.SetString(wnd.guid, path);
        wnd.Show();
        return wnd;
    }
    [OnOpenAsset]
    public static bool OnOpenAsset(int instanceID, int line){
        Object target = EditorUtility.InstanceIDToObject(instanceID);
        if(target is PlayerControllerAsset node){
            Open(target.name, instanceID);
            return true;
        }
        else return false;
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;
        m_VisualTreeAsset.CloneTree(root);
        graphView = root.Q<PCGraphView>();
        graphView.onNodeSelected = OnNodeSelected;
        graphView.onEdgeSelected = OnEdgeSelected;

        nodeInspector = root.Q<InspectorView>("NodeInspector");
        edgeInspector = root.Q<InspectorView>("EdgeInspector");
        overlay = root.Q<VisualElement>("Overlay");

        string path = EditorPrefs.GetString(guid);
        PlayerControllerAsset entry = AssetDatabase.LoadAssetAtPath<PlayerControllerAsset>(path);
        if(entry != null) LoadGraphView(entry);
        else{
            overlay.style.visibility = Visibility.Visible;
        }
    }

    private void LoadGraphView(PlayerControllerAsset node){
        graphView?.LoadGraph(node);
    }
#endregion Initialize

#region Callbacks
    public void OnSelectionChange() {
        if(EditorUtility.InstanceIDToObject(instanceID) == null) {
            graphView?.ClearGraph();
            overlay.style.visibility = Visibility.Visible;
        }
    }

    public void OnDestroy() {
        EditorPrefs.DeleteKey(guid);
    }

    public void OnNodeSelected(PCNodeView nodeView){
        nodeInspector?.UpdateSelection(nodeView);
    }
    public void OnEdgeSelected(PCEdgeView edge){

    }
#endregion Callbacks
}
}
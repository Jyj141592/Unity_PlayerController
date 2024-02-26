using Packages.Rider.Editor.ProjectGeneration;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using System.Collections.Generic;
using Unity.VisualScripting;


namespace PlayerController.Editor{
public class PCWindow : EditorWindow
{
    // prevent opening multiple windows for same asset
    public static Dictionary<string, PCWindow> openedWnd = new Dictionary<string, PCWindow>();
    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;
    [SerializeField]
    private StyleSheet styleSheet = default;
    private PCGraphView graphView;
    private ParameterView parameterView;
    private InspectorView nodeInspector;
    private TransitionInspector edgeInspector;
    private VisualElement overlay;
    private Button pingAsset;
    private ScrollView scrollView;
    private ListView listView;
    public int instanceID;
    //public string guid;
    public string path = null;

#region Initialize
    public static PCWindow Open(string title, int instanceID){
        string path = AssetDatabase.GetAssetPath(instanceID);
        PCWindow wnd;
        if(openedWnd.ContainsKey(path)){
            wnd = openedWnd[path];
            wnd.Focus();
            wnd.SetPositionToRoot();
            return wnd;
        }
        wnd = CreateInstance<PCWindow>();
        wnd.titleContent = new GUIContent(title);
        //wnd.guid = GUID.Generate().ToString();
        wnd.instanceID = instanceID;
        wnd.path = path;
        openedWnd.Add(path, wnd);
        //EditorPrefs.SetString(wnd.guid, path);
        wnd.Show();
        wnd.SetPositionToRoot();
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
        graphView.AddStyleSheet(styleSheet);
        graphView.onNodeSelected = OnNodeSelected;
        graphView.onEdgeSelected = OnEdgeSelected;

        parameterView = root.Q<ParameterView>();

        scrollView = root.Q<ScrollView>("NodeInfo");
        listView = root.Q<ListView>("TransitionList");
        edgeInspector = root.Q<TransitionInspector>();
        nodeInspector = root.Q<InspectorView>();
        nodeInspector.Init(listView, scrollView, graphView);
        overlay = root.Q<VisualElement>("Overlay");
        pingAsset = root.Q<Button>();
       

        //string path = EditorPrefs.GetString(guid);
        PlayerControllerAsset entry = AssetDatabase.LoadAssetAtPath<PlayerControllerAsset>(path);
        if(entry != null) {
             pingAsset.clicked += () => EditorGUIUtility.PingObject(entry);
            LoadGraphView(entry);
            parameterView.Init(entry);
        }
        else{
            overlay.style.visibility = Visibility.Visible;
        }
    }

    private void LoadGraphView(PlayerControllerAsset node){
        graphView?.LoadGraph(node);
        SetPositionToRoot();
    }

    private void SetPositionToRoot(){
        graphView?.SetPositionToRoot(position.width);
    }
#endregion Initialize

#region Callbacks
    public void OnSelectionChange() {
        if(AssetDatabase.LoadAssetAtPath<PlayerControllerAsset>(path) == null) {
            graphView?.ClearGraph();
            overlay.style.visibility = Visibility.Visible;
        }
    }

    public void OnDestroy() {
        //EditorPrefs.DeleteKey(guid);
        openedWnd.Remove(path);
        graphView.OnDestroy();
        nodeInspector.OnDestroy();
        parameterView.OnDestroy();
    }

    public void OnEnable() {
        if(path != null && !openedWnd.ContainsKey(path)){
            openedWnd.Add(path, this);
        }        
    }

    public void OnInspectorUpdate() {
        UpdateInspector();
    }

    public void OnNodeSelected(PCNodeView nodeView){
        nodeInspector?.UpdateInspector(nodeView);
    }
    public void OnEdgeSelected(PCEdgeView edge){
        edgeInspector?.UpdateInspector(edge);
    }
    public void UpdateInspector(){
        nodeInspector.Update();
        edgeInspector.Update();
    }
#endregion Callbacks
}
}
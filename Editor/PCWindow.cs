using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;


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
    private Button refresh;
    public int instanceID;
    public string path = null;
    private bool isPlaying = false;

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
        wnd.instanceID = instanceID;
        wnd.path = path;
        openedWnd.Add(path, wnd);
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
        pingAsset = root.Q<Button>("PingAsset");
        refresh = root.Q<Button>("Refresh");
       
        PlayerControllerAsset entry = AssetDatabase.LoadAssetAtPath<PlayerControllerAsset>(path);
        if(entry != null) {
            pingAsset.clicked += () => EditorGUIUtility.PingObject(entry);
            refresh.clicked += () => {
                graphView.OnDestroy();
                nodeInspector.OnDestroy();
                edgeInspector.OnDestroy();
                parameterView.OnDestroy();
                root.Clear();
                CreateGUI();
            };
            LoadGraphView(entry);
            parameterView.Init(entry);
            edgeInspector.Init(parameterView);
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
        var asset = AssetDatabase.LoadAssetAtPath<PlayerControllerAsset>(path);
        if(asset == null) {
            graphView?.ClearGraph();
            overlay.style.visibility = Visibility.Visible;
        }
        if(Application.isPlaying){
            if(Selection.activeGameObject){
                PlayerControl controller = Selection.activeGameObject.GetComponent<PlayerControl>();
                if(controller != null && controller.playerControllerAsset != null && controller.playerControllerAsset.guid.Equals(asset.guid)) {
                    graphView?.LoadGraph(controller.playerControllerAsset);
                    nodeInspector?.ClearInspector();
                    parameterView?.ChangeAsset(controller.playerControllerAsset);
                    edgeInspector?.ChangeParameterView(parameterView);
                    isPlaying = true;
                }
            }
        }
    }

    public void OnDestroy() {
        openedWnd.Remove(path);
        graphView.OnDestroy();
        nodeInspector.OnDestroy();
        edgeInspector.OnDestroy();
        parameterView.OnDestroy();
    }

    public void OnEnable() {
        if(path != null && !openedWnd.ContainsKey(path)){
            openedWnd.Add(path, this);
        }    
        isPlaying = false;    
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    public void OnInspectorUpdate() {
        if(isPlaying){
            graphView?.UpdateState();
        }
        nodeInspector?.Update();
        edgeInspector?.Update();
    }
    
    public void OnPlayModeStateChanged(PlayModeStateChange chg){
        switch(chg){
            case PlayModeStateChange.EnteredEditMode:
            isPlaying = false;
            var asset = AssetDatabase.LoadAssetAtPath<PlayerControllerAsset>(path);
            if(asset != null){
                graphView?.LoadGraph(asset);
                nodeInspector?.ClearInspector();
                parameterView?.ChangeAsset(asset);
                edgeInspector?.ChangeParameterView(parameterView);
            }
            break;
        }
    }
    public void OnNodeSelected(PCNodeView nodeView){
        nodeInspector?.UpdateInspector(nodeView);
    }
    public void OnEdgeSelected(PCEdgeView edge){
        edgeInspector?.UpdateInspector(edge);
    }
#endregion Callbacks
}
}
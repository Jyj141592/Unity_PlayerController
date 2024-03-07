using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace PlayerController.Editor{
public class ParameterView : VisualElement
{
    public new class UxmlFactory : UxmlFactory<ParameterView, VisualElement.UxmlTraits>{}
    private enum SearchOption{
        Name, Bool, Float, Int
    }
    private ToolbarPopupSearchField searchField;
    private ToolbarMenu menu;
    private ListView listView;
    private Dictionary<string, int> names;
    private int clickedIndex = -1;
    private double clickedTime = 0;
    private SearchOption searchOption = SearchOption.Name;
    private string searchName = null;
    public Action onAddOrDeleted;
    
    private PlayerControllerAsset asset;
    public ParameterList parameterList;
#region Initialize
    public ParameterView(){
        Undo.undoRedoPerformed += UndoRedoPerformed;
        names = new Dictionary<string, int>();
    }
    private void UndoRedoPerformed(){
        if(!Application.isPlaying) AssetDatabase.SaveAssets();
            LoadParameterView(searchOption, searchName); 
    }
    public void Init(PlayerControllerAsset asset){
        this.asset = asset;
        parameterList = asset.parameterList;
        // Initialize listview
        listView = this.Q<ListView>("ParameterList");
        listView.selectionType = SelectionType.Single;
        listView.reorderable = true;
        listView.reorderMode = ListViewReorderMode.Animated;
        listView.itemIndexChanged -= OnChangeListOrder;
        listView.itemIndexChanged += OnChangeListOrder;
        listView.RegisterCallback<KeyDownEvent>(OnKeyDown, TrickleDown.TrickleDown);
        listView.makeItem = () => {
            VisualElement container = new VisualElement();
            container.style.flexDirection = FlexDirection.Row;
            Label label = new Label();
            label.style.flexGrow = label.style.flexShrink = 1;
            label.RegisterCallback<MouseUpEvent>((ev) => {
                if(IsDoubleClicked(ev)){
                    ChangeParamName(label);
                }
            });
            container.Add(label);
            return container;
        };
        listView.bindItem = (e, j) => {
            BindItem(e, j);
        };     
                
        // Initialize toolbar searchfield
        searchField = this.Q<ToolbarPopupSearchField>();
        searchField.menu.AppendAction("Name", (action) => {
            if(searchOption != SearchOption.Name){
                SearchByName(searchName);
            }
        });
        searchField.menu.AppendAction("Int", (action) => {
            if(searchOption != SearchOption.Int){
                SearchInt(searchName);
            }
        });
        searchField.menu.AppendAction("Float", (action) => {
            if(searchOption != SearchOption.Float){
                SearchFloat(searchName);
            }
        });
        searchField.menu.AppendAction("Bool", (action) => {
            if(searchOption != SearchOption.Float){
                SearchBool(searchName);
            }
        });
        searchField.RegisterValueChangedCallback((callback) => {
            switch(searchOption){
                case  SearchOption.Name:
                SearchByName(callback.newValue);
                break;
                case SearchOption.Bool:
                SearchBool(callback.newValue);
                break;
                case SearchOption.Float:
                SearchFloat(callback.newValue);
                break;
                case SearchOption.Int:
                SearchInt(callback.newValue);
                break;
            }
        });

        // Initialize toolbar menu
        menu = this.Q<ToolbarMenu>();
        menu.menu.AppendAction("int", (action) => {
            if(!Application.isPlaying)
                AddParameter(ParameterType.Int);
        });
        menu.menu.AppendAction("float", (action) => {
            if(!Application.isPlaying)
                AddParameter(ParameterType.Float);
        });
        menu.menu.AppendAction("bool", (action) => {
            if(!Application.isPlaying)
                AddParameter(ParameterType.Bool);
        });

        LoadParameterView(SearchOption.Name, null);
    }

    public void ChangeAsset(PlayerControllerAsset asset){
        this.asset = asset;
        parameterList = asset.parameterList;
        listView.reorderable = !Application.isPlaying;
        LoadParameterView(SearchOption.Name, null);
    }
    
    private void LoadParameterView(SearchOption searchOption, string searchName){
        clickedIndex = -1;
        clickedTime = 0;
        listView.Clear();
        listView.itemsSource = null;
        listView.Rebuild();
        names.Clear();
        int i = 0;
        foreach(var p in parameterList.parameters){
            names.Add(p.name, i);
            i++;
        }
        switch(searchOption){
            case SearchOption.Name:
            SearchByName(searchName);
            break;
            case SearchOption.Bool:
            SearchBool(searchName);
            break;
            case SearchOption.Float:
            SearchFloat(searchName);
            break;
            case SearchOption.Int:
            SearchInt(searchName);
            break;
        }
    }
    public void BindItem(VisualElement e, int j){
            int i = names[(listView.itemsSource[j] as Parameter).name];
            Label label = e.Q<Label>();
            SerializedObject obj = new SerializedObject(asset);
            SerializedProperty property = obj.FindProperty("_parameterList").FindPropertyRelative("_parameters").GetArrayElementAtIndex(i);
            label.BindProperty(property.FindPropertyRelative("_name"));

            Toggle toggle = e.Q<Toggle>();
            if(toggle != null) e.Remove(toggle);
            IntegerField intField = e.Q<IntegerField>();
            if(intField != null) e.Remove(intField);
            FloatField floatField = e.Q<FloatField>();
            if(floatField != null) e.Remove(floatField);

            floatField = new FloatField();
            floatField.BindProperty(property.FindPropertyRelative("value"));           

            if(parameterList.parameters[i].paramType == ParameterType.Bool){
                toggle = new Toggle();
                toggle.SetValueWithoutNotify(parameterList.parameters[i].GetBool());
                toggle.RegisterValueChangedCallback((callback) => {
                    float val = callback.newValue ? 1 : 0;
                    floatField.value = val;
                });
                floatField.RegisterValueChangedCallback((callback) => {
                    toggle.SetValueWithoutNotify(callback.newValue > 0);
                });
                toggle.style.flexDirection = FlexDirection.Column;
                floatField.style.maxWidth = 0;
                floatField.style.maxHeight = 0;
                floatField.visible = false;

                toggle.Add(floatField);
                e.Add(toggle);
            }
            else if(parameterList.parameters[i].paramType == ParameterType.Int){
                intField = new IntegerField();
                intField.isDelayed = true;
                intField.SetValueWithoutNotify(parameterList.parameters[i].GetInt());
                intField.RegisterValueChangedCallback((callback) => {
                    float val = callback.newValue;
                    floatField.value = val;
                });
                intField.style.flexDirection = FlexDirection.Column;
                intField.style.minWidth = 35;
                
                floatField.RegisterValueChangedCallback((callback) => {
                    intField.SetValueWithoutNotify((int) callback.newValue);
                });
                floatField.style.maxWidth = 0;
                floatField.style.maxHeight = 0;
                floatField.visible = false;

                intField.Add(floatField);
                e.Add(intField);
            }
            else if(parameterList.parameters[i].paramType == ParameterType.Float){
                floatField.isDelayed = true;
                floatField.style.minWidth = 35;
                e.Add(floatField);
            }
    }
#endregion Initialize

#region SearchList
    private void SearchByName(string name){
        searchOption = SearchOption.Name;
        searchName = name;
        if(name == null || name.Equals("")) 
            listView.itemsSource = parameterList.parameters.ToList();
        else listView.itemsSource = parameterList.parameters.Where((p) => p.name.Contains(name)).ToList();

        listView.Rebuild();
    }
    private void SearchBool(string name){
        searchOption = SearchOption.Bool;
        searchName = name;
        if(name == null || name.Equals(""))
            listView.itemsSource = parameterList.parameters.Where((p) => p.paramType == ParameterType.Bool).ToList();
        else
            listView.itemsSource = parameterList.parameters.Where((p) => p.paramType == ParameterType.Bool && p.name.Contains(name)).ToList();
        listView.Rebuild();
    }
    private void SearchFloat(string name){
        searchOption = SearchOption.Float;
        searchName = name;
        if(name == null || name.Equals(""))
            listView.itemsSource = parameterList.parameters.Where((p) => p.paramType == ParameterType.Float).ToList();
        else
            listView.itemsSource = parameterList.parameters.Where((p) => p.paramType == ParameterType.Float && p.name.Contains(name)).ToList();
        listView.Rebuild();
    }
    private void SearchInt(string name){
        searchOption = SearchOption.Int;
        searchName = name;
        if(name == null || name.Equals(""))
            listView.itemsSource = parameterList.parameters.Where((p) => p.paramType == ParameterType.Int).ToList();
        else
            listView.itemsSource = parameterList.parameters.Where((p) => p.paramType == ParameterType.Int && p.name.Contains(name)).ToList();
        listView.Rebuild();
    }

#endregion SearchList

#region Modify List
    private void AddParameter(ParameterType type){
        string name = "New";
        switch(type){
            case ParameterType.Bool:
            name = "New Bool";
            break;
            case ParameterType.Float:
            name = "New Float";
            break;
            case ParameterType.Int:
            name = "New Int";
            break;
        }
        SerializedObject obj = new SerializedObject(asset);
        SerializedProperty p1 = obj.FindProperty("_parameterList");
        SerializedProperty p2 = p1.FindPropertyRelative("_parameters");
        int index = parameterList.parameters.Count;
        string uName = GetUniqueName(name);
        p2.InsertArrayElementAtIndex(index);
        p2.GetArrayElementAtIndex(index).boxedValue = new Parameter();
        p2.GetArrayElementAtIndex(index).FindPropertyRelative("_name").stringValue = uName;
        p2.GetArrayElementAtIndex(index).FindPropertyRelative("_paramID").intValue = Animator.StringToHash(uName);
        p2.GetArrayElementAtIndex(index).FindPropertyRelative("value").floatValue = 0;
        p2.GetArrayElementAtIndex(index).FindPropertyRelative("_paramType").SetEnumValue(type);
        obj.ApplyModifiedProperties();

        if(!Application.isPlaying)
            AssetDatabase.SaveAssets();
            
        LoadParameterView(SearchOption.Name, null);
        
        listView.SetSelection(index);
        clickedIndex = index;
        listView.ScrollToItem(index);
        Label label = listView.GetRootElementForId(index).Q<Label>();
        ChangeParamName(label);
        onAddOrDeleted?.Invoke();
    }

    private void ChangeParamName(Label label){
        TextField textField = new TextField();
        label.Add(textField);
        textField.SetValueWithoutNotify(label.text);
        textField.selectAllOnFocus = true;
        textField.style.paddingLeft = textField.style.marginLeft = 0;
        textField.isDelayed = true;
        textField.RegisterCallback<FocusOutEvent>((ev) => {
            string newName = textField.value;
            if(!newName.Equals(label.text)){
                names.Remove(label.text);
                newName = GetUniqueName(newName);
                names.Add(newName, clickedIndex);
                SerializedObject obj = new SerializedObject(asset);
                SerializedProperty property = obj.FindProperty("_parameterList").FindPropertyRelative("_parameters").GetArrayElementAtIndex(clickedIndex);
                property.FindPropertyRelative("_name").stringValue = newName;
                property.FindPropertyRelative("_paramID").intValue = Animator.StringToHash(newName);
                obj.ApplyModifiedProperties();
                onAddOrDeleted?.Invoke();
            }
            label.Remove(textField);
        });
        textField.Focus();
    }

    public void OnChangeListOrder(int oldPos, int newPos){
        SerializedObject obj = new SerializedObject(asset);
        SerializedProperty property = obj.FindProperty("_parameterList").FindPropertyRelative("_parameters");
        if(oldPos > newPos){
            for(int i = oldPos; i > newPos; i--){
                property.MoveArrayElement(i, i - 1);
            }
        }
        else{
            for(int i = oldPos; i < newPos; i++){
                property.MoveArrayElement(i, i + 1);
            }
        }
        obj.ApplyModifiedProperties();
        LoadParameterView(SearchOption.Name, null);
    }

    private void DeleteParameter(int index){
        SerializedObject obj = new SerializedObject(asset);
        SerializedProperty property = obj.FindProperty("_parameterList").FindPropertyRelative("_parameters");
        property.DeleteArrayElementAtIndex(index);
        obj.ApplyModifiedProperties();
        LoadParameterView(searchOption, searchName);
        onAddOrDeleted?.Invoke();
    }

    private List<string> FindParameterUsage(string paramName){
        List<string> transitions = new List<string>();
        transitions.AddRange(FindParameterUsageInNode(asset, paramName));
        transitions.AddRange(FindParameterUsageInNode(asset.anyState, paramName));
        foreach(var node in asset.nodes){
            transitions.AddRange(FindParameterUsageInNode(node, paramName));
        }
        return transitions;
    }
    private List<string> FindParameterUsageInNode(PCNode node, string paramName){
        List<string> transitions = new List<string>();
        foreach(var transition in node.transitions){
            foreach(var condition in transition.conditions){
                if(condition.paramName.Equals(paramName)){
                    transitions.Add(node.actionName + " -> " + transition.dest.actionName);
                    break;
                }
            }
        }
        return transitions;
    }
#endregion Modify List

#region Utility
    private bool IsDoubleClicked(MouseUpEvent ev){
        if(ev.button != 0){
            clickedTime = 0;
            clickedIndex = -1;
            return false;
        }
        double prevTime = clickedTime;
        int prevIdx = clickedIndex;
        clickedTime = EditorApplication.timeSinceStartup;
        clickedIndex = listView.selectedIndex;
        if(prevIdx == clickedIndex && clickedTime - prevTime < 0.3){
            return true;
        }
        return false;
    }

    private void OnKeyDown(KeyDownEvent ev){
        if(ev.keyCode == KeyCode.Delete){
            if(Application.isPlaying) return;
            int selected = listView.selectedIndex;
            if(selected < 0) return;
            List<string> usage = FindParameterUsage(parameterList.parameters[selected].name);
            if(usage.Count == 0){
                DeleteParameter(selected);
            }
            else{
                string useList = "It is used by : \n";
                foreach(string str in usage){
                    useList += str + "\n";
                }
                bool del = EditorUtility.DisplayDialog("Delete Parameter " + parameterList.parameters[selected].name + '?',
                useList, "Delete", "Cancel");
                if(del){
                    DeleteParameter(selected);
                }
            }
        }
    }

    private string GetUniqueName(string name){
        if(!names.ContainsKey(name)) return name;
        string newName = name;
        int i = 1;
        while(true){
            newName = name + $"({i})";
            if(!names.ContainsKey(newName)){
                break;
            }
            i++;
        } 
        return newName;
    }

    public void OnDestroy(){
        Undo.undoRedoPerformed -= UndoRedoPerformed;
        listView.UnregisterCallback<KeyDownEvent>(OnKeyDown);
    }
    public int ParameterIndex(string name){
        if(names.ContainsKey(name)){
            return names[name];
        }
        else return -1;
    }
#endregion Utility
}
}
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEngine;
using UnityEditor.UIElements;
using System.Linq;
using System;
using UnityEditor.Experimental.GraphView;
using Unity.VisualScripting;

namespace PlayerController.Editor{
public class InspectorView : VisualElement
{
    public new class UxmlFactory : UxmlFactory<InspectorView, VisualElement.UxmlTraits>{}
    private PCNodeView focused = null;
    public ScrollView scrollView;
    public ListView listView;
    private PCGraphView graphView;
    private PCEdgeView selected = null;
    public InspectorView(){
    }
    public void Init(ListView listView, ScrollView scrollView, PCGraphView graphView){
        this.listView = listView;
        this.scrollView = scrollView;
        this.graphView = graphView;
        
        listView.RegisterCallback<KeyDownEvent>(OnKeyDown, TrickleDown.TrickleDown);
        listView.itemIndexChanged += OnItemIndexChanged;
        listView.selectionChanged += OnSelectionChanged;
        Undo.undoRedoPerformed += UndoRedoPerformed;
        
    }

    private void UndoRedoPerformed(){
        focused = null;
        selected = null;
        this.scrollView.Clear();
        this.listView.itemsSource = null;
        this.listView.Rebuild();
    }
    
    public void UpdateInspector(PCNodeView nodeView) {
        ClearInspector();
        focused = nodeView;
        nodeView.onDeleted += OnNodeDeleted;
        bool foundName = false;
        SerializedObject obj = new SerializedObject(nodeView.node);
        if(nodeView.node is not PlayerControllerAsset && nodeView.node is not AnyState){
            Stack<Type> stack = new Stack<Type>();
            Type type = nodeView.node.GetType();
            while(type != null){
                stack.Push(type);
                type = type.BaseType;
            }
            while(stack.Count > 0){
                Type curType = stack.Pop();
                var fields = curType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                foreach(var field in fields){
                    if(!field.IsPublic && field.GetCustomAttribute<SerializeField>(true) == null) continue;
                    if(field.GetCustomAttribute<HideInInspector>(true) != null) continue;
                    if(field.GetCustomAttribute<NonSerializedAttribute>(true) != null) continue;
                    string name = PCEditorUtility.ToInspectorName(field.Name);
                    
                    
                    if(field.FieldType == typeof(int)){
                        IntegerField intField = new IntegerField(){
                            label = name
                        };
                        SetField(intField, field, obj);
                    }
                    else if(field.FieldType == typeof(float)){
                        FloatField floatField = new FloatField(){
                            label = name
                        };
                        SetField(floatField, field, obj);
                    }
                    else if(field.FieldType == typeof(bool)){
                        Toggle toggle = new Toggle(){
                            label = name
                        };
                        SetField(toggle, field, obj);
                    }
                    else if(field.FieldType == typeof(string)){
                        TextField textField = new TextField(){
                            label = name
                        };
                        textField.isDelayed = true;
                        if(!foundName && field.Name.Equals("_actionName")){
                            textField.value = (string) field.GetValue(nodeView.node);
                            textField.RegisterValueChangedCallback(callback => {
                                textField.SetValueWithoutNotify(callback.previousValue);
                                if(!Application.isPlaying){
                                    string newName = nodeView.OnNodeNameChanged(callback.previousValue, callback.newValue);
                                    textField.SetValueWithoutNotify(newName);
                                    listView.Rebuild();
                                }
                            });
                            scrollView.Add(textField);
                            foundName = true;
                        }
                        else SetField(textField, field, obj);
                    }
                    else if(field.FieldType.IsEnum){
                        EnumField enumField = new EnumField((System.Enum) field.GetValue(nodeView.node)){
                            label = name
                        };               
                        SetField(enumField, field, obj);
                    }
                    else if(field.FieldType == typeof(Vector2)){
                        Vector2Field vector2Field = new Vector2Field(){
                            label = name
                        };
                        SetField(vector2Field, field, obj);
                    }
                    else if(field.FieldType == typeof(Vector3)){
                        Vector3Field vector3Field = new Vector3Field(){
                            label = name
                        };
                        SetField(vector3Field, field, obj);
                    }
                }
            }
        }
        // Update transition list
        SetListView();
    }
    private void SetField<T>(BaseField<T> field, FieldInfo fieldInfo, SerializedObject obj){
        field.BindProperty(obj.FindProperty(fieldInfo.Name));
        scrollView.Add(field);
    }
    public void ClearInspector(){
        listView.itemsSource = null;
        listView.Rebuild();
        listView.ClearSelection();
        scrollView.Clear();
        if(focused != null){
            focused.onDeleted -= OnNodeDeleted;
            focused = null;
        }
        selected = null;
    }
    private void SetListView(){
        listView.Clear();
        listView.itemsSource = focused.outputPort.connections.ToList();
        listView.Rebuild();
        listView.ClearSelection();
    }
    private void OnItemIndexChanged(int oldVal, int newVal){
        focused.MoveTransitionIndex(oldVal, newVal);
        listView.itemsSource = focused.outputPort.connections.ToList();
        listView.Rebuild();
        listView.SetSelectionWithoutNotify(new List<int>(){newVal});
    }
    private void OnSelectionChanged(IEnumerable<object> selectedItems){
        if(selectedItems.Count() <= 0) return;
        PCEdgeView edge = (PCEdgeView) selectedItems.First();
        if(selected != null) RemoveSelection(selected);
        selected = edge;
        AddSelection(edge);
    }
    private void OnKeyDown(KeyDownEvent ev){
        if(ev.keyCode == KeyCode.Delete){
            PCEdgeView edge = (PCEdgeView) listView.selectedItem;
            if(edge != null){
                RemoveEdge(edge);
                listView.itemsSource = focused.outputPort.connections.ToList();
            }
        }
    }
    private void OnNodeDeleted(){
        ClearInspector();
    }

    public void OnDestroy(){
        Undo.undoRedoPerformed -= UndoRedoPerformed;
        listView.UnregisterCallback<KeyDownEvent>(OnKeyDown);
    }
    

    private void AddSelection(PCEdgeView edge){
        graphView.AddToSelection(edge);
    }

    private void RemoveEdge(PCEdgeView edge){
        graphView.DeleteEdge(edge);
        edge.input.Disconnect(edge);
        edge.output.Disconnect(edge);
        graphView.RemoveElement(edge);
    }

    private void RemoveSelection(PCEdgeView edge){
        graphView.RemoveFromSelection(edge);
    }

    public void Update(){
        if(focused != null && focused.updated){
            focused.updated = false;
            SetListView();
        }
    }
}
}
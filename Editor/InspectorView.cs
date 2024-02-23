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
using Codice.Client.BaseCommands.Merge.IncomingChanges;

namespace PlayerController.Editor{
public class InspectorView : VisualElement
{
    public new class UxmlFactory : UxmlFactory<InspectorView, VisualElement.UxmlTraits>{}
    private PCNodeView focused = null;
    public ScrollView scrollView;
    public ListView listView;
    public TransitionInspector transitionInspector;
    public InspectorView(){
    }
    public void Init(ListView listView, ScrollView scrollView, TransitionInspector inspector){
        this.listView = listView;
        this.scrollView = scrollView;
        transitionInspector = inspector;
        listView.itemIndexChanged += OnItemIndexChanged;
        listView.selectionChanged += OnSelectionChanged;
    }
    public void UpdateInspector(PCNodeView nodeView) {
        scrollView.Clear();
        listView.Clear();
        focused = nodeView;
        bool foundName = false;
        SerializedObject obj = new SerializedObject(nodeView.node);
        if(nodeView.node is not PlayerControllerAsset){
            var fields = nodeView.node.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).ToList();
            fields.Reverse();
            foreach(var field in fields){
                if(!field.IsPublic && field.GetCustomAttribute<SerializeField>(true) == null) continue;
                if(field.GetCustomAttribute<HideInInspector>(true) != null) continue;
                if(field.FieldType == typeof(int)){
                    IntegerField intField = new IntegerField(){
                        label = field.Name
                    };
                    SetField(intField, field, obj);
                }
                else if(field.FieldType == typeof(float)){
                    FloatField floatField = new FloatField(){
                        label = field.Name
                    };
                    SetField(floatField, field, obj);
                }
                else if(field.FieldType == typeof(bool)){
                    Toggle toggle = new Toggle(){
                        label = field.Name
                    };
                    SetField(toggle, field, obj);
                }
                else if(field.FieldType == typeof(string)){
                    TextField textField = new TextField(){
                        label = field.Name
                    };
                    textField.isDelayed = true;
                    SetField(textField, field, obj);
                    if(!foundName && field.Name.Equals("actionName")){
                        //textField.RegisterValueChangedCallback(callback => {
                            // string newName = nodeView.onNodeNameChanged.Invoke(callback.previousValue, callback.newValue);
                            // if(!newName.Equals(callback.newValue)){
                            //     textField.value = newName;
                            //     nodeView.nodeTitle.text = newName;
                            // }
                            // else{
                            //     nodeView.nodeTitle.text = callback.newValue;
                            // }
                        //});
                        
                        foundName = true;
                    }
                }
                else if(field.FieldType.IsEnum){
                    EnumField enumField = new EnumField((System.Enum) field.GetValue(nodeView.node)){
                        label = field.Name
                    };               
                    SetField(enumField, field, obj);
                }
                else if(field.FieldType == typeof(Vector2)){
                    Vector2Field vector2Field = new Vector2Field(){
                        label = field.Name
                    };
                    SetField(vector2Field, field, obj);
                }
                else if(field.FieldType == typeof(Vector3)){
                    Vector3Field vector3Field = new Vector3Field(){
                        label = field.Name
                    };
                    SetField(vector3Field, field, obj);
                }
            }
        }
        // Update transition list
        SetListView();
        listView.itemIndexChanged -= OnItemIndexChanged;
        listView.itemIndexChanged += OnItemIndexChanged;
    }
    private void SetField<T>(BaseField<T> field, FieldInfo fieldInfo, SerializedObject obj){
        if(!Application.isPlaying){
            field.BindProperty(obj.FindProperty(fieldInfo.Name));
        }
        else{
            field.RegisterValueChangedCallback(callback => {
                fieldInfo.SetValue(focused.node, callback.newValue);
            });
        }
        scrollView.Add(field);
    }
    private void SetListView(){
        listView.itemsSource = focused.outputPort.connections.ToList();
        listView.Rebuild();
        focused.updated = false;
    }
    private void OnItemIndexChanged(int oldVal, int newVal){
        Debug.Log(oldVal.ToString() + " -> " + newVal.ToString());
        
    }
    private void OnSelectionChanged(IEnumerable<object> selectedItems){

    }
    public void Update(){
        if(listView != null && focused != null && focused.updated){          
            SetListView();
        }
        if(focused != null && focused.deleted){
            listView.Clear();
            scrollView.Clear();
            focused = null;
        }
    }
}
}
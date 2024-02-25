using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Codice.CM.Client.Differences;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace PlayerController.Editor{
public class ParameterView : VisualElement
{
    public new class UxmlFactory : UxmlFactory<ParameterView, VisualElement.UxmlTraits>{}
    private ToolbarSearchField searchField;
    private ToolbarMenu menu;
    private ListView listView;
    private int clickedIndex = -1;
    private double clickedTime = 0;

    private List<string> test = new List<string>{"one", "two", "three", "four", "five"};
    
    public ParameterView(){}
    private ParameterList parameterList;
#region Initialize
    public void Init(ParameterList parameterList){
        // Initialize toolbar searchfield
        searchField = this.Q<ToolbarSearchField>();

        // Initialize toolbar menu
        menu = this.Q<ToolbarMenu>();
        menu.menu.AppendAction("int", (action) => {

        });
        menu.menu.AppendAction("float", (action) => {

        });
        menu.menu.AppendAction("bool", (action) => {

        });
        
        // Initialize listview
        listView = this.Q<ListView>("ParameterList");
        listView.selectionType = SelectionType.Single;
        listView.reorderable = true;
        listView.reorderMode = ListViewReorderMode.Animated;
        listView.makeItem = () => {
            VisualElement container = new VisualElement();
            container.style.flexDirection = FlexDirection.Row;
            Label label = new Label();
            label.style.flexGrow = label.style.flexShrink = 1;
            label.RegisterCallback<MouseDownEvent>((ev) => {
                if(IsDoubleClicked()){
                    ChangeParamName(label);
                }
            });
            container.Add(label);
            return container;
        };
        listView.bindItem = (e, i) => {
            Label label = e.Q<Label>();
            label.text = test[i];
            Toggle toggle = e.Q<Toggle>();
            if(toggle != null) return;
            toggle = new Toggle();
            toggle.style.minWidth = toggle.style.maxWidth = 50;
            e.Add(new Toggle());
        };
        
        this.parameterList = parameterList;

        LoadParameterView();
    }
    
    private void LoadParameterView(){
        clickedIndex = -1;
        clickedTime = 0;
        listView.itemsSource = test;

        listView.Rebuild();
    }
#endregion Initialize

#region Modify List
    private void ChangeParamName(Label label){
        TextField textField = new TextField();
        label.Add(textField);
        textField.SetValueWithoutNotify(label.text);
        textField.selectAllOnFocus = true;
        textField.style.paddingLeft = textField.style.marginLeft = 0;
        textField.isDelayed = true;
        textField.RegisterCallback<FocusOutEvent>((ev) => {
            Debug.Log("focused");
        });
        textField.Focus();
    }

#endregion Modify List

#region Utility
    private bool IsDoubleClicked(){
        double prevTime = clickedTime;
        int prevIdx = clickedIndex;
        clickedTime = EditorApplication.timeSinceStartup;
        clickedIndex = listView.selectedIndex;
        if(prevIdx == clickedIndex && clickedTime - prevTime < 0.3){
            //Debug.Log("double clicked");
            return true;
        }
        return false;
    }

    private string GetUniqueName(string name){
        return name;
    }
#endregion Utility
}
}
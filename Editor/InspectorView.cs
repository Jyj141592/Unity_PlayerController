using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEditor;

namespace PlayerController.Editor{
public class InspectorView : VisualElement
{
    public new class UxmlFactory : UxmlFactory<InspectorView, VisualElement.UxmlTraits>{}

    private UnityEditor.Editor editor;
    public InspectorView(){}
    public void UpdateSelection(PCNodeView nodeView) {
            Clear();

            UnityEngine.Object.DestroyImmediate(editor);

            editor = UnityEditor.Editor.CreateEditor(nodeView.node);
            IMGUIContainer container = new IMGUIContainer(() => {
                if (editor && editor.target) {
                    editor.OnInspectorGUI();
                }
            });
            Add(container);
    }
    // void UpdateSelection(PCEdgeView edge) {
    //         Clear();

    //         UnityEngine.Object.DestroyImmediate(editor);

    //         editor = UnityEditor.Editor.CreateEditor(edge.transition);
    //         IMGUIContainer container = new IMGUIContainer(() => {
    //             if (editor && editor.target) {
    //                 editor.OnInspectorGUI();
    //             }
    //         });
    //         Add(container);
    // }
}
}
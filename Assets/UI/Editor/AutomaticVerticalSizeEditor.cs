using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AutomaticVerticalSize))]
public class AutomaticVerticalSizeEditor : Editor {
    //overrides the GUI we normally get for the inspector
    //but if we us DrawDefaultInspector it draws it like usual
    public override void OnInspectorGUI() {
        
        DrawDefaultInspector();

        //creates button and it returns true when pressed, so can us in if statement
       if( GUILayout.Button("Recalc Size")) {

            //target is the object that the CustomEditor brings back
            //we cast it to a AutomaticVerticalSize Object, and use it's method AdjustSize()
            AutomaticVerticalSize myScript = ((AutomaticVerticalSize)target);
            myScript.AdjustSize();
        }
    }


}

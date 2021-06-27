using UnityEditor;
using UnityEngine;
 
public enum DisablingType
{
    ReadOnly = 2,
    DontDraw = 3
}

[CustomPropertyDrawer(typeof(DrawIfAttribute))]
public class DrawIfPropertyDrawer : PropertyDrawer
{
    // Reference to the attribute on the property.
    DrawIfAttribute drawIf;
 
    // Field that is being compared.
    SerializedProperty comparedField;
 
    // Height of the property.
    private float propertyHeight;
 
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return propertyHeight;
    }
 
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Set the global variables.
        drawIf = attribute as DrawIfAttribute;
        comparedField = property.serializedObject.FindProperty(drawIf.comparedPropertyName);

        object comparedFieldValue = null;
        
        switch (comparedField.propertyType)
        {
            case SerializedPropertyType.Boolean:
                comparedFieldValue = comparedField.boolValue;
                
                break;
            
            case SerializedPropertyType.Integer:
                comparedFieldValue = comparedField.intValue;
                break;
        }

        
        // Is the condition met? Should the field be drawn?
        bool conditionMet = false;
 
        if((bool) comparedFieldValue)
        {
            conditionMet = true;
        }
 
        // The height of the property should be defaulted to the default height.
        propertyHeight = base.GetPropertyHeight(property, label);
   
        // If the condition is met, simply draw the field. Else...
        if (conditionMet)
        {
            EditorGUI.PropertyField(position, property);
        }
        else
        {
            //...check if the disabling type is read only. If it is, draw it disabled, else, set the height to zero.
            GUI.enabled = false;
            EditorGUI.PropertyField(position, property);
            GUI.enabled = true;
            
        }
    }
}

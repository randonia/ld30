using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(TooltipAttribute))]
public class TooltipDrawer : PropertyDrawer
{
	/// <summary>
	/// http://answers.unity3d.com/questions/37177/additional-information-on-mouseover-in-the-inspect.html
	/// </summary>
	public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
	{
		var atr = (TooltipAttribute) attribute;
		var content = new GUIContent(label.text, atr.text);
		EditorGUI.PropertyField(position, prop, content);
	}
}

using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditorInternal;
using System.Reflection;


public class SortingController : MonoBehaviour {



	// Use this for initialization
	void Start () {
		renderer.sortingLayerName = "fx_mid";
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	private string[] GetSortingLayerNames() {
		var internalEditorUtilityType = typeof(InternalEditorUtility);
		PropertyInfo sortingLayersProperty = internalEditorUtilityType.GetProperty("sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic);
		return (string[])sortingLayersProperty.GetValue(null, new object[0]);
	}

}

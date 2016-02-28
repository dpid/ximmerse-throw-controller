using UnityEditor;
using UnityEngine;

namespace Ximmerse.Core{

	/// <summary>
	/// 
	/// </summary>
	public partial class SensorBaseEditor{
		
		/// <summary>
		/// 
		/// </summary>
		public  static void Draw2String(Object undoObj,ref string[] col0,ref string[] col1) {
			int i=0,imax=Mathf.Min(col0.Length,col1.Length);
			string text0,text1;

			GUILayout.BeginVertical();
			for(;i<imax;++i){
				GUI.changed=false;
				GUILayout.BeginHorizontal();
				text0=EditorGUILayout.TextField(col0[i]);
				text1=EditorGUILayout.TextField(col1[i]);
				GUILayout.EndHorizontal();
				if(GUI.changed){
					Undo.RecordObject(undoObj,"Modify Text");
					col0[i]=text0;
					col1[i]=text1;
					EditorUtility.SetDirty(undoObj);
				}
			}
			GUILayout.EndVertical();
		}
	}

}
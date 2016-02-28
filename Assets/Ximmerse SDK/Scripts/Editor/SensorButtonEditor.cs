using UnityEditor;

namespace Ximmerse.Core {

	/// <summary>
	/// 
	/// </summary>
	[CustomEditor(typeof(SensorButton))]
	public partial class SensorButtonEditor:Editor {

		protected bool m_IsEditMode=false;

		/// <summary>
		/// 
		/// </summary>
		public override void OnInspectorGUI() {
			DrawDefaultInspector();
			if(m_IsEditMode=EditorGUILayout.ToggleLeft("Edit Mode",m_IsEditMode)){
				SensorButton target_=target as SensorButton;
				SensorBaseEditor.Draw2String(target_,ref target_.displayName,ref target_.buttonName);
			}
		}
	}
}
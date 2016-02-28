using UnityEditor;

namespace Ximmerse.Core {

	/// <summary>
	/// 
	/// </summary>
	[CustomEditor(typeof(SensorAxis))]
	public partial class SensorAxisEditor:Editor {

		protected bool m_IsEditMode=false;

		/// <summary>
		/// 
		/// </summary>
		public override void OnInspectorGUI() {
			DrawDefaultInspector();
			if(m_IsEditMode=EditorGUILayout.ToggleLeft("Edit Mode",m_IsEditMode)){
				SensorAxis target_=target as SensorAxis;
				SensorBaseEditor.Draw2String(target_,ref target_.displayName,ref target_.axisName);
			}
		}
	}
}
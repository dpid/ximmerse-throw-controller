using UnityEngine;
using UnityEditor;

namespace Ximmerse.Animation{

	/// <summary>
	/// 
	/// </summary>
	[CustomEditor(typeof(HandAnimator))]
	public class HandAnimatorInspector:Editor {

		#region Fields

		protected bool m_ShowInEditor=false,m_TargetAwake=false;

		#endregion Fields

		#region Methods

		/// <summary>
		/// 
		/// </summary>
		public override void OnInspectorGUI() {
			DrawDefaultInspector();
			EditorGUILayout.Separator();
			//
			HandAnimator target_=target as HandAnimator;

			m_ShowInEditor=true;
			//if(EditorApplication.isPlaying){
			//	m_ShowInEditor=EditorGUILayout.ToggleLeft("Show In Editor",m_ShowInEditor);
			//}
			//if(m_ShowInEditor){if(!m_TargetAwake){
				//target_.AwakeForEditor();// TODO ???
				//m_TargetAwake=true;
			//}}

			if(DrawHandGUI("Left",target_.leftGesture)||DrawHandGUI("Right",target_.rightGesture)){if(m_ShowInEditor){
				target_.hands[HandControllerBase.k_LeftHand].SetGesture(target_.leftGesture);
				target_.hands[HandControllerBase.k_RightHand].SetGesture(target_.rightGesture);
			}}
		
		}

		/// <summary>
		/// 
		/// </summary>
		public static bool DrawHandGUI(string i_flag,HandGesture i_gesture) {
			bool ret=false,foldout=EditorGUILayout.Foldout(EditorPrefs.GetBool(i_flag+" Hand",false),i_flag+" Hand");
			EditorPrefs.SetBool(i_flag+" Hand",foldout);
			if(foldout){
				if(i_gesture==null){
					EditorGUILayout.HelpBox(
						"Please link a HandGesture asset to "+i_flag+" Gesture",
						MessageType.Warning
					);
				}else{
					HandGesture.Entry[] entries=i_gesture.entriesForEditor;
					float value;
					for(int i=0,imax=entries.Length;i<imax;++i){
						GUI.changed=false;
						value=EditorGUILayout.Slider(entries[i].id.ToString(),entries[i].value,-1.0f, 1.0f);
						if(GUI.changed){
							//Log.i("","");
							Undo.RecordObject(i_gesture,"Modify HandGesture");
							entries[i].value=value;
							EditorUtility.SetDirty(i_gesture);
							ret=true;
						}
					}
				}
			}
			return ret;
		}

		#endregion Methods
	
	}
}

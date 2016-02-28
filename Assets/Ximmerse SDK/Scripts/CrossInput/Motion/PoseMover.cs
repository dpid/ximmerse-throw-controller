using UnityEngine;
using Ximmerse.CrossInput;

namespace Ximmerse{

	/// <summary>
	/// A example how to display VirtualPose into the Unity scene.
	/// </summary>
	public class PoseMover:MonoBehaviour {

		#region Fields

		[SerializeField]protected Transform m_Transform;
		public string poseName;

		[Header("Setting")]
//		public bool useSpringAlgorithm=false;

		public bool useWorldSpace=true;//false;
		public bool useSmooth=true;

		public bool usePosition=true;
		public float smoothPosition=.5f;

		public bool useRotation=true;
		public float smoothRotation=.5f;

		[System.NonSerialized]protected VirtualPose m_Pose;
//#if ALPHA
//		protected SpringAlgorithm m_SpringAlgorithm;
//#endif

		#endregion Fields

		#region Unity Messages

		/// <summary>
		/// 
		/// </summary>
		protected virtual void Start() {
			m_Pose=CrossInputManager.VirtualPoseReference(poseName);
			if(m_Pose==null){
				Log.e("PoseMover",string.Format("Invalid Pose@{0}",poseName));
				Object.Destroy(this);
				return;
			}
			if(m_Transform==null) m_Transform=transform;
			//if(useWorldSpace)
		}
	
		/// <summary>
		/// 
		/// </summary>
		protected virtual void FixedUpdate() {
			//
			Vector3 position;
			Quaternion rotation;
			//
			#region Algorithm
			
//#if ALPHA
//			if(useSpringAlgorithm) {
//				m_SpringAlgorithm.Update();
//				position=m_SpringAlgorithm.position;
//				rotation=m_SpringAlgorithm.rotation;
//			}else 
//#endif
			{
				position=m_Pose.position;
				rotation=m_Pose.rotation;
			}

			#endregion Algorithm
			//
			if(useSmooth){
				#region Transform

				if(useWorldSpace){
					if(usePosition){
						m_Transform.position=Vector3.Lerp(m_Transform.position,/*m_Pose.*/position,smoothPosition);
					}
					if(useRotation){
						m_Transform.rotation=Quaternion.Slerp(m_Transform.rotation,/*m_Pose.*/rotation,smoothRotation);
					}
				}else{
					if(usePosition){
						m_Transform.localPosition=Vector3.Lerp(m_Transform.localPosition,/*m_Pose.*/position,smoothPosition);
					}
					if(useRotation){
						m_Transform.localRotation=Quaternion.Slerp(m_Transform.localRotation,/*m_Pose.*/rotation,smoothRotation);
					}
				}

				#endregion Transform
			}else{
				#region Transform

				if(useWorldSpace){
					if(usePosition){
						m_Transform.position=/*m_Pose.*/position;
					}
					if(useRotation){
						m_Transform.rotation=/*m_Pose.*/rotation;
					}
				}else{
					if(usePosition){
						m_Transform.localPosition=/*m_Pose.*/position;
					}
					if(useRotation){
						m_Transform.localRotation=/*m_Pose.*/rotation;
					}
				}

				#endregion Transform
			}
	
		}

		#endregion Unity Messages

	}
}
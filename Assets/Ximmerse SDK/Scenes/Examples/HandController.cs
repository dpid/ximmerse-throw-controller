using UnityEngine;
using Ximmerse.Animation;
using Ximmerse.CrossInput;

namespace Ximmerse.Examples {

	/// <summary>
	/// 
	/// </summary>
	public class HandController:MonoBehaviour {

		#region Fields

		public HandAnimator animator;
		public Transform handAnchor;
		public HandBones handType=HandBones.LeftHand;

		public string poseName="Left_Hand";
		public string triggerName="Left_Trigger";

		protected VirtualPose m_Pose;
		protected Transform m_HandTrans;
		protected HandControllerBase m_HandCtrl;

		protected float m_HandStretch;
		protected Quaternion m_HandLocalRot;

		#endregion Fields

		#region Unity Messages

		/// <summary>
		/// 
		/// </summary>
		protected virtual void Start(){
			if(animator==null){
				animator=transform.parent.GetComponent<HandAnimator>();//???
			}
			if(handAnchor==null){
				handAnchor=transform;
			}
			//
			m_Pose=CrossInputManager.VirtualPoseReference(poseName);
			m_HandTrans=animator.joints[handType].bone;
			m_HandCtrl=animator.GetHandController(handType);
			m_HandLocalRot=m_HandTrans.localRotation;//???
		}

		/// <summary>
		/// 
		/// </summary>
		protected virtual void Update(){
			if(true){
				m_HandStretch=HandJoint.N2A(1.0f-CrossInputManager.GetAxis(triggerName));// Inverse the trigger value.
				for(int i=0,imax=m_HandCtrl.fingers.Length;i<imax;++i){
					m_HandCtrl.fingers[i].Stretch(m_HandStretch);
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		protected virtual void FixedUpdate(){
			//
			m_HandTrans.position=m_Pose.position;
			m_HandTrans.rotation=m_Pose.rotation*m_HandLocalRot;
		}

		#endregion Unity Messages
	
	}

}
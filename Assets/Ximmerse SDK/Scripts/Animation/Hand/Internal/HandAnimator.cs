using System.Collections.Generic;
using UnityEngine;

namespace Ximmerse.Animation{
	
	/// <summary>
	/// 
	/// </summary>
	[ExecuteInEditMode]
	public class HandAnimator:MonoBehaviour {

		#region Fields

		public HandAvatar avatar;
		[System.NonSerialized]public HandJoint[] m_Joints;
		// TODO : ????
		[System.NonSerialized]public HandControllerBase[] hands;
		public Dictionary<HandBones,HandFinger> fingers;
		public Dictionary<HandBones,HandJoint> joints;
		
		//
		[SerializeField]protected HandGesture 
			m_LeftGesture,
			m_RightGesture
		;

		#endregion Fields

		#region Unity Messages

		/// <summary>
		/// 
		/// </summary>
		protected virtual void Awake(){
			//
			if(avatar==null) return;
			
			//
			Transform t=transform;
			avatar=avatar.Instantiate(t);// ???
			avatar.MapToJoints(t,ref m_Joints,ref joints);
			
			//
			hands=new HandControllerBase[2];
			hands[HandControllerBase.k_LeftHand]=new HandControllerBase(this,HandControllerBase.k_LeftHand,HandBones.LeftHand);
			hands[HandControllerBase.k_RightHand]=new HandControllerBase(this,HandControllerBase.k_RightHand,HandBones.RightHand);
			
			//
			fingers=new Dictionary<HandBones,HandFinger>();
			HandBones b;HandFinger f;
			for(int i=0,imax=5;i<imax;++i){
				//
				b=HandBones.LeftThumbProximal+3*i;
				fingers.Add(b,f=new HandFinger(this,i,b));
				hands[HandControllerBase.k_LeftHand].AddFinger(f);
				//
				b=HandBones.RightThumbProximal+3*i;
				fingers.Add(b,f=new HandFinger(this,i,b));
				hands[HandControllerBase.k_RightHand].AddFinger(f);
			}
			//
			if(m_LeftGesture!=null) hands[HandControllerBase.k_LeftHand].SetGesture(m_LeftGesture);
			if(m_RightGesture!=null) hands[HandControllerBase.k_RightHand].SetGesture(m_RightGesture);
			// Clean up...
		}

		/// <summary>
		/// 
		/// </summary>
		protected virtual void Update(){
#if UNITY_EDITOR
			if(m_Joints==null) return;
#endif
			for(int i=0,imax=m_Joints.Length;i<imax;++i){
				if(m_Joints[i].isDirty){
					m_Joints[i].Update();
				}
			}
		}

#if UNITY_EDITOR

		/// <summary>
		/// 
		/// </summary>
		public virtual void AwakeForEditor(){
			Awake();
		}

#endif

		#endregion Unity Messages

		#region Methods

		/// <summary>
		/// 
		/// </summary>
		public virtual HandControllerBase GetHandController(HandBones i_handId){
			switch(i_handId){
				case HandBones.LeftHand:
				return hands[HandControllerBase.k_LeftHand];
				case HandBones.RightHand:
				return hands[HandControllerBase.k_RightHand];
				default:
				return null;
			}
		}

		#endregion Methods

		#region Properties

		/// <summary>
		/// 
		/// </summary>
		public virtual HandGesture leftGesture{
			get{
				return m_LeftGesture;
			}
			set{
				m_LeftGesture=value;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual HandGesture rightGesture{
			get{
				return m_RightGesture;
			}
			set{
				m_RightGesture=value;
			}
		}

		#endregion Properties
	
	}

}
using System.Collections.Generic;

namespace Ximmerse.Animation{

	/// <summary>
	/// 
	/// </summary>
	public class HandControllerBase {

		#region Static

		public const int
			k_LeftHand   = 0,
			k_RightHand  = 1
		;

		#endregion Static

		#region Fields

		public HandAnimator animator;
		public int handType=k_LeftHand;
		public HandBones handId=HandBones.LeftHand;
		public HandFinger[] fingers;

		[System.NonSerialized]protected HandGesture m_PrevGesture,m_NowGesture;

		#endregion Fields

		#region Methods

		/// <summary>
		/// 
		/// </summary>
		public HandControllerBase(
			HandAnimator i_animator,
			int i_handType,
			HandBones i_handId
		){
			//
			animator=i_animator;
			handType=i_handType;
			handId=i_handId;
			//
			fingers=new HandFinger[5];
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual void AddFinger(HandFinger o_finger){
			o_finger.hand=this;
			fingers[o_finger.fingerId]=o_finger;
			//Log.i("HandControllerBase",(HandBones.Thumb+(o_finger.joints[0].boneId-handId-1)/3).ToString());
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual void SetGesture(HandGesture i_gesture){
			for(int i=0,imax=5;i<imax;++i){
				fingers[i].Stretch(
					i_gesture.GetStretch(i)
				);
			}
		}

		#endregion Methods

	}
}

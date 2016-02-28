using System.Collections.Generic;

namespace Ximmerse.Animation {

	/// <summary>
	/// 
	/// </summary>
	public class HandFinger {
		
		#region Static

		public const int
			k_Thumb=0,
			k_Index=1,
			k_Middle=2,
			k_Ring=3,
			k_Little=4//,
		;
		
		#endregion Static
		
		#region Fields

		public bool isDirty=false;

		public HandAnimator animator;
		public HandControllerBase hand;
		public int fingerId;
		public HandJoint[] joints;

		protected int m_NumJoints;
		protected List<float> m_HisValues=new List<float>();

		#endregion Fields

		#region Methods

		/// <summary>
		/// 
		/// </summary>
		public HandFinger(
			HandAnimator i_animator,
			int i_fingerId,
			HandBones i_boneId,int i_num=3
		) {
			animator=i_animator;
			fingerId=i_fingerId;
			joints=new HandJoint[i_num];
			m_NumJoints=0;
			for(;m_NumJoints<i_num;++m_NumJoints) {
				joints[m_NumJoints]=animator.joints[i_boneId+m_NumJoints];
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual void Update() {
			int imax=m_HisValues.Count;
			if(imax>=0) {
				Stretch(m_HisValues[imax-1]);
				m_HisValues.Clear();
			}
			isDirty=false;
		}

		/// <summary>
		/// Range[-1,1]
		/// </summary>
		public virtual void Stretch(float i_value) {
			//
			int i=(fingerId==k_Thumb)?1:0;
			for(;i<m_NumJoints;++i) {
				joints[i].Set(HandJoint.k_Stretch,i_value);
			}
		}

		/// <summary>
		/// Range[-1,1]
		/// </summary>
		public virtual void Stretch(int i_index,float i_value) {
			//
			joints[i_index].Set(HandJoint.k_Stretch,i_value);
		}

		/// <summary>
		/// Range[-1,1]
		/// </summary>
		public virtual void Spread(float i_value) {
			//
			joints[0].Set(HandJoint.k_Spread,i_value);
		}

		#endregion Methods

	}
}

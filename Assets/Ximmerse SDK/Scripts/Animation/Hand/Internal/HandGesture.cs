using System.Collections.Generic;
using UnityEngine;

namespace Ximmerse.Animation{

	/// <summary>
	/// <para>作用如同AnimationClip,这是一个静态帧的序列化.</para>
	/// <para>TDOO:初版只实现同一个手指内各个关节的延伸值.</para>
	/// </summary>
#if UNITY_5 && !UNITY_5_0
	[CreateAssetMenu(fileName="New Hand Gesture",menuName="Ximmerse SDK/Hand/Gesture",order=800)]
#endif
	public class HandGesture:ScriptableObject {

		#region Static

		/// <summary>
		/// 
		/// </summary>
		public static void LerpStretch(HandControllerBase o_ctrl,HandGesture i_from,HandGesture i_to,float i_t){
			float from,to;
			for(int i=0,imax=5;i<imax;++i){
				from=i_from.GetStretch(i);
				to  =i_to.GetStretch(i);
				o_ctrl.fingers[i].Stretch(
					from*(1.0f-i_t)+to*i_t
				);
			}
		}

		#endregion Static

		#region Data Struct

		/// <summary>
		/// 
		/// </summary>
		[System.Serializable]
		public class Entry{
			//public HandGesture gesture;
			public HandBones id;
			[Range(-1.0f, 1.0f)]
			public float value=0.0f;
		}

		#endregion Data Struct

		#region Fields

		/// <summary>
		/// 
		/// </summary>
		public bool inverse=false;

		/// <summary>
		/// 
		/// </summary>
		[SerializeField]protected Entry[] m_Entries=new Entry[5]{
			new Entry{/*gesture=this,*/id=HandBones.Thumb,value=0.0f},
			new Entry{/*gesture=this,*/id=HandBones.Index,value=0.0f},
			new Entry{/*gesture=this,*/id=HandBones.Middle,value=0.0f},
			new Entry{/*gesture=this,*/id=HandBones.Ring,value=0.0f},
			new Entry{/*gesture=this,*/id=HandBones.Little,value=0.0f}//,
		};
		/// <summary>
		/// TODO : ????
		/// </summary>
		public Dictionary<int,Entry> entries;

		[System.NonSerialized]protected bool m_IsInited=false;

		#endregion Fields

		#region Methods
		
		/// <summary>
		/// 
		/// </summary>
		protected virtual void Init(){
			//
			if(m_IsInited){
				return;
			}
			//
			int i=0,imax=m_Entries.Length;
			entries=new Dictionary<int,Entry>(imax);
			for(;i<imax;++i){
				entries.Add((int)(m_Entries[i].id-HandBones.Thumb),m_Entries[i]);
			}
			//
			m_IsInited=true;
		}

		/// <summary>
		/// 
		/// </summary>
		//public virtual int IndexOf(HandBones i_id){
		//	for(int i=0,imax=entries.Length;i<imax;++i){
		//		if(entries[i].id==i_id){
		//			return i;
		//		}
		//	}
		//	return -1;
		//}

		/// <summary>
		/// 
		/// </summary>
		public virtual float GetStretch(int i_id){
			//
			if(!m_IsInited){
				Init();
			}
			//
			if(entries.ContainsKey(i_id)){
				return entries[i_id].value;
			}
			return 0.0f;
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual float GetSpread(int i_id){
			throw new System.NotImplementedException();
		}

		#endregion Methods

		#region Properties
		
#if UNITY_EDITOR

		/// <summary>
		/// 
		/// </summary>
		public virtual Entry[] entriesForEditor{
			get{
				return m_Entries;
			}
		}
#endif

		#endregion Properties

	}
}

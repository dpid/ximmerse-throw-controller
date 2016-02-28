using System.Collections.Generic;
using UnityEngine;
using Ximmerse.IO;

namespace Ximmerse.Animation{

	/// <summary>
	/// 
	/// </summary>
#if UNITY_5 && !UNITY_5_0
	[CreateAssetMenu(fileName="New Hand Avatar",menuName="Ximmerse SDK/Hand/Avatar",order=800)]
#endif
	public class HandAvatar:ScriptableObject{

		#region Fields

		public VirtualFile m_TextAsset;
		[SerializeField]protected HandJoint[] m_PersistentJoints=new HandJoint[0];
		[System.NonSerialized]protected bool m_IsInited=false;
		
		#endregion Fields

		#region Methods

		#region Instantiate

		/// <summary>
		/// 
		/// </summary>
		public virtual HandAvatar Instantiate(Transform i_root){
			HandAvatar copy=this;//Object.Instantiate(this) as HandAvatar;
			copy.Init();
			return copy;
		}

		/// <summary>
		/// 初始化.
		/// </summary>
		protected virtual void Init() {
			//
			if(m_IsInited) return;
			//
			// Do something....
			//
			m_IsInited=true;
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual void MapToJoints(Transform i_root,ref HandJoint[] o_joints,ref Dictionary<HandBones,HandJoint> o_mapJoints){
			if(!m_IsInited) {
				Init();
			}
			//
			int i=0,imax=m_PersistentJoints.Length;
			o_joints=new HandJoint[imax];o_mapJoints=new Dictionary<HandBones,HandJoint>(imax);
			HandJoint joint;
			for(;i<imax;++i) {joint=m_PersistentJoints[i].Clone();
				joint.bone=i_root.FindChild(joint.path);
				o_joints[i]=joint;
				o_mapJoints.Add(joint.boneId,joint);
			}
		}

		#endregion Instantiate

		#region I/O

		/// <summary>
		/// 序列化到Csv文件.
		/// </summary>
		[ContextMenu("Save")]
		public virtual void Save(){
			if(m_TextAsset==null){
				return;
			}
			Log.e("HandAvatar","System.NotImplementedException");
		}

		/// <summary>
		/// 从Csv文件反序列化.
		/// </summary>
		[ContextMenu("Load")]
		public virtual void Load(){
			if(m_TextAsset==null){
				return;
			}
			string[] csvData,lines=m_TextAsset.ReadAllLines();
			int i=0,imax=lines.Length,j;
			m_PersistentJoints=new HandJoint[imax];
			for(;i<imax;++i) {
				if(!string.IsNullOrEmpty(lines[i])){
					csvData=lines[i].Split(',');
					m_PersistentJoints[i]=new HandJoint{
#if UNITY_EDITOR
						name=csvData[0],
#endif
						boneId=(HandBones)System.Enum.Parse(typeof(HandBones),csvData[0]),
						bone=null,
						path=csvData[j=1],
						value=new Vector3(float.Parse(csvData[++j]),float.Parse(csvData[++j]),float.Parse(csvData[++j])),
						minEulerAngles=new Vector3(float.Parse(csvData[++j]),float.Parse(csvData[++j]),float.Parse(csvData[++j])),
						maxEulerAngles=new Vector3(float.Parse(csvData[++j]),float.Parse(csvData[++j]),float.Parse(csvData[++j])),
						isDirty=true
					};
				}
			}
		}

		#endregion I/O

		#endregion Methods
	
	}

}
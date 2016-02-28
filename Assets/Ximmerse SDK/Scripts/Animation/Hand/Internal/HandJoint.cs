using UnityEngine;

namespace Ximmerse.Animation {
	
	/// <summary>
	/// 
	/// </summary>
	[System.Serializable]
	public class HandJoint/*:Object*/{

		#region Static

		public static readonly char[] SPLIT_CSV=new char[1]{','};
		public const int FREE=0x0,FREEZE_UP=0x1,FREEZE_DOWN=0x2;
		public static int k_Spread=1,k_Stretch=2;

		/// <summary>
		/// ???
		/// </summary>
		public static float A2N(float i_value){
			return (i_value+1.0f)*.5f;
		}

		/// <summary>
		/// ??? 
		/// </summary>
		public static float N2A(float i_value){
			return (i_value-.5f)*2.0f;
		}

#if UNITY_EDITOR

		/// <summary>
		/// 
		/// </summary>
		public static string V3ToCSV(Vector3 i_v){
			return i_v.x.ToString()+","+
				i_v.y.ToString()+","+
				i_v.z.ToString();
		}

		/// <summary>
		/// 获取节点node相对于节点root的路径.
		/// </summary>
		public static string GetChildPath(Transform i_root,Transform i_node){
			if(i_node==null) return "";//<!> No a node...
		
			string str=i_node.name;

			while((i_node=i_node.parent)!=null){//Up <!> No a parent...
				if(i_node==i_root) break;//<!> Enough path...
				str=i_node.name+"/"+str;
			}

			return str;
		}

#endif

		#endregion Static

		#region Fields

#if UNITY_EDITOR
		public string name;
#endif

		public HandBones boneId;
		public Transform bone;
		public string path;
		
		[System.NonSerialized]public HandJoint prev,next;

		/// <summary>
		/// 输入开关. @see Rigidbody.constraints
		/// </summary>
		public int[] constraints=new int[3];

		/// <summary>
		/// [-1, 1]
		/// </summary>
		public Vector3 value=Vector3.zero;

		/// <summary>
		/// 实时值,欧拉角.
		/// </summary>
		[System.NonSerialized]public Vector3 eulerAngles=Vector3.zero;

		/// <summary>
		/// 插值范围.
		/// </summary>
		public Vector3 minEulerAngles=Vector3.zero,maxEulerAngles=Vector3.zero;

		/// <summary>
		/// Dirty状态参数.
		/// </summary>
		/*[System.NonSerialized]*/public bool isDirty=true;

		#endregion Fields

		#region Methods

		/// <summary>
		/// 提交旋转.
		/// </summary>
		public void Update(){
			//
			if(!isDirty){
				return;
			}else{
				isDirty=false;
			}
			//Lerp
			eulerAngles.Set(
				Mathf.Lerp(minEulerAngles.x,maxEulerAngles.x,(value.x+1.0f)*.5f),
				Mathf.Lerp(minEulerAngles.y,maxEulerAngles.y,(value.y+1.0f)*.5f),
				Mathf.Lerp(minEulerAngles.z,maxEulerAngles.z,(value.z+1.0f)*.5f)
			);
			// Check
			if(float.IsNaN(eulerAngles.x)||
			   float.IsNaN(eulerAngles.y)||
			   float.IsNaN(eulerAngles.z)
			){
				return;
			}
			//Apply
			bone.localRotation=Quaternion.Euler(eulerAngles);
		}

#if UNITY_EDITOR

		/// <summary>
		/// 
		/// </summary>
		public string ToCSV(Transform i_root){
			return boneId.ToString()+","+GetChildPath(i_root,bone)+","+
				V3ToCSV(value)+","+
				V3ToCSV(minEulerAngles)+","+
				V3ToCSV(maxEulerAngles);
		}

#endif

		#endregion Methods

		#region APIs

		/// <summary>
		/// 
		/// </summary>
		public void Add(int i_axis,float i_amount){//RAW
			// Filter...
			if(i_amount==0.0f) return;
			int msk=constraints[i_axis];
			if((msk&FREEZE_UP)==FREEZE_UP){
				if(i_amount>0.0f) return;
			}
			if((msk&FREEZE_DOWN)==FREEZE_DOWN){
				if(i_amount<0.0f) return;
			}
			// Apply...
			this.value[i_axis]=Mathf.Clamp(this.value[i_axis]+i_amount,-1.0f, 1.0f);
			isDirty=true;
		}
		
		/// <summary>
		/// 
		/// </summary>
		public void Set(int i_axis,float i_value){//COPY
			float i_amount=i_value-this.value[i_axis];
			// Filter...
			if(i_amount==0.0f) return;
			int msk=constraints[i_axis];
			if((msk&FREEZE_UP)==FREEZE_UP){
				if(i_amount>0.0f) return;
			}
			if((msk&FREEZE_DOWN)==FREEZE_DOWN){
				if(i_amount<0.0f) return;
			}
			// Apply...
			this.value[i_axis]=Mathf.Clamp(i_value,-1.0f, 1.0f);
			isDirty=true;
		}

		/// <summary>
		/// 请使用Clone()实例化.
		/// </summary>
		public HandJoint Clone() {
			return new HandJoint {
				#region Fields

#if UNITY_EDITOR
				name=this.name,
#endif

				boneId=this.boneId,
				bone=this.bone,
				path=this.path,
		
				//[System.NonSerialized]HandJoint prev,next,

				/// <summary>
				/// 输入开关. @see Rigidbody.constraints
				/// </summary>
				constraints=(int[])this.constraints.Clone(),//=new int[3],

				/// <summary>
				/// [-1, 1]
				/// </summary>
				value=this.value,//=Vector3.zero,

				/// <summary>
				/// 实时值,欧拉角.
				/// </summary>
				eulerAngles=this.eulerAngles,//=Vector3.zero,

				/// <summary>
				/// 插值范围.
				/// </summary>
				minEulerAngles=this.minEulerAngles,//=Vector3.zero,
				maxEulerAngles=this.maxEulerAngles,//=Vector3.zero,

				/// <summary>
				/// Dirty状态参数.
				/// </summary>
				isDirty=this.isDirty//=true,

				#endregion Fields
			};
		}

		#endregion APIs
	
	}
}

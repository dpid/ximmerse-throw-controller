using UnityEngine;
using Ximmerse.CrossInput;
using Ximmerse.IO;

namespace Ximmerse.Core {

	/// <summary>
	/// 
	/// </summary>
	public class XHawkServiceBase:MonoBehaviour,IInputSource{

		#region Static

		public static XHawkServiceBase main;

		public static byte[] s_HelperBytes=new byte[12];

		/// <summary>
		/// 
		/// </summary>
		public static void ToVector3(
			ref Vector3 o_vec,ref Vector3 i_scale,
			byte[] buffer,int offset,bool i_isInverse=false){
			if(i_isInverse){
				int i=0,imax=3;
				while(imax-->0){
					ArrayUtil.MemCpy<byte>(s_HelperBytes,i+3,-1,buffer,offset+i,1,4);i+=4;
				}
				buffer=s_HelperBytes;offset=0;
			}
			o_vec.Set(
				System.BitConverter.ToSingle(buffer,offset+0)*i_scale.x,
				System.BitConverter.ToSingle(buffer,offset+4)*i_scale.y,
				System.BitConverter.ToSingle(buffer,offset+8)*i_scale.z
			);
		}

		#endregion Static

		#region Fields

		public string configPath;
		public string configSec;
		
		[SerializeField]protected Transform m_Pivot;
		public Transform pivot {
			get {
				if(m_Pivot==null) {
					VRContext vrCtx=VRContext.Main;
					if(vrCtx==null || vrCtx.centerEyeAnchor==null) {
						m_Pivot=new GameObject("XHawkInput Pivot").transform;
					}else {
						m_Pivot=vrCtx.centerEyeAnchor;
					}
					//DontDestroyOnLoad(m_Pivot);
				}
				return m_Pivot;
			}
		}

		public Vector3 sensitivity=Vector3.one*.001f;
		public BlobsID3D[] points=new BlobsID3D[2];
		
		protected int m_TimestampRec,m_TimestampParse;
		
		protected byte[] m_BufferRead;
		protected int m_OffsetRead=-1,m_SizeRead=-1;

		#endregion Fields
		
		#region IInputSource

		/// <summary>
		/// 
		/// </summary>
		public virtual int InitInput() {
			if(main==null){
				main=this;
			}else if(main!=this){
				// TODO:
			}
			Library.Init();
			//
			if(!string.IsNullOrEmpty(configPath)) {
				IniReader ir=IniReader.Open(Environment.CONFIG_PATH+configPath);
				if(ir!=null){
					ReadConfig(ir);
				}
			}
			//
			for(int i=0,imax=points.Length;i<imax;++i){
				points[i].OnStart(this);
			}
			//
			Open();
			return 0;
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual int ExitInput() {
			Close();
			Library.Exit();
			return 0;
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual int EnterInputFrame() {
			for(int i=0,imax=points.Length;i<imax;++i){
				points[i].OnUpdateOnMainThread();
			}
			return 0;
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual int ExitInputFrame() {
			return 0;
		}

		#endregion IInputSource

		#region Methods
		
		/// <summary>
		/// 
		/// </summary>
		protected virtual void ReadConfig(IniReader i_ir) {
			sensitivity=i_ir.TryParseVector3(configSec+"@Sensitivity",sensitivity);
			pivot.localPosition=i_ir.TryParseVector3(configSec+"@PivotPos",m_Pivot.localPosition);
			pivot.localRotation=Quaternion.Euler(i_ir.TryParseVector3(configSec+"@PivotRot",m_Pivot.localRotation.eulerAngles));
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual void Open() {
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual void Close() {
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual void Register(int id,IBlobListener listener) {
			int i=points[id].listeners.IndexOf(listener);
			if(i==-1){
				points[id].listeners.Add(listener);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual void Unregister(int id,IBlobListener listener) {
			int i=points[id].listeners.IndexOf(listener);
			if(i!=-1){
				points[id].listeners.RemoveAt(i);
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		public virtual void SwapPoints(int lhs,int rhs){
			BlobsID3D b=points[lhs];points[lhs]=points[rhs];points[rhs]=b;
		}
		
		/// <summary>
		/// 
		/// </summary>
		protected virtual void Parse(byte[] buffer,int offset,int count){
		}

		#endregion Methods
	
	}

}
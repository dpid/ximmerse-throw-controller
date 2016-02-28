using System;

namespace Ximmerse.IO {

	/// <summary>
	/// 
	/// </summary>
	public class VirtualStream:IStreamable,IStreamOpenCallback,IStreamReadCallback {

		#region Fields

		public IStreamable baseStream;
		public byte[] m_Buffer=null;
		public int m_BaseOffset=-1;
		protected int m_Offset;
		protected int m_Count;

		#endregion Fields

		#region Constructors

		/// <summary>
		/// 
		/// </summary>
		public VirtualStream(IStreamable stream) {
			baseStream=stream;
			m_Offset=-1;
		}

		/// <summary>
		/// 
		/// </summary>
		public VirtualStream(IStreamable stream,int offset,int count) {
			SetStreamPos(offset,count);
			//
			baseStream=stream;
			stream.SetOnStreamOpenListener(this);
			stream.SetOnStreamReadListener(this);
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual void SetStreamPos(int offset,int count) {
			m_Offset=offset;
			m_Count=count;
		}

		#endregion Constructors

		#region IStreamable

		protected IStreamOpenCallback m_OpenCallback;
		protected IStreamReadCallback m_ReadCallback;

		public string address {
			get {
				if(baseStream!=null) {
					return baseStream.address;
				}else {
					return "";
				}
			}
			set {
				return;
			}
		}

		public void Open() {
			if(baseStream!=null) {
				baseStream.Open();
			}
		}

		public void Close() {
			if(baseStream!=null) {
				baseStream.Close();
			}
		}

		public int Read(byte[] buffer,int offset,int count) {
			if(m_Count<count){// Get min size.
				count=m_Count;
			}
			//
			System.Array.Copy(
				m_Buffer,m_BaseOffset+m_Offset,
				buffer,offset,
				count
			);
			//Log.i("VirtualStream","Read:"+buffer.ToHexString());
			return count;
		}

		public int Write(byte[] buffer,int offset,int count) {
			return 0;
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual void SetOnStreamOpenListener(IStreamOpenCallback callback) {
			m_OpenCallback=callback;
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual void SetOnStreamReadListener(IStreamReadCallback callback) {
			m_ReadCallback=callback;
		}
		
		/// <summary>
		/// 
		/// </summary>
		public virtual void GetReadBuffer(out byte[] buffer,out int offset,out int count) {
			buffer = m_Buffer;
			offset = m_BaseOffset+m_Offset;
			count  = m_Count;
		}

		#endregion IStreamable

		#region Messages

		/// <summary>
		/// 
		/// </summary>
		public virtual void OnStreamOpenSuccess(IStreamable stream){
			if(m_OpenCallback!=null){
				m_OpenCallback.OnStreamOpenSuccess(this);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual void OnStreamOpenFailure(IStreamable stream){
			if(m_OpenCallback!=null){
				m_OpenCallback.OnStreamOpenFailure(this);
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		public virtual void OnStreamRead(byte[] buffer,int offset){
			if(m_Offset==-1){
				return;
			}
			//
			m_Buffer=buffer;
			m_BaseOffset=offset;
			//Log.i("VirtualStream",(m_BaseOffset + m_Offset)+"/"+ m_Buffer.Length + "/"+m_Count);
			//
			if(m_ReadCallback!=null){
				m_ReadCallback.OnStreamRead(this);
			}
			//
			m_Buffer=null;
			m_BaseOffset=-1;
		}
		
		/// <summary>
		/// 
		/// </summary>
		public virtual void OnStreamRead(IStreamable stream) {
			byte[] buffer;int offset,count;
			stream.GetReadBuffer(out buffer,out offset,out count);
			OnStreamRead(buffer,offset);
		}
			
		#endregion Messages

	}

}

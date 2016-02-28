#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN

using Winterdom.IO.FileMap;

namespace Ximmerse.IO {

	/// <summary>
	/// 
	/// </summary>
	public class SharedMemoryStream_Deprecated:IStreamable {

		#region Fields

		protected MemoryMappedFile m_File;
		protected MapViewStream m_Stream;

		protected bool m_IsOpen=false;
		protected string m_Address="";
		protected int m_MaxSize;
		protected int m_Offset;
		protected int m_Count;

		#endregion Fields

		#region Constructors

		/// <summary>
		/// 
		/// </summary>
		public SharedMemoryStream_Deprecated(string fileName,int maxSize,int offset,int count){
			SetStreamInfo(fileName,maxSize,offset,count);
		}

		#endregion Constructors

		#region Methods

		/// <summary>
		/// 
		/// </summary>
		public virtual SharedMemoryStream_Deprecated SetStreamInfo(string fileName,int maxSize,int offset,int count){
			//
			m_Address=fileName;
			m_MaxSize=maxSize;
			m_Offset=offset;
			m_Count=count;
			//
			return this;
		}

		#endregion Methods

		#region IStreamable

		/// <summary>
		/// 
		/// </summary>
		public virtual void Open() {
			if(m_IsOpen){
				Close();
			}
			//
			m_File=MemoryMappedFile.Create(null/*m_Address*/,MapProtection.PageReadWrite,m_MaxSize,m_Address);
			m_Stream=m_File.MapView(MapAccess.FileMapAllAccess,0/*m_Offset*/,m_MaxSize/*m_Count*/) as MapViewStream;
			//
			m_IsOpen=true;
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual void Close() {
			if(!m_IsOpen){
				return;
			}
			//
			m_File.Close();
			m_Stream.Close();
			//
			m_File=null;
			m_Stream=null;
			//
			m_IsOpen=false;
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual int Read(byte[] buffer,int offset,int count) {
			if(!m_IsOpen) return -1;
			if(m_Count<count) count=m_Count;// Get min size.
			count=m_Stream.Read(buffer,offset,count);
			return count;
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual int Write(byte[] buffer,int offset,int count) {
			if(!m_IsOpen) return -1;
			if(m_Count<count) count=m_Count;// Get min size.
			m_Stream.Write(buffer,offset,count);
			return count;
		}

		#region System.NotImplementedException()

		/// <summary>
		/// 
		/// </summary>
		public virtual void SetOnStreamOpenListener(IStreamOpenCallback callback) {
			//throw new System.NotImplementedException();
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual void SetOnStreamReadListener(IStreamReadCallback callback) {
			throw new System.NotImplementedException();
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual byte[] GetReadBuffer() {
			throw new System.NotImplementedException();
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual int GetReadSize() {
			throw new System.NotImplementedException();
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual void ResetReadBuffer() {
			throw new System.NotImplementedException();
		}

		protected byte[] m_Buffer;

		/// <summary>
		/// 
		/// </summary>
		public void GetReadBuffer(out byte[] buffer,out int offset,out int count) {
			//
			if(m_Buffer==null) {
				m_Buffer=new byte[m_MaxSize];
			}
			//
			buffer=m_Buffer;
			offset=m_Offset;
			count=m_Count;
			//
			//Open();
			m_Stream.Position=0;
			int ret=m_Stream.Read(buffer,0,m_MaxSize);
			//Close();
		}

		#endregion System.NotImplementedException()

		/// <summary>
		/// 
		/// </summary>
		public virtual string address {
			get {
				return m_Address;
			}
			set {
				m_Address=value;
			}
		}

		#endregion IStreamable
	
	}
}

#endif
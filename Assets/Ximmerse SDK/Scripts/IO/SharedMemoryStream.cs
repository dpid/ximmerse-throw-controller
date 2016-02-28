#if UNITY_EDITOR_WIN||UNITY_STANDALONE_WIN

using System.Runtime.InteropServices;

namespace Ximmerse.IO {

	/// <summary>
	/// 
	/// </summary>
	public class SharedMemoryStream:IStreamable {

		#region Natives

		public const string LIB_FILE_MAP="FileMap";

		[DllImport(LIB_FILE_MAP)]public static extern System.IntPtr filemap_alloc();
		[DllImport(LIB_FILE_MAP)]public static extern void filemap_free(ref System.IntPtr cmm);
		[DllImport(LIB_FILE_MAP,CharSet=CharSet.Unicode)]public static extern int filemap_open(System.IntPtr cmm,string name,int size);
		[DllImport(LIB_FILE_MAP)]public static extern int filemap_close(System.IntPtr cmm);
		[DllImport(LIB_FILE_MAP)]public static extern int filemap_read(System.IntPtr cmm,byte[] buffer,int offset,int count);
		[DllImport(LIB_FILE_MAP)]public static extern int filemap_write(System.IntPtr cmm,byte[] buffer,int offset,int count);
		[DllImport(LIB_FILE_MAP)]public static extern int filemap_read(System.IntPtr cmm,System.IntPtr buffer,int offset,int count);
		[DllImport(LIB_FILE_MAP)]public static extern int filemap_write(System.IntPtr cmm,System.IntPtr buffer,int offset,int count);

		#endregion Natives

		#region Fields

		protected System.IntPtr m_Cmm;

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
		public SharedMemoryStream(string fileName,int maxSize,int offset,int count){
			SetStreamInfo(fileName,maxSize,offset,count);
		}

		#endregion Constructors

		#region Methods

		/// <summary>
		/// 
		/// </summary>
		public virtual SharedMemoryStream SetStreamInfo(string fileName,int maxSize,int offset,int count){
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
			m_Cmm=filemap_alloc();
			filemap_open(m_Cmm,m_Address,m_MaxSize);
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
			if(m_Cmm!=System.IntPtr.Zero) {
				filemap_close(m_Cmm);
				filemap_free(ref m_Cmm);
			}
			//
			m_IsOpen=false;
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual int Read(byte[] buffer,int offset,int count) {
			if(!m_IsOpen) return -1;
			if(m_Count<count) count=m_Count;// Get min size.
			count=filemap_read(m_Cmm,buffer,offset,count);
			return count;
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual int Write(byte[] buffer,int offset,int count) {
			if(!m_IsOpen) return -1;
			if(m_Count<count) count=m_Count;// Get min size.
			/*count=*/filemap_write(m_Cmm,buffer,offset,count);
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
			count=filemap_read(m_Cmm,buffer,0,m_MaxSize);
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
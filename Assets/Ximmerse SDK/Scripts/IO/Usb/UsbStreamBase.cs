
namespace Ximmerse.IO.Usb {

	/// <summary>
	/// 
	/// </summary>
	public class UsbStreamBase:IStreamable {

		#region Fields

		protected string m_Address;
		protected int m_DID;
		protected int m_VID;
		protected int m_PID;

		protected byte[] m_BufferRead;
		protected int m_SizeRead;
		protected int m_SizeHasRead=0;
		protected bool m_IsOpen=false;

		protected IStreamOpenCallback m_OpenCallback;
		protected IStreamReadCallback m_ReadCallback;

		#endregion Fields

		#region Constructors

		/// <summary>
		/// 
		/// </summary>
		public UsbStreamBase(){
		}

		/// <summary>
		/// 
		/// </summary>
		public UsbStreamBase(string address):this(){
			m_Address=address;
		}

		/// <summary>
		/// 
		/// <para>When dId==-1,open the first usb device.</para>
		/// </summary>
		public UsbStreamBase(int vId,int pId,int dId=0):this(){
			m_VID=vId;
			m_PID=pId;
			m_DID=dId;
		}

		/// <summary>
		/// 
		/// <para>When dId==-1,open the first usb device.</para>
		/// </summary>
		public UsbStreamBase(string address,int vId,int pId,int dId=0):this(){
			m_Address=address;

			m_VID=vId;
			m_PID=pId;
			m_DID=dId;
		}

		#endregion Constructors

		#region IStreamable Methods

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
		public virtual int Read(byte[] buffer,int offset,int count) {
			return -1;
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual int Write(byte[] buffer,int offset,int count) {
			return -1;
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
			buffer = m_BufferRead;
			offset = 0;
			count  = m_SizeHasRead;
		}

		#endregion IStreamable Methods

		#region Properties

		/// <summary>
		/// 
		/// </summary>
		public virtual bool isOpen{
			get{
				return m_IsOpen;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual string address {
			get {
				return m_Address;
			}

			set {
				if(m_IsOpen) {
					Log.e("UsbStreamBase","Don't change hid address when the stream is opened.");
				}else {
					m_Address=value;
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual int readBufferSize {
			get {
				return m_SizeRead;
			}
			set {
				if(m_IsOpen) {
					Log.e("UsbStream","Don't resize read buffer when the stream is opened.");
				} else {
					m_SizeRead=value;
				}
			}
		}

		#endregion Properties

	}
}
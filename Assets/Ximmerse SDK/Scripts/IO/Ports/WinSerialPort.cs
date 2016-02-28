#if UNITY_EDITOR || UNITY_STANDALONE_WIN

//using System.IO.Ports;
//using System.Threading;
using Windows.IO.Ports;
using Windows.Threading;

namespace Ximmerse.IO.Ports {

	/// <summary>
	/// 
	/// </summary>
	public class WinSerialPort:IStreamable {

		#region Static

		public static string[] GetPortNames(){
			return SerialPort.GetPortNames();
		}

		#endregion Static

		#region Fields

		protected SerialPort m_Sp;
		protected Thread m_Thread;
		protected IStreamOpenCallback m_OpenCallback;
		protected IStreamReadCallback m_ReadCallback;
		protected int m_TId;
		protected byte[] m_BufferRead;
		protected int m_MaxRead,m_SizeRead;

		#endregion Fields

		#region Constructors

		/// <summary>
		/// 
		/// </summary>
		public WinSerialPort(string portName,int baudRate,int maxRead){
			m_Sp=new SerialPort(portName,baudRate);
			m_BufferRead=new byte[m_MaxRead=maxRead];
		}

		#endregion Constructors

		#region Methods

		/// <summary>
		/// 
		/// </summary>
		protected virtual int ReadThread(
			System.IntPtr lpThreadParameter
		) {
			int tId=++m_TId;
			while(tId==m_TId){
				m_SizeRead=m_Sp.Read(m_BufferRead,0,m_MaxRead);
				//Log.i("WinSerialPort",tId.ToString());
				if(m_ReadCallback!=null){
					m_ReadCallback.OnStreamRead(this);
				}
			}
			Log.i("WinSerialPort","Abort the SerialPort read thread....");
			return 0;
		}

		#endregion Methods

		#region IStreamable

		public virtual string address{
			get{
				return m_Sp.PortName;
			}
			set{
				m_Sp.PortName=value;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual void Open() {
			// Open
			if(m_Sp.IsOpen) Close();
			try{
				m_Sp.Open();
			}catch(System.Exception e){
				Log.e("WinSerialPort",e.ToString());
				return;
			}
			
			// Start the subThread.
			m_Thread=new Thread(ReadThread);
			m_Thread.IsBackground=true;
			m_Thread.Priority=ThreadPriority.AboveNormal;// ???
			m_Thread.Start();
		}
		
		/// <summary>
		/// 
		/// </summary>
		public virtual void Close() {
			//
			if(!m_Sp.IsOpen) return;
			// Stop the subThread.
			++m_TId;if(m_Thread!=null){
				m_Thread.Abort();
				m_Thread=null;
			}
			m_Sp.Close();
		}
		
		/// <summary>
		/// 
		/// </summary>
		public virtual int Read(byte[] buffer,int offset,int count) {
			if(!m_Sp.IsOpen) return -1;
			if(m_SizeRead<=0) return m_SizeRead=0;
			if(m_SizeRead>count){
				//_sizeRead=count;// ???
			}else{//if(m_SizeRead<=count)
				count=m_SizeRead;
			}
			System.Array.Copy(m_BufferRead,0,buffer,offset,count);
			m_SizeRead=0;// ???
			return count;
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual int Write(byte[] buffer,int offset,int count) {
			if(!m_Sp.IsOpen) return -1;
			m_Sp.Write(buffer,offset,count);
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
			buffer = m_BufferRead;
			offset = 0;
			count  = m_SizeRead;
		}

		#endregion IStreamable

	}
}

#endif
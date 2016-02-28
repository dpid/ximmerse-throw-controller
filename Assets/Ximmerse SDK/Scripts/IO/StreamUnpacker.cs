using Windows.Threading;

namespace Ximmerse.IO {

	/// <summary>
	/// 将动态IO流转成静态IO流的工具类(拼接和取包).
	/// </summary>
	public class StreamUnpacker:IStreamable,IStreamReadCallback {

		#region Static
		
		/// <summary>
		/// 
		/// </summary>
		public const int
			RESULT_OK                  =     0,
			RESULT_NO_PARSE_DATA       =    -1,
			RESULT_NO_ENOUGH_LENGTH    =    -2
		;

		/// <summary>
		/// 
		/// </summary>
		public static int Sum(byte[] buffer,int offset,int count){
			int ret=0;
			while(count-->0){
				ret+=buffer[offset++];
			}
			return ret;
		}

		#endregion Static
		
		#region Fields

		protected IStreamable m_RawStream;
		protected IStreamReadCallback m_ReadCallback;
		protected bool m_IsCompleted=false;

		/// <summary>
		/// 双可交换缓存.
		/// </summary>
		protected byte[] m_Buffer,m_BufferTmp;

		/// <summary>
		/// 解包的临时指针(解包接受数据指针).
		/// </summary>
		protected int m_PtrRec;

		/// <summary>
		/// 解包的临时指针(解包快照指针）.
		/// </summary>
		protected int m_PtrSnapshot;

		/// <summary>
		/// 解包成功的指针.
		/// </summary>
		protected int m_PtrBuffer;

		/// <summary>
		/// 解包成功的长度(固定).
		/// </summary>
		protected int m_SizeFixedBuffer;
		
		/// <summary>
		/// 接受缓存区大小.
		/// </summary>
		protected int m_SizeBuffer;
		
		/// <summary>
		/// 上一次接受数据的大小.
		/// </summary>
		protected int m_SizeRec;
		
		/// <summary>
		/// 接受数据的总大小.
		/// </summary>
		protected int m_SizeRecTotal;
		
		// <!-- Beta:线程阻塞读取模式,主要减少多余封装.

		/// <summary>
		/// 开启线程阻塞读取模式.反之,事件接收读取模式.
		/// </summary>
		protected bool m_UseReadThread=false;
		
		/// <summary>
		/// 解析线程
		/// </summary>
		protected Thread m_Thread;

		/// <summary>
		/// 线程存活flag.
		/// </summary>
		protected int m_TId=-1;
		
		// Beta -->
		
		#endregion Fields

		#region Constructors

		/// <summary>
		/// 
		/// </summary>
		public StreamUnpacker(int rawSize,int fixedSize){
			//
			m_PtrBuffer=m_SizeRec=m_SizeRecTotal=m_PtrRec=m_PtrSnapshot=0;
			m_SizeFixedBuffer=fixedSize;
			m_SizeBuffer=rawSize*4;
			 // Alloc
			m_Buffer=new byte[m_SizeBuffer];
			m_BufferTmp=new byte[m_SizeBuffer];
		}

		#endregion Constructors

		#region IStreamable

		/// <summary>
		/// 
		/// </summary>
		public virtual string address{
			get{
				return m_RawStream.address;
			}
			set{
				m_RawStream.address=value;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual void Open() {
			m_RawStream.Open();
			if(m_UseReadThread){//
				// <!-- Beta:线程阻塞读取模式,主要减少多余封装.

				m_RawStream.SetOnStreamReadListener(null);
				// Start the subThread.
				m_Thread=new Thread(ThreadProc);
				m_Thread.IsBackground=true;
				m_Thread.Priority=ThreadPriority.AboveNormal;// ???
				m_Thread.Start();
		
				// Beta -->
			}else{
				m_RawStream.SetOnStreamReadListener(this);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual void Close() {
			// <!-- Beta:线程阻塞读取模式,主要减少多余封装.
			
			// Stop the subThread.
			++m_TId;if(m_Thread!=null){
				m_Thread.Abort();
				m_Thread=null;
			}
			
			// Beta -->
			m_RawStream.Close();
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual int Read(byte[] buffer,int offset,int count) {
			if(m_IsCompleted){
				if(count>m_SizeFixedBuffer){// Get min size.
					count=m_SizeFixedBuffer;
				}
				System.Array.Copy(m_Buffer,m_PtrBuffer,buffer,offset,count);
				return count;
			}else{
				return 0;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual int Write(byte[] buffer,int offset,int count) {
			return m_RawStream.Write(buffer,offset,count);
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual void SetOnStreamOpenListener(IStreamOpenCallback callback) {
			m_RawStream.SetOnStreamOpenListener(callback);
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
			offset = m_PtrBuffer;
			count  = m_SizeFixedBuffer;
		}

		#endregion IStreamable

		#region Methods
		
		/// <summary>
		/// 
		/// </summary>
		public virtual StreamUnpacker SetStream(IStreamable i_stream,bool i_useReadThread=false){
			m_RawStream=i_stream;
			m_UseReadThread=i_useReadThread;
			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		protected virtual int ThreadProc(System.IntPtr lpThreadParameter) {
			int tId=++m_TId;
			while(tId==m_TId){
				OnStreamRead(m_RawStream);
			}
			Log.i("StreamUnpacker","Abort the StreamUnpacker read thread....");
			return 0;
		}

		/// <summary>
		/// 请用子类改写这个解包API.
		/// </summary>
		public virtual void OnStreamRead(IStreamable stream){
		}
		
		/// <summary>
		/// 读取的清理工作,如收集串口数据碎片.
		/// </summary>
		protected virtual void CleanUp(int i_result){
			
			switch(i_result){
				// Save small data.
				case RESULT_NO_PARSE_DATA://没有解包数据->数据破碎.
					//
					m_PtrRec=m_PtrRec+m_SizeRec;
					// Reset snapshot pointer.
					//m_PtrSnapshot=m_PtrRec;
					//Log.i("StreamUnpacker","RESULT_NO_PARSE_DATA");
				break;
				// Save broken data.
				case RESULT_NO_ENOUGH_LENGTH:// 数据破碎.
					//
					System.Array.Copy(
						m_Buffer,m_PtrSnapshot,m_BufferTmp,0,
						m_PtrRec=m_PtrRec+m_SizeRec-m_PtrSnapshot//(end=len-1)-_p+1=len-_p
					);
					byte[] c=m_Buffer;m_Buffer=m_BufferTmp;m_BufferTmp=c;// 交换双缓存.
					// Reset snapshot pointer.
					m_PtrSnapshot=0;
					//Log.i("StreamUnpacker","RESULT_NO_ENOUGH_LENGTH");
				break;
				default:
					m_PtrSnapshot=-1;
				break;
			}
		}

		#endregion Methods

	}
}
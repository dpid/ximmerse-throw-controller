using System.Runtime.InteropServices;

namespace Windows.Threading {
	
	#region Enum/Delegate

	/// <summary>
	/// 
	/// </summary>
	public enum ThreadPriority {
		Lowest=0,
		BelowNormal=1,
		Normal=2,
		AboveNormal=3,
		Highest=4,
	}
	
	/// <summary>
	/// 
	/// </summary>
	public delegate int PTHREAD_START_ROUTINE(
		System.IntPtr lpThreadParameter
	);

	#endregion Enum/Delegate

	/// <summary>
	/// 
	/// </summary>
	public class Thread {

		#region Natives

		/// <summary>
		/// 
		/// </summary>
		public const string
			LIB_KERNEL          ="kernel32.dll"
		;

		/// <summary>
		/// 
		/// </summary>
		[DllImport(LIB_KERNEL)]
		public static extern System.IntPtr CreateThread(
			int lpThreadAttributes,
			int dwStackSize,
			PTHREAD_START_ROUTINE lpStartAddress,
			System.IntPtr lpParameter,
			int dwCreationFlags,
			ref int lpThreadId
		);

		/// <summary>
		/// 
		/// </summary>
		[DllImport(LIB_KERNEL)]
		public static extern bool CloseHandle(
			System.IntPtr hObject
		);

		#endregion Natives

		#region Static

		protected static readonly System.IntPtr NULL_PTR=System.IntPtr.Zero;

		#endregion Static

		#region Fields

		public bool IsBackground;
		public ThreadPriority Priority;

		protected int m_ThreadId=-1;
		protected System.IntPtr m_NativeThread;
		protected PTHREAD_START_ROUTINE m_Routine;

		#endregion Fields

		#region Constructors

		/// <summary>
		/// 
		/// </summary>
		public Thread(PTHREAD_START_ROUTINE routine) {
			m_Routine=routine;
		}

		#endregion Constructors

		#region Methods

		/// <summary>
		/// 
		/// </summary>
		public virtual void Start() {
			if(m_NativeThread==NULL_PTR){
				m_NativeThread=CreateThread(0,0,m_Routine,NULL_PTR,0,ref m_ThreadId);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual void Abort() {
			if(m_NativeThread!=NULL_PTR){
				CloseHandle(m_NativeThread);
				m_NativeThread=NULL_PTR;
				Log.i("Windows.Threading.Thread","Thread is Aborted");
			}
		}

		#endregion Methods

	}
}

#if UNITY_EDITOR||UNITY_STANDALONE

using System.Collections.Generic;
using System.Runtime.InteropServices;
using Windows.Threading;

namespace Ximmerse.IO.Usb {

	/// <summary>
	/// 
	/// </summary>
	public class HidStream:UsbStreamBase {

		#region Static

		public static int s_NumHids=0;
		protected static bool s_Inited=false;

		/// <summary>
		/// 
		/// </summary>
		public static int InitLibrary(){
			if(!s_Inited){
				int ret=LibHidAPI.hid_init();
				if(ret==0)
					s_Inited=true;
				return ret;
			}
			return 0;
		}

		/// <summary>
		/// 
		/// </summary>
		public static int ExitLibrary(){
			//if(s_NumHids==0) {
			if(s_Inited){
				int ret=LibHidAPI.hid_exit();
				if(ret==0)
					s_Inited=false;
				return ret;
			}
			//}
			return 0;
		}

		/// <summary>
		/// 
		/// </summary>
		public static bool Exists(string path){
			return LibHidAPI.Exists(path);
		}
		
		/// <summary>
		/// 
		/// </summary>
		public static List<LibHidAPI.hid_device_info> EnumerateDevices(){
			return LibHidAPI.EnumerateDevices(0,0);
		}

		/// <summary>
		/// 
		/// </summary>
		public static List<LibHidAPI.hid_device_info> EnumerateDevices(int vId,int pId){
			return LibHidAPI.EnumerateDevices(vId,pId);
		}

		#endregion Static

		#region Fields

		public LibHidAPI.hid_device_info meta=new LibHidAPI.hid_device_info();
		protected System.IntPtr m_HidPtr;
		protected Thread m_Thread;
		protected int m_ThreadLock;

		#endregion  Fields

		#region Constructors
		
		/// <summary>
		/// 
		/// </summary>
		public HidStream(string address):base(address){
		}

		/// <summary>
		/// 
		/// <para>When dId==-1,open the first Hid device.</para>
		/// </summary>
		public HidStream(int vId,int pId,int dId=0):base(vId,pId,dId){
		}

		/// <summary>
		/// 
		/// <para>When dId==-1,open the first Hid device.</para>
		/// </summary>
		public HidStream(string address,int vId,int pId,int dId=0):base(address,vId,pId,dId){
		}

		#endregion Constructors

		#region Methods

		/// <summary>
		/// 
		/// </summary>
		public override void Open() {
			//InitLibrary();
			//
			if(m_IsOpen){
				Close();
			}
			//
			if(!string.IsNullOrEmpty(m_Address)){
				if(LibHidAPI.Exists(m_Address)){
					Open(m_Address);
					return;
				}
			}
			if(m_DID==-1){//???
				m_VID=m_PID=m_DID=0;
			}else{
			}
			Open(m_VID,m_PID,m_DID);
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual void Open(LibHidAPI.hid_device_info info){
			string path=info.path;//Marshal.PtrToStringAnsi()
			if(!string.IsNullOrEmpty(path)){
				Open(path);
				if(m_IsOpen){
					goto label_open_ok;
				}
			}
				Open(info.vendor_id,info.product_id,0);
				if(m_IsOpen){
					goto label_open_ok;
				}
			return;
label_open_ok:
			meta=info;
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual void Open(string i_path){
			if(m_IsOpen){
				Close();
			}
			m_HidPtr=LibHidAPI.hid_open_path(m_Address=i_path);
			if(m_HidPtr==LibHidAPI.INVALID_HANDLE_VALUE){
				if(m_OpenCallback!=null) {
					m_OpenCallback.OnStreamOpenFailure(this);
				}
			}else {
				m_IsOpen=true;s_NumHids++;
				if(m_OpenCallback!=null) {
					m_OpenCallback.OnStreamOpenSuccess(this);
				}
				// Open the read thread.
				if(m_ReadCallback!=null) {
					m_Thread=new Thread(ReadThread_Main);
					m_Thread.IsBackground=true;
					m_Thread.Priority=ThreadPriority.AboveNormal;
					m_Thread.Start();
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual void Open(int vId,int pId,int dId=0){
			var list=LibHidAPI.EnumerateDevices(vId,pId);
			if(dId<list.Count){
				Open(list[dId].path);
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		public override void Close(){
			// Forcibly close the read thread.
			if(m_Thread!=null) {
				++m_ThreadLock;
				m_Thread.Abort();
				m_Thread=null;
			}
			// Forcibly set callbacks to null to avoid accessing callbacks in the read thread.
			m_OpenCallback=null;
			m_ReadCallback=null;
			//
			if(m_IsOpen){
				if(m_HidPtr!=LibHidAPI.INVALID_HANDLE_VALUE) {
					LibHidAPI.hid_close(m_HidPtr);m_HidPtr=LibHidAPI.INVALID_HANDLE_VALUE;
				}
				s_NumHids--;//ExitLibrary();
				//
				m_IsOpen=false;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		protected virtual int ReadThread_Main(
			System.IntPtr lpThreadParameter
		) {
			//Log.i("Ximmerse.IO.Usb.HidStream","Start the read thread.");
			// Create the buffer.
			byte[] buffer=null;
			if(m_BufferRead==null||m_BufferRead.Length<m_SizeRead) {
				m_BufferRead=buffer=new byte[m_SizeRead];
			}else {
				buffer=m_BufferRead;
			}
			// Start the looper.
			int threadLock=++m_ThreadLock;
			while(threadLock==m_ThreadLock) {
				// Check the native pointer.
				if(m_HidPtr==LibHidAPI.INVALID_HANDLE_VALUE){goto label_read_thread_quit;}
				// Read the hid.
				m_SizeHasRead=ReadTimeout(buffer,0,m_SizeRead,5);
				// Invoke events.
				if(m_SizeHasRead>0){if(m_ReadCallback!=null) {
					m_ReadCallback.OnStreamRead(this);
				}}
			}
label_read_thread_quit:
			Log.i("Ximmerse.IO.Usb.HidStream","Abort the read thread...");
			return 0;
		}

		/// <summary>
		/// 
		/// </summary>
		public override int Read(byte[] buffer,int offset,int count){
			if(m_SizeHasRead<=0) return m_SizeHasRead=0;
			if(m_SizeHasRead<count) {// Get min size.
				count=m_SizeHasRead;
			}
			System.Array.Copy(m_BufferRead,0,buffer,offset,count);
			m_SizeHasRead=0;// ???
			return count;
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual int ReadTimeout(byte[] buffer,int offset,int count,int milliseconds){
			if(!m_IsOpen){
				return 0;
			}
			System.IntPtr ptr=Marshal.AllocHGlobal(count);// Alloc
			int ret=LibHidAPI.hid_read_timeout(m_HidPtr,ptr,count,milliseconds);// Do
			if(ret>0) {
				Marshal.Copy(ptr,buffer,offset,ret);// Copy
			}
			Marshal.FreeHGlobal(ptr);// Free
			return ret;
		}

		/// <summary>
		/// 
		/// </summary>
		public override int Write(byte[] buffer,int offset,int count){
			if(!m_IsOpen){
				return -1;
			}
			System.IntPtr ptr=Marshal.AllocHGlobal(count);// Alloc
			Marshal.Copy(buffer,offset,ptr,count);// Copy
			LibHidAPI.hid_write(m_HidPtr,ptr,count);// Do
			Marshal.FreeHGlobal(ptr);// Free
			return 0;
		}

		#endregion Methods

	}

}

#endif
#if !UNITY_EDITOR&&UNITY_ANDROID

using Ximmerse.IO;
using Ximmerse.IO.Usb;

namespace Ximmerse.Core {

	/// <summary>
	/// For Android building.
	/// </summary>
	public partial class LibXHawkAPI {

		#region Nested Types

		/// <summary>
		/// A proxy to convert managed buffer to X-Hawk library's read buffer.
		/// </summary>
		public class UsbProxy:IStreamReadCallback {

			public byte[] buffer;
			public int offset;
			public int count;

			/// <summary>
			/// 
			/// </summary>
			public void OnStreamRead(IStreamable stream) {
				stream.GetReadBuffer(out buffer,out offset,out count);
				//Log.i("LibXHawkAPI$UsbProxy",buffer.ToHexString(offset,count));
				XHawkSetReadBuffer(buffer,offset,count);
			}
		}

		#endregion Nested Types

		#region Static

		/// <summary>
		/// X-Hawk library name.
		/// </summary>
		public const string LIB_XHAWK="xhawk";

		public static UsbStreamBase s_UsbStream;
		public static UsbProxy s_UsbProxy;

		#endregion Static
	
		#region Managed

		/// <summary>
		/// Init library.
		/// </summary>
		public static int Init() {
			int ret=XHawkInit();
			if(s_UsbStream==null) {
				//
				s_UsbStream=new UsbStream(0x1F3B,0x10FF);
				s_UsbStream.readBufferSize=64;
				((UsbStream)s_UsbStream).SetInterfaceFilters(3,0,1);
				s_UsbStream.SetOnStreamReadListener(s_UsbProxy=new UsbProxy());
				s_UsbStream.Open();
			}
			return ret;
		}

		/// <summary>
		/// Exit library.
		/// </summary>
		public static int Exit() {
			int ret=XHawkExit();
			if(s_UsbStream!=null) {
				s_UsbStream.Close();
				s_UsbStream=null;
			}
			return ret;
		}

		#endregion Managed

	}

}

#endif
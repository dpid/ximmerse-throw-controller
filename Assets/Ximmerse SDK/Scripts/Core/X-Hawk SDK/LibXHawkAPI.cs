using System.Runtime.InteropServices;

namespace Ximmerse.Core {

	/// <summary>
	/// X-Hawk library API.
	/// </summary>
	public partial class LibXHawkAPI {

		#region Constants & Nested Types

#if UNITY_EDITOR || UNITY_STANDALONE

		/// <summary>
		/// X-Hawk library name.
		/// </summary>
		public const string LIB_XHAWK="X-Hawk";

#endif

		/// <summary>
		/// Use for read callback.
		/// </summary>
		public delegate void VoidDelegate();

		/// <summary>
		/// (Will be deprecated)
		/// </summary>
		protected static int[] dataframe_offset=new int[4]{0,1,0,1};

		/// <summary>
		/// The X-Cobra data in X-Hawk library.
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct joyinfo {
			// Flags
			public byte           id;
			public byte           found_mask;
			// Gamepad
			public float          joystick_x;
			public float          joystick_y;
			public float          trigger;
			public int            buttons;
			// Motion
			[MarshalAs(UnmanagedType.ByValArray,SizeConst=3)]
			public float[]        position;
			[MarshalAs(UnmanagedType.ByValArray,SizeConst=4)]
			public float[]        rotation;
			[MarshalAs(UnmanagedType.ByValArray,SizeConst=3)]
			public float[]        eulerAngles;

			public void Init() {
				position    = new float[3];
				rotation    = new float[4];
				eulerAngles = new float[3];
			}
		}

		#endregion Constants & Nested Types

		#region Natives
		
		/** Common **/
		[DllImport(LIB_XHAWK)]public static extern int XHawkInit();
		[DllImport(LIB_XHAWK)]public static extern int XHawkExit();

		[DllImport(LIB_XHAWK)]public static extern int XHawkOpen();
		[DllImport(LIB_XHAWK)]public static extern int XHawkClose();

		/** Read **/

		// Get the number of joysticks
		[DllImport(LIB_XHAWK)]public static extern int XHawkGetJoystickCount();
		// Get a joystick info from read cache
		[DllImport(LIB_XHAWK)]public static extern int XHawkGetJoystick(int index,ref joyinfo joystick);
		// Get all joystick infos from read cache
		[DllImport(LIB_XHAWK)]public static extern int XHawkGetAllJoysticks(joyinfo[] joysticks);

		// Get a newest joystick info
		[DllImport(LIB_XHAWK)]public static extern int XHawkGetNewestJoystick(int index,ref joyinfo joystick);
		// Get all newest joystick infos
		[DllImport(LIB_XHAWK)]public static extern int XHawkGetAllNewestJoysticks(joyinfo[] joysticks);

		/** Write **/
		// send message to joystick,reserved,not implemented yet
		[DllImport(LIB_XHAWK)]public static extern int XHawkSendMessage(int index,int Msg,int wParam,int lParam);

		/** Utilities **/

		[DllImport(LIB_XHAWK)]public static extern int XHawkGetReadBuffer(byte[] buffer,int offset,int count);
		[DllImport(LIB_XHAWK)]public static extern int XHawkSetReadBuffer(byte[] buffer,int offset,int count);
		[DllImport(LIB_XHAWK)]public static extern void XHawkBytesToEuler(float[] dest,byte[] src,int offset);
		
		/** Misc **/

		// set a read listener,if you use this function,use XHawkGetJoystick or XHawkGetAllJoysticks on callback function
		[DllImport(LIB_XHAWK)]public static extern int XHawkSetReadListener(VoidDelegate read_callback);
		// read the first frame if true,read last frame if false
		[DllImport(LIB_XHAWK)]public static extern int XHawkSetReadFirstFrame(bool is_first_frame);

		//[DllImport(LIB_XHAWK)]public static extern void XHawkParseDataframe(dataframe* dest, dataframe_raw* src);
		[DllImport(LIB_XHAWK)]public static extern void XHawkSetDataframeOffset(int[] value);
		[DllImport(LIB_XHAWK)]public static extern void XHawkSetPositionScale(float[] value);

		
		[DllImport(LIB_XHAWK,EntryPoint="XHawkBytesToEuler")]public static extern void BytesToEuler(float[] dest,byte[] src,int offset);
		
		#endregion Natives

		#region Managed

#if UNITY_EDITOR || UNITY_STANDALONE

		/// <summary>
		/// Init library.
		/// </summary>
		public static int Init() {
			int ret=XHawkInit();
			if(ret==0) {
				XHawkOpen();
				XHawkSetReadFirstFrame(false);
				XHawkSetDataframeOffset(dataframe_offset);
			}
			return ret;
		}

		/// <summary>
		/// Exit library.
		/// </summary>
		public static int Exit() {
			XHawkClose();
			int ret=XHawkExit();
			return ret;
		}

#endif

		/// <summary>
		/// Update managed data to the newest data.
		/// </summary>
		public static int Update(joyinfo[] joys) {
			int i=0;
			int ret=XHawkGetNewestJoystick(i,ref joys[i]);++i;
			if(ret==0) {
				ret=XHawkGetJoystick(i,ref joys[i]);
			}
			return ret;
		}

		#endregion Managed

		#region Will be deprecated

		/// <summary>
		/// (Will be deprecated)
		/// </summary>
		protected static void Swap(ref int a,ref int b) {
			int c=a;a=b;b=c;
		}
	
		/// <summary>
		/// (Will be deprecated)
		/// </summary>
		public static void SwapControllers(bool blob,bool ble) {
			int i=0;
			if(blob) {
				Swap(ref dataframe_offset[i],ref dataframe_offset[i+1]);
			}i+=2;
			if(ble){
				Swap(ref dataframe_offset[i],ref dataframe_offset[i+1]);
			}i+=2;
			//
			XHawkSetDataframeOffset(dataframe_offset);
		}

		#endregion Will be deprecated

	}

}
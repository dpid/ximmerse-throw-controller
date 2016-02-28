using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Ximmerse.IO.Usb {

	/// <summary>
	/// 
	/// </summary>
	public /*sealed*/ class LibHidAPI{

//#if SUPPORT_XHAWK_SDK
		public const string LIB_HID=Ximmerse.Core.LibXHawkAPI.LIB_XHAWK;
//#else
//		public const string LIB_HID="hidapi";
//#endif

		#region Native Methods

		/** hidapi info structure */
		public struct hid_device_info {

			/** Platform-specific device path */
			[MarshalAs(UnmanagedType.LPStr)]
			public string path;
			/** Device Vendor ID */
			public ushort vendor_id;
			/** Device Product ID */
			public ushort product_id;
			/** Serial Number */
			[MarshalAs(UnmanagedType.LPStr)]
			public string serial_number;
			/** Device Release Number in binary-coded decimal,
			    also known as Device Version Number */
			public ushort release_number;
			/** Manufacturer String */
			[MarshalAs(UnmanagedType.LPWStr)]
			public string manufacturer_string;
			/** Product String */
			[MarshalAs(UnmanagedType.LPWStr)]
			public string product_string;
			/** Usage Page for this Device/Interface
			    (Windows/Mac only). */
			public ushort usage_page;
			/** Usage for this Device/Interface
			    (Windows/Mac only).*/
			public ushort usage;
			/** The USB interface which this logical device
			    represents. Valid on both Linux implementations
			    in all cases, and valid on the Windows implementation
			    only if the device contains more than one interface. */
			public int interface_number;

			/** Pointer to the next device */
			public IntPtr next;
		};
		
		/// <summary>
		/// 
		/// </summary>
		[DllImport(LIB_HID,CallingConvention=CallingConvention.Cdecl)]
		public extern static int hid_init();

		/// <summary>
		/// 
		/// </summary>
		[DllImport(LIB_HID,CallingConvention=CallingConvention.Cdecl)]
		public extern static int hid_exit();

		/// <summary>
		/// 
		/// </summary>
		[DllImport(LIB_HID,CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr hid_enumerate(
			ushort vendor_id,
			ushort product_id
		);

		/// <summary>
		/// 
		/// </summary>
		[DllImport(LIB_HID,CallingConvention=CallingConvention.Cdecl)]
		public extern static void hid_free_enumeration(
			IntPtr devs
		);

		/// <summary>
		/// 
		/// </summary>
		[DllImport(LIB_HID,CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr hid_open(
			ushort vendor_id,
			ushort product_id,
			IntPtr serial_number
		);

		/// <summary>
		/// 
		/// </summary>
		[DllImport(LIB_HID,CallingConvention=CallingConvention.Cdecl)]
		public extern static IntPtr hid_open_path(
			IntPtr path
		);

		/// <summary>
		/// 
		/// </summary>
		[DllImport(LIB_HID,CallingConvention=CallingConvention.Cdecl)]
		public extern static void hid_close(
			IntPtr device
		);
		
		/// <summary>
		/// 
		/// </summary>
		[DllImport(LIB_HID,CallingConvention=CallingConvention.Cdecl)]
		public extern static int hid_read(
			IntPtr device,
			IntPtr data,
			int length
		);

		/// <summary>
		/// 
		/// </summary>
		[DllImport(LIB_HID,CallingConvention=CallingConvention.Cdecl)]
		public extern static int hid_read_timeout(
			IntPtr device,
			IntPtr data,
			int length,
			int milliseconds
		);

		/// <summary>
		/// 
		/// </summary>
		[DllImport(LIB_HID,CallingConvention=CallingConvention.Cdecl)]
		public extern static int hid_write(
			IntPtr device,
			IntPtr data,
			int length
		);

		#endregion Native Methods

		#region Static Fields

		/// <summary>
		/// 
		/// </summary>
		public static readonly IntPtr INVALID_HANDLE_VALUE=IntPtr.Zero;
		protected static Type s_TypeInfo=typeof(hid_device_info);

		#endregion Static Fields

		#region Static Methods

		/// <summary>
		/// 
		/// </summary>
		public static IntPtr hid_open_path(
			string path
		){
			IntPtr ptr_str=Marshal.StringToHGlobalAnsi(path);
			IntPtr device=hid_open_path(ptr_str);
			// Free ?
			return device;
		}
		
		/// <summary>
		/// 
		/// </summary>
		public static bool Exists(string path) {
			return  EnumerateDevices().FindIndex((x)=>(x.path==path))!=-1;
		}
		
		/// <summary>
		/// 
		/// </summary>
		public static List<hid_device_info> EnumerateDevices(int vid=0,int pid=0) {
			List<hid_device_info> ret=new List<hid_device_info>();
			IntPtr iterator=hid_enumerate((ushort)vid,(ushort)pid);
			hid_device_info info;
			while(iterator!=INVALID_HANDLE_VALUE) {
				info=(hid_device_info)Marshal.PtrToStructure(iterator,s_TypeInfo);
				ret.Add(info);
				iterator=info.next;
			}
			hid_free_enumeration(iterator);
			return ret;
		}

		#endregion Static Methods

	}

}
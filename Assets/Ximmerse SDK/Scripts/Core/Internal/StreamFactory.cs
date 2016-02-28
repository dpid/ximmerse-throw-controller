using Ximmerse.IO;
using Ximmerse.IO.Ports;

namespace Ximmerse.Core {

	/// <summary>
	/// 
	/// </summary>
	public class StreamFactory {
	
		/// <summary>
		/// 
		/// </summary>
		public static void GetStreamSize(string i_productName,ref int o_rawSize,ref int o_fixedSize){
			switch(i_productName){
				case "X-Cobra Beta 0":
					o_rawSize=10+3;o_fixedSize=10;
				break;
				case "X-Cobra Beta 2":
				case "X-Swift Alpha 0":
					o_rawSize=20;o_fixedSize=o_rawSize-3;
				break;
				case "X-Template 0":
					o_rawSize=8;o_fixedSize=o_rawSize-3;
				break;
				case "X-Cobra Alpha 0":
					o_rawSize=8;o_fixedSize=o_rawSize-3;
				break;
			}
		}
	
		/// <summary>
		/// 
		/// </summary>
		public static void GetStreamPos(string i_productName,ref int o_rawSize,ref int o_offset,ref int o_fixedSize){
			switch(i_productName){
				case "X-Cobra Alpha 0":
					o_offset    =          0;// Usb rips the frame header.
					o_rawSize   =         10;
					o_fixedSize =         10;
				break;
				case "X-Cobra Alpha 1":
				case "X-Swift Alpha 1":
					o_offset    =          1;
					o_rawSize   =         20;
					o_fixedSize =o_rawSize-3;
				break;
				default:
					Log.e("StreamFactory", "Unknown device@" + i_productName);
				break;
			}
		}
	
		/// <summary>
		/// 
		/// </summary>
		public static StreamUnpacker GetStreamUnpacker(string i_productName,IStreamable i_rawStream,int i_rawSize,int i_fixedSize){
			switch(i_productName){
				case "X-Cobra Beta 0":
				return new StreamUnpacker_v0 (i_rawSize,i_fixedSize).SetStream(i_rawStream/*,true*/);
				case "X-Cobra Beta 2":
				case "X-Template 0":
				case "X-Swift Alpha 0":
				return new StreamUnpacker_v0b(i_rawSize,i_fixedSize).SetStream(i_rawStream/*,true*/);
			}
			return null;
		}
	
		/// <summary>
		/// 
		/// </summary>
		public static IStreamable GetStream(string i_productName,int i_index,string i_address,out int o_fixedSize,out bool o_isDynamic){
			int rawSize=0,offset=0;
			//
			IStreamable stream=null;
			o_fixedSize=0;
			o_isDynamic=false;
			//
			switch(i_productName){
				case "X-Cobra Alpha 1":
				case "X-Swift Alpha 1":
					GetStreamPos(i_productName,ref rawSize,ref offset,ref o_fixedSize);
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
					stream=new SharedMemoryStream(i_address,rawSize,offset,o_fixedSize);
#elif UNITY_ANDROID
					// Firstly,read the bluetooth address from text file on Android platform.
					// You can download the bluetooth setting apk from :
					IniReader ir=IniReader.Open(Environment.SD_CARD_ROOT+"Ximmerse Runtime/Configs/Common.txt");
					if(ir==null) {
						Log.e("StreamFactory","No such file:"+Environment.SD_CARD_ROOT+"Ximmerse Runtime/Configs/Common.txt");
					}else{
						i_address=ir.TryParseString(i_address+"@Address",i_address);
					}
					Log.e("StreamFactory","i_address="+i_address);
					stream=new VirtualStream(
						new BleSerialPort(i_address),
						offset,o_fixedSize
					);
					o_isDynamic=true;
#endif
				break;
				case "X-Cobra Beta 0":
				case "X-Cobra Beta 2":
				case "X-Swift Alpha 0":
					GetStreamSize(i_productName,ref rawSize,ref o_fixedSize);
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
					BleSerialPort sp=new BleSerialPort(i_address);
#elif UNITY_ANDROID
					BleSerialPort sp=new BleSerialPort(i_address);
#endif
					StreamUnpacker unpacker=GetStreamUnpacker(i_productName,sp,rawSize,o_fixedSize);
					o_isDynamic=true;stream=unpacker;
				break;
				case "X-Cobra Alpha 0":
					if(XHawkServiceBase.main==null){
						Log.e("StreamFactory","Initialize XimBlobTracking before DeviceBase");
						return null;
					}
					XHawkService usbSrv=XHawkServiceBase.main as XHawkService;
					GetStreamPos(i_productName,ref usbSrv.bleReadRawSize,ref usbSrv.bleReadOffset,ref usbSrv.bleReadSize);
					usbSrv.UpdateStreamPos(i_index);
					o_fixedSize=usbSrv.bleReadSize;
					o_isDynamic =true;stream=usbSrv.bleStreams[i_index];
				break;
				default:
					Log.e("StreamFactory","Unknown device@"+i_productName);
				break;
			}
			return stream;
		}

	}

}
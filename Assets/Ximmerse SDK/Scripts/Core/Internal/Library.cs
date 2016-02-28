
#if UNITY_EDITOR_WIN||UNITY_STANDALONE_WIN
using Csr.Bluetooth;
#elif UNITY_ANDROID
#endif

using UnityEngine;
using Ximmerse.IO;
using Ximmerse.IO.Usb;

namespace Ximmerse.Core {


	/// <summary>
	/// 
	/// </summary>
	public class Library {
		
		/// <summary>
		/// 
		/// </summary>
		public static bool s_IsInited=false;
		
		/// <summary>
		/// 
		/// </summary>
		public static void Init(){
			if(s_IsInited){
				return;
			}
			//
			switch(Application.platform){
				case RuntimePlatform.Android:
					// TODO:It needs to dump ini files on Android.
					Log.i("Library","Try to dump ini files on Android.");
					try{
						FileSystem.CopyResourcesToDirectory("",Environment.CONFIG_PATH,false);
					} catch(System.Exception e) {
						Log.w("Library",e.ToString()+"\nYou need create \""+Environment.CONFIG_PATH+"\" by yourself.");
					}
				break;
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
				case RuntimePlatform.WindowsEditor:
				case RuntimePlatform.WindowsPlayer:
					Environment.AddEnvironmentVariable("PATH",Environment.PLUGIN_PATH);
					HidStream.InitLibrary();
				break;
#endif
			}
			//
			s_IsInited=true;
		}
	
		/// <summary>
		/// 
		/// </summary>
		public static void Exit(){
			if(!s_IsInited){
				return;
			}
			switch(Application.platform){
				case RuntimePlatform.Android:
				break;
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
				case RuntimePlatform.WindowsEditor:
				case RuntimePlatform.WindowsPlayer:
					HidStream.ExitLibrary();
					BluetoothAdapter.CloseDefaultAdapter();
				break;
#endif
			}
			s_IsInited=false;
		}
	}

}
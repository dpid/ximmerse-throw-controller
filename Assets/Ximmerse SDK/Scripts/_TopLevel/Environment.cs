#if !WINDOWS_X86&&!WINDOWS_X64
#define WINDOWS_X86
#endif

namespace Ximmerse{

	/// <summary>
	/// 
	/// </summary>
	public class Environment {

		#region Const

		public static readonly string
			SDK_PATH=UnityEngine.Application.dataPath+"/Ximmerse SDK/"
			,
#if !UNITY_EDITOR && UNITY_ANDROID
			SD_CARD_ROOT=GetSDPath(),
#endif
			PLUGIN_PATH=UnityEngine.Application.dataPath+"/Plugins/"
#if UNITY_EDITOR
	#if WINDOWS_X86
			+"x86/"
	#elif WINDOWS_X64
			+"x86_64/"
	#endif
#endif
			,
			CONFIG_PATH=
#if UNITY_EDITOR
			SDK_PATH+"Resources/Configs/"
#elif UNITY_STANDALONE_WIN
			UnityEngine.Application.streamingAssetsPath+"/Configs/"
#elif UNITY_ANDROID
			SD_CARD_ROOT+"Ximmerse Runtime/Configs/"+Android.App.Activity.currentActivity.GetPackageName()+"/"
#endif
		;
		
		#endregion Const

		#region Static

		/// <summary>
		/// 
		/// </summary>
		public static string GetSDPath() {
#if !UNITY_EDITOR && UNITY_ANDROID
			using(var c=new UnityEngine.AndroidJavaClass("android.os.Environment")) {
				using(var i=c.CallStatic<UnityEngine.AndroidJavaObject>("getExternalStorageDirectory")) {
					return i.Call<string>("getPath")+"/";
				}
			}
#else
			return null;
#endif
		}

		/// <summary>
		/// 
		/// </summary>
		public static void AddEnvironmentVariable(string variable, string value){
			string str=System.Environment.GetEnvironmentVariable(variable);
			if(str.IndexOf(value)==-1){
				System.Environment.SetEnvironmentVariable(variable,str+";"+value);
			}
		}
		
		#endregion Static

	}
}
using UnityEngine;
using System.IO;

namespace Ximmerse.IO {

	/// <summary>
	/// 
	/// </summary>
	public class FileSystem {

		#region Unity Extension

		/// <summary>
		/// 
		/// </summary>
		public static void CopyResourcesToDirectory(string source,string destination,bool overwrite=true) {
			//
			Object[] assets=Resources.LoadAll(source,typeof(TextAsset));
			if(assets==null||assets.Length==0) {
				throw new IOException("No such file or directory");
				//return;
			}
			//
			if(!Directory.Exists(destination)){
				Directory.CreateDirectory(destination);
			}
			if(!destination.EndsWith("\\")&&!destination.EndsWith("/")) {
				destination=destination+"/";
			}
			//
			TextAsset ta;
			string fileName;
			for(int i=0,imax=assets.Length;i<imax;++i) {ta=assets[i] as TextAsset;
				if(ta!=null) {
					fileName=destination+ta.name+".txt";
					if(overwrite||!File.Exists(fileName)) {
						File.WriteAllBytes(fileName,ta.bytes);
					}
				}
			}
		}

		#endregion Unity Extension

		#region System.IO Extension

		/// <summary>
		/// 
		/// </summary>
		public static void CopyDirectory(string source,string destination) {
			if (destination.StartsWith(source.EndsWith("\\")?source:(source+"\\"),System.StringComparison.CurrentCultureIgnoreCase)) {
				throw new IOException("You can't copy parent directory to child directory");
			}
			//
			CopyDirectory(new DirectoryInfo(source),new DirectoryInfo(destination));
		}

		/// <summary>
		/// See : http://www.java2s.com/Tutorial/CSharp/0300__File-Directory-Stream/CopyDirectory.htm
		/// </summary>
		protected static void CopyDirectory(DirectoryInfo source,DirectoryInfo destination) {
			//
			if(!source.Exists) {
				return;
			}
			if(!destination.Exists) {
				destination.Create();
			}
			//
			string destinationFullName=destination.FullName+@"\";
			// Copy all files.
			FileInfo[] files=source.GetFiles();
			for(int i=0,imax=files.Length;i<imax;++i) {
				if(files[i].Extension.ToLower()!=".meta") {// Don't copy *.meta files in Unity3D Engine.
					files[i].CopyTo(destinationFullName+files[i].Name,true);
				}
			}
			// Process subdirectories.
			DirectoryInfo[] dirs=source.GetDirectories();
			for(int i=0,imax=dirs.Length;i<imax;++i) {
				// Call CopyDirectory() recursively.
				CopyDirectory(dirs[i],new DirectoryInfo(destinationFullName+dirs[i].Name));
			}
		}

		#endregion System.IO Extension

	}
}

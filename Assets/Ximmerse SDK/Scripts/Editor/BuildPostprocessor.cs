using UnityEditor;
using UnityEditor.Callbacks;
using Ximmerse.IO;

namespace Ximmerse.MyEditor {

	public class BuildPostprocessor {

		[PostProcessBuild(1)]
		public static void OnPostprocessBuild(BuildTarget target,string pathToBuiltProject) {
			switch(target) {
				case BuildTarget.Android:
					//Log.i("BuildPostprocessor","Android will dump configuration files from \""+Environment.CONFIG_PATH+"\"");
				break;
				case BuildTarget.StandaloneWindows:
				case BuildTarget.StandaloneWindows64:
					//
					string configPath=Environment.CONFIG_PATH;
					string targetPath=pathToBuiltProject.Replace(".exe","_Data/StreamingAssets/Configs");

					FileSystem.CopyDirectory(configPath,targetPath);
					Log.i("BuildPostprocessor","FileSystem.CopyDirectory("+configPath+", "+targetPath+")");
				break;
			}
		}
	}

}

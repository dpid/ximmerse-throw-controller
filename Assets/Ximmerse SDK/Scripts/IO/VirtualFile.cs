using System.IO;
using UnityEngine;

namespace Ximmerse.IO {

	/// <summary>
	/// 
	/// </summary>
#if UNITY_5 && !UNITY_5_0
	[CreateAssetMenu(fileName="New Virtual File",menuName="Ximmerse SDK/Virtual File",order=800)]
#endif
	public class VirtualFile:ScriptableObject {

		/// <summary>
		/// 
		/// </summary>
		public enum Type:byte{
			PlayerPrefs           =1<<0,
			Resources             =1<<1,
			FileSystem            =1<<2,
			FileSystem_Resources  =Type.FileSystem|Type.Resources
		}
		
		public static readonly string[]
			SPLIT_LINE=new string[3]{"\r","\n","\r\n"};

		public Type m_Type=Type.FileSystem_Resources;
		public string m_Path;
		public TextAsset m_TextAsset;

		[System.NonSerialized]protected string m_Text=null;
		[System.NonSerialized]protected string[] m_Lines;

		/// <summary>
		/// 
		/// </summary>
		public virtual string ReadAllText(){
#if UNITY_EDITOR
			if(!UnityEditor.EditorApplication.isPlaying){
				m_Text=null;
			}
#endif
			if(string.IsNullOrEmpty(m_Text)) {

				#region ReadAllText

				switch(m_Type){
					case Type.PlayerPrefs:
						if(m_TextAsset!=null){
							m_Text=PlayerPrefs.GetString(m_Path,m_TextAsset.text);
						}else{
							m_Text=PlayerPrefs.GetString(m_Path,"");
						}
					break;
					case Type.Resources:
						if(m_TextAsset!=null){
							m_Text=m_TextAsset.text;
						}
					break;
					case Type.FileSystem:
						if(File.Exists(m_Path)){
							m_Text=File.ReadAllText(m_Path);
						}
					break;
					case Type.FileSystem_Resources:
						if(File.Exists(m_Path)){
							m_Text=File.ReadAllText(m_Path);
						}else if(m_TextAsset!=null){
							m_Text=m_TextAsset.text;
						}
					break;
				}

				#endregion ReadAllText

			}
			return m_Text;
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual string[] ReadAllLines(){
#if UNITY_EDITOR
			if(!UnityEditor.EditorApplication.isPlaying){
				m_Lines=null;
			}
#endif
			if(m_Lines==null) {

				#region ReadAllLines

				switch(m_Type){
					case Type.PlayerPrefs:
						m_Lines=ReadAllText().Split(SPLIT_LINE,System.StringSplitOptions.RemoveEmptyEntries);
					break;
					case Type.Resources:
						m_Lines=ReadAllText().Split(SPLIT_LINE,System.StringSplitOptions.RemoveEmptyEntries);
					break;
					case Type.FileSystem:
						if(File.Exists(m_Path)){
							m_Lines=File.ReadAllLines(m_Path);
						}
					break;
					case Type.FileSystem_Resources:
						if(File.Exists(m_Path)){
							m_Lines=File.ReadAllLines(m_Path);
						}else if(m_TextAsset!=null){
							m_Lines=ReadAllText().Split(SPLIT_LINE,System.StringSplitOptions.RemoveEmptyEntries);
						}
					break;
				}

				#endregion ReadAllLines

			}
			return m_Lines;
		}
	}
}

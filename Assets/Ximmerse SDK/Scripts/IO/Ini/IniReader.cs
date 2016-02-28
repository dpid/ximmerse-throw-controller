//REF : http://en.wikipedia.org/wiki/INI_file

#if !INI_CUSTOM
#define INI_SIMPLE
#endif

using System.Collections.Generic;
using System.IO;

namespace Ximmerse.IO{

	/// <summary>
	/// 读取一个*.ini文件.
	/// </summary>
	public partial class IniReader:CustomReader{

		public static bool useTrim=true;

		public static Dictionary<string,IniReader> s_Pool=
			new Dictionary<string,IniReader>();
	
		/// <summary>
		/// 
		/// </summary>
		public static IniReader Open(string i_path){
			if(s_Pool.ContainsKey(i_path)){
				return s_Pool[i_path];
			}
			//
			if(!File.Exists(i_path)){
				Log.e("IniReader","No such file or directory : "+i_path);
				return null;
			}
			//
			IniReader r=new IniReader(File.ReadAllBytes(i_path));
			r.Parse();
			s_Pool.Add(i_path,r);
			return r;
		}

		#region Symbol

		public static readonly char[] SPLIT_EQUALS=new char[1]{'='};
		public const char SEC_BEGIN='[',SEC_END=']',SEC_DOT='@',INI_COM=';';

		#endregion//Symbol

		public Dictionary<string,string> dic;
	
		public IniReader(byte[] i_bytes):base(i_bytes){
		}

		/// <summary>
		/// 
		/// </summary>
		public override void Parse(){

#if INI_SIMPLE
			string[] data;
			string line,sec="";
			int p;
			if(dic==null) dic=new Dictionary<string,string>();
		
			while(canRead){
				line=ReadLine(true);
			
				if(string.IsNullOrEmpty(line)) continue;

				switch(line[0]){
					case SEC_BEGIN:
						p=line.LastIndexOf(SEC_END);// -1->ERROR 1->EMPTY
						sec=(p<=1)?"":(line.Substring(1,p-1)+SEC_DOT);
						//UnityEngine.Debug.Log(p.ToString()+","+sec);
					break;
					case INI_COM:
						//continue;
					break;
					default:
						data=line.Split(SPLIT_EQUALS);
						if(data.Length<2) {
							Log.w("IniReader",line);
							continue;
						}
						//try{
						if(useTrim){
							dic.Add(sec+data[0].Trim(),data[1].Trim().Replace("\\n","\n"));
						}else{
							dic.Add(sec+data[0],data[1].Replace("\\n","\n"));
						}
						//}catch(System.Exception e) {
						//	Log.e("IniReader",e+"\n"+sec+data[0]+","+data[1]);
						//}
					break;
				}
			}
#endif

		}

		public string this[string key]{
			get{
				if(dic==null||!dic.ContainsKey(key))
					return "";
				else
					return dic[key];
			}
		}

	}
}
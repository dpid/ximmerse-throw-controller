
using UnityEngine;
namespace Ximmerse.IO{

	// 分析字符串的实现...
	public partial class IniReader {

		#region Simple

		/// <summary>
		/// 
		/// </summary>
		public virtual string TryParseString(string key,string dval){
			if(dic.ContainsKey(key)){
			key=dic[key];
			if(!string.IsNullOrEmpty(key)){
				return key;
			}
			}
			return dval;
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual int TryParseInt(string key,int dval){
			if(dic.ContainsKey(key)){
			key=dic[key];
			if(!string.IsNullOrEmpty(key)){
				return int.Parse(key);
			}
			}
			return dval;
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual int TryParseHex(string key,int dval){
			if(dic.ContainsKey(key)){
			key=dic[key];
			if(!string.IsNullOrEmpty(key)){
				return System.Convert.ToInt32(key,16);
			}
			}
			return dval;
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual float TryParseFloat(string key,float dval){
			if(dic.ContainsKey(key)){
			key=dic[key];
			if(!string.IsNullOrEmpty(key)){
				return float.Parse(key);
			}
			}
			return dval;
		}

		#endregion Simple

		#region Array

		/// <summary>
		/// 
		/// </summary>
		public virtual string[] TryParseStringArray(string key,string[] dval){// RAW
			if(dic.ContainsKey(key)){
			key=dic[key];
			if(!string.IsNullOrEmpty(key)){
				return key.Split(SPLIT_CSV);
			}
			}
			return dval;
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual byte[] TryParseHexArray(string key,byte[] dval){// RAW
			if(dic.ContainsKey(key)){
			key=dic[key];
			if(!string.IsNullOrEmpty(key)){
				string[] csv=key.Split(SPLIT_CSV);int i=csv.Length;
				var ret=new byte[i];
				while(i-->0) ret[i]=System.Convert.ToByte(csv[i],16);
				return ret;
			}
			}
			return dval;
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual short[] TryParseHexArray(string key,short[] dval){// COPY
			if(dic.ContainsKey(key)){
			key=dic[key];
			if(!string.IsNullOrEmpty(key)){
				string[] csv=key.Split(SPLIT_CSV);int i=csv.Length;
				var ret=new short[i];
				while(i-->0) ret[i]=System.Convert.ToInt16(csv[i],16);
				return ret;
			}
			}
			return dval;
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual int[] TryParseHexArray(string key,int[] dval){// COPY
			if(dic.ContainsKey(key)){
			key=dic[key];
			if(!string.IsNullOrEmpty(key)){
				string[] csv=key.Split(SPLIT_CSV);int i=csv.Length;
				var ret=new int[i];
				while(i-->0) ret[i]=System.Convert.ToInt32(csv[i],16);
				return ret;
			}
			}
			return dval;
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual int[] TryParseIntArray(string key,int[] dval){// Copy
			if(dic.ContainsKey(key)){
			key=dic[key];
			if(!string.IsNullOrEmpty(key)){
				string[] csv=key.Split(SPLIT_CSV);int i=csv.Length;
				var ret=new int[i];
				while(i-->0) ret[i]=int.Parse(csv[i]);
				return ret;
			}
			}
			return dval;
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual float[] TryParseFloatArray(string key,float[] dval){// RAW
			if(dic.ContainsKey(key)){
			key=dic[key];
			if(!string.IsNullOrEmpty(key)){
				string[] csv=key.Split(SPLIT_CSV);int i=csv.Length;
				var ret=new float[i];
				while(i-->0) ret[i]=float.Parse(csv[i]);
				return ret;
			}
			}
			return dval;
		}

		#endregion Array

		#region UnityEngine

		public static readonly char[] SPLIT_CSV=new char[1]{','};
		
		/// <summary>
		/// 
		/// </summary>
		public virtual Vector4 TryParseVector4(string key,Vector4 dval){//RAW
			if(dic.ContainsKey(key)){
			key=dic[key];
			if(!string.IsNullOrEmpty(key)){
				string[] csv=key.Split(SPLIT_CSV);
				int i=0;
				return new Vector4(
					float.Parse(csv[i++]),
					float.Parse(csv[i++]),
					float.Parse(csv[i++]),
					float.Parse(csv[i++])//,
				);
			}
			}
			return dval;
		}

		#region Token

		/// <summary>
		/// 
		/// </summary>
		public virtual Vector3 TryParseVector3(string key,Vector3 dval){//COPY
			if(dic.ContainsKey(key)){
			key=dic[key];
			if(!string.IsNullOrEmpty(key)){
				string[] csv=key.Split(SPLIT_CSV);
				int i=0;
				return new Vector3(
					float.Parse(csv[i++]),
					float.Parse(csv[i++]),
					float.Parse(csv[i++])//,
				);
			}
			}
			return dval;
		}
		
		/// <summary>
		/// 
		/// </summary>
		public virtual Vector2 TryParseVector2(string key,Vector2 dval){//COPY
			if(dic.ContainsKey(key)){
			key=dic[key];
			if(!string.IsNullOrEmpty(key)){
				string[] csv=key.Split(SPLIT_CSV);
				int i=0;
				return new Vector2(
					float.Parse(csv[i++]),
					float.Parse(csv[i++])//,
				);
			}
			}
			return dval;
		}

		#endregion Token

		#region Token

		/// <summary>
		/// 
		/// </summary>
		public virtual Quaternion TryParseQuaternion(string key,Quaternion dval){//COPY
			if(dic.ContainsKey(key)){
			key=dic[key];
			if(!string.IsNullOrEmpty(key)){
				string[] csv=key.Split(SPLIT_CSV);
				int i=0;
				return new Quaternion(
					float.Parse(csv[i++]),
					float.Parse(csv[i++]),
					float.Parse(csv[i++]),
					float.Parse(csv[i++])//,
				);
			}
			}
			return dval;
		}
		
		/// <summary>
		/// 
		/// </summary>
		public virtual Rect TryParseRect(string key,Rect dval){//COPY
			if(dic.ContainsKey(key)){
			key=dic[key];
			if(!string.IsNullOrEmpty(key)){
				string[] csv=key.Split(SPLIT_CSV);
				int i=0;
				return new Rect(
					float.Parse(csv[i++]),
					float.Parse(csv[i++]),
					float.Parse(csv[i++]),
					float.Parse(csv[i++])//,
				);
			}
			}
			return dval;
		}
		
		/// <summary>
		/// 
		/// </summary>
		public virtual Color TryParseColor(string key,Color dval){//COPY
			if(dic.ContainsKey(key)){
			key=dic[key];
			if(!string.IsNullOrEmpty(key)){
				string[] csv=key.Split(SPLIT_CSV);
				int i=0;
				return new Color(
					float.Parse(csv[i++]),
					float.Parse(csv[i++]),
					float.Parse(csv[i++]),
					float.Parse(csv[i++])//,
				);
			}
			}
			return dval;
		}

		#endregion Token
		
		#endregion UnityEngine

	}
}
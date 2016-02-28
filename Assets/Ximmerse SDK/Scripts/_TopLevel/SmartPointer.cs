using System.Collections.Generic;

namespace Ximmerse{

	/// <summary>
	/// 
	/// </summary>
	public class SmartPointer {

		public List<object> refObjs=new List<object>();

		/// <summary>
		/// 
		/// </summary>
		public virtual void AddRef(object obj){
			int i=refObjs.IndexOf(obj);
			if(i==-1){
				refObjs.Add(obj);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual void RemoveRef(object obj){
			int i=refObjs.IndexOf(obj);
			if(i!=-1){
				refObjs.RemoveAt(i);
			}
		}
	
	}
}

using UnityEngine;
using UnityEngine.UI;

namespace Ximmerse.UI {
	
	/// <summary>
	/// 
	/// </summary>
	public class UIVector3Field:MonoBehaviour {

		#region Fields

		[Header("Data")]
		public string m_Field="";
		public string m_Format="0.000";
		public Vector3 m_Value=Vector3.zero;
		[Header("UI")]
		public Text m_Label;
		public InputField[] m_Texts=new InputField[3];
		//public Text[] m_Texts=new Text[3];

		#endregion Fields

		#region Functions

		protected virtual void Start(){
			Refresh();
		}

		public virtual string field{
			get{
				return m_Field;
			}
			set{
				if(value==m_Field){
					return;
				}
				m_Field=value;
				Refresh();
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual Vector3 value{
			get{
				return m_Value;
			}
			set{
				if(value==m_Value){
					return;
				}
				m_Value=value;
				Refresh();
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual void Refresh(){
			m_Label.text=m_Field;
			int i=3;
			while(i-->0){
				m_Texts[i].text=m_Value[i].ToString(m_Format);
			}
		}

		#endregion Functions

	}

}
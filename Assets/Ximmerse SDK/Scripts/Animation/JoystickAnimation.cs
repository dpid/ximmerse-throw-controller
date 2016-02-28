using UnityEngine;
using Ximmerse.CrossInput;

namespace Ximmerse.Animation {

	/// <summary>
	/// 
	/// </summary>
	public class JoystickAnimation:MonoBehaviour {

		#region Nested Types

		/// <summary>
		/// 
		/// </summary>
		[System.Serializable]
		public class AxisEntry{

			public string[] keys=new string[2];
			[SerializeField]private VirtualAxis[] m_VirtualAxes=new VirtualAxis[2];

			public Transform target;
			public int[] axesRemap=new int[2]{0,2};
			public Vector3 center=Vector3.zero,
				           one=new Vector3( 1.0f,0.0f, 1.0f);

			/// <summary>
			/// 
			/// </summary>
			public void Start(string i_fmt){
				int i=keys.Length;
				while(i-->0){
					m_VirtualAxes[i]=CrossInputManager.VirtualAxisReference(this,string.Format(i_fmt,keys[i]));
				}
				Update();
			}

			/// <summary>
			/// 
			/// </summary>
			public void Update() {
				if(target==null) return;
				//
				Vector3 pos=target.localPosition;
				for(int aId,i=0,imax=keys.Length;i<imax;++i){aId=axesRemap[i];
					pos[aId]=center[aId]+one[aId]*m_VirtualAxes[i].GetValue;
				}
				target.localPosition=pos;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		[System.Serializable]
		public class ButtonEntry{
			public string key;
			private VirtualButton m_VirtualButton;

			[Header("Position")]
			public Transform target;
			public int axesRemap=0;
			public float from=0.0f,
				         to  =0.0f;
			[System.NonSerialized]public float t;

			/// <summary>
			/// 
			/// </summary>
			public void Start(string i_fmt){
				m_VirtualButton=CrossInputManager.VirtualButtonReference(this,string.Format(i_fmt,key));
				Update();
			}

			/// <summary>
			/// 
			/// </summary>
			public void Update() {
				if(target==null) return;
				//
				if(m_VirtualButton.GetButtonDown){// Tween ????
					t=1.0f;
				}else
				if(m_VirtualButton.GetButtonUp){// Tween ????
					t=0.0f;
				}
				//
				Vector3 pos=target.localPosition;
				pos[axesRemap]=Mathf.Lerp(from,to,t);
				target.localPosition=pos;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		[System.Serializable]
		public class TriggerEntry{
			public string key;
			private VirtualAxis m_VirtualAxis;

			[Header("EulerAngles")]
			public Transform target;
			public Vector3 from=Vector3.zero,
				           to  =Vector3.zero;

			/// <summary>
			/// 
			/// </summary>
			public void Start(string i_fmt){
				m_VirtualAxis=CrossInputManager.VirtualAxisReference(this,string.Format(i_fmt,key));
				Update();
			}

			/// <summary>
			/// 
			/// </summary>
			public void Update() {
				if(target==null) return;
				//
				target.localRotation=Quaternion.Euler
					(Vector3.Lerp(from,to,m_VirtualAxis.GetValue));
			}
		}

		#endregion Nested Types

		#region Fields
		
		[SerializeField]protected string m_Format="{0}";
		[SerializeField]protected AxisEntry[] m_Axes=new AxisEntry[0];
		[SerializeField]protected ButtonEntry[] m_Buttons=new ButtonEntry[0];
		[SerializeField]protected TriggerEntry[] m_Triggers=new TriggerEntry[0];
		
		#endregion Fields

		#region Unity Messages

		/// <summary>
		/// 
		/// </summary>
		protected virtual void Start() {
			int i,imax;
			for(i=0,imax=m_Axes.Length;i<imax;++i) {
				m_Axes[i].Start(m_Format);
			}
			for(i=0,imax=m_Buttons.Length;i<imax;++i) {
				m_Buttons[i].Start(m_Format);
			}
			for(i=0,imax=m_Triggers.Length;i<imax;++i) {
				m_Triggers[i].Start(m_Format);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		protected virtual void Update() {
			int i,imax;
			for(i=0,imax=m_Axes.Length;i<imax;++i) {
				m_Axes[i].Update();
			}
			for(i=0,imax=m_Buttons.Length;i<imax;++i) {
				m_Buttons[i].Update();
			}
			for(i=0,imax=m_Triggers.Length;i<imax;++i) {
				m_Triggers[i].Update();
			}
		}
		
		#endregion Unity Messages
	
	}

}
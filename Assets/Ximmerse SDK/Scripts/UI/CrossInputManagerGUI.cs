using UnityEngine;
using UnityEngine.UI;
using Ximmerse.CrossInput;

namespace Ximmerse.UI {

	/// <summary>
	/// 
	/// </summary>
	public class CrossInputManagerGUI:MonoBehaviour {

		#region Enum/Struct

		/// <summary>
		/// 
		/// </summary>
		public enum AxisType:byte{
			TYPE_1D,
			TYPE_2D
		}

		/// <summary>
		/// 
		/// </summary>
		[System.Serializable]
		public class AxisEntry{
			public AxisType type=AxisType.TYPE_1D;
			public string[] keys=new string[2];
			private VirtualAxis[] m_VirtualAxes=new VirtualAxis[2];
			[SerializeField]private Slider m_Slider=null;
			[SerializeField]private RectTransform m_Thumb=null;
			[SerializeField]private Vector3 m_Center=Vector3.zero;
			[SerializeField]private float m_Radius=128f;

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
			public void Update(){
				switch(type){
					case AxisType.TYPE_1D:
						m_Slider.value=m_VirtualAxes[0].GetValue;
					break;
					case AxisType.TYPE_2D:
						m_Thumb.localPosition=
							m_Center+
							new Vector3(
								m_Radius*m_VirtualAxes[0].GetValue,
								m_Radius*m_VirtualAxes[1].GetValue,
								0.0f
							);
					break;
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		[System.Serializable]
		public class ButtonEntry{
			public string key;
			private VirtualButton m_VirtualButton;
			[SerializeField]private Button m_Button=null;
			private Graphic m_Graphic=null;
			[SerializeField]private Color m_Color=Color.red;
			private Color m_ColorOri=Color.red;
			//private PointerEventData eventData=new PointerEventData(EventSystem.current);

			/// <summary>
			/// 
			/// </summary>
			public void Start(string i_fmt){
				m_VirtualButton=CrossInputManager.VirtualButtonReference(this,string.Format(i_fmt,key));
				m_Graphic=m_Button.GetComponent<Graphic>();
				m_ColorOri=m_Graphic.color;
				Update();
			}

			/// <summary>
			/// 
			/// </summary>
			public void Update(){
				if(m_VirtualButton.GetButtonDown){
					//Log.i("CrossInputManagerGUI",m_VirtualButton.name);
					//m_Button.OnPointerDown(s_EventData);
					m_Graphic.color=m_Color;
				}else
				if(m_VirtualButton.GetButtonUp){
					//m_Button.OnPointerUp(s_EventData);
					m_Graphic.color=m_ColorOri;
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		[System.Serializable]
		public class PoseEntry{
			public string key;
			private VirtualPose m_VirtualPose;
			[SerializeField]private UIVector3Field m_Position=null,m_Rotation=null;
			
			/// <summary>
			/// 
			/// </summary>
			public void Start(string i_fmt){
				m_VirtualPose=CrossInputManager.VirtualPoseReference(this,string.Format(i_fmt,key));
				Update();
			}

			/// <summary>
			/// 
			/// </summary>
			public void Update(){
				//Log.i("CrossInputManagerGUI",m_VirtualPose.position.ToString("0.000"));
				if(m_Position!=null) m_Position.value=m_VirtualPose.position;
				if(m_Rotation!=null) m_Rotation.value=m_VirtualPose.rotation.eulerAngles;
			}
		}

		[System.Serializable]
		public class JoystickEntry{
			[SerializeField]private string m_Format="J0_{0}";
			[SerializeField]private AxisEntry[] m_Axes=new AxisEntry[0];
			[SerializeField]private ButtonEntry[] m_Buttons=new ButtonEntry[0];
			[SerializeField]private PoseEntry[] m_Poses=new PoseEntry[0];

			/// <summary>
			/// 
			/// </summary>
			public void Start(){
				int i,imax;
				for(i=0,imax=m_Axes.Length;i<imax;++i){
					m_Axes[i].Start(m_Format);
				}
				for(i=0,imax=m_Buttons.Length;i<imax;++i){
					m_Buttons[i].Start(m_Format);
				}
				for(i=0,imax=m_Poses.Length;i<imax;++i){
					m_Poses[i].Start(m_Format);
				}
				Update();
			}

			/// <summary>
			/// 
			/// </summary>
			public void Update(){
				int i,imax;
				for(i=0,imax=m_Axes.Length;i<imax;++i){
					m_Axes[i].Update();
				}
				for(i=0,imax=m_Buttons.Length;i<imax;++i){
					m_Buttons[i].Update();
				}
				for(i=0,imax=m_Poses.Length;i<imax;++i){
					m_Poses[i].Update();
				}
			}
		}

		#endregion Enum/Struct

		#region Fields

		public static /*readonly*/UnityEngine.EventSystems.PointerEventData s_EventData=
			new UnityEngine.EventSystems.PointerEventData(UnityEngine.EventSystems.EventSystem.current);

		public JoystickEntry[] m_Joysticks=new JoystickEntry[2];

		#endregion Fields

		#region Unity Messages

		protected virtual void Start(){
			for(int i=0,imax=m_Joysticks.Length;i<imax;++i){
				m_Joysticks[i].Start();
			}
		}

		protected virtual void Update(){
			for(int i=0,imax=m_Joysticks.Length;i<imax;++i){
				m_Joysticks[i].Update();
			}
		}

		#endregion Unity Messages
	
	}

}
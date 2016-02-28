using UnityEngine;

namespace Ximmerse.Core {

	#region Enum

	/// <summary>
	/// Hand the X-Cobra controller is bound to.
	/// </summary>
	public enum XCobraHands:int{
		UNKNOWN = -1,
		LEFT ,
		RIGHT ,
	}

	/// <summary>
	/// X-Cobra controller state mask.
	/// </summary>
	public enum XCobraState:int{
		NOT_FOUND       =0,
		POSITION_FOUND  =1,
		ROTATION_FOUND  =2,
		POSE_FOUND      =3,
	}

	/// <summary>
	/// X-Cobra controller axis index.
	/// </summary>
	public enum XCobraAxis:int{
		JoystickX       =0,
		JoystickY       =1,
		Trigger         =2
	}
	
	/// <summary>
	/// X-Cobra controller button mask.
	/// </summary>
	/// <remarks>
	/// The TRIGGER button is set when the Trigger value is greater than the TriggerButtonThreshold.
	/// </remarks>
	public enum XCobraButtons:int{
		Start     = 1<<1,
		One       = 1<<0,
		Two       = 1<<2,
		Three     = 1<<3,
		Trigger   = 1<<7,
		//Four      = 1<<0,
		//Bumper    = 1<<0,
		//Joystick  = 1<<0,
	}

	#endregion Enum

	/// <summary>
	/// XCobraController objects provide access to X-Cobra controllers data.
	/// </summary>
	public class XCobraController{

		#region Const

		/// <summary>
		/// The axis index.
		/// </summary>
		public const int
			k_JoystickX = (int)XCobraAxis.JoystickX,
			k_JoystickY = (int)XCobraAxis.JoystickY,
			k_Trigger   = (int)XCobraAxis.Trigger;

		/// <summary>
		/// The default trigger button threshold constant.
		/// </summary>
		public const float DefaultTriggerButtonThreshold = 0.9f;

		#endregion Const

		#region Fields

		protected XHawkInput m_XHawkInput;

		protected XCobraState m_State;
		protected XCobraHands m_Hand;
		protected XCobraHands m_HandBind;
		protected XCobraButtons m_Buttons;
		protected XCobraButtons m_ButtonsPrevious;
		//protected float /*m_Trigger*/m_Axes[k_Trigger];
		protected float m_TriggerButtonThreshold = DefaultTriggerButtonThreshold;
		//protected float /*m_JoystickX*/m_Axes[k_JoystickX];
		//protected float /*m_JoystickY*/m_Axes[k_JoystickY];
		protected Vector3 m_Position;
		protected Quaternion m_Rotation;

		protected float[] m_Axes=new float[3];
		protected Quaternion m_RotationError=Quaternion.identity;

		#endregion Fields

		#region Internal Methods & Properties

		internal void Awake(XHawkInput context){
			//
			m_XHawkInput=context;
			m_State=XCobraState.NOT_FOUND;
			//
			m_Hand=XCobraHands.UNKNOWN;
			m_HandBind=XCobraHands.UNKNOWN;
			m_Buttons=0;
			m_ButtonsPrevious=0;
			/*m_Trigger*/m_Axes[k_Trigger]=0.0f;
			/*m_JoystickX*/m_Axes[k_JoystickX]=0.0f;
			/*m_JoystickY*/m_Axes[k_JoystickY]=0.0f;
			m_Position.Set(0.0f,0.0f,0.0f);
			m_Rotation.Set(0.0f,0.0f,0.0f,1.0f);
		}

		internal void Update(ref LibXHawkAPI.joyinfo joy){
			m_State=(XCobraState)joy.found_mask;
			m_Hand=(XCobraHands)joy.id;
			m_ButtonsPrevious=m_Buttons;
			m_Buttons=(XCobraButtons)joy.buttons;
			/*m_Trigger*/m_Axes[k_Trigger]=joy.trigger;
			/*m_JoystickX*/m_Axes[k_JoystickX]=joy.joystick_x;
			/*m_JoystickY*/m_Axes[k_JoystickY]=joy.joystick_y;
			m_Position.Set(joy.position[0],joy.position[1],joy.position[2]);
			m_Rotation.Set(joy.rotation[0],joy.rotation[1],joy.rotation[2],joy.rotation[3]);
			if(/*m_Trigger*/m_Axes[k_Trigger]>TriggerButtonThreshold){
				m_Buttons|=XCobraButtons.Trigger;
			}
			// TODO
			//
			if(m_XHawkInput==null) {
				return;
			}
			if(/*m_JoystickX*/m_Axes[k_JoystickX]*/*m_JoystickX*/m_Axes[k_JoystickX]<=m_XHawkInput.axisDeadzone*m_XHawkInput.axisDeadzone) {
				/*m_JoystickX*/m_Axes[k_JoystickX]=0.0f;
			}
			if(/*m_JoystickY*/m_Axes[k_JoystickY]*/*m_JoystickY*/m_Axes[k_JoystickY]<=m_XHawkInput.axisDeadzone*m_XHawkInput.axisDeadzone) {
				/*m_JoystickY*/m_Axes[k_JoystickY]=0.0f;
			}
			if((m_State&XCobraState.POSITION_FOUND)==0 &&// Hide?
				m_XHawkInput.autoHideXCobra) {
				m_Position.Set(-1024f,-1024f,-1024f);
			}else {
				m_Position.Scale(m_XHawkInput.sensitivity);
			}
		}

		internal XCobraHands HandBind{
			get{return m_HandBind;}
			set{m_HandBind=value;}
		}

		#endregion Internal Methods & Properties

		#region Public Methods & Properties

		/// <summary>
		/// The X-Cobra controller enabled state.
		/// </summary>
		public XCobraState State{get{return m_State;}}

		/// <summary>
		/// Hand the X-Cobra controller bound to, which could be UNKNOWN.
		/// </summary>
		public XCobraHands Hand{get{return ((m_Hand==XCobraHands.UNKNOWN) ? m_HandBind : m_Hand);}	}

		/// <summary>
		/// Value of trigger from released (0.0) to pressed (1.0).
		/// </summary>
		public float Trigger{get{return /*m_Trigger*/m_Axes[k_Trigger];}}

		/// <summary>
		/// Value of joystick X axis from left (-1.0) to right (1.0).
		/// </summary>
		public float JoystickX{get{return /*m_JoystickX*/m_Axes[k_JoystickX];}}

		/// <summary>
		/// Value of joystick Y axis from bottom (-1.0) to top (1.0).
		/// </summary>
		public float JoystickY{get{return /*m_JoystickY*/m_Axes[k_JoystickY];}}

		/// <summary>
		/// The X-Cobra controller position in Unity coordinates.
		/// </summary>
		public Vector3 Position{get{return m_XHawkInput.pivot.TransformPoint(m_Position);}}
		
		/// <summary>
		/// The raw X-Cobra controller position value.
		/// </summary>
		public Vector3 PositionRaw{get{return m_Position;}}

		/// <summary>
		/// The X-Cobra controller rotation in Unity coordinates.
		/// </summary>
		public Quaternion Rotation{get{return m_RotationError*m_Rotation;}}
		
		/// <summary>
		/// The raw X-Cobra controller rotation value.
		/// </summary>
		public Quaternion RotationRaw{get{return m_Rotation;}}

		/// <summary>
		/// The value which the Trigger value must pass to register a TRIGGER button press.  This value can be set.
		/// </summary>
		public float TriggerButtonThreshold{
			get{return m_TriggerButtonThreshold;}
			set{m_TriggerButtonThreshold=value;}
		}

		/// <summary>
		/// Returns true if the button parameter is being pressed.
		/// </summary>
		public bool GetButton(XCobraButtons button){
			return ((button&m_Buttons)!=0);
		}

		/// <summary>
		/// Returns true if the button parameter was pressed this frame.
		/// </summary>
		public bool GetButtonDown(XCobraButtons button){
			return ((button&m_Buttons)!=0)&&((button&m_ButtonsPrevious)==0);
		}

		/// <summary>
		/// Returns true if the button parameter was released this frame.
		/// </summary>
		public bool GetButtonUp(XCobraButtons button){
			return ((button&m_Buttons)==0)&&((button&m_ButtonsPrevious)!=0);
		}

		// <!--

		/// <summary>
		/// Value of joystick axis from bottom (-1.0) to top (1.0).
		/// </summary>
		public float GetAxis(XCobraAxis axis) {
			return m_Axes[(int)axis];
		}

		/// <summary>
		/// Only reset yaw.
		/// </summary>
		public void ResetRotation() {
			m_RotationError=Quaternion.Inverse(Quaternion.Euler(new Vector3(0f,m_Rotation.eulerAngles.y,0f)));
		}

		// -->

		#endregion Public Methods & Properties

	}
}
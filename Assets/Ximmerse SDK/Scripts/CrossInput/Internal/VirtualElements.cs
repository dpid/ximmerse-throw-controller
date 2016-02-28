using UnityEngine;

namespace Ximmerse.CrossInput {

	// virtual axis and button classes - applies to mobile input
	// Can be mapped to touch joysticks, tilt, gyro, etc, depending on desired implementation.
	// Could also be implemented by other input devices - kinect, electronic sensors, etc
	public class VirtualAxis:SmartPointer {
		public string name;/* {
				get;
				protected set;
			}*/
		protected float m_Value;
		public bool matchWithInputManager {
			get;
			protected set;
		}


		public VirtualAxis(string name)
			: this(name,true) {
		}


		public VirtualAxis(string name,bool matchToInputSettings) {
			this.name=name;
			matchWithInputManager=matchToInputSettings;
		}


		// removes an axes from the cross platform input system
		public void Remove() {
			CrossInputManager.UnRegisterVirtualAxis(name);
		}

		protected int _timestamp;

		// a controller gameobject (eg. a virtual thumbstick) should update this class
		public void Update(float value) {
			//m_Value=value;
		// <!-- TODO
			if(_timestamp==CrossInputManager.timestamp/*Time.frameCount*/){// Fix...
				m_Value+=value;
			}else{
				_timestamp=CrossInputManager.timestamp/*Time.frameCount*/;
				m_Value=value;
			}
		// TODO -->
		}


		public float GetValue {
			get {
				return m_Value;
			}
		}


		public float GetValueRaw {
			get {
				return m_Value;
			}
		}
	}

	// a controller gameobject (eg. a virtual GUI button) should call the
	// 'pressed' function of this class. Other objects can then read the
	// Get/Down/Up state of this button.
	public class VirtualButton:SmartPointer {
		public string name;/* {
				get;
				protected set;
			}*/
		public bool matchWithInputManager {
			get;
			protected set;
		}

		protected int m_LastPressedFrame=-5;
		protected int m_ReleasedFrame=-5;
		protected bool m_Pressed;


		public VirtualButton(string name)
			: this(name,true) {
		}


		public VirtualButton(string name,bool matchToInputSettings) {
			this.name=name;
			matchWithInputManager=matchToInputSettings;
		}


		// A controller gameobject should call this function when the button is pressed down
		public void Pressed() {
			if(m_Pressed) {
				return;
			}
			m_Pressed=true;
			m_LastPressedFrame=CrossInputManager.timestamp/*Time.frameCount*/;
		}


		// A controller gameobject should call this function when the button is released
		public void Released() {
			m_Pressed=false;
			m_ReleasedFrame=CrossInputManager.timestamp/*Time.frameCount*/;
		}


		// the controller gameobject should call Remove when the button is destroyed or disabled
		public void Remove() {
			CrossInputManager.UnRegisterVirtualButton(name);
		}


		// these are the states of the button which can be read via the cross platform input system
		public bool GetButton {
			get {
				return m_Pressed;
			}
		}


		public bool GetButtonDown {
			get {
				return m_LastPressedFrame-CrossInputManager.timestamp/*Time.frameCount*/==-1;
			}
		}


		public bool GetButtonUp {
			get {
				return (m_ReleasedFrame==CrossInputManager.timestamp/*Time.frameCount*/-1);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public bool Update(bool isPress,bool isPressPrev) {
			if(isPress!=isPressPrev) {
				if(isPress) {
				if(!m_Pressed){
					Pressed();
				}
				}else if(m_Pressed){
					Released();
				}
			}
			return isPress;
		}
	}
}

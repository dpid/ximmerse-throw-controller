using System.Collections.Generic;
using UnityEngine;

namespace Ximmerse.CrossInput{

	/// <summary>
	/// 
	/// </summary>
	public class VirtualController:IJoystick{

		#region Static

		public const int
			LEFT_HAND=0,
			RIGHT_HAND=1
		;
		
		public static VirtualController[] controllers=new VirtualController[4];

		/// <summary>
		/// 
		/// </summary>
		public static VirtualController Instantiate(string fmtAxis,string[] axes,string fmtButton,string[] buttons,string poseName){
			VirtualController hj=new VirtualController();
				//(new GameObject("New HandJoystick",
				//typeof(HandJoystick))).GetComponent<HandJoystick>();
			hj.InitInput(fmtAxis,axes,fmtButton,buttons,poseName);
			return hj;
		}

		#endregion Static

		#region CrossInput

		protected VirtualAxis[] m_Axes;
		protected VirtualButton[] m_Buttons;
		protected Dictionary<string,VirtualAxis> m_MapAxes;
		protected Dictionary<string,VirtualButton> m_MapButtons;

		public VirtualPose pose;
		public VirtualVibration vibration;


		public virtual void InitInput(string fmtAxis,string[] axes,string fmtButton,string[] buttons,string poseName){
			int i,imax;
			//
			i=0;imax=axes.Length;
			m_Axes=new VirtualAxis[imax];m_MapAxes=new Dictionary<string,VirtualAxis>(imax);
			for(;i<imax;++i){
				m_Axes[i]=CrossInputManager.VirtualAxisReference(this,string.Format(fmtAxis,axes[i]),true);
				m_MapAxes.Add(axes[i],m_Axes[i]);// Add the raw input name.
			}
			//
			i=0;imax=buttons.Length;
			m_Buttons=new VirtualButton[imax];m_MapButtons=new Dictionary<string,VirtualButton>(imax);
			for(;i<imax;++i){
				m_Buttons[i]=CrossInputManager.VirtualButtonReference(this,string.Format(fmtButton,buttons[i]),true);
				m_MapButtons.Add(buttons[i],m_Buttons[i]);// Add the raw input name.
			}
			//
			pose=CrossInputManager.VirtualPoseReference(this,poseName,true);
		}

		#endregion CrossInput

		#region IJoystick
		
		/// <summary>
		/// 
		/// </summary>
		public virtual float GetAxis(string axisName){
			if(!m_MapAxes.ContainsKey(axisName)) return 0.0f;
			return m_MapAxes[axisName].GetValue;
		}
		
		/// <summary>
		/// 
		/// </summary>
		public virtual float GetAxisRaw(string axisName){
			if(!m_MapAxes.ContainsKey(axisName)) return 0.0f;
			return m_MapAxes[axisName].GetValueRaw;
		}
		
		/// <summary>
		/// 
		/// </summary>
		public virtual bool GetButton(string buttonName){
			if(!m_MapButtons.ContainsKey(buttonName)) return false;
			return m_MapButtons[buttonName].GetButton;
		}
		
		/// <summary>
		/// 
		/// </summary>
		public virtual bool GetButtonDown(string buttonName){
			if(!m_MapButtons.ContainsKey(buttonName)) return false;
			return m_MapButtons[buttonName].GetButtonDown;
		}
		
		/// <summary>
		/// 
		/// </summary>
		public virtual bool GetButtonUp(string buttonName){
			if(!m_MapButtons.ContainsKey(buttonName)) return false;
			return m_MapButtons[buttonName].GetButtonUp;
		}
		
		/// <summary>
		/// 
		/// </summary>
		public virtual float GetAxis(int axisId){
			return m_Axes[axisId].GetValue;
		}
		
		/// <summary>
		/// 
		/// </summary>
		public virtual float GetAxisRaw(int axisId){
			return m_Axes[axisId].GetValueRaw;
		}
		
		/// <summary>
		/// 
		/// </summary>
		public virtual bool GetButton(int buttonId){
			return m_Buttons[buttonId].GetButton;
		}
		
		/// <summary>
		/// 
		/// </summary>
		public virtual bool GetButtonDown(int buttonId){
			return m_Buttons[buttonId].GetButtonDown;
		}
		
		/// <summary>
		/// 
		/// </summary>
		public virtual bool GetButtonUp(int buttonId){
			return m_Buttons[buttonId].GetButtonUp;
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual int GetButtonPress(string buttonName) {
			throw new System.NotImplementedException();
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual int GetButtonPress(int buttonId) {
			throw new System.NotImplementedException();
		}

		#endregion IJoystick

		#region Beta

		/// <summary>
		/// -1 -> Stop
		/// </summary>
		public virtual void SetVibration(int waveType,float delay,float duration) {
			if(vibration!=null) {
				vibration.SetVibration(waveType,delay,duration);
			}
		}

		#endregion Beta

	}

}
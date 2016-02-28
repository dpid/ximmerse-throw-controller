using UnityEngine;
using Ximmerse.CrossInput;

namespace Ximmerse.Core {

	/// <summary>
	/// 
	/// </summary>
#if UNITY_5 && !UNITY_5_0
	[UnityEngine.CreateAssetMenu(fileName="New X-Hawk Map",menuName="Ximmerse SDK/X-Hawk Map Setting",order=800)]
#endif
	public class XHawkMapSetting:InputMapSetting {

		#region Fields

		private XHawkInputManager m_Manager;
		private XCobraController m_Controller;
		private int m_Index;

		#endregion Fields

		#region Map Fields

		public bool rawPose          = false;
		public string pose           = "Hand";
		public string axis_JoystickX = "Horizontal";
		public string axis_JoystickY = "Vertical";
		public string axis_Trigger   = "Trigger";
		public string btn_Start      = "Start";
		public string btn_One        = "One";
		public string btn_Two        = "Two";
		public string btn_Three      = "Three";
		public string btn_Trigger    = "Trigger";

		#endregion Map Fields
		
		#region Methods

		/// <summary>
		/// 
		/// </summary>
		public void InitInput(XHawkInputManager manager,int index,string fmt) {
			m_Manager = manager;
			m_Index   = index;
			//
			m_Controller=XHawkInputManager.GetController((XCobraHands)m_Index);
			if(m_Controller==null) return;
			//
			InitInput(manager,fmt);
		}

		/// <summary>
		/// 
		/// </summary>
		public override void ResetInput() {
			m_Controller.ResetRotation();
		}

		/// <summary>
		/// 
		/// </summary>
		public override void UpdateInput() {
			//
			if(m_Controller==null) return;

			#region Update

			int i=0;
			if(rawPose) {
				m_VirtualPoses[i].position=m_Controller.PositionRaw;m_VirtualPoses[i].rotation=m_Controller.RotationRaw;
			}else {
				m_VirtualPoses[i].position=m_Controller.Position;m_VirtualPoses[i].rotation=m_Controller.Rotation;
			}
			i=0;
			m_VirtualAxes[i].Update(m_Controller.JoystickX);++i;
			m_VirtualAxes[i].Update(m_Controller.JoystickY);++i;
			m_VirtualAxes[i].Update(m_Controller.Trigger  );++i;
			i=0;
			m_IsPress[i]=m_VirtualButtons[i].Update(m_Controller.GetButton(XCobraButtons.Start  ),m_IsPress[i]);++i;
			m_IsPress[i]=m_VirtualButtons[i].Update(m_Controller.GetButton(XCobraButtons.One    ),m_IsPress[i]);++i;
			m_IsPress[i]=m_VirtualButtons[i].Update(m_Controller.GetButton(XCobraButtons.Two    ),m_IsPress[i]);++i;
			m_IsPress[i]=m_VirtualButtons[i].Update(m_Controller.GetButton(XCobraButtons.Three  ),m_IsPress[i]);++i;
			m_IsPress[i]=m_VirtualButtons[i].Update(m_Controller.GetButton(XCobraButtons.Trigger),m_IsPress[i]);++i;
			
			#endregion Update

		}

		#region Get Strings

		/// <summary>
		/// 
		/// </summary>
		public override string[] GetPoseStrings(){
			if(m_Poses!=null) return m_Poses;
			return m_Poses=new string[1]{
				pose
			};
		}
		
		/// <summary>
		/// 
		/// </summary>
		public override string[] GetAxisStrings(){
			if(m_Axes!=null) return m_Axes;
			return m_Axes=new string[3]{
				axis_JoystickX ,
				axis_JoystickY ,
				axis_Trigger   ,
			};
		}
		
		/// <summary>
		/// 
		/// </summary>
		public override string[] GetButtonStrings(){
			if(m_Buttons!=null) return m_Buttons;
			return m_Buttons=new string[5]{
				btn_Start      ,
				btn_One        ,
				btn_Two        ,
				btn_Three      ,
				btn_Trigger    ,
			};
		}

		#endregion Get Strings
		
		#endregion Methods

	}
}

using UnityEngine;
using Ximmerse.CrossInput;

namespace Ximmerse.Core {

	/// <summary>
	/// 
	/// </summary>
#if UNITY_5 && !UNITY_5_0
	[CreateAssetMenu(fileName="New Trigger Sensor",menuName="Ximmerse SDK/Sensor/Trigger",order=800)]
#endif
	public class SensorTrigger:SensorBase {

		#region Fields

		[UnityEngine.Header("Trigger")]
		public string axisName;
		public string buttonName;
		
		[UnityEngine.SerializeField]protected int m_Min=-128,m_Max=127;

		public float deadZone=0.0f;
		/// <summary>
		/// See TriggerButtonThreshold.
		/// </summary>
		public float buttonThreshold=.75f;

		[System.NonSerialized]protected float m_Trigger;
		[System.NonSerialized]protected VirtualAxis m_Axis;
		[System.NonSerialized]protected VirtualButton m_Button;

		#endregion Fields

		#region Override

		/// <summary>
		/// 
		/// </summary>
		public override void StartInput() {
			//
			m_Axis=CrossInputManager.VirtualAxisReference(
				this,
				string.Format(device.fmtJoy,axisName),
				true
			);
			//
			m_Button=CrossInputManager.VirtualButtonReference(
				this,
				string.Format(device.fmtJoy,buttonName),
				true
			);
			//
			device.axes.Add(axisName);
			device.buttons.Add(buttonName);
		}

		/// <summary>
		/// 
		/// </summary>
		public override void UpdateInput() {
			//
			m_Axis.Update(m_Trigger>deadZone?m_Trigger:0.0f);
			//
			if(m_Trigger>=buttonThreshold) {
				m_Button.Pressed();
			} else {
				m_Button.Released();
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public override int Parse(byte[] buffer,int offset,int count) {
			m_Trigger=(float)((ToSbyte(buffer[offset])-m_Min)/(float)(m_Max-m_Min));
			return 1;
		}

		#endregion Override
	
	}

}
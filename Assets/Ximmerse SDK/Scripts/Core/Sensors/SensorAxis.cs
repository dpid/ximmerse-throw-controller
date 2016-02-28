using UnityEngine;
using Ximmerse.CrossInput;

namespace Ximmerse.Core {

	/// <summary>
	/// 
	/// </summary>
#if UNITY_5 && !UNITY_5_0
	[CreateAssetMenu(fileName="New Axis Sensor",menuName="Ximmerse SDK/Sensor/Axis",order=800)]
#endif
	public class SensorAxis:SensorBase {

		#region Fields

		[UnityEngine.Header("Axis")]
		//
#if UNITY_EDITOR
		public string[] displayName=new string[0];
#endif
		public string[] axisName=new string[0];
		//
		[UnityEngine.SerializeField]protected int m_Min=-128,m_Max=127;
		[UnityEngine.SerializeField]protected float[] m_KAxes=new float[0];
		public float deadZone=0.0f;
		
		[System.NonSerialized]protected int m_NumAxes;
		[System.NonSerialized]protected float m_SqrDeadZone;
		[System.NonSerialized]protected VirtualAxis[] m_Axes; 
		[System.NonSerialized]protected float[] m_AxesValue;

		#endregion Fields

		#region Override

		public override void AwakeInput() {
			m_NumAxes=axisName.Length;
			m_SqrDeadZone=deadZone*deadZone;
		}

		/// <summary>
		/// 
		/// </summary>
		public override void StartInput() {
			//
			m_Axes=new VirtualAxis[m_NumAxes];
			m_AxesValue=new float[m_NumAxes];
			for(int i=0;i<m_NumAxes;++i){
				m_Axes[i]=CrossInputManager.VirtualAxisReference(
					this,
					string.Format(device.fmtJoy,axisName[i]),
					true
				);
				m_AxesValue[i]=0.0f;
			}
			//
			device.axes.AddRange(axisName);
		}

		/// <summary>
		/// 
		/// </summary>
		public override void UpdateInput() {
			for(int i=0;i<m_NumAxes;++i){
				m_Axes[i].Update(
					(m_AxesValue[i]*m_AxesValue[i]>m_SqrDeadZone)?m_AxesValue[i]:0.0f
				);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public override int Parse(byte[] buffer,int offset,int count) {
			for(int i=0;i<m_NumAxes;++i){
				float axisValue=((ToSbyte(buffer[offset+i])-m_Min)/(float)(m_Max-m_Min))*2.0f-1.0f;
				m_AxesValue[i]=m_KAxes[i]*axisValue;
			}
			return bufferSize;
		}
		
		#endregion Override
	
	}

}
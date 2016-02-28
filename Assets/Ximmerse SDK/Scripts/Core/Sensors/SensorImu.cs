using UnityEngine;
using Ximmerse.CrossInput;

namespace Ximmerse.Core {

	/// <summary>
	/// 
	/// </summary>
#if UNITY_5 && !UNITY_5_0
	[CreateAssetMenu(fileName="New Imu Sensor",menuName="Ximmerse SDK/Sensor/Imu",order=800)]
#endif
	public class SensorImu:SensorBase {

		#region Static
		
		public const int IMU_BUFFER_SIZE=6;
		protected static float[] s_FloatsHelper=new float[4];
		
		#endregion Static

		#region Fields

		[System.NonSerialized]public Quaternion runtimeRotation=Quaternion.identity;
		[System.NonSerialized]public Vector3 rawEuler=Vector3.zero;
		[System.NonSerialized]protected VirtualPose m_Pose;

		[Header("Transform")]
		public string poseName;
		public bool canReset=true;
		[System.NonSerialized]public Quaternion rotation=Quaternion.identity;
		[System.NonSerialized]public Vector3 eulerAngles=Vector3.zero;

		#endregion Fields

		#region Override

		/// <summary>
		/// 
		/// </summary>
		public override void AwakeInput() {
			bufferSize=IMU_BUFFER_SIZE;
			//deviceRotation=Quaternion.Euler(deviceEuler);
		}

		/// <summary>
		/// 
		/// </summary>
		public override void StartInput() {
			if(canReset){
				device.resetables.Add(this);
			}
			m_Pose=CrossInputManager.VirtualPoseReference(
				this,
				string.Format(device.fmtJoy,poseName),
				true
			);
		}

		/// <summary>
		/// 
		/// </summary>
		public override void UpdateInput() {
			//if(m_Pose!=null){
			//	m_Pose.rotation=rotation;
			//}
		}

		/// <summary>
		/// 
		/// </summary>
		public override void ResetInput() {
			runtimeRotation=Quaternion.Inverse(Quaternion.Euler(new Vector3(0f,rawEuler.y,0f)));
			//runtimeRotation=Quaternion.Inverse(Quaternion.Euler(rawEuler));
		}

		/// <summary>
		/// 
		/// </summary>
		public override int Parse(byte[] buffer,int offset,int count) {
			//
			int i=0;
			LibXHawkAPI.BytesToEuler(s_FloatsHelper,buffer,offset);
			rawEuler.Set(s_FloatsHelper[i++],s_FloatsHelper[i++],s_FloatsHelper[i++]);
			//
			m_Pose.rawEulerAngles=rawEuler;//
			//rawEuler=(Quaternion.Euler(rawEuler)*deviceRotation).eulerAngles;
			//
			rotation=runtimeRotation*Quaternion.Euler(rawEuler);
			eulerAngles=rotation.eulerAngles;
			// TODO -->
			// Just do it.
			m_Pose.rotation=rotation;
			m_Pose.DispatchEvent(VirtualPose.EVENT_UPDATE_ROTATION);
			return IMU_BUFFER_SIZE;
		}

		#endregion Override
	
	}
}

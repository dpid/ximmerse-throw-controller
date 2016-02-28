using UnityEngine;
using Ximmerse.CrossInput;

namespace Ximmerse.Core {

	/// <summary>
	/// 
	/// </summary>
#if UNITY_5 && !UNITY_5_0
	[CreateAssetMenu(fileName="New Blob Sensor",menuName="Ximmerse SDK/Sensor/Blob",order=800)]
#endif
	public class SensorBlob:SensorBase,IBlobListener {

		#region Fields

		[Header("Transform")]
		public string poseName;
		public int blobId=0;
		public Vector3 position;
		[System.NonSerialized]protected VirtualPose m_Pose;

		#endregion Fields

		#region Override

		/// <summary>
		/// 
		/// </summary>
		public override void StartInput() {
			XHawkServiceBase.main.Register(blobId,this);
			m_Pose=CrossInputManager.VirtualPoseReference(
				this,
				string.Format(device.fmtJoy,poseName),
				true
			);
		}
		
		/// <summary>
		/// 
		/// </summary>
		public override int Parse(byte[] buffer,int offset,int count) {
			return 0;// ???
		}
		
		/// <summary>
		/// 
		/// </summary>
		public override void UpdateInput() {
			m_Pose.position=position;
		}

		/// <summary>
		/// 
		/// </summary>
		public override void OnDestroyInput() {
			XHawkServiceBase.main.Unregister(blobId,this);
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual void OnBlobUpdate(BlobsID3D blob){
			position=blob.position;
		}

		#endregion Override
	
	}

}
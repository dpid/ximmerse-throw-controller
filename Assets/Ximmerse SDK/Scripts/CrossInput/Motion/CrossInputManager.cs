using UnityEngine;

namespace Ximmerse.CrossInput{

	// 体感方面CrossInput支持.

	/// <summary>
	/// 
	/// </summary>
	public partial class CrossInputManager{
		
		
		/// <summary>
		/// 
		/// </summary>
		public static void RegisterVirtualPose(VirtualPose pose) {
			activeInput.RegisterVirtualPose(pose);
		}
		
		/// <summary>
		/// 
		/// </summary>
		public static void UnRegisterVirtualPose(string name) {
			activeInput.UnRegisterVirtualPose(name);
		}


		// returns a reference to a named virtual button.
		public static VirtualPose VirtualPoseReference(object caller,string name,bool canAdd=false) {
			VirtualPose item=null;
			if(PoseExists(name)){
				item=VirtualPoseReference(name);
			}else{
				RegisterVirtualPose(item=new VirtualPose(name));
			}
			if(item!=null){
				item.AddRef(caller);
			}
			return item;
		}


		// returns a reference to a named virtual button if it exists otherwise null
		public static VirtualPose VirtualPoseReference(string name) {
			return activeInput.VirtualPoseReference(name);
		}

		public static bool PoseExists(string name) {
			return activeInput.PoseExists(name);
		}

		public static Vector3 GetPosePosition(string name){
			return activeInput.GetPosePosition(name);
		}

		public static Quaternion GetPoseRotation(string name){
			return activeInput.GetPoseRotation(name);
		}

		public static void SetPose(string name,Vector3 position,Quaternion rotation){
			activeInput.SetPose(name,position,rotation);
		}

		public static void SetPosePosition(string name,Vector3 position){
			activeInput.SetPosePosition(name,position);
		}

		public static void SetPoseRotation(string name,Quaternion rotation){
			activeInput.SetPoseRotation(name,rotation);
		}
	
	}
}
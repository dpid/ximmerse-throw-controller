using UnityEngine;
using System.Collections.Generic;

namespace Ximmerse.CrossInput{

	// 体感方面CrossInput支持.
	
	/// <summary>
	/// 
	/// </summary>
	public partial class VirtualInput{
		
		public Dictionary<string,VirtualPose> m_VirtualPoses=new Dictionary<string,VirtualPose>();

		public virtual bool PoseExists(string name) {
			return m_VirtualPoses.ContainsKey(name);
		}


		public virtual void RegisterVirtualPose(VirtualPose pose) {
			// check if already have a buttin with that name and log an error if we do
			if(m_VirtualPoses.ContainsKey(pose.name)) {
				Debug.LogError("There is already a virtual pose named "+pose.name+" registered.");
			} else {
				// add any new poses
				m_VirtualPoses.Add(pose.name,pose);

				// if we dont want to match to the input manager then always use a virtual axis
				if(!pose.matchWithInputManager) {
					m_AlwaysUseVirtual.Add(pose.name);
				}
			}
		}


		public virtual void UnRegisterVirtualPose(string name) {
			// if we have a pose with this name then remove it from our dictionary of registered poses
			if(m_VirtualPoses.ContainsKey(name)) {
				m_VirtualPoses.Remove(name);
			}
		}


		// returns a reference to a named virtual poses if it exists otherwise null
		public virtual VirtualPose VirtualPoseReference(string name) {
			return m_VirtualPoses.ContainsKey(name)?m_VirtualPoses[name]:null;
		}
		protected void AddPose(string name) {
			// we have not registered this button yet so add it, happens in the constructor
			CrossInputManager.RegisterVirtualPose(new VirtualPose(name));
		}

		public virtual Vector3 GetPosePosition(string name){
			if(m_VirtualPoses.ContainsKey(name)) {
				return m_VirtualPoses[name].position;
			}

			AddPose(name);
			return m_VirtualPoses[name].position;
		}

		public virtual Quaternion GetPoseRotation(string name){
			if(m_VirtualPoses.ContainsKey(name)) {
				return m_VirtualPoses[name].rotation;
			}

			AddPose(name);
			return m_VirtualPoses[name].rotation;
		}

		public virtual void SetPose(string name,Vector3 position,Quaternion rotation){
			if(!m_VirtualPoses.ContainsKey(name)) {
				AddPose(name);
			}
			VirtualPose pose=m_VirtualPoses[name];
			pose.position=position;
			pose.rotation=rotation;
		}

		public virtual void SetPosePosition(string name,Vector3 position){
			if(!m_VirtualPoses.ContainsKey(name)) {
				AddPose(name);
			}
			VirtualPose pose=m_VirtualPoses[name];
			pose.position=position;
		}

		public virtual void SetPoseRotation(string name,Quaternion rotation){
			if(!m_VirtualPoses.ContainsKey(name)) {
				AddPose(name);
			}
			VirtualPose pose=m_VirtualPoses[name];
			pose.rotation=rotation;
		}
	
	}
}
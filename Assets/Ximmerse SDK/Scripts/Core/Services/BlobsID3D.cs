using System.Collections.Generic;
using UnityEngine;
using Ximmerse.CrossInput;

namespace Ximmerse.Core {

	/// <summary>
	/// 
	/// </summary>
	public interface IBlobListener {

		/// <summary>
		/// 
		/// </summary>
		void OnBlobUpdate(BlobsID3D blob);
	}
	
	/// <summary>
	/// 
	/// </summary>
	[System.Serializable]
	public class BlobsID3D {

		/// <summary>
		/// 
		/// </summary>
		public const int SIZE=16;

		#region Fields

		//
		public string poseName;
		public int id;
		public Vector3 position=Vector3.zero;
		//

		/// <summary>
		/// 
		/// </summary>
		[System.NonSerialized]public bool isDirty=false;

		/// <summary>
		/// 
		/// </summary>
		[System.NonSerialized]public XHawkServiceBase tracker;

		/// <summary>
		/// 
		/// </summary>
		[System.NonSerialized]public VirtualPose pose;

		/// <summary>
		/// 
		/// </summary>
		public List<IBlobListener> listeners=new List<IBlobListener>();

		#endregion Fields

		#region Methods

		public void OnStart(XHawkServiceBase i_tracker){
			tracker=i_tracker;
			if(!string.IsNullOrEmpty(poseName)){
				pose=CrossInputManager.VirtualPoseReference(i_tracker,poseName,true);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public void OnUpdate(){
			// If hidden,apply directly.
			if(id==-1) {
				pose.position=pose.rawPosition=position;
				pose.DispatchEvent(VirtualPose.EVENT_MISS_POSITION);
				return;
			}
			//
			isDirty=true;
			int i=0,imax=listeners.Count;
			for(;i<imax;++i){
				listeners[i].OnBlobUpdate(this);
			};
			pose.DispatchEvent(VirtualPose.EVENT_UPDATE_POSITION);
		}

		/// <summary>
		/// 
		/// </summary>
		public void OnUpdateOnMainThread(){
			if(pose!=null){if(isDirty){
				Transform pivot=tracker.pivot;
				if(pivot==null) pose.position=pose.rawPosition=position;
				else pose.position=pivot.TransformPoint(pose.rawPosition=position);
				isDirty=false;
			}}
		}

		/// <summary>
		/// 
		/// </summary>
		public override string ToString() {
			return "{Point name=\""+poseName+"\" id=\""+id+"\" position=\""+position.ToString("0.000000")+"\"}";
		} 

		#endregion Methods

	}

}
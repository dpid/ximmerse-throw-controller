using UnityEngine;

namespace Ximmerse.CrossInput{

	#region Nested Types

	public delegate void VirtualPoseDelegate(VirtualPose pose);

	public enum TrackingResult:int {
		Miss          = 0,
		PositionFound = 1,
		RotationFound = 2
	}

	#endregion Nested Types
		
	/// <summary>
	/// 体感方面CrossInput支持.
	/// </summary>
	public class VirtualPose:SmartPointer{

		#region Const

		public const int
			 EVENT_UPDATE_POSITION = 1
			,EVENT_MISS_POSITION   = 2
			,EVENT_UPDATE_ROTATION = 3
			,EVENT_MISS_ROTATION   = 4
		;

		#endregion Const

		#region Fields

		public string name{
			get;
			private set;
		}
		public bool matchWithInputManager {
			get;
			private set;
		}

		public Transform transform;
		public Transform parent;

		public int id;
		public TrackingResult trackingResult=TrackingResult.Miss;

		public Vector3 position;
		public Quaternion rotation;

		/// <summary>
		/// Only for Ximmerse productions.
		/// </summary>
		public Vector3 rawPosition;

		/// <summary>
		/// Only for Ximmerse productions.
		/// </summary>
		public Vector3 rawEulerAngles;

		public VirtualPoseDelegate onUpdate,onMiss;

		#endregion Fields

		#region Constructors

		public VirtualPose(string name)
			: this(name,true) {
		}

		/// <summary>
		/// 
		/// </summary>
		public VirtualPose(string name,bool matchToInputSettings) {
			this.name=name;
			matchWithInputManager=matchToInputSettings;
			//
			position=Vector3.one*-1024f;
			rotation=Quaternion.identity;
		}

		#endregion Constructors

		#region Methods

		/// <summary>
		/// 
		/// </summary>
		public virtual void Get(float[] o_pos,float[] o_rot){
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual void Set(float[] i_pos,float[] i_rot){
			int i;
			//
			i=0;
			position.Set(
				i_pos[i++],
				i_pos[i++],
				i_pos[i++]//,
			);
			//
			i=0;
			rotation.Set(
				i_rot[i++],
				i_rot[i++],
				i_rot[i++],
				i_rot[i++]//,
			);
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual void DispatchEvent(int eventID) {

			#region Dispatch Event

			switch(eventID) {
				case EVENT_UPDATE_POSITION:
					trackingResult|=TrackingResult.PositionFound;
					if(onUpdate!=null) {
						onUpdate(this);
					}
				break;
				case EVENT_MISS_POSITION:
					trackingResult&=~(TrackingResult.PositionFound);
					if(onMiss!=null) {
						onMiss(this);
					}
				break;
				case EVENT_UPDATE_ROTATION:
					trackingResult|=TrackingResult.RotationFound;
					if(onUpdate!=null) {
						onUpdate(this);
					}
				break;
				case EVENT_MISS_ROTATION:
					trackingResult&=~(TrackingResult.RotationFound);
					if(onMiss!=null) {
						onMiss(this);
					}
				break;
			}

			#endregion Dispatch Event

		}

		#endregion Methods

		#region Properties

		/// <summary>
		/// 
		/// </summary>
		public virtual Transform Transform {
			get {
				if(transform==null) {
					PoseMover mover=(new GameObject(name,typeof(PoseMover))).GetComponent<PoseMover>();
					transform=mover.transform;
					transform.parent=parent;
				}
				return transform;
			}
		}

		#endregion Properties

	}
}
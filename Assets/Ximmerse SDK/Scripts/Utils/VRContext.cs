//#define OVR_SDK

#if !OVR_SDK&&!UVR_SDK
	#if UNITY_5 && !UNITY_5_0
		#define UVR_SDK
	#else
//		#define OVR_SDK
	#endif
#endif

//
// Your CameraRig            (goUvrCamRig)
//     OVRCameraRig          (goOvrCamRig)
//     CenterEyeAnchor       (centerEyeAnchor)

using UnityEngine;
using UnityEngine.VR;

/// <summary>
/// 
/// </summary>
public class VRContext:MonoBehaviour {

	#region Static

	/// <summary>
	/// 
	/// </summary>
	public static VRContext main;

	/// <summary>
	/// 
	/// </summary>
	public static VRContext Main {
		get {
			if(main==null) {
				main=FindObjectOfType<VRContext>();
			}
			return main;
		}
	}

	#endregion Static

	#region Fields

	/// <summary>
	/// OVR的Camera组件.
	/// </summary>
	public GameObject goOvrCamRig,goUvrCamRig;
	
	/// <summary>
	/// 
	/// </summary>
	public ActivateList contextOvr,contextUvr;

	/// <summary>
	/// 所有关于"头"的抽象物体.
	/// </summary>
	public Transform centerEyeAnchor;

	#region Hands

	public bool autoCreateHandAnchors=false;

	/// <summary>
	/// 
	/// </summary>
	public Transform leftHandAnchor;

	/// <summary>
	/// 
	/// </summary>
	public Transform rightHandAnchor;

	#endregion Hands

	public KeyOrButton btnReset;
	public UnityEngine.Events.UnityAction onRecenter;

	#endregion Fields

	#region Unity Messages

	/// <summary>
	/// 
	/// </summary>
	protected virtual void Awake() {
		if(main==null) {
			main=this;
		}else if(main!=this) {
			Log.e("VRContext","Only one instance can be run!!!");
		}

		//
		//if(goOvrCamRig==null) goOvrCamRig=gameObject;

#if OVR_SDK
		//
		if(goOvrCamRig!=null){
			centerEyeAnchor.parent=goOvrCamRig.transform.FindChild("TrackingSpace");
			//
			centerEyeAnchor.localPosition  =          Vector3.zero;
			centerEyeAnchor.localRotation  =   Quaternion.identity;
			centerEyeAnchor.localScale     =           Vector3.one;
			//
			goOvrCamRig.SetActive(true);
		}
		if(contextOvr!=null){
			contextOvr.SetValueForce(true);
		}
		//
		if(goUvrCamRig!=null){
			goUvrCamRig.GetComponent<Camera>().enabled=false;
		}
		if(contextUvr!=null){
			contextUvr.SetValueForce(false);
		}
#elif UVR_SDK
		//
		if(goUvrCamRig!=null){
			centerEyeAnchor.parent=goUvrCamRig.transform;
			//
			goUvrCamRig.GetComponent<Camera>().enabled=true;
		}
		if(contextUvr!=null){
			contextUvr.SetValueForce(true);
		}
		//
		if(goOvrCamRig!=null){
			goOvrCamRig.SetActive(false);
		}
		if(contextOvr!=null){
			contextOvr.SetValueForce(false);
		}
#endif

	}
	
	/// <summary>
	/// 
	/// </summary>
	protected virtual void Update(){
		if(btnReset.GetAnyDown()){
#if OVR_SDK
			OVRManager.display.RecenterPose();
#elif CARDBOARD_SDK
			Cardboard.SDK.Recenter();
#elif UVR_SDK
			InputTracking.Recenter();
#endif
			if(onRecenter!=null){
				onRecenter.Invoke();
			}
		}
	}

	#endregion Unity Messages

}
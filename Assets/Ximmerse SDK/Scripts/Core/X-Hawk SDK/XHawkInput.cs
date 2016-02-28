using UnityEngine;

namespace Ximmerse.Core {

	/// <summary>
	/// XHawkInput provides an interface for accessing X-Cobra controllers.
	/// </summary>
	/// <remarks>
	/// This script should be bound to a GameObject in the scene so that its Awake(), Update() and OnDestroy() methods are called.This can be done by adding the XHawkInput prefab to a scene.
	/// The public static interface to the XCobraController objects provides a user friendly way to integrate X-Cobra controllers into your application.
	/// </remarks>
	public class XHawkInput:MonoBehaviour {

		#region Static

		/// <summary>
		/// Max number of X-Cobra controllers allowed by X-Hawk.
		/// </summary>
		public const int MAX_CONTROLLERS = 2;

		/// <summary>
		/// 
		/// </summary>
		protected static XCobraController[] m_Controllers = new XCobraController[MAX_CONTROLLERS];

		/// <summary>
		/// Access to XCobraController objects.
		/// </summary>
		public static XCobraController[] Controllers { get { return m_Controllers; } }

		// <summary>
		/// Gets the XCobraController object bound to the specified hand.
		/// </summary>
		public static XCobraController GetController(XCobraHands hand) {
			for(int i = 0;i<MAX_CONTROLLERS;i++) {
				if((m_Controllers[i]!=null)&&(m_Controllers[i].Hand==hand)) {
					return m_Controllers[i];
				}
			}
			return null;
		}

		/// <summary>
		/// The X-Hawk Transform in Unity3D.
		/// </summary>
		public Transform pivot {
			get {
				//
				if(m_Pivot==null) {
					VRContext vrCtx=VRContext.Main;
					if(vrCtx==null || vrCtx.centerEyeAnchor==null) {
						m_Pivot=new GameObject("XHawkInput Pivot").transform;
					}else {
						m_Pivot=vrCtx.centerEyeAnchor;
					}
					//DontDestroyOnLoad(m_Pivot);
				}
				return m_Pivot;
			}
		}

		#region Swap

		// These functions will be deprecated in the future.

		// <!--

		/// <summary>
		/// Left XCobraController swap data with right XCobraController.
		/// </summary>
		public static void SwapControllers() {
			LibXHawkAPI.SwapControllers(true,true);
		}
	
		/// <summary>
		///  Left XCobraController swap blob data with right XCobraController.
		/// </summary>
		public static void SwapBlobs() {
			LibXHawkAPI.SwapControllers(true,false);
		}
	
		/// <summary>
		/// Left XCobraController swap ble data with right XCobraController.
		/// </summary>
		public static void SwapBles() {
			LibXHawkAPI.SwapControllers(false,true);
		}

		// -->

		#endregion Swap

		#endregion Static

		#region Fields
		
		[Header("Tracking")]

		/// <summary>
		/// 
		/// </summary>
		[SerializeField]protected Transform m_Pivot;
		public bool autoHideXCobra=true;
		public Vector3 sensitivity=Vector3.one*0.001f;// X-Hawk units are in mm
		
		[Header("Input")]

		/// <summary>
		/// 
		/// </summary>
		public float axisDeadzone=0.1f;

		protected LibXHawkAPI.joyinfo[] m_JoyInfos;

		#endregion Fields

		#region Unity Messages

		/// <summary>
		///  Initialize the X-Hawk and allocate the X-Cobra Controllers.
		/// </summary>
		protected virtual void Awake() {
			//
			int ret=LibXHawkAPI.Init();
			m_JoyInfos=new LibXHawkAPI.joyinfo[2];
			for(int i=0;i<MAX_CONTROLLERS;++i) {
				m_JoyInfos[i].Init();
				m_Controllers[i]=new XCobraController();
				m_Controllers[i].Awake(this);
				m_Controllers[i].HandBind=(XCobraHands)i;// Modify the hand type.
			}
			//
			if(ret!=0) {
				enabled=false;
				return;
			}
		}

		/// <summary>
		///  Update the static controller data once per frame.
		/// </summary>
		protected virtual void Update() {
			int ret=LibXHawkAPI.Update(m_JoyInfos);
			//Log.i("XHawkInput",ret.ToString());
			if(ret==0) {
				for(int i=0;i<MAX_CONTROLLERS;++i) {
					m_Controllers[i].Update(ref m_JoyInfos[i]);
				}
			}
		}

		/// <summary>
		/// Exit X-Hawk library.
		/// </summary>
		protected virtual void OnDestroy() {
			LibXHawkAPI.Exit();
		}

		#endregion Unity Messages

	}

}
using UnityEngine;
using UnityEngine.EventSystems;
using Ximmerse.CrossInput;

namespace Ximmerse.UI{

	/// <summary>
	/// 
	/// </summary>
	public class UIRayCursor:MonoBehaviour{

		#region Static

		protected static Vector3 s_Fwd=Vector3.forward;
		protected static RaycastHit s_HitInfo=new RaycastHit();

		#endregion Static

		#region Fields

		//
		public Transform target;
		public LayerMask m_LayerMask;
		[System.NonSerialized]public int layerMask;
		public float distance=1.0f;
		public string button0="Fire1";
		public float clickTime=.5f;
		//
		protected PointerEventData m_PointerData=new PointerEventData(EventSystem.current);
		protected RaycastHit m_HitInfo,m_HitInfoPrev=s_HitInfo;
		protected float m_TimeClick;

		#endregion Fields

		#region Unity Messages

		/// <summary>
		/// 
		/// </summary>
		protected virtual void Awake() {
			if(target==null){
				target=transform;
			}
			layerMask=m_LayerMask.value;
		}
		
		/// <summary>
		/// 
		/// </summary>
		protected virtual void Update() {
			if(Physics.Raycast(target.position,target.rotation*s_Fwd,out m_HitInfo,distance,layerMask)){
				//
				UIDelegate ui=m_HitInfo.transform.GetComponent<UIDelegate>();
				if(ui==null) return;
				// Raw
				// Enter first time.
				if(m_HitInfo.collider!=m_HitInfoPrev.collider){
					if(ui.onPointerEnter!=null){
						ui.onPointerEnter(m_PointerData);
					}
					if(m_HitInfoPrev.transform!=null){
						UIDelegate uiPrev=m_HitInfoPrev.transform.GetComponent<UIDelegate>();
						if(uiPrev!=null){
							if(uiPrev.onPointerExit!=null){
								uiPrev.onPointerExit(m_PointerData);
							}
						}
					}
				}
				//
				if(CrossInputManager.GetButtonDown(button0)){
					if(ui.onPointerDown!=null){
						ui.onPointerDown(m_PointerData);
					}
					m_TimeClick=Time.time+clickTime;
				}else if(CrossInputManager.GetButtonUp(button0)){
					if(ui.onPointerUp!=null){
						ui.onPointerUp(m_PointerData);
					}
					if(Time.time<=m_TimeClick){
						if(ui.onPointerClick!=null){
							ui.onPointerClick(m_PointerData);
						}
						m_TimeClick=float.MaxValue;
					}
				}
				m_HitInfoPrev=m_HitInfo;
			}else{
				// Copy
				if(m_HitInfoPrev.transform!=null){
					UIDelegate uiPrev=m_HitInfoPrev.transform.GetComponent<UIDelegate>();
					if(uiPrev!=null){
						if(uiPrev.onPointerExit!=null){
							uiPrev.onPointerExit(m_PointerData);
						}
					}
					m_HitInfoPrev=s_HitInfo;
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		protected virtual void OnDrawGizmosSelected() {
			Gizmos.color=new Color(1.0f,0,0,0.5f);
			Gizmos.DrawSphere(m_HitInfo.point,.075f);
			//if(target!=null) {
			//	Gizmos.color=Color.green;
			//	Gizmos.DrawRay(target.position,target.TransformDirection(s_Fwd)*distance);
			//}
		}
		
		#endregion Unity Messages
	
	}
}
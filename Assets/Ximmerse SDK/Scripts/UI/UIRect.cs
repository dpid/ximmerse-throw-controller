using UnityEngine;

namespace Ximmerse.UI{

	/// <summary>
	/// 
	/// </summary>
	public interface IResizer{

		/// <summary>
		/// 
		/// </summary>
		void OnSizeChanged();

	}

	/// <summary>
	/// Ximmerse UI事件的基本组件.
	/// </summary>
	[AddComponentMenu("UI/Rect")]
	public class UIRect:MonoBehaviour,IResizer {

		#region Static

		public delegate void VoidDelegate();

		/// <summary>
		/// /*强制所有*/组件更新尺寸.
		/// </summary>
		public /*static*/ VoidDelegate onSizeChanged;

		// 辅助结构体.
		public static Rect s_HelperRect;
		public static Vector3 s_HelperCenter=Vector3.zero,s_HelperSize=Vector3.zero;

		#endregion Static

		#region Fields

		[System.NonSerialized]protected Transform m_Transform;
		[System.NonSerialized]protected RectTransform m_RectTransform;
	
		public BoxCollider m_BoxCollider;
		public bool isTrigger;
		public float sizeZ=1.0f;

		#endregion Fields

		#region Unity Messages

		/// <summary>
		/// 
		/// </summary>
		protected virtual void Awake(){
			//
			m_RectTransform=this.GetComponent<RectTransform>();m_Transform=transform;
			//
			if(m_BoxCollider==null)
				m_BoxCollider=this.GetComponent<BoxCollider>();
			if(m_BoxCollider==null){
				m_BoxCollider=gameObject.AddComponent<BoxCollider>();
			}
			m_BoxCollider.isTrigger=isTrigger;
			//
			onSizeChanged+=OnSizeChanged;
			OnSizeChanged();
		}
	
		/// <summary>
		/// 
		/// </summary>
		protected virtual void OnDestroy(){
			onSizeChanged-=OnSizeChanged;
		}

		#endregion Unity Messages

		#region IResizer

		/// <summary>
		/// 
		/// </summary>
		public virtual void OnSizeChanged(){
			//
			s_HelperRect=m_RectTransform.rect;
			s_HelperCenter.Set(
				s_HelperRect.x+s_HelperRect.width*.5f,
				s_HelperRect.y+s_HelperRect.height*.5f,
			0f);
			s_HelperSize.Set(
				s_HelperRect.width,
				s_HelperRect.height,
			sizeZ);
			//
			m_BoxCollider.center=s_HelperCenter;
			m_BoxCollider.size=s_HelperSize;
		}

		#endregion IResizer

	}
}
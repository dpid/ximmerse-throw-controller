//#define USE_DEBUG

using UnityEngine;
using UnityEngine.UI;

namespace Ximmerse.UI {

	/// <summary>
	/// 
	/// </summary>
	public class UILayout:MonoBehaviour {

		#region Const/Enum

		public enum Methods:int{
			Fast,
			Precise,
		}
			
			
		public const int 
			s_Sleep          =-1,
			s_Awake          = 0,
			s_ParentUpdated  = 1;

		#endregion Const/Enum

		#region Helper APIs

		/// <summary>
		/// 
		/// </summary>
		public static void GetRect(Transform parent,RectTransform child,ref Rect rect){
			rect=child.rect;
			rect.min=parent.InverseTransformPoint(
				child.TransformPoint(rect.min)
			);
			rect.max=parent.InverseTransformPoint(
				child.TransformPoint(rect.max)
			);
		}

		/// <summary>
		/// 
		/// </summary>
		public static void GetRectSize(RectTransform rectTrans,ref int o_w,ref int o_h) {
			o_w=o_h=0;
			Rect rect=new Rect();
			int numChildren=rectTrans.childCount;
			float l=0.0f,r=0.0f,t=0.0f,b=0.0f;
			if(numChildren>=1){
				GetRect(
					rectTrans,
					(rectTrans.GetChild(0)) as RectTransform,
					ref rect
				);
				l=rect.xMin;
				r=rect.xMax;
				b=rect.yMin;
				t=rect.yMax;
			}
			if(numChildren>=2){
				foreach(RectTransform child in rectTrans){
					GetRect(
						rectTrans,
						child,
						ref rect
					);
					l=Mathf.Min(rect.xMin,l);
					r=Mathf.Max(rect.xMax,r);
					b=Mathf.Min(rect.yMin,b);
					t=Mathf.Max(rect.yMax,t);
#if USE_DEBUG
					Debug.Log(string.Format(
						"l={0},r={1},t={2},b={3}",
						l,r,t,b
					));
#endif
				}
			}
			o_w=(int)(r-l);
			o_h=(int)(t-b);
		}
		
		#endregion Helper APIs

		#region Fields
	
		public bool autoResize=false;
		public RectTransform parent,content;
		
		[Header("Layout")]
		public RectOffset padding=new RectOffset();

		public Methods method=Methods.Precise;
		public RectTransform.Axis align=RectTransform.Axis.Vertical;
		public float childSize=-1f;
		
		protected GridLayoutGroup m_LayoutGroup;
		protected Rect m_ChildRect;
		protected bool m_IsDirty=false;
		protected int m_Phase,m_Wait;

		#endregion Fields
		
		#region Unity Messages
		
		/// <summary>
		/// 
		/// </summary>
		protected virtual void Awake(){
			if(content==null){
				content=GetComponent<RectTransform>();
			}
			parent=content.parent as RectTransform;
			m_LayoutGroup=content.GetComponent<GridLayoutGroup>();
			if(m_LayoutGroup!=null){
				padding=m_LayoutGroup.padding;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		protected virtual void Update(){
			if(m_IsDirty){
				if(--m_Wait>=0){
					return;
				}
				switch(m_Phase){
					case s_Awake:
						Rect rect;
						switch(method){
							case Methods.Fast:
								if(childSize==-1f){
									rect=m_ChildRect=(content.GetChild(0) as RectTransform).rect;
									childSize=(align==RectTransform.Axis.Horizontal)?rect.width:rect.height;
								}
								if(m_LayoutGroup!=null){
									if(align==RectTransform.Axis.Horizontal){
										m_LayoutGroup.cellSize=new Vector2(childSize,parent.rect.height);
									}else{
										m_LayoutGroup.cellSize=new Vector2(parent.rect.width,childSize);
									}
								}
								content.SetSizeWithCurrentAnchors(align,content.childCount*childSize);
								m_Phase=s_Sleep;
								m_IsDirty=false;
							break;
							case Methods.Precise:
								rect=parent.rect;
								content.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,m_PWidth=rect.width);
								content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical  ,m_PHeight=rect.height);
#if USE_DEBUG
								print(string.Format("w:{0} h:{1}",rect.width,rect.height));
#endif
								m_Wait=2;
								m_Phase=s_ParentUpdated;
							break;
						}
					break;
					case s_ParentUpdated:
						int w=0,h=0;
						switch(method){
							case Methods.Fast:
							break;
							case Methods.Precise:
								GetRectSize(content,ref w,ref h);
								content.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,w +padding.left+padding.right);
								content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical  ,h +padding.bottom+padding.top);
							break;
						}
#if USE_DEBUG
						print(string.Format("w:{0} h:{1}",w,h));
#endif
						m_Phase=s_Sleep;
						m_IsDirty=false;
					break;
				}
			}else if(autoResize){
				if(IsParentSizeChanged()){
					Refresh();
					return;
				}
			}
		}
		
		#endregion Unity Messages

		#region APIs

		[System.NonSerialized]protected float m_PWidth=-1.0f,m_PHeight=-1.0f;

		/// <summary>
		/// 
		/// </summary>
		public virtual bool IsParentSizeChanged() {
			if(m_PWidth<=0.0f||m_PHeight<=0.0f){
				return false;
			}
			Rect rect=parent.rect;
			return rect.width!=m_PWidth||rect.height!=m_PHeight;
		}
		
		/// <summary>
		/// 
		/// </summary>
		[ContextMenu("Refresh")]
		public virtual void Refresh(){
			//Rect rect=parent.rect;
			m_Wait=2;
			m_Phase=s_Awake;
			m_IsDirty=true;
		}

		#endregion APIs

	}

}
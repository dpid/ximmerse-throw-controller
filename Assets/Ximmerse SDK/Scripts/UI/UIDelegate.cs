using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace Ximmerse.UI{

	/// <summary>
	/// 
	/// </summary>
	public delegate void PointerDelegate(PointerEventData i_data);

	/// <summary>
	/// 
	/// </summary>
	[AddComponentMenu("UI/Delegate")]
	public class UIDelegate:UIRect{

		#region Public

		public static List<UIDelegate> instances=new List<UIDelegate>();
	
		public int evtLvl=0;
		public float time=0.0f;

		[System.NonSerialized]public bool isCached=false;

		#endregion//Public

		#region Event

		//
		public PointerDelegate onPointerClick;
		public PointerDelegate onPointerDown;
		public PointerDelegate onPointerUp;
		public PointerDelegate onPointerEnter;
		public PointerDelegate onPointerExit;
		public PointerDelegate onBeginDrag;
		public PointerDelegate onEndDrag;
		public PointerDelegate onDrag;
	
		#endregion//Event

		#region Func

		protected virtual void Start(){
			instances.Add(this);
			Init();//StartCoroutine(InitDelayed(2.0f));
		}

		public virtual System.Collections.IEnumerator InitDelayed(float i_delay){
			yield return new WaitForSeconds(i_delay);
			Init();
		}

		public virtual void Init(){
			if(isCached) return;
			//
			IPointerClickHandler iPointerClick;
			IPointerDownHandler iPointerDown;
			IPointerUpHandler iPointerUp;
			IPointerEnterHandler iPointerEnter;
			IPointerExitHandler iPointerExit;
			IBeginDragHandler iBeginDrag;
			IEndDragHandler iEndDrag;
			IDragHandler iDrag;
			//
			MonoBehaviour[] coms=this.GetComponents<MonoBehaviour>();
			MonoBehaviour com;
			for(int i=0,imax=coms.Length;i<imax;++i){com=coms[i];
			//foreach(MonoBehaviour com in this.GetMonoBehaviours<MonoBehaviour>()){
				// <0>
				iPointerClick=com as IPointerClickHandler;
				if(iPointerClick!=null){
					onPointerClick+=iPointerClick.OnPointerClick;
				}else{
				// <0>
				iPointerDown=com as IPointerDownHandler;
				if(iPointerDown!=null){
					onPointerDown+=iPointerDown.OnPointerDown;
				}
				// <0>
				iPointerUp=com as IPointerUpHandler;
				if(iPointerUp!=null){
					onPointerUp+=iPointerUp.OnPointerUp;
				}
				}
				// <0>
				iPointerEnter=com as IPointerEnterHandler;
				if(iPointerEnter!=null){
					onPointerEnter+=iPointerEnter.OnPointerEnter;
				}
				// <0>
				iPointerExit=com as IPointerExitHandler;
				if(iPointerExit!=null){
					onPointerExit+=iPointerExit.OnPointerExit;
				}
				// <0>
				iBeginDrag=com as IBeginDragHandler;
				if(iBeginDrag!=null){
					onBeginDrag+=iBeginDrag.OnBeginDrag;
				}
				// <0>
				iEndDrag=com as IEndDragHandler;
				if(iEndDrag!=null){
					onEndDrag+=iEndDrag.OnEndDrag;
				}
				// <0>
				iDrag=com as IDragHandler;
				if(iDrag!=null){
					onDrag+=iDrag.OnDrag;
				}
			}
			isCached=true;
		}

		#endregion//Func

	}

}
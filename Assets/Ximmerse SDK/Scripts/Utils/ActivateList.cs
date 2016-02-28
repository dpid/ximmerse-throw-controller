using UnityEngine;
using System.Collections;

/// <summary>
/// <para>Activate or Deactivate a lot of GameObjects and Behaviours by a boolean variable.</para>
/// <para>NOTE:it's a Binary Choice Model.For an example,it can make a better choice between normal mode and OVR mode.<para>
/// </summary>
public class ActivateList:MonoBehaviour {

	#region Helper

	/// <summary>
	/// 
	/// </summary>
	public static void SetGameObjects(GameObject[] gos,bool value) {
		GameObject item;
		for(int i=0,imax=gos.Length;i<imax;++i) {
			item=gos[i];
			//foreach(GameObject item in gos){
			if(item!=null) {
				item.SetActive(value);
			}
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public static void SetEachGameObject(GameObject[] gos,bool[] values) {
		GameObject item;
		for(int i=0,imax=gos.Length;i<imax;++i) {
			item=gos[i];
			//foreach(GameObject item in gos){
			if(item!=null) {
				item.SetActive(values[i]);
			}
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public static void SetBehaviours<T>(T[] coms,bool value) where T:Behaviour {
		T item;
		for(int i=0,imax=coms.Length;i<imax;++i) {
			item=coms[i];
			//foreach(Behaviour item in gos){
			if(item!=null) {
				item.enabled=value;
			}
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public static void SetColliders<T>(T[] coms,bool value) where T:Collider {
		T item;
		for(int i=0,imax=coms.Length;i<imax;++i) {
			item=coms[i];
			//foreach(Behaviour item in gos){
			if(item!=null) {
				item.enabled=value;
			}
		}
	}

#if UNITY_EDITOR

	/// <summary>
	/// 
	/// </summary>
	[ContextMenu("Add To Actives")]
	protected virtual void AddToActives(){
		UnityEditor.ArrayUtility.AddRange<GameObject>(ref goActives,UnityEditor.Selection.gameObjects);
	}

	/// <summary>
	/// 
	/// </summary>
	[ContextMenu("Add To Deactives")]
	protected virtual void AddToDeactives(){
		UnityEditor.ArrayUtility.AddRange<GameObject>(ref goDeactives,UnityEditor.Selection.gameObjects);
	}

#endif

	#endregion Helper

	public bool doOnAwake=false,doOnStart=false;
	public bool value=true;

	#region List

	public GameObject[]
		goActives=new GameObject[0];
	public Behaviour[]
		comActives=new Behaviour[0];

	public GameObject[]
		goDeactives=new GameObject[0];
	public Behaviour[]
		comDeactives=new Behaviour[0];

	#endregion List

	#region Unity Messages

	/// <summary>
	/// 
	/// </summary>
	protected virtual void Awake() {
		if(doOnAwake)
			SetValueForce(value);
	}

	/// <summary>
	/// 
	/// </summary>
	protected virtual void Start() {
		if(doOnStart)
			SetValueForce(value);
	}

	#endregion Unity Messages

	#region API

	/// <summary>
	/// 
	/// </summary>
	public virtual void Enable() {
		SetValue(true);
	}

	/// <summary>
	/// 
	/// </summary>
	public virtual void Disable() {
		SetValue(false);
	}

	/// <summary>
	/// 
	/// </summary>
	public virtual void Toggle() {
		SetValue(!value);
	}

	/// <summary>
	/// 
	/// </summary>
	public virtual void SetValueForce(bool val) {
		if(val==value)
			SetValue(!(value=!value));
		else
			SetValue(!value);
	}

	/// <summary>
	/// 
	/// </summary>
	public virtual void SetValue(bool val) {
		if(value==val) {
			return;
		}
		value=val;

		SetGameObjects(goActives,value);
		SetBehaviours<Behaviour>(comActives,value);

		SetGameObjects(goDeactives,!value);
		SetBehaviours<Behaviour>(comDeactives,!value);
	}

	#endregion API

}

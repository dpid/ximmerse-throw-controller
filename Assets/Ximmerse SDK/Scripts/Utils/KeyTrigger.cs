using UnityEngine;
using System.Collections;

public class KeyTrigger : MonoBehaviour {

	[System.Serializable]
	public class KeyEntry {
		public KeyOrButton key;
		//public bool value=true;
		public UnityEngine.Events.UnityEvent onTrigger;/*,onFalse;

		public void Toggle() {
			value=!value;
			if(value) {
				onTrue.Invoke();
			} else {
				onFalse.Invoke();
			}
		}*/
	}

	public KeyEntry[] keys=new KeyEntry[0];
	protected int _numKeys=-1;


	/// <summary>
	/// 
	/// </summary>
	protected virtual void Start() {
		_numKeys=keys.Length;
	}

	/// <summary>
	/// 
	/// </summary>
	protected virtual void Update() {
		for(int i=0;i<_numKeys;++i) {
			if(keys[i].key.GetAnyDown()) {
				keys[i].onTrigger.Invoke();
			}
		}
	}
}


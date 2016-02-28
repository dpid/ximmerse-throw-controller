using UnityEngine;

/// <summary>
/// Disable Ximmerse.Log.dll's print function.
/// </summary>
public class DoNotLog:MonoBehaviour {

	/// <summary>
	/// 
	/// </summary>
	protected virtual void Awake() {
		Log.s_Filter=0;
	}

}

using UnityEngine;
using System.Collections;

/// <summary>
/// Call UnityEngine.Application by UnityEngine.Events.UnityEvent,etc.
/// </summary>
public partial class MonoApplication:MonoBehaviour {
	// Application.();
	
	public UnityEngine.Events.UnityEvent onAsyncScene;

	/// <summary>
	/// 
	/// </summary>
	public IEnumerator Coroutine(AsyncOperation async) {
		yield return async;
		onAsyncScene.Invoke();
	}

	/// <summary>
	/// 
	/// </summary>
	public void ReloadLevel(){
		Application.LoadLevel(Application.loadedLevel);
	}

	/// <summary>
	/// 
	/// </summary>
	public void ReloadLevelAsync(){
		StartCoroutine(Coroutine(Application.LoadLevelAsync(Application.loadedLevel)));
	}

	#region UnityEngine.Application
	
	/// <summary>
	/// 
	/// </summary>
	public void OpenURL(string url) {
		Application.OpenURL(url);
	}

	/// <summary>
	/// 
	/// </summary>
	public void Quit(){
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying=false;
#else
		Application.Quit();
#endif
	}

	/// <summary>
	/// 
	/// </summary>
	public void CancelQuit(){
		Application.CancelQuit();
	}

	/// <summary>
	/// 
	/// </summary>
	public void LoadLevelByIndex(int index){
		Application.LoadLevel(index);
	}

	/// <summary>
	/// 
	/// </summary>
	public void LoadLevelByName(string name){
		Application.LoadLevel(name);
	}

	/// <summary>
	/// 
	/// </summary>
	public void LoadLevelAdditiveByIndex(int index){
		Application.LoadLevelAdditive(index);
	}

	/// <summary>
	/// 
	/// </summary>
	public void LoadLevelAdditiveByName(string name){
		Application.LoadLevelAdditive(name);
	}

	/// <summary>
	/// 
	/// </summary>
	public void LoadLevelAsyncByIndex(int index){
		StartCoroutine(Coroutine(Application.LoadLevelAsync(index)));
	}

	/// <summary>
	/// 
	/// </summary>
	public void LoadLevelAsyncByName(string name){
		StartCoroutine(Coroutine(Application.LoadLevelAsync(name)));
	}

	/// <summary>
	/// 
	/// </summary>
	public void LoadLevelAdditiveAsyncByIndex(int index){
		StartCoroutine(Coroutine(Application.LoadLevelAdditiveAsync(index)));
	}

	/// <summary>
	/// 
	/// </summary>
	public void LoadLevelAdditiveAsyncByName(string name){
		StartCoroutine(Coroutine(Application.LoadLevelAdditiveAsync(name)));
	}
	
	#endregion UnityEngine.Application

}
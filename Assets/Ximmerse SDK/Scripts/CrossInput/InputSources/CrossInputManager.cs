using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ximmerse.CrossInput {

	/// <summary>
	/// 
	/// </summary>
	public partial class CrossInputManager:MonoBehaviour {

		public static CrossInputManager main;

		public bool useCrossInput=true,dontDestroyOnLoad=true;
		[SerializeField]protected MonoBehaviour[] m_Sources=new MonoBehaviour[0];
		public List<IInputSource> sources;

		[System.NonSerialized]public int numSources;
		protected WaitForEndOfFrame _waitEof;

		/// <summary>
		/// 
		/// </summary>
		protected virtual void Awake() {
			//
			if(main!=null) {
				if(main!=this) {
					Object.Destroy(this.gameObject);
					return;
				}
			}
			//
			if(!useCrossInput) {
				// Roll back...
				CrossInputManager.activeInput=new StandaloneInput();
				Object.Destroy(this.gameObject);
				return;
			}

			CrossInputManager.activeInput=new CustomizeInput();
			main=this;
			if(dontDestroyOnLoad) {
				Object.DontDestroyOnLoad(this.gameObject);
			}

			IInputSource source;
			numSources=0;
			int i=0,imax=m_Sources.Length;
			sources=new List<IInputSource>(imax);
			for(;i<imax;++i) {
				// Fix gameObject...
				if(m_Sources[i]==null||!m_Sources[i].gameObject.activeInHierarchy)
					continue;
				source=m_Sources[i] as IInputSource;
				if(source!=null) {
					if(source.enabled) {
						if(source.InitInput()==0) {
							sources.Add(source);
							numSources++;
						}
					}
				}
			}
			//
			PrintLogOnStartup();
			//
			_waitEof=new WaitForEndOfFrame();
			StartCoroutine(UpdateOnEof());
		}

		/// <summary>
		/// 
		/// </summary>
		protected virtual void Update() {
			CrossInputManager.timestamp=Time.frameCount;
			for(int i=0;i<numSources;++i) {
				sources[i].EnterInputFrame();
			}
		}

		/// <summary>
		/// 
		/// </summary>
		protected virtual IEnumerator UpdateOnEof() {
			while(enabled) {
				yield return _waitEof;
				for(int i=0;i<numSources;++i) {
					sources[i].ExitInputFrame();
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		protected virtual void OnDestroy() {
			CrossInputManager.timestamp=Time.frameCount;
			for(int i=0;i<numSources;++i) {
				sources[i].ExitInput();
			}
		}

		/// <summary>
		/// 
		/// </summary>
		protected virtual void PrintLogOnStartup(){
			//
			System.Text.StringBuilder sb=
				new System.Text.StringBuilder();
			sb.AppendLine(numSources.ToString());
			//
			Log.i("CrossInputManager",sb.ToString());
		}

	}

}
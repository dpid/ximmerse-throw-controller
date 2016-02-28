using System.Collections.Generic;
using UnityEngine;

public class Juggler:MonoBehaviour,IAnimatable {
	
	/// <summary>
	/// 
	/// </summary>
	public static Juggler Main {
		get{
			if(main==null){
				main=new GameObject("New Juggler",typeof(Juggler)).GetComponent<Juggler>();
				Object.DontDestroyOnLoad(main);
			}
			return main;
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public static Juggler main;

	public List<IAnimatable> m_List=new List<IAnimatable>(16);
	
	public bool asMain;
	public float elapsedTime=0.0f;
	protected bool m_IsLock;

	#region Unity Messages

	/// <summary>
	/// 
	/// </summary>
	protected virtual void Awake() {
		elapsedTime=0.0f;
		if(asMain){
		if(main==null){
			main=this;
			//Object.DontDestroyOnLoad(main);
		}}
	}

	/// <summary>
	/// 
	/// </summary>
	protected virtual void Update() {
		AdvanceTime(Time.deltaTime);
	}

	#endregion Unity Messages

	#region IAnimatable

	/// <summary>
	/// 
	/// </summary>
	public virtual void AdvanceTime(float deltaTime) {
		int imax=m_List.Count;
		int currentIndex=0;
		int i;

		elapsedTime+=deltaTime;
		if(imax==0)
			return;

		// there is a high probability that the "advanceTime" function modifies the list 
		// of animatables. we must not process new objects right now (they will be processed
		// in the next frame), and we need to clean up any empty slots in the list.

		for(i=0;i<imax;++i) {
			IAnimatable item=m_List[i];
			if(item!=null) {
				// shift object into empty slots along the way
				if(currentIndex!=i) {
					m_List[currentIndex]=item;
					m_List[i]=null;
				}

				item.AdvanceTime(deltaTime);
				++currentIndex;
			}
		}

		if(currentIndex!=i) {
			imax=m_List.Count; // count might have changed!
			while(i<imax) {
				m_List[currentIndex++]=m_List[i++];
			}
			m_List.RemoveRange(currentIndex,imax-currentIndex);
		}
	}

	#endregion IAnimatable

	#region APIs

	public virtual void Add(IAnimatable item) {
		//print(Time.realtimeSinceStartup);
		int i=m_List.IndexOf(item);
		if(i==-1) {
			m_List.Add(item);
		}
	}

	public virtual void Remove(IAnimatable item) {
		//print(Time.realtimeSinceStartup);
		int i=m_List.IndexOf(item);
		if(i!=-1) {
			m_List[i]=null;
		}
	}

	public virtual DelayedCall DelayCall(DelayedCall.Call call,float delay) {
		if(call==null)
			return null;
		
		DelayedCall delayedCall=DelayedCall.Pop();
		delayedCall.juggler=this;
		delayedCall.Init(
			delay,0.0f,DelayedCall.k_OneShot,call,null
		);
		Add(delayedCall);
		return delayedCall;
	}

	public virtual DelayedCall RepeatCall(DelayedCall.Call call,float delay,float duration,int repeatCount) {
		if(call==null)
			return null;
		
		DelayedCall delayedCall=DelayedCall.Pop();
		delayedCall.juggler=this;
		delayedCall.Init(
			delay,duration,repeatCount,call,null
		);
		Add(delayedCall);
		return delayedCall;
	}

	public virtual DelayedCall UpdateCall(DelayedCall.Call call,float delay,float duration) {
		if(call==null)
			return null;
		
		DelayedCall delayedCall=DelayedCall.Pop();
		delayedCall.juggler=this;
		delayedCall.Init(
			delay,duration,DelayedCall.k_EveryFrame,call,null
		);
		Add(delayedCall);
		return delayedCall;
	}

	#endregion APIs
}
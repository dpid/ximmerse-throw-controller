using System.Collections.Generic;

/// <summary>
/// 
/// </summary>
public class DelayedCall:IAnimatable {

	//
	public static DelayedCall current;

	public const int
		k_OneShot=-1,
		k_EveryFrame=-2;

	public static readonly object[] s_EmptyArgs=new object[0];
	public delegate void Call(
#if DELAYED_CALL_HAS_ARGS
		object[] args
#endif
);
	/// <summary>
	/// 
	/// </summary>
	public static void Free(ref DelayedCall delayedCall){
		if(delayedCall!=null){
		if(delayedCall.m_IsRunning){
			delayedCall.Abort();
		}}
		delayedCall=null;
	}

	#region Pooling

	public static List<DelayedCall> s_Pool=new List<DelayedCall>(16);
	public static int s_LenPool=0;

	/// <summary>
	/// 
	/// </summary>
	public static DelayedCall Pop(){
		DelayedCall ret=null;
		if(s_LenPool==0){
			ret=new DelayedCall();
		}else{
			--s_LenPool;
			ret=DelayedCall.s_Pool[s_LenPool];
			s_Pool.RemoveAt(s_LenPool);
		}
		ret.m_IsRunning=true;
		return ret;
	}

	/// <summary>
	/// 
	/// </summary>
	public static void Push(DelayedCall item){
		int i=s_Pool.IndexOf(item);
		if(i!=-1){
			Log.w("DelayedCall","Push failed...");
			return;
		}else{
			item.Reset();// ResetToBeginning
			s_Pool.Add(item);++s_LenPool;// Push
		}
	}

	#endregion Pooling

	#region Members

	public Juggler juggler=null;
	public float delay=0f;
	public float duration=1f;
	public int repeatCount=k_OneShot;
	public Call call=null;
	public object[] args=s_EmptyArgs;

	protected bool m_IsRunning=false;
	protected bool m_Started=false;
	protected bool m_IsDelay=false;
	protected float m_StartTime=0f;
	protected float m_Duration=0f;
	protected int m_RepeatCount=0;

	#endregion Members

	#region Built-in

	/// <summary>
	/// 
	/// </summary>
	protected DelayedCall(){
	}

	/// <summary>
	/// 
	/// </summary>
	public virtual void Abort() {
		if(m_Started){if(m_IsRunning){
			juggler.Remove(this);
			Push(this);
			m_IsRunning=false;
		}}
	}

	/// <summary>
	/// 
	/// </summary>
	public virtual void Init(
		float i_delay,float i_duration,int i_repeatCount,
		Call i_call,object[] i_args=null
	) {
		delay=i_delay;
		duration=i_duration;
		repeatCount=i_repeatCount;
		call=i_call;
		args=i_args;
	}

	/// <summary>
	/// 
	/// </summary>
	public virtual void Reset() {

		juggler=null;
		delay=0f;
		duration=1f;
		repeatCount=k_OneShot;
		call=null;
		args=s_EmptyArgs;

		m_IsRunning=false;
		m_Started=false;
		m_IsDelay=false;
		m_StartTime=0f;
		m_Duration=0f;
		m_RepeatCount=0;

	}

	#endregion Built-in

	#region IAnimatable

	/// <summary>
	/// 
	/// </summary>
	public virtual void AdvanceTime(float deltaTime) {
		float delta=deltaTime;
		float time=juggler.elapsedTime;

		if(!m_Started) {// Init Timer
			m_Started=m_IsDelay=true;
			m_StartTime=time+delay;
			m_Duration=0.0f;
			m_RepeatCount=0;
		}

		if(time<m_StartTime){
			return;
		}else if(m_IsDelay){
			m_IsDelay=false;
			if(repeatCount>k_OneShot){// Fix the [Delay Mode : Repeat]
				m_Duration+=delta;
				InvokeCall();
				++m_RepeatCount;
				if(m_RepeatCount>=repeatCount) {
					Abort();
					return;
				}
				return;
			}
		}

		switch(repeatCount){
			case k_OneShot:// Delay Mode : One Shot
				InvokeCall();
				Abort();
				return;
			//break;
			case k_EveryFrame:// Delay Mode : Every Frame
				m_Duration+=delta;
				InvokeCall();
				if(m_Duration>=duration){
					Abort();
					return;
				}
			break;
			default:// Delay Mode : Repeat
				m_Duration+=delta;
				if(m_Duration>=duration){
					m_Duration-=duration;// @ Reserve time error.
					InvokeCall();
					++m_RepeatCount;
					if(m_RepeatCount>=repeatCount){
						Abort();
						return;
					}
				}
			break;
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public virtual void InvokeCall(){
		if(call!=null){
			current=this;
			call.Invoke();
			current=null;
		}
	}

	#endregion IAnimatable

}
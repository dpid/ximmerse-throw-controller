using UnityEngine;
using Ximmerse.CrossInput;

namespace Ximmerse.Core {

	/// <summary>
	/// 
	/// </summary>
#if UNITY_5 && !UNITY_5_0
	[CreateAssetMenu(fileName="New Button Sensor",menuName="Ximmerse SDK/Sensor/Button",order=800)]
#endif
	public class SensorButton:SensorBase {

		#region Fields

		[UnityEngine.Header("Button")]
#if UNITY_EDITOR
		public string[] displayName=new string[0];
#endif
		public string[] buttonName=new string[0];

		[Tooltip("Template Field,Don't modify by yourself.")]
		public int[] buttonOffset=new int[4]{3,2,0,1};

		[System.NonSerialized]protected int m_NumButtons;
		[System.NonSerialized]protected VirtualButton[] m_Buttons;

		[System.NonSerialized]protected int m_TimeParsePrev=-1;
		[System.NonSerialized]protected int m_ButtonValue,m_ButtonValuePrev;

		#endregion Fields

		#region Override

		/// <summary>
		/// 
		/// </summary>
		public override void AwakeInput() {
			//base.Awake();
			m_NumButtons=buttonName.Length;
			//if(bufferSize!=1){
			//	bufferSize=1;// Max buttons : 8 ????
			//}
		}

		/// <summary>
		/// 
		/// </summary>
		public override void StartInput() {
			//
			m_Buttons=new VirtualButton[m_NumButtons];
			for(int i=0;i<m_NumButtons;++i){
				m_Buttons[i]=CrossInputManager.VirtualButtonReference(
					this,
					string.Format(device.fmtJoy,buttonName[i]),
					true
				);
			}
			m_ButtonValuePrev=m_ButtonValue=0;
			//
			device.buttons.AddRange(buttonName);
		}

		/// <summary>
		/// 
		/// </summary>
		public override void UpdateInput() {
			int new_,old_,offset;
			for(int i=0;i<m_NumButtons;++i){
				offset=buttonOffset[i];
				new_=(m_ButtonValue    >>offset)&0x1;
				old_=(m_ButtonValuePrev>>offset)&0x1;
				if(new_!=old_){
					if(new_==1){
						//Log.i("SensorButton",m_Buttons[i].name+" is Pressed");
						m_Buttons[i].Pressed();
					}else{
						//Log.i("SensorButton",m_Buttons[i].name+" is Released");
						m_Buttons[i].Released();
					}
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public override int Parse(byte[] buffer,int offset,int count) {
			if(CrossInputManager.timestamp>m_TimeParsePrev){//???
				m_ButtonValuePrev=m_ButtonValue;
				m_ButtonValue=buffer[offset];
				m_TimeParsePrev=CrossInputManager.timestamp;
			}
			return bufferSize;// 1
		}

		#endregion Override
	
	}
}

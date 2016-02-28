using UnityEngine;
using UnityEngine.UI;
using Ximmerse;
using Ximmerse.Core;

/// <summary>
/// 
/// </summary>
public class XHawkInputTest:MonoBehaviour {

	/// <summary>
	/// 
	/// </summary>
	public static string GetButtonString(XCobraController ctrl) {
		string buttonsText = "";
		foreach(XCobraButtons button in System.Enum.GetValues(typeof(XCobraButtons))) {
			if(ctrl.GetButton(button)) {
				if(buttonsText!="") {
					buttonsText+=" | ";
				}
				buttonsText+=button;
			}
		}
		if(buttonsText=="") {
			buttonsText="NONE";
		}
		return buttonsText;
	}
	[SerializeField]protected Text m_MainText;
	[SerializeField]protected Text[] m_Text = new Text[0];
	[System.NonSerialized]protected XCobraController[] m_Controllers;
//#if !UNITY_EDITOR&&UNITY_ANDROID
//#else
	[System.NonSerialized]protected byte[] m_Buffer;
	[System.NonSerialized]protected int m_Size=64;
//#endif

	/// <summary>
	/// 
	/// </summary>
	protected virtual void Start() {
		m_Controllers=XHawkInput.Controllers;
//#if !UNITY_EDITOR&&UNITY_ANDROID
//#else
		m_Buffer=new byte[m_Size];
//#endif
	}

	/// <summary>
	/// 
	/// </summary>
	protected virtual void Update() {
		XCobraController ctrl;
		string button="";
		if(m_MainText!=null) {
//#if !UNITY_EDITOR&&UNITY_ANDROID
//			m_MainText.text=LibXHawkAPI.XHawkGetJoystickCount().ToString()+"\r\n"+LibXHawkAPI.s_UsbProxy.buffer.ToHexString();
//#else
			int size=LibXHawkAPI.XHawkGetReadBuffer(m_Buffer,0,m_Size);
			m_MainText.text=LibXHawkAPI.XHawkGetJoystickCount().ToString()+"\r\n"+m_Buffer.ToHexString(0,size);
//#endif
		}
		for(int i=0,imax=m_Text.Length;i<imax;++i) {
			ctrl=m_Controllers[i];
			button=GetButtonString(ctrl);
			m_Text[i].text=string.Format(
				"Hand={0}\nState={1}\nJoystickX={2}\nJoystickY={3}\nTrigger={4}\nButtons={5}\nPosition={6}\nRotation={7}\n",
				ctrl.Hand,
				ctrl.State,
				ctrl.JoystickX,
				ctrl.JoystickY,
				ctrl.Trigger,
				button,
				ctrl.Position.ToString("0.000"),
				ctrl.Rotation.eulerAngles.ToString("0.000")
			);
		}
	}
}

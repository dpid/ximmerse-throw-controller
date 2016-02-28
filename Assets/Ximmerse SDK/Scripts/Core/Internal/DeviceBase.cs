using System.Collections.Generic;
using UnityEngine;
using Ximmerse.CrossInput;
using Ximmerse.IO;

namespace Ximmerse.Core {

	/// <summary>
	/// 
	/// </summary>
	public class DeviceBase:MonoBehaviour,IStreamReadCallback,IStreamOpenCallback,IInputSource {

		#region For Editor

#if UNITY_EDITOR

		/// <summary>
		/// 
		/// </summary>
		protected static int CompareByName(Object x,Object y){
			return string.Compare(x.name,y.name);
		}

		/// <summary>
		/// 
		/// </summary>
		[ContextMenu("Add Sensors")]
		protected virtual void AddSensors(){
			var list=new System.Collections.Generic.List<SensorBase>(m_Sensors);
			Object[] objs=UnityEditor.Selection.objects;
			for(int i=0,imax=objs.Length;i<imax;++i){
				if(objs[i] is SensorBase){
					list.Add(objs[i] as SensorBase);
				}
			}
			list.Sort(CompareByName);
			m_Sensors=list.ToArray();
			/**/
		}

#endif

		#endregion For Editor

		#region Static

		public static DeviceBase[] devices=new DeviceBase[4];
		
		/// <summary>
		/// 
		/// </summary>
		public virtual void SwapDevices(DeviceBase lhs,DeviceBase rhs){
			//
			lhs.m_Stream.SetOnStreamReadListener(null);
			rhs.m_Stream.SetOnStreamReadListener(null);
			//
			IStreamable stream=lhs.m_Stream;lhs.m_Stream=rhs.m_Stream;rhs.m_Stream=stream;
			//
			lhs.m_Stream.SetOnStreamReadListener(lhs);
			rhs.m_Stream.SetOnStreamReadListener(rhs);
		}

		#endregion Static

		#region Fields

		[Header("Common")]

		public string configPath;
		public string configSec;
		
		[Header("Device")]
		public string deviceName="";
		public int deviceId=0;
		public string deviceAddress="";

		//[Header("Debug")]
		//public UnityEngine.UI.Text textUnitTest;
		//public bool useUnitTest=true;
		//public float rateUnitTest=.5f;
		//public string strUnitTest="";

		/// <summary>
		/// 
		/// </summary>
		public IniReader config{
			get{
				if(m_IniReader==null){
					if(!string.IsNullOrEmpty(configPath)) {
						m_IniReader=IniReader.Open(Environment.CONFIG_PATH+configPath);
					}
				}
				return m_IniReader;
			}
		}
		[System.NonSerialized]protected IniReader m_IniReader;

		[Header("Input")]
		/// <summary>
		/// 
		/// </summary>
		public bool asVirtualController=true;
		public string fmtJoy;

		public string poseName;
		[System.NonSerialized]public List<string> axes,buttons;

		public bool canReset=true;
		public KeyOrButton keyReset;

		[Header("")]
		[SerializeField]protected SensorBase[] m_Sensors=new SensorBase[0];

		[System.NonSerialized]public bool isDirty=false;
		[System.NonSerialized]public SensorBase[] sensors;
		[System.NonSerialized]public List<SensorBase> resetables;//
		
		[System.NonSerialized]protected VirtualController m_VirtualController;
		[System.NonSerialized]protected IStreamable m_Stream;
		[System.NonSerialized]protected bool m_IsDynamicStream=false;
		[System.NonSerialized]protected byte[] m_Buffer;

		[System.NonSerialized]protected int m_NumSensors,m_BufferSize,m_BufferOffset;
		
		//Unit Test

		protected int m_CountRec=0;
		[System.NonSerialized]protected int m_CountRecPrev=0;
		[System.NonSerialized]protected float m_TimeUT=0.0f,m_TimeUTPrev=0.0f;

		#endregion Fields
		
		#region Unity Messages

		/// <summary>
		/// 
		/// </summary>
		protected virtual void Awake(){
		}

		/// <summary>
		/// 
		/// </summary>
		protected virtual void Update() {

			#region Reset

			if(canReset){
				if(keyReset.GetAnyDown()){
					for(int i=0,imax=resetables.Count;i<imax;++i){
						resetables[i].ResetInput();
					}
				}
			}

			#endregion Reset
		
		}

		/// <summary>
		/// 
		/// </summary>
		protected virtual void OnDestroy(){
			// Don't call ExitInput() in OnDestroy(),
			// it will dispose in CrossInputManager.OnDestroy().
			//ExitInput();
		}

		/// <summary>
		/// 
		/// </summary>
		//protected virtual void OnGUI(){
		//	if(useUnitTest){
		//		m_TimeUT=Time.realtimeSinceStartup;
		//		if(m_TimeUT-m_TimeUTPrev>rateUnitTest){
		//			strUnitTest=string.Format(
		//				"Rate:{0} Delta:{1}s",
		//				(m_CountRec-m_CountRecPrev)/(m_TimeUT-m_TimeUTPrev),
		//				(m_TimeUT-m_TimeUTPrev)
		//			);
		//			m_TimeUTPrev=m_TimeUT;
		//			m_CountRecPrev=m_CountRec;
		//			//
		//			if(textUnitTest!=null){
		//				textUnitTest.text=strUnitTest;
		//			}
		//		}
		//	}
		//}

		#endregion Unity Messages

		#region Other Messages

		/// <summary>
		/// 
		/// </summary>
		public virtual void OnStreamOpenSuccess(IStreamable stream) {
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual void OnStreamOpenFailure(IStreamable stream) {
			Ximmerse.IO.Ports.BluetoothScannerGUI.RequestAddress(OpenStream);
#if !UNITY_EDITOR && UNITY_ANDROID
#endif
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual void OpenStream(string i_path) {
			if(m_Stream!=null){
				m_Stream.address=i_path;
				m_Stream.Open();
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual void OnStreamRead(IStreamable stream) {
			//
			if(m_Stream==stream) {
				//if(useUnitTest) {
				//	++m_CountRec;
				//}
				stream.GetReadBuffer(out m_Buffer,out m_BufferOffset,out m_BufferSize);
				//Log.i("DeviceBase","OnStreamRead()@size : "+size);return;
				Parse(m_Buffer,m_BufferOffset,m_BufferSize);
				isDirty=true;
			}
		}

		#endregion Other Messages

		#region Methods

		/// <summary>
		/// 
		/// </summary>
		protected virtual void LoadConfig(){
			//
			if(m_IniReader==null){
				Log.e("DeviceBase","Loading config file failed.");
				return;
			}else{
			}
			fmtJoy=m_IniReader.TryParseString(configSec+"@JoystickFormat",fmtJoy);
			//
			deviceName=m_IniReader.TryParseString(configSec+"@DeviceName",deviceName);
			deviceId=m_IniReader.TryParseInt(configSec+"@DeviceId",deviceId);
			// Priority:IniReader > PlayerPrefs > Prefab value.
			string sec=configSec+"@DeviceAddress";
			deviceAddress=m_IniReader.TryParseString(
				sec,
				PlayerPrefs.GetString(sec,deviceAddress)
			);
			PlayerPrefs.SetString(sec,deviceAddress);// ???
			//
		}
		
		/// <summary>
		/// 
		/// </summary>
		public virtual void InitSensors() {
			int i=m_Sensors.Length;
			sensors=new SensorBase[m_NumSensors=i];
			resetables=new List<SensorBase>(m_NumSensors);
			m_BufferSize=0;
			while(i-->0) {
				sensors[i]=m_Sensors[i].Clone<SensorBase>();
				//m_Sensors[i].InitFromIniFile(m_IniReader);
				sensors[i].device=this;
				m_BufferSize+=sensors[i].bufferSize;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual void DeinitSensors() {
			for(int i=0;i<m_NumSensors;++i){
				sensors[i].OnDestroyInput();
			}
			// ???
			m_NumSensors=0;sensors=null;
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual int GetVirtualControllerId(){
			switch(deviceId) {
				case 0:
				return VirtualController.LEFT_HAND;
				case 1:
				return VirtualController.RIGHT_HAND;
				default:
				return -1;
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		public virtual void SwapDeviceWith(int i_otherId){
			SwapDevices(this,devices[i_otherId]);
		}
		
		/// <summary>
		/// 
		/// </summary>
		public virtual int Parse(byte[] buffer,int offset,int count) {
			int i=0,p=0;
			for(;i<m_NumSensors;++i) {
				p+=sensors[i].Parse(
					buffer,
					offset+p,
					count-p
				);
			}
			return p;
		}

		#endregion Methods

		#region IInputSource

		/// <summary>
		/// 
		/// </summary>
		public virtual int InitInput() {
			// Init Library.
			Library.Init();
			// Load config.
			m_IniReader=config;
			if(m_IniReader!=null){
				LoadConfig();
			}
			// Init self.
			InitSensors();
			// Open device stream.
			int ret=0;

			#region Stream

			m_Stream=StreamFactory.GetStream(
				deviceName,deviceId,deviceAddress,
				out m_BufferSize,out m_IsDynamicStream
			);
			if(m_Stream==null){
				Log.e("DeviceBase","No such stream with \""+deviceName+"\"");
				return -1;
			}
			//m_Buffer=new byte[m_BufferSize];
			m_Stream.SetOnStreamOpenListener(this);
			if(m_IsDynamicStream){
				m_Stream.SetOnStreamReadListener(this);
			}
			try{
				m_Stream.Open();
			}catch(System.Exception e){
				Log.w("DeviceBase",e.ToString());
				ret=-1;
			}

			#endregion Stream

			// Init sensors.
			axes=new List<string>();buttons=new List<string>();
			if(ret==0){
				for(int i=0;i<m_NumSensors;++i){
					sensors[i].StartInput();
				}
			}
			//
			
			// TODO
			
			#region Beta

			if(devices[deviceId]==null){
				devices[deviceId]=this;
			}else{
				Log.w("DeviceBase","The same id device exists!!!");
			}

			if(asVirtualController) {
				int hjId=GetVirtualControllerId();
				if(hjId<0) {
					Log.w("DeviceBase","Invalid virtual controller id");
				}else if(VirtualController.controllers[hjId]==null) {
				} else {
					Log.w("DeviceBase","The same type HandJoystick exists!!!");
					//return -1;
				}
				m_VirtualController=VirtualController.Instantiate
					(fmtJoy,axes.ToArray(),fmtJoy,buttons.ToArray(),poseName);
				VirtualController.controllers[hjId]=m_VirtualController;
			}

			#endregion Beta

			return ret;
		}
		
		/// <summary>
		/// 
		/// </summary>
		public virtual int ExitInput() {
			
			// TODO

			#region Beta

			if(devices[deviceId]==this){
				devices[deviceId]=null;
			}

			if(asVirtualController && m_VirtualController!=null) {
				int hjId=GetVirtualControllerId();
				if(hjId<0) {
					Log.w("DeviceBase","Invalid virtual controller id");
				}else if(VirtualController.controllers[hjId]==m_VirtualController){
					VirtualController.controllers[hjId]=null;
				}else {
				}
			}

			#endregion Beta

			//
			DeinitSensors();
			//
			if(m_Stream==null){
				return -1;
			}else{
				m_Stream.Close();
				m_Stream=null;
			}
			// Release the Library resource at last.
			Library.Exit();
			return 0;
		}
		
		/// <summary>
		/// 
		/// </summary>
		public virtual int EnterInputFrame() {
			//
			if(m_Stream==null){
				return -1;
			}
			//
			if(m_IsDynamicStream){if(!isDirty){
				return 0;
			}}else{
				m_Stream.GetReadBuffer(out m_Buffer,out m_BufferOffset,out m_BufferSize);
				Parse(m_Buffer,m_BufferOffset,m_BufferSize);
			}
			//
			for(int i=0;i<m_NumSensors;++i){
				sensors[i].UpdateInput();
			}
			//
			isDirty=false;
			return 0;
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual int ExitInputFrame() {
			//
			if(m_Stream==null){
				return -1;
			}
			//
			return 0;
		}

		#endregion IInputSource

	}
}

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN || UNITY_ANDROID

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Ximmerse.UI;

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
using Csr.Bluetooth;
#elif UNITY_ANDROID
using Android.Bluetooth;
using Java.Util;
#endif

namespace Ximmerse.IO.Ports {

	/// <summary>
	/// 
	/// </summary>
	public class BluetoothScannerGUI:MonoBehaviour,BluetoothScanner.ICallback ,IBluetoothGattCallback {

		#region Static

		public delegate void StringDelegate(string str);
		
		public static BluetoothScannerGUI main;

		/// <summary>
		/// 
		/// </summary>
		public static void RequestAddress(StringDelegate i_req){
			if(main==null){
				Log.e("BluetoothScannerGUI","No BluetoothScannerGUI instance here.");
				return;
			}
			main.AddRequest(i_req);
		}

		#endregion Static

		#region Fields

		public bool asMain=true;
		public bool showOnAwake=true;

		/// <summary>
		/// If true,this script will display as IBluetoothGattCallback.
		/// </summary>
		public bool debugMode=false;

		public Button m_BtnCnnt,m_BtnStart,m_BtnStop;
		public InputField m_TxtAddress,m_TxtReport;
		protected bool m_IsDirty=false;
		[System.NonSerialized]protected string m_Text;

		// Device List

		[SerializeField]protected BluetoothConnectButton m_PrefabCnn;
		[SerializeField]protected Transform m_LayoutCnns;
		[System.NonSerialized]protected UILayout m_UILayout;
		protected int m_NumBtnCnnsUsed=0,m_NumBtnCnnsInsted=0;
		[System.NonSerialized]public List<BluetoothConnectButton> m_BtnCnns=new List<BluetoothConnectButton>();

		protected GameObject m_Go;
		protected bool m_IsVisible=true;

		protected BluetoothScanner m_Scanner;
		protected Queue<StringDelegate> m_QueueRequest=new Queue<StringDelegate>();

		#endregion Fields

		#region Unity Messages

		/// <summary>
		/// 
		/// </summary>
		protected virtual void Awake(){
			m_Go=gameObject;
			//
/*#if UNITY_EDITOR_WIN
			m_Scanner=new BluetoothScanner(this);
			m_BtnStart.onClick.AddListener(()=>{
				m_Scanner.deviceList.Add(new BluetoothScanner.DeviceInfo{name="Virtual Device",address="00:00:00:00:00"});
				OnDeviceListChanged();
			}
			);
#else*/
			m_Scanner=new BluetoothScanner(this);
			//m_BtnCnnt.onClick.AddListener(OnClick_m_BtnCnnt);
			m_BtnStart.onClick.AddListener(m_Scanner.StartScan);
			m_BtnStop.onClick.AddListener(m_Scanner.StopScan);
//#endif
			//
			if(m_LayoutCnns!=null) {
				m_UILayout=m_LayoutCnns.GetComponent<UILayout>();
			}
			if(asMain){
				if(main==null){
					main=this;
				}else if(main!=this){
					Log.e("BluetoothScannerGUI","The main instance exists.");
				}
			}
			if(!showOnAwake){
				m_Go.SetActive(m_IsVisible=false);
			}
			if(debugMode){
				AddRequest(OpenDevice);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		protected virtual void Update(){
			if(m_IsDirty){
				if(m_Gatt!=null){
					if(m_TxtReport!=null){
						m_TxtReport.text=m_Text;
					}
				}else{
					UpdateGUI();//m_TxtReport.text=m_Text;
				}
				m_IsDirty=false;
			}
		}

		/// <summary>
		/// TODO : ????
		/// </summary>
		protected virtual void OnDestroy(){
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
			BluetoothAdapter.CloseDefaultAdapter();
#endif
		}

		#endregion Unity Messages
		
		#region Other Messages

		/// <summary>
		/// 
		/// </summary>
		public virtual void OnDeviceListChanged() {
			System.Text.StringBuilder sb=new System.Text.StringBuilder();
			for(int i=0,imax=m_Scanner.deviceList.Count;i<imax;++i){
				sb.AppendLine(m_Scanner.deviceList[i].ToString());
			}
			m_Text=sb.ToString();m_IsDirty=true;
		}
		
		#endregion Other Messages

		#region GUI APIs

		/// <summary>
		/// 
		/// </summary>
		public virtual void Show(){
			m_Go.SetActive(m_IsVisible=true);
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual void Hide(){
			m_Go.SetActive(m_IsVisible=false);
		}

		/// <summary>
		/// 
		/// </summary>
		protected virtual void UpdateGUI() {

			#region Check size

			int i,imax=m_Scanner.deviceList.Count;
			if(imax!=m_NumBtnCnnsUsed) {// Size is changed...
				if(imax>m_NumBtnCnnsUsed){
					//  i-> begin of unused items.
					for(i=m_NumBtnCnnsUsed;i<m_NumBtnCnnsInsted;++i){// Show the hidden items.
						m_BtnCnns[i].Show();
					}
					//  i-> end of m_BtnCnns.
					for(i=m_NumBtnCnnsInsted;i<imax;++i){// Instantiate the new items.
						m_BtnCnns.Add(m_PrefabCnn.Clone(m_LayoutCnns).StartGUI(this));
					}
					// Update the UI layout.
					if(m_NumBtnCnnsInsted<i){if(m_UILayout!=null){
						m_UILayout.Refresh();
					}}
					//
					m_NumBtnCnnsInsted=i;
				}else{
					for(i=m_NumBtnCnnsUsed-1;i>=imax;--i){// Hide the unused items.
						m_BtnCnns[i].Hide();
					}
				}
				//
				m_NumBtnCnnsUsed=imax;
			}

			#endregion Check size

			i=0;
			for(;i<m_NumBtnCnnsUsed;++i) {
				m_BtnCnns[i].UpdateGUI(m_Scanner.deviceList[i]);
			}
		}

		#endregion GUI APIs

		#region Request APIs

		/// <summary>
		/// 
		/// </summary>
		public virtual void AddRequest(StringDelegate i_req){
			if(i_req!=null){
				m_QueueRequest.Enqueue(i_req);
			}
			if(!m_IsVisible){
				Show();
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual void Connect(string i_address) {
			int i=m_QueueRequest.Count;
			if(i>0){
				Log.i("BluetoothScannerGUI","Try Open "+i_address);
				StringDelegate req=m_QueueRequest.Dequeue();--i;// Dequeue
				if(req!=null){
					req.Invoke(i_address);
				}
				if(i==0){
					Hide();
				}
			}

		}

		#endregion Request APIs

		#region IBluetoothGattCallback
		
		protected BluetoothAdapter m_Adapter;
		protected BluetoothGattCallback m_Cb;
		protected BluetoothGatt m_Gatt;

		/// <summary>
		/// 
		/// </summary>
		public virtual void OpenDevice(string i_address) {
			string text=i_address;//m_TxtAddress.text;
			if(!string.IsNullOrEmpty(text)){
				if(m_Adapter==null){
					m_Adapter=BluetoothAdapter.GetDefaultAdapter();
					m_Cb=new BluetoothGattCallback(this);
				}
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN//
				Log.i(text,NativeMethods.libcsr_ext_address_to_string_1(m_Adapter.AddressPool[text]));
				m_Gatt=m_Adapter.GetRemoteDevice(text).ConnectGatt(Csr.Bluetooth.Object.NULL_PTR,false,m_Cb);
#elif UNITY_ANDROID
				m_Gatt=m_Adapter.GetRemoteDevice(text).ConnectGatt(null,false,m_Cb);
#endif
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual void OnCharacteristicChanged(BluetoothGatt gatt,BluetoothGattCharacteristic characteristic) {
			m_IsDirty=true;m_Text=characteristic.GetValue().ToHexString();
			if(m_TxtReport==null){
				Log.i("BluetoothScannerGUI",m_Text);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual void OnCharacteristicRead(BluetoothGatt gatt,BluetoothGattCharacteristic characteristic,int status) {
			m_IsDirty=true;m_Text=characteristic.GetValue().ToHexString();
			if(m_TxtReport==null){
				Log.i("BluetoothScannerGUI",m_Text);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual void OnConnectionStateChange(BluetoothGatt gatt,int status,int newState) {
			if(newState==BluetoothProfile.STATE_CONNECTED) {
				gatt.DiscoverServices();
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual void OnServicesDiscovered(BluetoothGatt gatt,int status) {
			Log.i("BluetoothScannerGUI","OnServicesDiscovered:"+status.ToString());
			if(status==BluetoothGatt.GATT_SUCCESS){
				BluetoothGattService srv=gatt.GetService(UUID.FromString("0000faea-0000-1000-8000-00805f9b34fb"));
				BluetoothGattCharacteristic chr=srv.GetCharacteristic(UUID.FromString("0000faeb-0000-1000-8000-00805f9b34fb"));
				gatt.SetCharacteristicNotification(chr,true);
			}
		}

		#endregion IBluetoothGattCallback

	}
}

#endif
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_ANDROID

#if UNITY_EDITOR || UNITY_STANDALONE
using Csr.Bluetooth;
#elif UNITY_ANDROID
using Java.Util;
using Android.Bluetooth;
#endif

using UnityEngine;

namespace Ximmerse.IO.Ports {

	/// <summary>
	/// 
	/// </summary>
	public partial class BleSerialPort:IStreamable,IBluetoothGattCallback {
		
		#region UUID

		public static UUID s_UuidServ=UUID.FromString("0000faea-0000-1000-8000-00805f9b34fb");
		public static UUID s_UuidRx = UUID.FromString("0000faec-0000-1000-8000-00805f9b34fb");
		public static UUID s_UuidTx  =UUID.FromString("0000faeb-0000-1000-8000-00805f9b34fb");
		public static UUID s_UuidCCCD=UUID.FromString("00002902-0000-1000-8000-00805f9b34fb");

		#endregion UUID

		#region Constructors

		/// <summary>
		/// 
		/// </summary>
		public BleSerialPort(string address) {
			//
			m_Address=address;
			//
			m_UuidServ=s_UuidServ;
			m_UuidRx  =s_UuidRx;
			m_UuidTx  =s_UuidTx;
			m_UuidCCCD=s_UuidCCCD;
		}

		#endregion Constructors

		#region IStreamable

		protected bool m_IsOpen=false;
		protected IStreamOpenCallback m_OpenCallback;
		protected IStreamReadCallback m_ReadCallback;
		protected byte[] m_BufferRead;
		protected int m_SizeRead;
		
		/// <summary>
		/// 
		/// </summary>
		public virtual void Open() {
			if(m_Device!=null) Close();
#if UNITY_EDITOR || UNITY_STANDALONE
			BluetoothAdapter adapter=BluetoothAdapter.GetDefaultAdapter();
#elif UNITY_ANDROID
			using(BluetoothAdapter adapter=BluetoothAdapter.GetDefaultAdapter())
#endif
			{
				if(!adapter.IsEnabled()) return;
				m_Device=adapter.GetRemoteDevice(m_Address);
				// <!-- 
				if(m_Device==null){
					if(m_OpenCallback!=null){
						m_OpenCallback.OnStreamOpenFailure(this);
					}else{
						Log.e("BleSerialPort","No m_OpenCallback");
						//BluetoothScannerGUI.RequestAddress(Open);
					}
					return;
				}
				// -->
				m_Gatt=m_Device.ConnectGatt(
#if UNITY_EDITOR || UNITY_STANDALONE
					Csr.Bluetooth.Object.NULL_PTR,
#elif UNITY_ANDROID
					Android.App.Activity.currentActivity.m_SealedPtr,
#endif
					false,
					m_Callback=new BluetoothGattCallback(this)
				);
			}
			m_BufferRead=new byte[m_SizeRead=0];
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual void Close() {
			if(m_Device==null) return;
			//
			m_Gatt.Close();
#if !UNITY_EDITOR && UNITY_ANDROID
			m_Gatt.Dispose();
			m_Device.Dispose();
#endif
			m_Gatt=null;m_Device=null;
			m_Callback=null;
			//
			m_IsOpen=false;
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual int Read(byte[] buffer,int offset,int count) {
			if(!m_IsOpen) return -1;
			if(m_SizeRead<=0) return m_SizeRead=0;
			if(m_SizeRead>count){
				//_sizeRead=count;// ???
			}else{//if(m_SizeRead<=count)
				count=m_SizeRead;
			}
			System.Array.Copy(m_BufferRead,0,buffer,offset,count);
			m_SizeRead=0;// ???
			return count;
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual int Write(byte[] buffer,int offset,int count) {
			if(!m_IsOpen) return -1;
			byte[] tmpBuffer=new byte[count];
			System.Array.Copy(buffer,offset,tmpBuffer,0,count);
			return WriteRXCharacteristic(tmpBuffer);
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual void SetOnStreamOpenListener(IStreamOpenCallback callback) {
			m_OpenCallback=callback;
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual void SetOnStreamReadListener(IStreamReadCallback callback) {
			m_ReadCallback=callback;
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual void GetReadBuffer(out byte[] buffer,out int offset,out int count) {
			buffer = m_BufferRead;
			offset = 0;
			count  = m_SizeRead;
		}

		#endregion IStreamable

		#region Bluetooth

		protected UUID m_UuidServ,m_UuidRx,m_UuidTx,m_UuidCCCD;

		protected BluetoothDevice m_Device;
		protected BluetoothGatt m_Gatt;
		protected string m_Address;
		protected BluetoothGattCallback m_Callback;
		
		/// <summary>
		/// 
		/// </summary>
		public virtual void OnCharacteristicChanged(BluetoothGatt gatt,BluetoothGattCharacteristic characteristic) {
			//Log.i("BleSerialPort","OnCharacteristicChanged");
			m_BufferRead=characteristic.GetValue();m_SizeRead=m_BufferRead.Length;
			if(m_ReadCallback!=null){
				m_ReadCallback.OnStreamRead(this);
			}else{
				//Log.e("BleSerialPort","m_ReadCallback==null");
			}
			//m_Buffer=null;
		}
		
		/// <summary>
		/// 
		/// </summary>
		public virtual void OnCharacteristicRead(BluetoothGatt gatt,BluetoothGattCharacteristic characteristic,int status) {
			//Log.i("BleSerialPort","OnCharacteristicRead"+m_ReadCallback);
			m_BufferRead=characteristic.GetValue();m_SizeRead=m_BufferRead.Length;
			if(m_ReadCallback!=null){
				m_ReadCallback.OnStreamRead(this);
			}else{
				//Log.e("BleSerialPort","m_ReadCallback==null");
			}
			//m_Buffer=null;
		}
		
		/// <summary>
		/// 
		/// </summary>
		public virtual void OnConnectionStateChange(BluetoothGatt gatt,int status,int newState) {
			if (newState == BluetoothProfile.STATE_CONNECTED) {
				bool result=gatt.DiscoverServices();
				Log.i("BleSerialPort","DiscoverServices:"+result);
			} else if (newState == BluetoothProfile.STATE_DISCONNECTED) {
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		public virtual void OnServicesDiscovered(BluetoothGatt gatt,int status) {
			Log.i("BleSerialPort","OnServicesDiscovered:"+status.ToString());
			if (status == BluetoothGatt.GATT_SUCCESS) {
				EnableTXNotification();
				m_IsOpen=true;
				if(m_OpenCallback!=null){
					m_OpenCallback.OnStreamOpenSuccess(this);
				}
			} else {
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		public virtual void SetUUIDs(string uuidServ,string uuidRx,string uuidTx,string uuidCCCD){
			//UUID.FromString(ref m_Uuid,uuid);
			UUID.FromString(ref m_UuidServ,uuidServ);
			UUID.FromString(ref m_UuidRx,uuidRx);
			UUID.FromString(ref m_UuidTx,uuidTx);
			UUID.FromString(ref m_UuidCCCD,uuidCCCD);
		}
		
		/// <summary>
		/// 
		/// </summary>
		public virtual void EnableTXNotification() {
			if(m_Gatt==null) {
				Log.e("BleSerialPort","No BluetoothGatt to EnableTXNotification!!!");
				return;
			}

			// Get the service.
			BluetoothGattService RxService=m_Gatt
					.GetService(m_UuidServ);
			if(RxService==null) {
				Log.e("BleSerialPort","RxService==null");
				return;
			}

			// Get the characteristic.
			BluetoothGattCharacteristic txChar=RxService
					.GetCharacteristic(m_UuidTx);
			if(txChar==null) {
				Log.e("BleSerialPort","txChar==null");
				return;
			}

			// Set the characteristic notification.
			bool result=m_Gatt.SetCharacteristicNotification(txChar,true);
			if(!result) {
				Log.e("BleSerialPort","m_Gatt.SetCharacteristicNotification(txChar,true) failed!!!");
			}

			// (Set the characteristic notification ???).
			BluetoothGattDescriptor descriptor = txChar
					.GetDescriptor(m_UuidCCCD);
			descriptor.SetValue(BluetoothGattDescriptor.ENABLE_NOTIFICATION_VALUE);
			m_Gatt.WriteDescriptor(descriptor);

			Log.i("BleSerialPort","EnableTXNotification successfully.");
		}
		
		/// <summary>
		/// 
		/// </summary>
		public virtual int WriteRXCharacteristic(byte[] value) {
			BluetoothGattService rxService=m_Gatt
					.GetService(m_UuidServ);
			if(rxService==null) {
				return -1;
			}
			BluetoothGattCharacteristic rxChar=rxService
					.GetCharacteristic(m_UuidRx);
			if(rxChar==null) {
				return -2;
			}
			rxChar.SetValue(value);
			bool status=m_Gatt.WriteCharacteristic(rxChar);

			return (status)?0:-3;
		}

		#endregion Bluetooth

		#region Properties

		/// <summary>
		/// 
		/// </summary>
		public virtual string PortName {
			get{
				return m_Address;
			}
			set{
				m_Address=value;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual string address{
			get{
				return m_Address;
			}
			set{
				m_Address=value;
			}
		}

		#endregion Properties

	}

}

#endif
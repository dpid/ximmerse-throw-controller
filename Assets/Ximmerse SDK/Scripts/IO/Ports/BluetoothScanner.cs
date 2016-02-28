#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_ANDROID

using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
using Csr.Bluetooth;
#elif UNITY_ANDROID
using Android.Os;
using Android.Bluetooth;
#endif

namespace Ximmerse.IO.Ports {

	/// <summary>
	/// 
	/// </summary>
	public class BluetoothScanner:BluetoothAdapter.ILeScanCallback {

		#region Nested Types

		/// <summary>
		/// 
		/// </summary>
		public interface ICallback{
			void OnDeviceListChanged();
		}

		/// <summary>
		/// 
		/// </summary>
		[System.Serializable]
		public class DeviceInfo{
			public string name;
			public string address;

			public override string ToString() {
				return "{DeviceInfo name=\""+name+"\" address=\""+address+"\"}";
			}
		}

		#endregion Nested Types

		#region Fields

		public List<DeviceInfo> deviceList=new List<DeviceInfo>();
		public ICallback callback;

		protected BluetoothAdapter m_Adapter;
		protected BluetoothAdapter.LeScanCallback m_Cb;

		#endregion Fields

		#region Methods

		/// <summary>
		/// 
		/// </summary>
		public BluetoothScanner(ICallback i_callback){
			callback=i_callback;
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual void StartScan(){
			if(m_Adapter!=null){
				StopScan();
			}
			if(m_Cb==null){
				m_Cb=new BluetoothAdapter.LeScanCallback(this);
			}
			m_Adapter=BluetoothAdapter.GetDefaultAdapter();
			m_Adapter.StartLeScan(m_Cb);
			Log.i("BluetoothScanner","StartScan");
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual void StopScan(){
			if(m_Adapter==null){
				return;
			}
			m_Adapter.StopLeScan(m_Cb);
#if !UNITY_EDITOR_WIN && UNITY_ANDROID
			AndroidPtr.Free<BluetoothAdapter>(ref m_Adapter);
#endif
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual void OnLeScan(BluetoothDevice device,int rssi,byte[] scanRecord) {
			// Get Address.
			string address=device.GetAddress();
			if(string.IsNullOrEmpty(address)){
				return;
			}
			Log.i("BluetoothScanner",address);
			//
			int i=deviceList.FindIndex((x)=>(x.address==address));
			if(i==-1){
				deviceList.Add(new DeviceInfo{name=device.GetName(),address=address});
				if(callback!=null){
					callback.OnDeviceListChanged();
				}
			}else{
				address=device.GetName();
				if(!string.IsNullOrEmpty(address)){
					deviceList[i].name=address;
				}
				if(callback!=null){
					callback.OnDeviceListChanged();
				}
			}
		}

		#endregion Methods

	}

}

#endif
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Ximmerse.IO.Ports {

	/// <summary>
	/// 用BluetoothScannerGUI的连接特定地址蓝牙设备的按钮.
	/// </summary>
	public class BluetoothConnectButton:MonoBehaviour,IPointerClickHandler {

		#region Fields

		public BluetoothScannerGUI scanner;

		[System.NonSerialized]protected bool m_IsVisible=false;
		[System.NonSerialized]protected GameObject m_Go;
		[System.NonSerialized]protected Transform m_Trans;
		
		[SerializeField]protected Text m_TxtName,m_TxtAddress;
		[System.NonSerialized]protected BluetoothScanner.DeviceInfo m_Info;

		#endregion Fields

		#region Methods

		/// <summary>
		/// 
		/// </summary>
		protected virtual void DoAwake(){
			m_Go=gameObject;
			m_Trans=transform;
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual BluetoothConnectButton StartGUI(BluetoothScannerGUI i_scanner){
			scanner=i_scanner;
			Show();
			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual void UpdateGUI(BluetoothScanner.DeviceInfo i_info) {
			// Fix Hide() on OnPointerClick().
			if(!m_IsVisible){
				Show();
			}
			//
			m_Info=i_info;
			if(m_TxtName!=null){m_TxtName.text=m_Info.name;}
			if(m_TxtAddress!=null){m_TxtAddress.text=m_Info.address;}
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual void OnPointerClick(PointerEventData eventData) {
			if(scanner!=null){
				scanner.Connect(m_Info.address);
				Hide();//
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual void Show() {
			m_Go.SetActive(m_IsVisible=true);
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual void Hide() {
			m_Go.SetActive(m_IsVisible=false);
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual BluetoothConnectButton Clone(Transform i_parent) {
			if(m_Trans==null){
				Log.i("BluetoothConnectButton","Init the prefab.");
				DoAwake();
			}

			BluetoothConnectButton copy=Object.Instantiate(this) as BluetoothConnectButton;
			copy.DoAwake();
			copy.m_Trans.SetParent(i_parent);//copy.m_Trans.parent=i_parent;
			copy.m_Trans.localPosition=Vector3.zero;
			copy.m_Trans.localRotation=Quaternion.identity;
			copy.m_Trans.localScale=m_Trans.localScale;
			return copy;
		}

		#endregion Methods

	}
}

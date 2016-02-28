#if UNITY_EDITOR || UNITY_ANDROID

// TODO : "if(usbReq==m_UsbReqIn)" maybe cost a lot of time,so move it to android java.

#if ! USB_HANDLER_IN_ANDROID&&!USB_HANDLER_IN_UNITY
#define USB_HANDLER_IN_ANDROID//UNITY
#endif

//#define DO_NOT_READ

using Android.Hardware.Usb;
using Android.Os;
using Java.Nio;
using System.Collections.Generic;
using UnityEngine;

namespace Ximmerse.IO.Usb {

	/// <summary>
	/// 
	/// </summary>
	public class UsbStream:UsbStreamBase{

#if USB_HANDLER_IN_ANDROID

		/// <summary>
		/// 
		/// </summary>
		public class CallbackProxy:AndroidJavaProxy {
			public UsbStream usbStream;
			public CallbackProxy(UsbStream usbStream):base("android.unity.SafeUsbHandler$Callback") {
				this.usbStream=usbStream;
			}
			public virtual void onStreamRead(AndroidJavaObject buffer) {
				usbStream.OnStreamRead(buffer);
			}
		}

#endif

		#region Android.Hardware.Usb Fields

		protected UsbManager m_UsbMgr;
		protected UsbDevice m_UsbDevice;
		protected UsbInterface m_UsbIntf;
		protected UsbDeviceConnection m_UsbConnection;

		//
		protected UsbRequest m_UsbReqIn;
		protected ByteBuffer m_UsbBufferIn;
		//
		protected NativeThread m_ReadThread;
		protected int m_ReadThreadId=0;

		protected int m_Class=-1,m_Subclass=-1,m_Protocol=-1;

		#endregion Android.Hardware.Usb Fields

		#region Constructors
		
		/// <summary>
		/// 
		/// </summary>
		public UsbStream(string address):base(address){
		}

		/// <summary>
		/// 
		/// <para>When dId==-1,open the first usb device.</para>
		/// </summary>
		public UsbStream(int vId,int pId,int dId=0):base(vId,pId,dId){
		}

		/// <summary>
		/// 
		/// <para>When dId==-1,open the first usb device.</para>
		/// </summary>
		public UsbStream(string address,int vId,int pId,int dId=0):base(address,vId,pId,dId){
		}

		#endregion Constructors
		
		#region Android.Hardware.Usb Methods

		/// <summary>
		/// 
		/// </summary>
		public virtual void OpenDeviceSafely(UsbDevice device){
			if(m_UsbMgr.HasPermission(device)){
				OpenDevice(device);
			}else {
				m_UsbMgr.RequestPermission(device,OpenDevice);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual void OpenDevice(UsbDevice device){
			//
			if((m_UsbIntf=FindInterface(device))==null){
				Log.e("UsbStream","Can't find the UsbInterface"+device.ToString()+".");
				OnOpenFailure();
				return;
			}
			//
			m_UsbConnection=m_UsbMgr.OpenDevice(m_UsbDevice=device);
			m_UsbConnection.ClaimInterface(m_UsbIntf,true);
			//
			m_UsbReqIn=new UsbRequest();
			m_UsbReqIn.Initialize(m_UsbConnection,m_UsbIntf.intIn);

			// Check read buffer size.
			if(m_SizeRead==-1) {
				Log.i("UsbStream","Use default read buffer size:"+(m_SizeRead=64).ToString());
			}

#if USB_HANDLER_IN_UNITY
			// 
			m_UsbBufferIn=ByteBuffer.Allocate(m_SizeRead);
			//
			m_ReadThread=new NativeThread(ReadThread_Step);//Looper
			m_ReadThread.StartLoop();//
#elif USB_HANDLER_IN_ANDROID
			AndroidJavaObject handler=new AndroidJavaObject
				("android.unity.SafeUsbHandler",m_UsbConnection.m_SealedPtr,m_UsbReqIn.m_SealedPtr,m_SizeRead);
			handler.Call("setCallback",new CallbackProxy(this));
			m_ReadThread=new NativeThread(handler);
			m_ReadThread.Start2();
#endif
			//
			UsbManager.main.onUsbDeviceDetached+=OnUsbDeviceDetached;
			if(m_OpenCallback!=null){
				m_OpenCallback.OnStreamOpenSuccess(this);
			}
			//
			m_IsOpen=true;
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual bool MatchDevice(UsbDevice device){
			if(m_VID!=0) {
				if(device.GetVendorId()!=m_VID) {
					return false;
				}
			}
			if(m_PID!=0) {
				if(device.GetProductId()!=m_PID) {
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual void SetInterfaceFilters(int i_cls,int i_subcls,int i_protocol) {
			m_Class=i_cls;
			m_Subclass=i_subcls;
			m_Protocol=i_protocol;
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual UsbInterface FindInterface(UsbDevice device){
			UsbInterface usbIntf=null;
			List<UsbInterface> usbIntfs=device.GetInterfaces();
			int i=0,imax=usbIntfs.Count;
			if(m_Class!=-1&&m_Subclass!=-1&&m_Protocol!=-1){// Get the given UsbInterface.
				for(;i<imax;++i){usbIntf=usbIntfs[i];
					if(usbIntf.GetInterfaceClass()==m_Class){
					if(usbIntf.GetInterfaceSubclass()==m_Subclass){
					if(usbIntf.GetInterfaceProtocol()==m_Protocol){
						break;
					}}}
				}
			}else{
				usbIntf=usbIntfs[0];
			}
			//m_Class=m_Subclass=m_Protocol=-1;//Reset filters.???
			
			return usbIntf;
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual void ReadThread_Looper(){
			int threadId=++m_ReadThreadId;
			while(threadId==m_ReadThreadId) {
				ReadThread_Step();
			}
			// Cleanup
		}

		/// <summary>
		/// Fix the issue : JNI ERROR (app bug): local reference table overflow (max=512)
		/// </summary>
		public virtual void ReadThread_Step() {
			if(m_UsbReqIn.Queue(m_UsbBufferIn,m_SizeRead)==true) {
				using(UsbRequest usbReq=m_UsbConnection.RequestWait()) {
					// (mUsbConnection.requestWait() is blocking
					if(usbReq==m_UsbReqIn) {
						// this is an actual receive
						// do receive processing here (send to consumer)
						//
#if !DO_NOT_READ
						m_BufferRead=m_UsbBufferIn.Array();
						m_SizeHasRead=m_BufferRead.Length;
#endif
						if(m_ReadCallback!=null) {
							m_ReadCallback.OnStreamRead(this);
						}
					} else {
					}
				}
			} else {
			}
			m_UsbBufferIn.Clear();
		}

#if USB_HANDLER_IN_ANDROID

		/// <summary>
		/// 
		/// </summary>
		public virtual void OnStreamRead(AndroidJavaObject buffer) {
			// this is an actual receive
			// do receive processing here (send to consumer)
			//
#if !DO_NOT_READ
			m_BufferRead=buffer.Call<byte[]>("array");
			m_SizeHasRead=m_BufferRead.Length;
#endif
			if(m_ReadCallback!=null) {
				m_ReadCallback.OnStreamRead(this);
			}
		}

#endif

		/// <summary>
		/// 
		/// </summary>
		protected virtual void OnOpenFailure() {
			UsbManager.main.onUsbDeviceAttached+=OnUsbDeviceAttached;
			if(m_OpenCallback!=null) {
				m_OpenCallback.OnStreamOpenFailure(this);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		protected virtual void OnUsbDeviceAttached(UsbDevice device){
			if(m_VID==device.GetVendorId()&&m_PID==device.GetProductId()){
				OpenDeviceSafely(device);
				UsbManager.main.onUsbDeviceAttached-=OnUsbDeviceAttached;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		protected virtual void OnUsbDeviceDetached(UsbDevice device){
			if(m_UsbDevice.GetDeviceName()==device.GetDeviceName()){
				Close();
				UsbManager.main.onUsbDeviceDetached-=OnUsbDeviceDetached;
			}
		}

		#endregion Android.Hardware.Usb Methods

		#region IStreamable
		
		/// <summary>
		/// 
		/// </summary>
		public override void Open() {
			//
			if(m_IsOpen){
				Close();
			}
			//
			m_UsbMgr=UsbManager.main;
			List<UsbDevice> deviceList=m_UsbMgr.GetDeviceList();

			#region My Filter

			if(!string.IsNullOrEmpty(m_Address)){
				UsbDevice device=deviceList.Find((x)=>(x.GetDeviceName()==m_Address));
				if(device!=null){
					deviceList.Clear();
					deviceList.Add(device);
					m_DID=-1;
				}
			}

			if(m_DID==-1){//???
				m_DID=0;
			}else{
				deviceList=deviceList.FindAll(
					MatchDevice
				);
			}
			int i=deviceList.Count;
			Log.i("UsbStream",string.Format("Find {0} usb device(s)",i));
			if(m_DID>i-1){// i-1 means the last one.when <= last one,you got it.
				Log.e("UsbStream","Can't find the UsbDevice.");
				OnOpenFailure();
				return;
			}

			#endregion My Filter
		
			
			OpenDeviceSafely(deviceList[m_DID]);
		}

		/// <summary>
		/// 
		/// </summary>
		public override void Close() {
			// Forcibly close the read thread.
			if(m_ReadThread!=null) {
				m_ReadThread.Abort();m_ReadThread=null;
			}
			if(m_IsOpen){
				//
				m_UsbConnection.ReleaseInterface(m_UsbIntf);
				m_UsbConnection.Close();

				// Clean up
				UsbManager.main.onUsbDeviceAttached-=OnUsbDeviceAttached;
				UsbManager.main.onUsbDeviceDetached-=OnUsbDeviceDetached;
				AndroidPtr.Free(ref m_UsbDevice);
				AndroidPtr.Free(ref m_UsbIntf);
				AndroidPtr.Free(ref m_UsbConnection);
				AndroidPtr.Free(ref m_UsbReqIn);
				//
				m_IsOpen=false;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public override int Read(byte[] buffer,int offset,int count) {
			if(!m_IsOpen) return -1;
			if(m_SizeHasRead<=0) return m_SizeHasRead=0;
			if(m_SizeHasRead>count){
				//_sizeRead=count;// ???
			}else{//if(m_SizeRead<=count)
				count=m_SizeHasRead;
			}
			System.Array.Copy(m_BufferRead,0,buffer,offset,count);
			m_SizeHasRead=0;
			return count;
		}
		
		/// <summary>
		/// 
		/// </summary>
		public override int Write(byte[] buffer,int offset,int count) {
			return 0;
		}

		#endregion IStreamable

	}
}

#endif

using UnityEngine;
using Ximmerse.IO;
using Ximmerse.IO.Usb;
	
namespace Ximmerse.Core {

	/// <summary>
	/// 
	/// </summary>
	public class XHawkService:XHawkServiceBase,IStreamOpenCallback,IStreamReadCallback {
		
		#region For Editor

#if UNITY_EDITOR

		/// <summary>
		/// 
		/// </summary>
		[ContextMenu("Print UsbDevices")]
		protected virtual void PrintUsbDevices() {
			var devices=HidStream.EnumerateDevices().ToArray();
			var sb=new System.Text.StringBuilder();
			for(int i=0,imax=devices.Length;i<imax;++i) {
				sb.AppendLine(string.Format("name:{0} path:{1}",devices[i].product_string,devices[i].path));
			}
			Log.v("XHawkService","Devices:\n"+sb.ToString());
		}

#endif

		#endregion For Editor

		#region Fields

		[Header("Usb")]
		public int vId;
		public int pId;
		public bool safeReadMode=true;

		[SerializeField]protected bool m_IsDebug=true;
		[SerializeField]protected UnityEngine.UI.InputField m_Text;

#if UNITY_EDITOR||UNITY_STANDALONE_WIN
		protected HidStream m_UsbStream;
#else
		protected UsbStreamBase m_UsbStream;
#endif

		[System.NonSerialized]public VirtualStream[] bleStreams=new VirtualStream[2];

		#endregion Fields

		#region Unity Messages

		protected bool m_IsGUIDirty=false;
		protected int m_TimestampRecPrev=0;
		protected float m_TimePrev=-1f;
		protected string m_RateText="";
		[System.NonSerialized]public int bleReadOffset=-1;
		[System.NonSerialized]public int bleReadRawSize=-1;
		[System.NonSerialized]public int bleReadSize=-1;
		protected string m_TextRead="";

		/// <summary>
		/// 
		/// </summary>
		protected virtual void Update(){
			if(m_IsDebug&&m_IsGUIDirty){if(m_Text!=null){
				int i=0;
				m_Text.text="Rate : "+m_RateText+" TimePrev : "+m_TimePrev+" CountPrev : "+m_TimestampRecPrev+
					" Count : "+m_TimestampRec.ToString()+"\n"+
					points[i++].ToString()+"\n"+
					points[i++].ToString()+"\n"+
					m_TextRead;
				m_IsGUIDirty=false;
			}}
		}

		/// <summary>
		/// 
		/// </summary>
		protected virtual void OnGUI(){
			if(m_IsDebug){
				float time=Time.realtimeSinceStartup;
				if(time-m_TimePrev>=1.0f){
					m_RateText=((m_TimestampRec-m_TimestampRecPrev)/(time-m_TimePrev)).ToString("0.000");
					//
					m_TimestampRecPrev=m_TimestampRec;
					m_TimePrev=time;
				}
			}
		}

		#endregion Unity Messages

		#region Methods
		
		/// <summary>
		/// 
		/// </summary>
		protected override void ReadConfig(IniReader i_ir) {
			base.ReadConfig(i_ir);
			vId=i_ir.TryParseHex(configSec+"@VID",vId);
			pId=i_ir.TryParseHex(configSec+"@PID",pId);
			//
			safeReadMode=(i_ir.TryParseInt(configSec+"@SafeReadMode",(safeReadMode) ? 1 : 0)==1);
		}

		/// <summary>
		/// 
		/// </summary>
		public override void Open() {
			//
			bleStreams=new VirtualStream[2]{
				new VirtualStream(null),
				new VirtualStream(null)
			};
			//
			Juggler.Main.DelayCall(OpenDelayed,0.5f);
		}
		
		/// <summary>
		/// 
		/// </summary>
		protected virtual void OpenDelayed(){
			// Fix the size.
			if(m_SizeRead<=0) {
				m_SizeRead=64;
			}
			// Fix points.
			if(points.Length==0) {
				int i=2;
				points=new BlobsID3D[i];
				while(i-->0) {
					points[i]=new BlobsID3D();
					points[i].poseName=(i==0)?"Left_Hand":"Right_Hand";
					points[i].id=i;
					points[i].OnStart(this);
				}
			}
			// Open the steam.
			if(m_UsbStream==null){
				// Fix VID & PID
				if(vId*pId==0) {
					vId=0x1f3b;pId=0x10ff;
				}
				//
				m_UsbStream=new
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
					HidStream
#elif UNITY_ANDROID
					UsbStream
#endif
					
					(vId,pId,/*0x1f3b,0x10ff,*/0);
				//
				m_UsbStream.readBufferSize=m_SizeRead;
				m_UsbStream.SetOnStreamOpenListener(this);
#if !UNITY_EDITOR && UNITY_ANDROID
				((UsbStream)m_UsbStream).SetInterfaceFilters(3,0,1);
#endif
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
				if(safeReadMode) {
					if(m_BufferRead==null){
						m_BufferRead=new byte[m_SizeRead];
					}
				}else
#endif
				{
					m_UsbStream.SetOnStreamReadListener(this);
				}
			}
			m_UsbStream.Open();
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual void UpdateStreamPos(int i_index) {
			if(i_index<0||i_index>=2){
				Log.e("XHawkService","No such ble stream @id:"+i_index);
				return;
			}
			bleStreams[i_index].SetStreamPos
				(bleReadOffset,bleReadSize);
		}

		/// <summary>
		/// 
		/// </summary>
		public override void Close() {
			m_UsbStream.Close();
		}
		
#if UNITY_EDITOR || UNITY_STANDALONE_WIN

		/// <summary>
		/// 
		/// </summary>
		public override int EnterInputFrame() {
			//
			if(safeReadMode){
			if(m_UsbStream!=null){
				int size;
				while(true){// @See void OnStreamRead(IStreamable stream);
					++m_TimestampRec;
					size=m_UsbStream.ReadTimeout(m_BufferRead,0,m_SizeRead,2);
					if(size>0){
						if(m_IsDebug){
							m_TextRead=m_BufferRead.ToHexString();
						}
						Parse(m_BufferRead,0,size);
						//
						m_IsGUIDirty=true;
						//Log.i("m_UsbStream.ReadTimeout",size.ToString());
					}else{
						break;
					}
				}
			}}
			//
			return base.EnterInputFrame();
		}

#endif

		/// <summary>
		/// 
		/// </summary>
		protected override void Parse(byte[] buffer,int offset,int count){
			offset+=4;// Skip id field???
			int i,imax=2;
			int id,idOffset=offset+12*imax;

			#region Blob tracking result.

			for(i=0;i<imax;++i){
				points[i].id=-1;//Reset id.
			}
			for(i=0;i<imax;++i){
				id=(sbyte)buffer[idOffset+i];
				if(id!=-1){
					points[id].id=id;
					ToVector3(ref points[id].position,ref sensitivity,buffer,offset,true);
					points[id].OnUpdate();
					offset+=12;
				}
			}
			for(i=0;i<imax;++i){
				if(points[i].id==-1){// If hidden.
					points[i].position.Set(
						-1024f,-1024f,-1024f
					);
					points[i].OnUpdate();
				}
			}
			//
			offset=idOffset+2;// Skip the flag.

			#endregion Blob tracking result.

			#region Ble serial port result.

			idOffset=offset+10*2;
			FixBleStreamId(buffer,idOffset);

			for(i=0;i<imax;++i){
				id=(int)buffer[idOffset+i];
				if(id>=0&&id<2) {
					bleStreams[id].OnStreamRead(buffer,offset);
				}else {
					//Log.e("XHawkService","Unidentified BleStream:"+id+"@"+(idOffset+i)+" offset:"+offset+"\n"+buffer.ToHexString());
				}
				offset+=bleReadRawSize;
			}

			#endregion Ble serial port result.

		}

		/// <summary>
		/// 
		/// </summary>
		protected virtual void FixBleStreamId(byte[] o_buffer,int i_idOffset) {
			int id0=i_idOffset+0,id1=i_idOffset+1;
			if(o_buffer[id0]==3){
				if(o_buffer[id1]==3){
					return;
				}
			}
			if(o_buffer[id0]==o_buffer[id1]) {
				for(int i=0;i<2;++i){
					o_buffer[i_idOffset+i]=(byte)i;
				}
			}
		}

		#endregion Methods

		#region IStreamCallback

		/// <summary>
		/// 
		/// </summary>
		public virtual void OnStreamOpenSuccess(IStreamable stream) {
			for(int i=0,imax=bleStreams.Length;i<imax;++i){
				bleStreams[i].OnStreamOpenSuccess(stream);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual void OnStreamOpenFailure(IStreamable stream) {
			for(int i=0,imax=bleStreams.Length;i<imax;++i){
				bleStreams[i].OnStreamOpenFailure(stream);
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		public virtual void OnStreamRead(IStreamable stream) {
			++m_TimestampRec;
			//
			stream.GetReadBuffer(out m_BufferRead,out m_OffsetRead,out m_SizeRead);
			//if(size>0) {
				if(m_IsDebug) {
					m_TextRead=m_BufferRead.ToHexString();
				}
				Parse(m_BufferRead,m_OffsetRead,m_SizeRead);
				//
				m_IsGUIDirty=true;
			//}
		}

		#endregion IStreamCallback

	}

}
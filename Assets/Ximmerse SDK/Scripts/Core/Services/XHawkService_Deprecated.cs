#define XIM_MACE_TCP_DATA_FORMAT_0_0_0
#define SUB_THREAD

using System.Net.Sockets;
using System.Threading;
using UnityEngine;
using Ximmerse.IO;

namespace Ximmerse.Core {

	/// <summary>
	/// 
	/// </summary>
	public class XHawkService_Deprecated:XHawkServiceBase{
		
		#region Fields
		
		[Header("TCP")]
		public string ip;
		public int port;
		public bool sendAnswer=false;
		
		protected Thread m_Thread;
		protected bool m_IsRunning;
		
		#endregion Fields
		
		#region APIs

		/// <summary>
		/// 
		/// </summary>
		protected override void ReadConfig(IniReader i_ir) {
			base.ReadConfig(i_ir);
			ip=i_ir.TryParseString(configSec+"@IP",ip);
			port=i_ir.TryParseInt(configSec+"@Port",port);
		}

		/// <summary>
		/// 
		/// </summary>
		public override void Open() {
			if(m_IsRunning){
				Close();
			}
#if SUB_THREAD
			m_Thread=new Thread(TcpSubThread);
			m_Thread.IsBackground=m_IsRunning=true;
			m_Thread.Priority=System.Threading.ThreadPriority.Highest;
			m_Thread.Start();
#else
			AsyncThread();
#endif
		}

		/// <summary>
		/// 
		/// </summary>
		public override void Close() {
			if(m_Thread!=null) {
				KillSubThread();
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		protected virtual void KillSubThread() {
			m_IsRunning=false;
			// Clean up.
			if(m_ArTcp!=null){
				m_Tcp.EndConnect(m_ArTcp);
			}
			if(m_ArStream!=null){
				m_Stream.EndRead(m_ArStream);
			}
			if(m_Stream!=null) {
				m_Stream.Dispose();
				m_Stream=null;
			}
			if(m_Tcp!=null) {
				m_Tcp.Close();
				m_Tcp=null;
			}
			if(m_Thread!=null) {
				m_Thread.Abort();
				m_Thread=null;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		protected override void Parse(byte[] buffer,int offset,int count){
			int i=points.Length,imax,id;
			while(i-->0) {
				points[i].id=-1;
			}
			imax=count/BlobsID3D.SIZE;
			m_TimestampParse+=imax;
			i=0;while(i<imax) {
#if XIM_MACE_TCP_DATA_FORMAT_0_0_0
				id=System.BitConverter.ToInt32(buffer,offset+12);
				if(id!=-1) {
					points[id].id=id;
					ToVector3(ref points[id].position,ref sensitivity,buffer,offset);
					points[id].OnUpdate();
				}
#endif
				//
				offset+=BlobsID3D.SIZE;
				++i;
			}
		}

		#endregion APIs

		#region TCP
		
		protected TcpClient m_Tcp;
		protected NetworkStream m_Stream;
		protected System.IAsyncResult m_ArTcp;
		protected System.IAsyncResult m_ArStream;
		protected byte[] m_Answer=System.Text.Encoding.UTF8.GetBytes("answer");

		protected virtual void TcpSubThread() {
			//
			m_Tcp=new TcpClient();
			try{
				m_Tcp.Connect(ip,port);
			}catch(System.Exception e){
				Log.i("XHawkService",e.ToString());
				return;
			}
			
			if(m_Tcp.Connected) {
			} else {
				return;
			}

			m_SizeRead = BlobsID3D.SIZE*(points.Length);//*2;
			m_BufferRead=new byte[m_SizeRead];
		
			m_Stream=m_Tcp.GetStream();
			while(m_IsRunning) {
				OnTcpThreadRead();
			}

			// Clean up.
			m_Stream.Dispose();
			m_Tcp.Close();
		}
		
		/// <summary>
		/// 
		/// </summary>
		protected virtual void TcpAsyncThread() {
			//Log.i("XHawkService","SubThread Start");

			m_Tcp=new TcpClient();
			try{
				m_ArTcp=m_Tcp.BeginConnect(ip,port,OnTcpAsyncThreadStart,m_Tcp);
			}catch(System.Exception e){
				Log.i("XHawkService",e.ToString());
				return;
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		protected virtual void OnTcpAsyncThreadStart(System.IAsyncResult ar=null) {
			Log.i("XHawkService","AsycThread Start");
			m_SizeRead=BlobsID3D.SIZE*(points.Length)*4;
			m_BufferRead=new byte[m_SizeRead];
		
			m_Stream=m_Tcp.GetStream();
			try{
				m_ArStream=m_Stream.BeginRead(m_BufferRead,0,m_SizeRead,
					new System.AsyncCallback(OnTcpThreadRead),m_Tcp);
			}catch(System.Exception e){
				Log.i("XHawkService",e.ToString());
				return;
			}
		}


		/// <summary>
		/// 
		/// </summary>
		protected virtual void OnTcpThreadRead(System.IAsyncResult ar=null) {
			int length=m_Stream.Read(m_BufferRead,0,m_SizeRead);
			++m_TimestampRec;
			//return;
#if !NEWEST_DATA
			Parse(m_BufferRead,0,length);
#else
			if(length>=BlobsID3D.SIZE*2)
				OnRead(_buffer,
				       length-BlobsID3D.SIZE*2,BlobsID3D.SIZE*2
				);
#endif
			if(sendAnswer){
				m_Stream.Write(m_Answer,0,6);
			}
			if(ar!=null){
				try{
					m_ArStream=m_Stream.BeginRead(m_BufferRead,0,m_SizeRead,
						new System.AsyncCallback(OnTcpThreadRead),m_Tcp);
				}catch(System.Exception e){
					Log.i("XHawkService",e.ToString());
					return;
				}
			}
		}

		#endregion TCP

	}

}
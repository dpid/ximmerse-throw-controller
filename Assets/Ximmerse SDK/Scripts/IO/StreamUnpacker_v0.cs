// 未经过单元测试,所以不清楚错误率多少...

namespace Ximmerse.IO {

	/// <summary>
	/// 第一版协议.
	/// </summary>
	public class StreamUnpacker_v0:StreamUnpacker {

		/// <summary>
		/// 
		/// </summary>
		public StreamUnpacker_v0(int rawSize,int fixedSize):base(rawSize,fixedSize){
		}

		#region Override

		/// <summary>
		/// 
		/// </summary>
		public override void OnStreamRead(IStreamable stream) {
			// Read
			if(m_PtrSnapshot==-1){
				// Reset pointer.
				m_PtrRec=m_PtrSnapshot=0;
			}
			m_SizeRec=stream.Read(m_Buffer,m_PtrRec,m_SizeBuffer-m_PtrRec);
			if(m_SizeRec==0){
				return;
			}
			m_SizeRecTotal+=m_SizeRec;
			// Parse
			int
				i=m_PtrSnapshot,// Continue last work.
				imax=m_PtrRec+m_SizeRec,
				end=imax-1
			;
			int sum,tail,result=RESULT_NO_PARSE_DATA;
			for(;i<imax;++i){//
				if(m_Buffer[i]==0x5A){
					if(end>=i+2+m_SizeFixedBuffer){// pEnd
						if(m_Buffer[i+1]==0xA5){
							//
							sum=Sum(m_Buffer,i+2,m_SizeFixedBuffer);// pStart of FixedBuffer
							//
							tail=m_Buffer[i+2+m_SizeFixedBuffer];
							if((sum&0xFF)-tail==0||(sum&0xFF)-tail==1){
								if(m_ReadCallback!=null){
									//
									m_IsCompleted=true;m_PtrBuffer=i+2;// pStart of FixedBuffer
									// 
									m_ReadCallback.OnStreamRead(this);
									// Clean up.
									m_IsCompleted=false;m_PtrBuffer=-1;
								}
								i=i+2+m_SizeFixedBuffer;//+1;
								result=RESULT_OK;// ????
								m_PtrSnapshot=i;// 保存快照.
							}else{
								//Log.i("StreamUnpacker","Sum failed...");
							}
						}
					}else{// 长度不够,需要拼接.
						result=RESULT_NO_ENOUGH_LENGTH;// ????
						m_PtrSnapshot=i;// 保存快照.
						break;
					}
				}
			}
			// Clean up.
			CleanUp(result);
		}

		#endregion Override
	
	}
}

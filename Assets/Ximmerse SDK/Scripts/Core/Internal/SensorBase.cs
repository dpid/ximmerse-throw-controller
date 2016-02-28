using UnityEngine;

namespace Ximmerse.Core {

	/// <summary>
	/// 
	/// </summary>
	public class SensorBase:ScriptableObject {

		#region Static
		
		/// <summary>
		/// 
		/// </summary>
		public static int ToSbyte(byte value){
			int s=value&0x80;
			if(s==0){
				return value;
			}else{
				value=(byte)~value;
				return -(value+1);
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		public static int/*short*/ ToInt16(byte[] i_buff,int i_start){
			int s=1;
			if((i_buff[i_start]&0x80)!=0)
				s=-1;
			return s*(((i_buff[i_start]<<8)&0x7F00)|i_buff[i_start+1]);
		}
		
		#endregion Static

		#region Fields

		[System.NonSerialized]public DeviceBase device;
		public int bufferSize=1;

		#endregion Fields

		#region Messages & Methods

		/// <summary>
		/// 
		/// </summary>
		public virtual void AwakeInput() {
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual void StartInput() {
		}

		/// <summary>
		/// 把Sensor信息写到Unity公共内存中.
		/// </summary>
		public virtual void UpdateInput() {
		}
		
		/// <summary>
		/// 
		/// </summary>
		public virtual void ResetInput(){
		}
		
		/// <summary>
		/// 
		/// </summary>
		public virtual void OnDestroyInput() {
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual int Parse(byte[] buffer,int offset,int count) {
			return bufferSize;
		}
		
		/// <summary>
		/// 
		/// </summary>
		public virtual T Clone<T>()where T:SensorBase{
			T copy=Object.Instantiate(this) as T;
			copy.AwakeInput();
			return copy;
		}
		
		/// <summary>
		/// 
		/// </summary>
		public override string ToString() {
			return string.Format(
				"{Device:{0},Sensor:{1}}",
				device.ToString(),
				base.ToString()
			);
		}

		#endregion Messages & Methods
	
	}
}

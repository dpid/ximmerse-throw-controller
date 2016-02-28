using UnityEngine;
using System.Text;
using System.IO;

namespace Ximmerse.IO{

	/// <summary>
	/// See NGUI.ByteReader.cs
	/// </summary>
	public class CustomReader{

		#region Text

		public static Encoding UTF8=Encoding.UTF8;
	
		/// <summary>
		/// Read a single line from the buffer.
		/// </summary>
		public static string ReadLine(byte[] buffer,int start,int count){//@see NGUI.ByteReader.cs
#if UNITY_FLASH
			// Encoding.UTF8 is not supported in Flash :(
			StringBuilder sb = new StringBuilder();

			int max = start + count;

			for (int i = start; i < max; ++i)
			{
				byte byte0 = buffer[i];

				if ((byte0 & 128) == 0)
				{
					// If an UCS fits 7 bits, its coded as 0xxxxxxx. This makes ASCII character represented by themselves
					sb.Append((char)byte0);
				}
				else if ((byte0 & 224) == 192)
				{
					// If an UCS fits 11 bits, it is coded as 110xxxxx 10xxxxxx
					if (++i == count) break;
					byte byte1 = buffer[i];
					int ch = (byte0 & 31) << 6;
					ch |= (byte1 & 63);
					sb.Append((char)ch);
				}
				else if ((byte0 & 240) == 224)
				{
					// If an UCS fits 16 bits, it is coded as 1110xxxx 10xxxxxx 10xxxxxx
					if (++i == count) break;
					byte byte1 = buffer[i];
					if (++i == count) break;
					byte byte2 = buffer[i];

					if (byte0 == 0xEF && byte1 == 0xBB && byte2 == 0xBF)
					{
						// Byte Order Mark -- generally the first 3 bytes in a Windows-saved UTF-8 file. Skip it.
					}
					else
					{
						int ch = (byte0 & 15) << 12;
						ch |= (byte1 & 63) << 6;
						ch |= (byte2 & 63);
						sb.Append((char)ch);
					}
				}
				else if ((byte0 & 248) == 240)
				{
					// If an UCS fits 21 bits, it is coded as 11110xxx 10xxxxxx 10xxxxxx 10xxxxxx 
					if (++i == count) break;
					byte byte1 = buffer[i];
					if (++i == count) break;
					byte byte2 = buffer[i];
					if (++i == count) break;
					byte byte3 = buffer[i];

					int ch = (byte0 & 7) << 18;
					ch |= (byte1 & 63) << 12;
					ch |= (byte2 & 63) << 6;
					ch |= (byte3 & 63);
					sb.Append((char)ch);
				}
			}
			return sb.ToString();
#else
			return UTF8.GetString(buffer,start,count);
#endif
		}

		#endregion//Text

		#region Stream

		protected byte[] _buffer;
		protected int _offset=0,_end,_length;

		#endregion//Stream

		#region Constructor

		public CustomReader(){
	
		}

		public CustomReader(byte[] i_bytes){
			_buffer=i_bytes;_end=(_length=_buffer.Length)-1;
		}
	
		#endregion//Constructor

		#region Reader

		/// <summary>
		/// 
		/// </summary>
		public virtual void Parse(){
		}

		/// <summary>
		/// Read a single line from the buffer.
		/// </summary>
		public string ReadLine(){//@see NGUI.ByteReader.cs
			return ReadLine(true); 
		}

		/// <summary>
		/// Read a single line from the buffer.
		/// </summary>
		public string ReadLine(bool skipEmptyLines){//@see NGUI.ByteReader.cs
			int max=_buffer.Length;

			// Skip empty characters
			if(skipEmptyLines) {
				while(_offset<max&&_buffer[_offset]<32) ++_offset;
			}

			int end=_offset;

			if(end<max) {
				for(;;) {
					if(end<max) {
						int ch=_buffer[end++];
						if(ch!='\n'&&ch!='\r') continue;
					} else ++end;

					string line=ReadLine(_buffer,_offset,end-_offset-1);
					_offset=end;
					return line;
				}
			}
			_offset=max;
			return null;
		}

		public virtual bool canRead{
			get{
				if(_buffer==null) return false;
				return _offset<=_end;
			}
		}

		#endregion//Reader

	}
}
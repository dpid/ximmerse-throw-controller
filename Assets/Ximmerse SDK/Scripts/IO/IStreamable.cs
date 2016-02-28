
namespace Ximmerse.IO {
	
	/// <summary>
	/// 
	/// </summary>
	public interface IStreamable {

		#region Classic

		/// <summary>
		/// 
		/// </summary>
		string address{
			get;
			set;
		}

		/// <summary>
		/// 
		/// </summary>
		void Open();

		/// <summary>
		/// 
		/// </summary>
		void Close();

		/// <summary>
		/// 
		/// </summary>
		int Read(byte[] buffer,int offset,int count);

		/// <summary>
		/// 
		/// </summary>
		int Write(byte[] buffer,int offset,int count);

		#endregion Classic

		#region Events

		void SetOnStreamOpenListener(IStreamOpenCallback callback);
		void SetOnStreamReadListener(IStreamReadCallback callback);

		#endregion Events

		#region Faster APIs

		/// <summary>
		/// Reduce using Array.Copy().
		/// </summary>
		void GetReadBuffer(out byte[] buffer,out int offset,out int count);

		#endregion Faster APIs

	}
}
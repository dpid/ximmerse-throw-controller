namespace Ximmerse.IO {

	/// <summary>
	/// 
	/// </summary>
	public interface IStreamOpenCallback {
		void OnStreamOpenSuccess(IStreamable stream);
		void OnStreamOpenFailure(IStreamable stream);
	}
}

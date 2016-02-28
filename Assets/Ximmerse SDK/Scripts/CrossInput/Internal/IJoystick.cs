
namespace Ximmerse.CrossInput {

	/// <summary>
	/// 
	/// </summary>
	public interface IJoystick {

		/// <summary>
		/// 
		/// </summary>
		float GetAxis(string axisName);

		/// <summary>
		/// 
		/// </summary>
		float GetAxisRaw(string axisName);

		/// <summary>
		/// 
		/// </summary>
		bool GetButton(string buttonName);

		/// <summary>
		/// 
		/// </summary>
		bool GetButtonDown(string buttonName);

		/// <summary>
		/// 
		/// </summary>
		bool GetButtonUp(string buttonName);

		/// <summary>
		/// 
		/// </summary>
		float GetAxis(int axisId);

		/// <summary>
		/// 
		/// </summary>
		float GetAxisRaw(int axisId);

		/// <summary>
		/// 
		/// </summary>
		bool GetButton(int buttonId);

		/// <summary>
		/// 
		/// </summary>
		bool GetButtonDown(int buttonId);

		/// <summary>
		/// 
		/// </summary>
		bool GetButtonUp(int buttonId);

		// <!-- Trial

		/// <summary>
		/// 
		/// </summary>
		int GetButtonPress(string buttonName);

		/// <summary>
		/// 
		/// </summary>
		int GetButtonPress(int buttonId);

		// Trial -->

	}
}
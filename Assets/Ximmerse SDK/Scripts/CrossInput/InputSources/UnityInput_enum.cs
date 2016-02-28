
namespace Ximmerse.CrossInput{

	/// <summary>
	/// 
	/// </summary>
	public partial class UnityInput {

		#region Const

		public const int s_AxisStart=0,s_ButtonStart=20,s_NumAxes=40;

		/// <summary>
		/// 
		/// </summary>
		public static string[] s_Joystick_0=new string[41]{
			"Joystick 0 Axis 1",
			"Joystick 0 Axis 2",
			"Joystick 0 Axis 3",
			"Joystick 0 Axis 4",
			"Joystick 0 Axis 5",
			"Joystick 0 Axis 6",
			"Joystick 0 Axis 7",
			"Joystick 0 Axis 8",
			"Joystick 0 Axis 9",
			"Joystick 0 Axis 10",
			"Joystick 0 Axis 11",
			"Joystick 0 Axis 12",
			"Joystick 0 Axis 13",
			"Joystick 0 Axis 14",
			"Joystick 0 Axis 15",
			"Joystick 0 Axis 16",
			"Joystick 0 Axis 17",
			"Joystick 0 Axis 18",
			"Joystick 0 Axis 19",
			"Joystick 0 Axis 20",
			"Joystick 0 Button 1",
			"Joystick 0 Button 2",
			"Joystick 0 Button 3",
			"Joystick 0 Button 4",
			"Joystick 0 Button 5",
			"Joystick 0 Button 6",
			"Joystick 0 Button 7",
			"Joystick 0 Button 8",
			"Joystick 0 Button 9",
			"Joystick 0 Button 10",
			"Joystick 0 Button 11",
			"Joystick 0 Button 12",
			"Joystick 0 Button 13",
			"Joystick 0 Button 14",
			"Joystick 0 Button 15",
			"Joystick 0 Button 16",
			"Joystick 0 Button 17",
			"Joystick 0 Button 18",
			"Joystick 0 Button 19",
			"Joystick 0 Button 20",
			""
		};

		#endregion Const

		#region Static Methods

		/// <summary>
		/// 
		/// </summary>
		public static int GetAxis(string[] joystick,float threshold){
			float f,sqrDead=threshold*threshold;
			for(int i=s_AxisStart;i<s_ButtonStart;++i){
				f=UnityEngine.Input.GetAxis(joystick[i]);
				if(f*f>sqrDead){
					return i;
				}
			}
			return -1;
		}

		/// <summary>
		/// 
		/// </summary>
		public static int GetButton(string[] joystick){
			for(int i=s_ButtonStart;i<s_NumAxes;++i){
				if(UnityEngine.Input.GetButtonDown(joystick[i])){
					return i;
				}
			}
			return -1;
		}

		#endregion Static Methods

	}

}
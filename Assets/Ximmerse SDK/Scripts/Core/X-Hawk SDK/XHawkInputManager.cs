using Ximmerse.CrossInput;

namespace Ximmerse.Core {

	/// <summary>
	/// 
	/// </summary>
	public class XHawkInputManager:XHawkInput,IInputSource {
		
		#region Fields

		public string[] controllerFormats=new string[2]{"Left_{0}","Right_{0}"};
		public XHawkMapSetting[] mapSettings=new XHawkMapSetting[2];

		public bool asVirtualControllers=true;
		public KeyOrButton btnReset=new KeyOrButton(
			new UnityEngine.KeyCode[0],
			new string[2]{"Left_Start","Right_Start"}
		);

		#endregion Fields

		#region Unity Messages

		/// <summary>
		/// 
		/// </summary>
		protected override void Awake() {
		}

		/// <summary>
		/// 
		/// </summary>
		protected override void Update() {
			if(btnReset.GetAnyDown()) {
				for(int i=0,imax=mapSettings.Length;i<imax;++i) {
					mapSettings[i].ResetInput();
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		protected override void OnDestroy() {
		}

		#endregion Unity Messages

		#region IInputSource

		/// <summary>
		/// 
		/// </summary>
		public virtual int InitInput() {
			base.Awake();
			//
			int ret=(enabled)?0:-1;
			if(ret==0) {
				int i=0,imax=mapSettings.Length;

				if(!CheckMappings(ref imax)) {
					Log.e("XHawkInputManager","CheckMappings() failed.");
					return -2;
				}

				for(;i<imax;++i) {
					//
					if(mapSettings[i]==null) {
						Log.e("XHawkInputManager","mapSettings["+i+"]==null");
						return -3;
					}
					// 
					mapSettings[i]=mapSettings[i].Clone<XHawkMapSetting>();//Instantiate.
					mapSettings[i].InitInput(this,i,controllerFormats[i]);
					if(asVirtualControllers) {
						VirtualController.controllers[i]=mapSettings[i].ToVirtualController(controllerFormats[i]);
					}
				}
			}
			return ret;
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual int ExitInput() {
			base.OnDestroy();
			return 0;
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual int EnterInputFrame() {
			base.Update();
			for(int i=0,imax=2;i<imax;++i) {
				mapSettings[i].UpdateInput();
			}
			//
			return 0;
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual int ExitInputFrame() {
			return 0;
		}

		#endregion IInputSource

		#region Methods

		/// <summary>
		/// 
		/// </summary>
		protected virtual bool CheckMappings(ref int count) {
			switch(count) {
				case 1:
					mapSettings=new XHawkMapSetting[2] {
						mapSettings[0],
						mapSettings[0]
					};
					count=2;
				break;
				case 2:
				break;
				default:
				return false;
			}
			return true;
		}

		#endregion Methods

	}
}

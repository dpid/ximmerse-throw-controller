using System.Collections.Generic;
using UnityEngine;
using Ximmerse.IO;

namespace Ximmerse.CrossInput {

	/// <summary>
	/// Unity Standalone Input在CrossInput兼容方案.
	/// </summary>
	public partial class UnityInput:MonoBehaviour,IInputSource {
		
		#region Data Struct
		
		[System.Serializable]
		public class EntryString{
			public string key,value;
			
			/// <summary>
			/// 
			/// </summary>
			public EntryString():this("Null"){
			}
			
			/// <summary>
			/// 
			/// </summary>
			public EntryString(string i_key){
				key=value=i_key;
			}
			
			/// <summary>
			/// 
			/// </summary>
			public string ToCSV(){
				return key+","+value;
			}
		}
		
		#endregion Data Struct

		#region Fields
		
		/// <summary>
		/// 映射鼠标到CrossInput.(VR模式鼠标基本没用了??)
		/// </summary>
		public bool mapMouse=false;

		public EntryString[]
			axes=new EntryString[4]{
				new EntryString("Horizontal"),new EntryString("Vertical"),
				new EntryString("Mouse X"),new EntryString("Mouse Y")
			},
			buttons=new EntryString[4]{
				new EntryString("Fire1"),new EntryString("Fire2"),
				new EntryString("Fire3"),new EntryString("Jump")
			}
		;
		public int numAxes=-1,numButtons=-1;

		public Dictionary<string,EntryString> mapInput;

		protected VirtualAxis[] _aHandles;
		protected VirtualButton[] _bHandles;

		#endregion Fields
		
		#region IInputSource

		/// <summary>
		/// 
		/// </summary>
		public virtual int InitInput() {
			// Check number of virtual elements.
			if(numAxes<0)
				numAxes=axes.Length;
			if(numButtons<0)
				numButtons=buttons.Length;
			// Register virtual elements.
			mapInput=new Dictionary<string,EntryString>(numAxes+numButtons);
			int i;
			// VirtualAxis
			_aHandles=new VirtualAxis[numAxes];
			for(i=0;i<numAxes;++i){
				_aHandles[i]=CrossInputManager.VirtualAxisReference(this,axes[i].key,true);
				mapInput.Add(axes[i].key,axes[i]);
			}
			// VirtualButton
			_bHandles=new VirtualButton[numButtons];
			for(i=0;i<numButtons;++i){
				_bHandles[i]=CrossInputManager.VirtualButtonReference(this,buttons[i].key,true);
				mapInput.Add(buttons[i].key,buttons[i]);
			}

			// Load Setting
			// Load();
			return 0;
		}

		/// <summary>
		/// 
		/// </summary>
		public int ExitInput() {
			// Save Setting
			// Save();
			return 0;
		}

		/// <summary>
		/// 
		/// </summary>
		public int EnterInputFrame() {
			if(mapMouse){
				Vector3 vec=Input.mousePosition;
				CrossInputManager.SetVirtualMousePositionX(vec.x);
				CrossInputManager.SetVirtualMousePositionY(vec.y);
				CrossInputManager.SetVirtualMousePositionZ(vec.z);
			}

			int i;
			for(i=0;i<numAxes;++i){
				_aHandles[i].Update(Input.GetAxisRaw(axes[i].value));
			}
			for(i=0;i<numButtons;++i){
				if(Input.GetButtonDown(buttons[i].value)){
					_bHandles[i].Pressed();
				}else if(Input.GetButtonUp(buttons[i].value)){
					_bHandles[i].Released();
				}

			}
			return 0;
		}

		/// <summary>
		/// 
		/// </summary>
		public int ExitInputFrame() {
			return 0;
		}

		#endregion IInputSource

	}
}
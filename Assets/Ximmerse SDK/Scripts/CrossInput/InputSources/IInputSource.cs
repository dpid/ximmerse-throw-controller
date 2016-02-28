using UnityEngine;
using System.Collections;

namespace Ximmerse.CrossInput {

	/// <summary>
	/// 视为一个输入源的接口
	/// </summary>
	public interface IInputSource {

		/// <summary>
		/// 
		/// </summary>
		bool enabled{
			get;
			set;
		}

		/// <summary>
		/// 初始化输入.
		/// </summary>
		int InitInput();

		/// <summary>
		/// 退出输入.
		/// </summary>
		int ExitInput();

		/// <summary>
		/// 重置输入.
		/// </summary>
		//void ResetInput();

		/// <summary>
		/// 进入输入帧(写入输入缓存).
		/// </summary>
		int EnterInputFrame();

		/// <summary>
		/// 退出输入帧(Cleanup).
		/// </summary>
		int ExitInputFrame();

	}
}
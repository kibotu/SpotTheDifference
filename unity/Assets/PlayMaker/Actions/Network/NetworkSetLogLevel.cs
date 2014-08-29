// (c) Copyright HutongGames, LLC 2010-2012. All rights reserved.

#if !(UNITY_FLASH || UNITY_NACL || UNITY_METRO || UNITY_WP8)

using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Network)]
	[Tooltip("Set the log textures for network messages. Default is Off.\n\nOff: Only report errors, otherwise silent.\n\nInformational: Report informational messages like connectivity events.\n\nFull: Full debug textures logging down to each individual message being reported.")]
	public class NetworkSetLogLevel : FsmStateAction
	{	
		[Tooltip("The log textures")]
		public NetworkLogLevel logLevel;


		public override void Reset()
		{
			logLevel = NetworkLogLevel.Off;
			
		}

		public override void OnEnter()
		{
			Network.logLevel = logLevel;
			
			Finish();
		}
	}
}

#endif
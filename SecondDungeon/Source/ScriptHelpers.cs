using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecondDungeon.Source
{
	[Serializable]
	public enum ScriptDescription
	{
		None,
		HealPlayer,
		GivePlayerGold,
		StartDialogueTree, // with ID reference
		ShowMessage, // with string message
		AssignDialogueTree,
		GiveQuest, // string quest name
	}

	[Serializable]
	public class ScriptObject
	{
		public ScriptDescription Script { get; set; }
		public object Value { get; set; }
	}

	public class ScriptHelpers
	{
		public static ScriptDescription Scripts;

		public static void Execute(string script)
		{
			//System.Runtime.Exe
		}

		public static void Execute(ScriptObject script)
		{
			Execute(script.Script, script.Value);
		}

		public static void Execute(ScriptDescription script, object value)
		{
			switch (script)
			{
				case ScriptDescription.GivePlayerGold:
					int gold = 0;
					if (int.TryParse(value.ToString(), out gold))
					{
						Global.Player.Info.Gold += gold;
					}
					SoundPlayer.PlaySound(Sound.PickupCoins);
					break;
				case ScriptDescription.HealPlayer:
					int health;
					if (int.TryParse(value.ToString(), out health))
					{
						Global.Player.Info.Health += health;
					}
					break;

			}
		}
	}
}

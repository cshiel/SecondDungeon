using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace SecondDungeon.Source
{
	public enum Sound
	{
		OpenDoor,
		DoorLocked,
		PickupCoins,
		MagicAttack,
		MeleeAttack,
		Walk,
		InterfaceClick,
	}

	public class SoundPlayer
	{
		private static Dictionary<Sound, SoundEffect> sounds = new Dictionary<Sound, SoundEffect>();

		public static void LoadSounds(Game game)
		{
			sounds.Add(Sound.OpenDoor, game.Content.Load<SoundEffect>("jail_cell_door"));
			sounds.Add(Sound.DoorLocked, game.Content.Load<SoundEffect>("door_lock"));
			sounds.Add(Sound.PickupCoins, game.Content.Load<SoundEffect>("RPG Sound Pack\\Inventory\\coin"));
			sounds.Add(Sound.MagicAttack, game.Content.Load<SoundEffect>("RPG Sound Pack\\battle\\magic1"));
			sounds.Add(Sound.MeleeAttack, game.Content.Load<SoundEffect>("RPG Sound Pack\\battle\\swing2"));
			sounds.Add(Sound.InterfaceClick, game.Content.Load<SoundEffect>("RPG Sound Pack\\interface\\interface6"));
		}
		public static void PlaySound(Sound sound)
		{
			var effect = sounds[sound];
			effect.Play();
		}
	}
}

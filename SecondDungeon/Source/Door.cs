using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RogueSharp;

namespace SecondDungeon.Source
{
	[Serializable]
	public class Door
	{
		public Door()
		{
			IsOpen = false;
			OpenType = 101;
			ClosedType = 100;
			//Key = 1;
		}

		public void TryOpen(int key)
		{
			if (key == Key)
			{
			}
			else
			{
			}
		}

		public void Open()
		{
			if (IsOpen == false)
			{
				SoundPlayer.PlaySound(Sound.OpenDoor);
			}
			IsOpen = true;
		}

		public int Key { get; set; }
		public int X { get; set; }
		public int Y { get; set; }
		public int OpenType { get; set; }
		public int ClosedType { get; set; }
		public bool IsOpen { get; set; }

	}
}

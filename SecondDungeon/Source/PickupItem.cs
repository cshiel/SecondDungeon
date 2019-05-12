using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecondDungeon.Source
{
	[Serializable]
	public class PickupItem
	{
		public PickupItem()
		{
			SpriteType = 200;
		}

		public void Pickup()
		{
			if (UseOnPickup)
			{
				Use();
			}
			else
			{
				Global.Player.AddItem(this);
			}
		}

		public void Use()
		{
			ScriptHelpers.Execute(Script, Parameter);
		}

		public bool UseOnPickup { get; set; }
		public string Name { get; set; }
		public string Description { get; set; } // todo
		public ScriptDescription Script;
		public object Parameter;
		public int X { get; set; }
		public int Y { get; set; }
		public int SpriteType { get; set; }
	}
}
